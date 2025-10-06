using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using NLog;
using NLog.Config;
using NLog.Targets;
using UnityEngine;
using Logger = NLog.Logger;

public static class CustomLogger
{
    private static readonly ConcurrentDictionary<string, Logger> Loggers = new ConcurrentDictionary<string, Logger>();
    private static readonly string LogDirectory;
    private static bool _isInitialized;

    /// <summary>
    /// NLog initialization with Android compatibility
    /// </summary>
    static CustomLogger()
    {
        try
        {
            // Set up log directory
            LogDirectory = Path.Combine(Application.persistentDataPath, "Logs");

            // Ensure log directory exists
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            InitializeLogger();
            _isInitialized = true;

            // Log successful initialization
            Info("Logger initialized successfully");
            Info($"Log directory: {LogDirectory}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize CustomLogger: {ex.Message}");
            _isInitialized = false;
        }
    }

    private static void InitializeLogger()
    {
        LogManager.Setup().SetupExtensions(s => s.RegisterTarget<UnityDebugLogTarget>("UnityDebugLog"));
        var config = new LoggingConfiguration();

        // Configuration for file logging with archive management
        var logFile = new FileTarget("logfile")
        {
            FileName = Path.Combine(LogDirectory, "${shortdate}.log"),
            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message} ${exception:format=tostring}",
            KeepFileOpen = false, // Changed to false for Android compatibility
            AutoFlush = true,
            ArchiveFileName = Path.Combine(LogDirectory, "archive", "{#}.log"),
            ArchiveSuffixFormat = "yyyyMMdd",
            ArchiveEvery = FileArchivePeriod.Day,
            MaxArchiveFiles = 7, // Keep one week of logs
            ArchiveOldFileOnStartup = true,
            CreateDirs = true // Ensure directories are created
        };

        // Configuration for Unity console logging
        var logUnity = new UnityDebugLogTarget
        {
            Layout = "${date:format=HH\\:mm\\:ss.fff}|${level:uppercase=true}|${logger}|${message} ${exception:format=tostring}"
        };

        // Add rules for different log levels
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);
        config.AddRule(LogLevel.Info, LogLevel.Fatal, logUnity);

        LogManager.Configuration = config;
    }

    /// <summary>
    /// Ensures the log directory exists and is writable
    /// </summary>
    private static bool EnsureLogDirectoryExists()
    {
        try
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            // Test write access
            string testFile = Path.Combine(LogDirectory, "test.txt");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to access log directory: {ex.Message}");
            return false;
        }
    }

    public static void CleanupOldLogs(int daysToKeep = 7)
    {
        try
        {
            if (!Directory.Exists(LogDirectory)) return;

            var currentDate = DateTime.Now;
            var files = Directory.GetFiles(LogDirectory, "*.log");

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var ageInDays = (currentDate - fileInfo.CreationTime).TotalDays;

                if (ageInDays > daysToKeep)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to delete old log file {file}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during log cleanup: {ex.Message}");
        }
    }

    #region Public Logging Methods

    public static void Info(string message,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!_isInitialized) return;
        Log(LogLevel.Info, message, callerMemberName, callerFilePath, callerLineNumber);
    }

    public static void Warn(string message,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!_isInitialized) return;
        Log(LogLevel.Warn, message, callerMemberName, callerFilePath, callerLineNumber);
    }

    public static void Error(string message,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!_isInitialized) return;
        Log(LogLevel.Error, message, callerMemberName, callerFilePath, callerLineNumber);
    }

    public static void Error(Exception exception, string message = "",
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (!_isInitialized) return;
        Log(LogLevel.Error, exception, message, callerMemberName, callerFilePath, callerLineNumber);
    }

    #endregion

    private static void Log(LogLevel logLevel, string message,
        string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0)
    {
        Log(logLevel, null, message, callerMemberName, callerFilePath, callerLineNumber);
    }

    private static void Log(LogLevel logLevel, Exception exception, string message = "",
        string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0)
    {
        try
        {
            string logMessage = $"({callerMemberName}:{callerLineNumber}) {message}";
            var logger = GetLogger(callerFilePath);
            logger.Log(logLevel, exception, logMessage);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Logging failed: {ex.Message}");
        }
    }

    private static Logger GetLogger(string callerFilePath)
    {
        return Loggers.GetOrAdd(callerFilePath, path => LogManager.GetLogger(Path.GetFileName(path)));
    }

    public static string GetLogDirectory()
    {
        return LogDirectory;
    }
}
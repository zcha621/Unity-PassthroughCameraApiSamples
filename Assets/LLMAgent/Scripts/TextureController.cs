using UnityEngine;
using UnityEngine.Rendering;
using PassthroughCameraSamples;
using Unity.RenderStreaming;
using Unity.WebRTC;
using UnityEngine.Experimental.Rendering;
using System.Collections;

public class TextureController : MonoBehaviour
{
    [Header("Required Components")]
    public WebCamTextureManager webCamTextureManager;
    public VideoStreamSender videoStreamSender;
    
    [Header("Debug Settings")]
    public bool enableDebugLogs = true;
    public bool showDetailedLogs = false;
    
    [Header("Performance Settings")]
    [Range(15, 90)]
    public int targetFrameRate = 30;
    public bool limitTextureUpdates = true;
    
    [Header("Quest 3 Specific Settings")]
    public bool forceTextureConversion = false;
    public bool useReadPixelsFallback = true;
    
    private Texture2D texture2D;
    private RenderTexture tempRenderTexture;
    private bool isStreamingActive = false;
    private int frameUpdateCount = 0;
    private float lastFrameTime = 0f;
    private float frameInterval;
    private bool hasLoggedFirstFrame = false;
    
    void Start()
    {
        if (enableDebugLogs)
            Debug.Log("TextureController: Starting initialization...");
            
        // Calculate frame interval for performance limiting
        frameInterval = 1f / targetFrameRate;
        
        // Configure VideoStreamSender
        if (videoStreamSender != null)
        {
            videoStreamSender.source = VideoStreamSource.Texture;
            if (enableDebugLogs)
                Debug.Log("TextureController: VideoStreamSender configured for Texture source");
        }
        else
        {
            Debug.LogError("TextureController: VideoStreamSender is not assigned!");
            return;
        }
        
        // Start texture conversion coroutine
        StartCoroutine(TextureConversionLoop());
    }
    
    IEnumerator TextureConversionLoop()
    {
        if (enableDebugLogs)
            Debug.Log("TextureController: Starting texture conversion loop...");
            
        // Wait for WebCamTexture to be available and playing
        yield return new WaitUntil(() => webCamTextureManager != null && 
                                        webCamTextureManager.WebCamTexture != null && 
                                        webCamTextureManager.WebCamTexture.isPlaying);
        
        // Add additional wait for Quest 3 camera initialization
        if (Application.platform == RuntimePlatform.Android)
        {
            yield return new WaitForSeconds(0.5f); // Give Quest 3 camera time to stabilize
        }
        
        if (enableDebugLogs)
            Debug.Log("TextureController: WebCamTexture is available and playing, setting up conversion...");
            
        // Setup texture conversion
        SetupTextureConversion();
        
        // Main conversion loop
        while (true)
        {
            if (webCamTextureManager.WebCamTexture != null && webCamTextureManager.WebCamTexture.isPlaying)
            {
                // Update texture if needed
                if (ShouldUpdateTexture())
                {
                    UpdateTexture();
                }
            }
            
            yield return new WaitForEndOfFrame();
        }
    }
    
    void SetupTextureConversion()
    {
        var webCamTexture = webCamTextureManager.WebCamTexture;
        if (webCamTexture == null)
        {
            Debug.LogError("TextureController: WebCamTexture is null during setup!");
            return;
        }
        
        // Get WebRTC supported format
        GraphicsFormat supportedFormat = WebRTC.GetSupportedGraphicsFormat(SystemInfo.graphicsDeviceType);
        
        if (enableDebugLogs)
        {
            Debug.Log($"TextureController: WebCamTexture format: {webCamTexture.graphicsFormat}");
            Debug.Log($"TextureController: WebRTC supported format: {supportedFormat}");
            Debug.Log($"TextureController: WebCamTexture size: {webCamTexture.width}x{webCamTexture.height}");
            Debug.Log($"TextureController: WebCamTexture isPlaying: {webCamTexture.isPlaying}");
            Debug.Log($"TextureController: Graphics Device: {SystemInfo.graphicsDeviceType}");
            Debug.Log($"TextureController: Platform: {Application.platform}");
            Debug.Log($"TextureController: WebCamTexture didUpdateThisFrame: {webCamTexture.didUpdateThisFrame}");
        }
        
        // Check if direct assignment is possible (but force conversion for Quest 3 if needed)
        bool isFormatCompatible = webCamTexture.graphicsFormat == supportedFormat;
        bool isSizeValid = webCamTexture.width > 0 && webCamTexture.height > 0;
        bool shouldForceConversion = forceTextureConversion || 
                                   (Application.platform == RuntimePlatform.Android && 
                                    SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan);
        
        if (isFormatCompatible && isSizeValid && !shouldForceConversion)
        {
            // Direct assignment - no conversion needed
            if (enableDebugLogs)
                Debug.Log("TextureController: Direct assignment possible - no conversion needed");
                
            try
            {
                // Validate the texture format before assignment
                WebRTC.ValidateGraphicsFormat(webCamTexture.graphicsFormat);
                videoStreamSender.sourceTexture = webCamTexture;
                isStreamingActive = true;
                
                if (enableDebugLogs)
                    Debug.Log("TextureController: Direct assignment successful");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"TextureController: Direct assignment failed: {e.Message}. Falling back to conversion.");
                SetupTextureConversionPath(webCamTexture, supportedFormat);
            }
        }
        else
        {
            // Format conversion needed
            if (enableDebugLogs)
                Debug.Log($"TextureController: Format conversion needed (Compatible: {isFormatCompatible}, Valid size: {isSizeValid}, Force conversion: {shouldForceConversion})");
                
            SetupTextureConversionPath(webCamTexture, supportedFormat);
        }
    }
    
    void SetupTextureConversionPath(WebCamTexture webCamTexture, GraphicsFormat supportedFormat)
    {
        try
        {
            // Create target Texture2D with WebRTC-supported format
            texture2D = new Texture2D(
                webCamTexture.width, 
                webCamTexture.height, 
                supportedFormat, 
                0, // mipCount
                TextureCreationFlags.None
            );
            
            if (enableDebugLogs)
                Debug.Log($"TextureController: Created Texture2D {texture2D.width}x{texture2D.height} with format {supportedFormat}");
            
            // Create intermediate RenderTexture for reliable conversion
            tempRenderTexture = new RenderTexture(
                webCamTexture.width,
                webCamTexture.height,
                0,
                WebRTC.GetSupportedRenderTextureFormat(SystemInfo.graphicsDeviceType)
            );
            tempRenderTexture.Create();
            
            if (enableDebugLogs)
                Debug.Log($"TextureController: Created RenderTexture {tempRenderTexture.width}x{tempRenderTexture.height}");
            
            // Validate the created texture format
            WebRTC.ValidateGraphicsFormat(texture2D.graphicsFormat);
            
            // Assign the conversion target to VideoStreamSender
            videoStreamSender.sourceTexture = texture2D;
            isStreamingActive = true;
            
            if (enableDebugLogs)
                Debug.Log("TextureController: Texture conversion path setup complete");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"TextureController: Failed to setup texture conversion: {e.Message}");
        }
    }
    
    bool ShouldUpdateTexture()
    {
        if (!isStreamingActive || texture2D == null)
            return false;
            
        // Performance limiting
        if (limitTextureUpdates)
        {
            float currentTime = Time.time;
            if (currentTime - lastFrameTime < frameInterval)
                return false;
            lastFrameTime = currentTime;
        }
        
        // For Quest 3, be more aggressive about frame updates initially
        if (frameUpdateCount < 10)
        {
            return true;
        }
        
        // Check if there's new frame data
        return webCamTextureManager.WebCamTexture.didUpdateThisFrame || frameUpdateCount % 3 == 0;
    }
    
    void UpdateTexture()
    {
        if (texture2D == null || tempRenderTexture == null)
            return;
            
        var webCamTexture = webCamTextureManager.WebCamTexture;
        if (webCamTexture == null || !webCamTexture.isPlaying)
            return;
        
        frameUpdateCount++;
        
        try
        {
            // Use optimized path for Quest 3/Android
            if (Application.platform == RuntimePlatform.Android && useReadPixelsFallback)
            {
                UpdateTextureWithReadPixels(webCamTexture);
            }
            else
            {
                UpdateTextureWithConvert(webCamTexture);
            }
            
            // Log first successful frame update
            if (!hasLoggedFirstFrame && enableDebugLogs)
            {
                Debug.Log($"TextureController: First frame successfully updated! Resolution: {texture2D.width}x{texture2D.height}");
                hasLoggedFirstFrame = true;
            }
            
            if (showDetailedLogs && frameUpdateCount % 60 == 0)
            {
                Debug.Log($"TextureController: Updated texture (frame {frameUpdateCount})");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"TextureController: Texture update failed: {e.Message}");
        }
    }
    
    void UpdateTextureWithReadPixels(WebCamTexture webCamTexture)
    {
        // Use ReadPixels method for better Quest 3 compatibility
        Graphics.Blit(webCamTexture, tempRenderTexture);
        
        var previousActive = RenderTexture.active;
        RenderTexture.active = tempRenderTexture;
        
        texture2D.ReadPixels(new Rect(0, 0, tempRenderTexture.width, tempRenderTexture.height), 0, 0);
        texture2D.Apply();
        
        RenderTexture.active = previousActive;
    }
    
    void UpdateTextureWithConvert(WebCamTexture webCamTexture)
    {
        // Try Graphics.ConvertTexture first
        try
        {
            Graphics.ConvertTexture(webCamTexture, texture2D);
        }
        catch (System.Exception)
        {
            // Fallback to ReadPixels method
            UpdateTextureWithReadPixels(webCamTexture);
        }
    }
    
    void OnDestroy()
    {
        if (enableDebugLogs)
            Debug.Log("TextureController: Cleaning up resources...");
            
        isStreamingActive = false;
        
        // Clean up resources
        if (texture2D != null)
        {
            Destroy(texture2D);
            texture2D = null;
        }
        
        if (tempRenderTexture != null)
        {
            tempRenderTexture.Release();
            Destroy(tempRenderTexture);
            tempRenderTexture = null;
        }
    }
    
    // Public methods for debugging and control
    public void LogTextureStatus()
    {
        Debug.Log("=== TextureController Status ===");
        Debug.Log($"Streaming Active: {isStreamingActive}");
        Debug.Log($"Frame Updates: {frameUpdateCount}");
        Debug.Log($"Has Logged First Frame: {hasLoggedFirstFrame}");
        
        if (webCamTextureManager?.WebCamTexture != null)
        {
            var wct = webCamTextureManager.WebCamTexture;
            Debug.Log($"WebCamTexture: {wct.width}x{wct.height}, Playing: {wct.isPlaying}, Format: {wct.graphicsFormat}");
            Debug.Log($"WebCamTexture didUpdateThisFrame: {wct.didUpdateThisFrame}");
        }
        
        if (texture2D != null)
            Debug.Log($"Texture2D: {texture2D.width}x{texture2D.height}, Format: {texture2D.graphicsFormat}");
            
        if (videoStreamSender?.sourceTexture != null)
            Debug.Log($"VideoStreamSender.sourceTexture: {videoStreamSender.sourceTexture.width}x{videoStreamSender.sourceTexture.height}");
    }
    
    public void ToggleDetailedLogs()
    {
        showDetailedLogs = !showDetailedLogs;
        Debug.Log($"TextureController: Detailed logs {(showDetailedLogs ? "enabled" : "disabled")}");
    }
    
    public void ForceTextureUpdate()
    {
        if (isStreamingActive && texture2D != null)
        {
            UpdateTexture();
            Debug.Log("TextureController: Forced texture update");
        }
    }
}

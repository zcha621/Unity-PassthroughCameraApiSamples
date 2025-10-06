using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Meta.Voice.Samples.Dictation;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

// Custom certificate handler to allow insecure connections for development
public class AcceptAllCertificatesSignedWithASpecificKeyPublicKey : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Always return true to accept all certificates (for development only)
        return true;
    }
}

public class AnythingLLM : MonoBehaviour
{
    [Header("LLM Settings")]
    public string apiUrl = "http://130.216.208.87:3001/api/v1/workspace/agent0/chat"; // Updated URL
    public string apiKey = "your_api_key_if_needed"; // leave blank if not used

    [Header("UI")]
    public TMP_InputField userInputField;
    public TMP_Text responseText;
    public TMP_Text timerText; // Add a text element for displaying the timer

    [Header("Voice SDK Components")]
    public TTSSpeaker speaker; // Reference to the TTS speaker component
    public DictationActivation dictationActivator; // Reference to the DictationActivation component

    // Timer variables
    private float requestStartTime;
    private float responseReceivedTime;
    private float speakingEndTime;
    private bool isWaitingForResponse = false;
    private bool isSpeaking = false;
    private float speakingStartTime;

    private void Start()
    {
        // Log a warning about HTTP connections
        Debug.LogWarning("Using HTTP connection. For production, ensure the following Unity Player Settings are configured:");
        Debug.LogWarning("1. Go to Edit > Project Settings > Player > Publishing Settings > Configuration");
        Debug.LogWarning("2. Set 'Internet Client Server Capability' if on UWP");
        Debug.LogWarning("3. For WebGL: Ensure CORS is properly configured on the server");
        Debug.LogWarning("4. Consider using HTTPS for production deployments");
    }
    
    private void Update()
    {
        // Update the timer text if waiting for response
        if (isWaitingForResponse || isSpeaking)
        {
            UpdateTimerDisplay();
        }
        if(Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.One))
        {
            dictationActivator.ToggleActivation();
        }
        
    }
    public void UpdateTranscripts(string transcript)
    {
        userInputField.text = transcript;
        Debug.Log(transcript);
        SendPrompt();
    }
    public void SendPrompt()
    {
        // Start the timer when sending the request
        requestStartTime = Time.time;
        isWaitingForResponse = true;
        isSpeaking = false;


        // Clear any previous timing info
        UpdateTimerDisplay();

        string prompt = userInputField.text;
        StartCoroutine(SendLLMRequest(prompt));
    }
    
    IEnumerator SendLLMRequest(string prompt)
    {
        // Construct the JSON payload
        string jsonPayload = JsonUtility.ToJson(new PromptData { message = prompt });

        // Create WWWForm to help with HTTP handling
        WWWForm form = new WWWForm();
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        
        // Try different UnityWebRequest construction methods
        UnityWebRequest request;
        
        // Method 1: Try with UnityWebRequest.Put which sometimes works better with HTTP
        try
        {
            request = UnityWebRequest.Put(apiUrl, bodyRaw);
            request.method = "POST"; // Override method to POST
            request.SetRequestHeader("Content-Type", "application/json");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to create request with PUT method: " + e.Message);
            // Fallback to basic constructor
            request = new UnityWebRequest(apiUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
        }
        
        // Add certificate handler to allow insecure connections
        request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
        request.disposeCertificateHandlerOnDispose = true;
        
        // Set timeout to avoid hanging
        request.timeout = 30;

        // Headers
        if (!string.IsNullOrEmpty(apiKey))
        {
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        }

        // Log the request details for debugging
        Debug.Log($"Sending request to: {apiUrl}");
        Debug.Log($"Request method: {request.method}");
        Debug.Log($"Request payload: {jsonPayload}");

        // Send request and wait for response
        yield return request.SendWebRequest();

        // Record the time when response is received
        responseReceivedTime = Time.time;
        isWaitingForResponse = false;

        // Enhanced error reporting
        Debug.Log($"Request completed with result: {request.result}");
        Debug.Log($"Response Code: {request.responseCode}");
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseJson = request.downloadHandler.text;
            Debug.Log("LLM Response: " + responseJson);

            // Extract textResponse using regex
            try
            {
                // Look for "textResponse":"[content]" pattern
                string pattern = "\"textResponse\":\"(.*?)\"(?=,|\\})";
                Match match = Regex.Match(responseJson, pattern, RegexOptions.Singleline);

                if (match.Success)
                {
                    // Get the captured content and unescape it
                    string textResponse = match.Groups[1].Value;
                    textResponse = Regex.Unescape(textResponse);
                    responseText.text = textResponse;
                    Debug.Log("Extracted text response: " + textResponse);

                    // Optionally, use the TTS speaker to speak the response
                    if (speaker != null)
                    {
                        // Register for the speaker's OnStart event
                        speaker.Events.OnStartSpeaking.AddListener(OnSpeakingStart);
                        // Register for the speaker's OnComplete event
                        speaker.Events.OnComplete.AddListener(OnSpeakingComplete);
                        isSpeaking = true;
                        speaker.Speak(textResponse);


                    }
                    else
                    {
                        // If no speaker, mark as complete now
                        speakingEndTime = responseReceivedTime;
                    }
                }
                else
                {
                    responseText.text = "Could not extract text response.";
                    Debug.LogWarning("Failed to extract textResponse using regex");
                    speakingEndTime = responseReceivedTime; // No speaking, so end time is same as response time
                }
            }
            catch (System.Exception e)
            {
                responseText.text = "Error parsing response: " + e.Message;
                Debug.LogError("Error parsing response: " + e.Message);
                speakingEndTime = responseReceivedTime; // No speaking, so end time is same as response time
            }
        }
        else
        {
            // Detailed error reporting
            string errorMessage = $"LLM Request Failed:\n";
            errorMessage += $"Result: {request.result}\n";
            errorMessage += $"Response Code: {request.responseCode}\n";
            errorMessage += $"Error: {request.error}\n";
            
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                errorMessage += "\nThis appears to be a connection error. Common causes:\n";
                errorMessage += "- HTTP connections blocked by Unity security settings\n";
                errorMessage += "- Network connectivity issues\n";
                errorMessage += "- Server not responding\n";
                errorMessage += "- Firewall blocking the connection\n";
                errorMessage += "\nTo fix HTTP connection issues:\n";
                errorMessage += "1. Try using HTTPS instead of HTTP\n";
                errorMessage += "2. Check Unity Player Settings for network permissions\n";
                errorMessage += "3. Verify the server is accessible from your network\n";
            }
            
            Debug.LogError(errorMessage);
            responseText.text = $"Connection Error: {request.error}\nSee Console for details.";
            speakingEndTime = responseReceivedTime;
        }

        // Clean up
        request.Dispose();
        
        // Update the display with the final response time
        UpdateTimerDisplay();
    }

    private void OnSpeakingComplete(TTSSpeaker speaker, TTSClipData data)
    {
        // Remove the listener to avoid duplicate events
        speaker.Events.OnComplete.RemoveListener(OnSpeakingComplete);

        // Record the time when speaking is complete
        speakingEndTime = Time.time;
        isSpeaking = false;

        // Update the display with the final speaking time
        UpdateTimerDisplay();

        Debug.Log($"Speaking completed in: {speakingEndTime - responseReceivedTime:F2} seconds");
    }
    private void OnSpeakingStart(TTSSpeaker speaker, string _)
    {
        speaker.Events.OnStartSpeaking.RemoveListener(OnSpeakingStart); // Clean up
        speakingStartTime = Time.time;
        Debug.Log($"Speaking started at: {speakingStartTime:F2} (think time: {speakingStartTime - responseReceivedTime:F2} seconds)");
        UpdateTimerDisplay(); // So it reflects right away
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        float currentTime = Time.time;
        string displayText = $"Request sent: 0.00s\n";

        if (isWaitingForResponse)
        {
            float elapsed = currentTime - requestStartTime;
            displayText = $"Waiting for response: {elapsed:F2}s";
        }
        else
        {
            float responseTime = responseReceivedTime - requestStartTime;
            displayText = $"Response received in: {responseTime:F2}s\n";

            if (speakingStartTime > 0 && isSpeaking)
            {
                float TTSTime = speakingStartTime - responseReceivedTime;
                float speakingTime = currentTime - speakingStartTime;
                displayText += $"TTS time: {TTSTime:F2}s\n";
                displayText += $"Speaking for: {speakingTime:F2}s";
            }
            else if (speakingEndTime > 0 && speakingStartTime > 0)
            {
                float TTSTime = speakingStartTime - responseReceivedTime;
                float speakingTime = speakingEndTime - speakingStartTime;
                float totalTime = speakingEndTime - requestStartTime;
                displayText += $"Think time: {TTSTime:F2}s\n";
                displayText += $"Speaking completed in: {speakingTime:F2}s\n";
                displayText += $"Total time: {totalTime:F2}s";
            }
        }

        timerText.text = displayText;
    }

    [System.Serializable]
    public class PromptData
    {
        public string message;
        public string mode = "chat";
    }
}
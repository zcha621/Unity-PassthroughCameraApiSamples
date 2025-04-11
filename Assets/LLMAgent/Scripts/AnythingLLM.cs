using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Meta.WitAi.TTS.Utilities;
using System.Text.RegularExpressions;
using System;
using Meta.WitAi.TTS.Data;

public class AnythingLLM : MonoBehaviour
{
    [Header("LLM Settings")]
    public string apiUrl = "http://localhost:3001/api/v1/chat"; // change if deployed remotely
    public string apiKey = "your_api_key_if_needed"; // leave blank if not used

    [Header("UI")]
    public TMP_InputField userInputField;
    public TMP_Text responseText;
    public TMP_Text timerText; // Add a text element for displaying the timer

    [Header("Voice SDK Components")]
    public TTSSpeaker speaker; // Reference to the TTS speaker component

    // Timer variables
    private float requestStartTime;
    private float responseReceivedTime;
    private float speakingEndTime;
    private bool isWaitingForResponse = false;
    private bool isSpeaking = false;
    private float speakingStartTime;


    private void Update()
    {
        // Update the timer text if waiting for response
        if (isWaitingForResponse || isSpeaking)
        {
            UpdateTimerDisplay();
        }
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

        // Create a UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Headers
        request.SetRequestHeader("Content-Type", "application/json");
        if (!string.IsNullOrEmpty(apiKey))
        {
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        }

        // Send request and wait for response
        yield return request.SendWebRequest();

        // Record the time when response is received
        responseReceivedTime = Time.time;
        isWaitingForResponse = false;

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseJson = request.downloadHandler.text;
            Debug.Log("LLM Response: " + responseJson);

            // Extract textResponse using regex
            try
            {
                // Look for "textResponse":"[content]" pattern
                string pattern = "\"textResponse\":\"(.*?)\"(?=,)";
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
            Debug.LogError("LLM Request Error: " + request.error);
            responseText.text = $"Error: {request.error}";
            speakingEndTime = responseReceivedTime; // No speaking, so end time is same as response time
        }

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
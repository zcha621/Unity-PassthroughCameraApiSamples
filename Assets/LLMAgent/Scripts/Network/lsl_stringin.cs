using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using LSL4Unity.Utils;
public class lsl_stringin : InletAStringInlet
{
    protected override void Process(string[] newSample, double timestamp)
    {
        if(newSample.Length != 2)
        {
            Debug.LogError("Received invalid data");
            return;
        }
        ProcessData(newSample[0], newSample[1]);
    }

    private void ProcessData(string type, string msg)
    {
        // Implement your logic to process the received data here
        //CustomLogger.Info($"Received Data - Type: {type}, Message: {msg}");
        GameEvents.OnLSLRecv?.Invoke(type, msg);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using LSL4Unity.Utils;
using UnityEngine.Events;


public class lsl_pos_inlet : AFloatInlet
{
    public UnityEvent<Vector3, Quaternion> OnPosUpdated = new UnityEvent<Vector3, Quaternion>();
    public UnityEvent<List<Vector3>, List<Quaternion>> OnPointCloudUpdated = new UnityEvent<List<Vector3>, List<Quaternion>>();
    void Reset()
    {
        StreamName = "Unity.Pose";
    }

    protected override void OnStreamAvailable()
    {

    }

    protected override void Process(float[] newSample, double timestamp)
    {
        //debug all the samples
        CustomLogger.Info($"Processing sample at {timestamp}: {string.Join(", ", newSample)}");
        
    }
}

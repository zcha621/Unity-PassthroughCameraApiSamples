using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
public class lsl_stringout : MonoBehaviour
{
    [SerializeField]
    protected string StreamName = "unity_lsl";
    [SerializeField]
    protected string StreamType = "unity_lsl";
    private StreamOutlet outlet;
    private string[] sample = { "","" };
    private int channel_num = 2; // first type, second msg
    void Awake()
    {
        GameEvents.OnLSLSend.AddListener(m_push_sample);
        var hash = new Hash128();
        hash.Append(StreamName);
        hash.Append(StreamType);
        hash.Append(gameObject.GetInstanceID());
        StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, channel_num, LSL.LSL.IRREGULAR_RATE,
            channel_format_t.cf_string, hash.ToString());
        outlet = new StreamOutlet(streamInfo);
    }

    //private void OnEnable()
    //{
    //    GameEvents.OnLSLSend.AddListener(m_push_sample);
    //}
    //private void OnDisable()
    //{
    //    GameEvents.OnLSLSend.RemoveListener(m_push_sample);
    //}

    //void Start()
    //{
    //    var hash = new Hash128();
    //    hash.Append(StreamName);
    //    hash.Append(StreamType);
    //    hash.Append(gameObject.GetInstanceID());
    //    StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, channel_num, LSL.LSL.IRREGULAR_RATE,
    //        channel_format_t.cf_string, hash.ToString());
    //    outlet = new StreamOutlet(streamInfo);

    //}

    void m_push_sample(string type, string msg)
    {
        if(outlet!=null)
        {
            sample[0] = type;
            sample[1] = msg;
            outlet.push_sample(sample);
        }
        
    }
}

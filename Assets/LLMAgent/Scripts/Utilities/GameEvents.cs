using System;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventBoolInt : UnityEvent<bool, int> { }
public class UnityEventBool : UnityEvent<bool> { }
public class UnityEventInt : UnityEvent<int> { }
public class UnityEventFloat : UnityEvent<float> { }
public class UnityEventGameObject : UnityEvent<GameObject> { }
public class UnityEvent_GameObject_Vector3 : UnityEvent<GameObject,Vector3> { }
public class UnityEvent_GameObject_Bool : UnityEvent<GameObject,bool> { }
public class UnityEventVector3 : UnityEvent<Vector3> { }
public class UnityEvent_Str : UnityEvent<string> { }
public class UnityEvent_StrStr : UnityEvent<string,string> { }

public class GameEvents
{
    //--------- Score
    public static readonly UnityEvent OnTaskFinished = new UnityEvent();
    public static readonly UnityEvent OnHelpRequested = new UnityEvent();
    public static readonly UnityEvent OnTie = new UnityEvent();
    public static readonly UnityEvent OnUserWin = new UnityEvent();
    public static readonly UnityEvent OnUserLose = new UnityEvent();
    public static readonly UnityEvent OnAIPlay = new UnityEvent();
    public static readonly UnityEvent OnGameStart = new UnityEvent();
    public static readonly UnityEvent OnGameEnd = new UnityEvent();
    public static readonly UnityEvent OnHelpStop = new UnityEvent();
    public static readonly UnityEvent OnArrivedAtTarget = new UnityEvent();//start talking animation and suggestion speech
    public static readonly UnityEvent OnSpeechDone = new UnityEvent();
    public static readonly UnityEvent OnSpeechQueued = new UnityEvent();
    public static readonly UnityEvent OnGreetStart = new UnityEvent();
    public static readonly UnityEvent OnGreetStop = new UnityEvent();
    public static readonly UnityEvent OnIntroStart = new UnityEvent();
    public static readonly UnityEvent OnIntroStop = new UnityEvent();
    public static readonly UnityEvent OnGazeAway = new UnityEvent();
    public static readonly UnityEvent OnProgramRun = new UnityEvent();
    public static readonly UnityEvent OnProgramStop = new UnityEvent();
    public static readonly UnityEvent OnTraining = new UnityEvent();
    public static readonly UnityEvent OnEnemyTakeDamage = new UnityEvent();

    public static readonly UnityEvent_Str OnTimeOut = new UnityEvent_Str();
    public static readonly UnityEvent_Str OnGreeting = new UnityEvent_Str();
    public static readonly UnityEvent_Str OnWin = new UnityEvent_Str();
    public static readonly UnityEvent_Str OnLose = new UnityEvent_Str();
    public static readonly UnityEvent_Str OnGameTie = new UnityEvent_Str();
    public static readonly UnityEvent_Str OnIntro = new UnityEvent_Str();
    public static readonly UnityEvent_Str SaveData = new UnityEvent_Str();
    public static readonly UnityEvent_Str SaveGazeData = new UnityEvent_Str();
    public static readonly UnityEvent_Str OnZmqRecvStr = new UnityEvent_Str();
    public static readonly UnityEvent_Str OnTalkAboutStr = new UnityEvent_Str();
    public static readonly UnityEvent_Str OnUserStudyInit = new UnityEvent_Str();

    public static readonly UnityEvent_Str OnZmqSendStr = new UnityEvent_Str();
    public static readonly UnityEvent_StrStr OnZmqSendStrStr = new UnityEvent_StrStr();
    public static readonly UnityEvent_StrStr OnLSLSend = new UnityEvent_StrStr();
    public static readonly UnityEvent_StrStr OnLSLRecv = new UnityEvent_StrStr();
    // score
    public static readonly UnityEventFloat OnScoreChanged = new UnityEventFloat();

    public static readonly UnityEventBool OnAnswered = new UnityEventBool();
    public static readonly UnityEventBool OnUserClick = new UnityEventBool();
    public static readonly UnityEventBool OnMutualGazeEnabled = new UnityEventBool();
    
    

    public static readonly UnityEventGameObject OnClickPiece = new UnityEventGameObject();
    public static readonly UnityEventGameObject OnClickNetworkPieceRec = new UnityEventGameObject();
    public static readonly UnityEventGameObject OnClickNetworkPieceSend = new UnityEventGameObject();
    public static readonly UnityEventGameObject OnTargetObjectPerceived = new UnityEventGameObject();
    public static readonly UnityEventGameObject OnAnyPieceMove = new UnityEventGameObject();
    public static readonly UnityEventGameObject OnFirstPieceMove = new UnityEventGameObject();
    public static readonly UnityEventGameObject OnWeaponSpawn = new UnityEventGameObject();
    //public static readonly UnityEvent_GameObject_Bool OnTargetObjectPerceived = new UnityEvent_GameObject_Bool();

    public static readonly UnityEventInt QueryTaskType = new UnityEventInt();
    public static readonly UnityEventVector3 InitPos = new UnityEventVector3();
    public static readonly UnityEvent RequestNetTest = new UnityEvent();
    public static readonly UnityEvent OnClientShutDown = new UnityEvent();


    public delegate GameObject ObtainGameObject();
    public static ObtainGameObject OnPieceSelected;


    public static readonly UnityEvent_Str OnSpeechRecognized = new UnityEvent_Str();
    public static readonly UnityEvent_Str OnChatGPTGenerated = new UnityEvent_Str();
    public static readonly UnityEvent OnChatFinished = new UnityEvent();

    public static readonly UnityEventVector3 OnTargetPerceived = new UnityEventVector3();

    public static readonly UnityEvent_StrStr OnSuggestSpeechThesis = new UnityEvent_StrStr();


    public static readonly UnityEvent_Str SetMarker = new UnityEvent_Str();
    public static readonly UnityEvent OnBaseLineDone = new UnityEvent();
    public static readonly UnityEvent<GameObject> OnObjSpawn = new UnityEvent<GameObject>();
    public static readonly UnityEvent<GameObject> OnObjDestroy = new UnityEvent<GameObject>();

}

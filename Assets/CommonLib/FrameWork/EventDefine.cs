using UnityEngine;
using System.Collections;

public enum EGameEvent
{
    eGameEvent_ErrorStr = 1,
    eGameEvent_ConnectServerFail,
    eGameEvent_ConnectServerSuccess,

    eGameEvent_ReConnectSuccess,
    eGameEvent_ReConnectFail,
    eGameEvent_ReReady,

}

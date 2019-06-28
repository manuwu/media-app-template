using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFDUtils
{
    AndroidJavaObject InterfaceObj;

    public WFDUtils(SvrNativeInterface svrNI)
    {
        if (svrNI != null)
            InterfaceObj = svrNI.GetUtils(UtilsType.WFDUtils);

        LogTool.Log("创建WFDUtils");
    }

    public void SetWfdOnConnectListener(string gameObjectName, string functionName)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("setOnConnectListener", gameObjectName, functionName);
    }

    public bool GetWfdConnectStatus()
    {
        bool isOpen = false;

        if (InterfaceObj != null)
            isOpen = InterfaceObj.Call<bool>("isConnectWfd");

        return isOpen;
    }

    public void Resume()
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("resume");
    }

    public void Pause()
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("pause");
    }
}

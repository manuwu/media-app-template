/*
 * 2018-5-28
 * 黄秋燕 Shemi
 * 音量调节接口
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeUtils {
    AndroidJavaObject InterfaceObj;

    public VolumeUtils(SvrNativeInterface svrNI)
    {
        if (svrNI != null)
            InterfaceObj = svrNI.GetUtils(UtilsType.VolumeUtils);

        LogTool.Log("创建VolumeUtils");
    }

    public int GetMaxVolume()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            return InterfaceObj.Call<int>("getMaxVolume");
        else
            return 0;
#else
        return 0;
#endif
    }

    public int GetCurrentVolume()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
        {
            int volume = InterfaceObj.Call<int>("getCurrentVolume");
            Svr.SvrLog.Log("GetCurrentVolume:"+ volume);
            return volume;
        }
            
        else
            return 0;
#else
        return 0;
#endif
    }

    public void SetVolume(int volume)
    {
        Svr.SvrLog.Log("SetVolume:"+ volume);
#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            InterfaceObj.Call("setVolume", volume);
#endif
        GetCurrentVolume();
    }

    public void SetVolumeChangedEvent(string gameObjectName, string functionName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            InterfaceObj.Call("setVolumeChangedListener", gameObjectName, functionName);
#endif
    }
}

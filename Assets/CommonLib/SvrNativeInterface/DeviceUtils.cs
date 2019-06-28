using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeviceUtils : MonoBehaviour {
    AndroidJavaObject InterfaceObj;

    public Action<bool> OnEyeProtectionChangedCallback;

    public DeviceUtils(SvrNativeInterface svrNI)
    {
        if (svrNI != null)
        {
            InterfaceObj = svrNI.GetUtils(UtilsType.DeviceUtils);
            SetOnBrightnessListener();
            LogTool.Log("创建DeviceUtils");
        }
    }

    public void SetOnBrightnessListener()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        DeviceUtilsListener listener = new DeviceUtilsListener(this);

        if (InterfaceObj != null)
            InterfaceObj.Call("setListener", listener);
#endif
    }

    public void Resume()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            InterfaceObj.Call("resume");
#endif
    }

    public void Pause()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
            InterfaceObj.Call("pause");
#endif
    }

    public void SetEyeProtectionChangedEvent(bool open)
    {
        if (OnEyeProtectionChangedCallback != null)
            OnEyeProtectionChangedCallback(open);
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    public sealed class DeviceUtilsListener : AndroidJavaProxy
    {
        private DeviceUtils oTGContent;
        public DeviceUtilsListener(DeviceUtils oTG) : base("com.ssnwt.vr.androidmanager.DeviceUtils$Listener")
        {
            oTGContent = oTG;
        }

        public void onEyeProtection(bool open)
        {
            oTGContent.SetEyeProtectionChangedEvent(open);
        }
    }
#endif
}

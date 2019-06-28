/*
 * 2018-5-29
 * 黄秋燕 Shemi
 * 亮度调节接口
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BrightnessUtils {
    AndroidJavaObject InterfaceObj;

    public Action<int> OnBrightnessChangedCallback;

    public BrightnessUtils(SvrNativeInterface svrNI)
    {
        if (svrNI != null)
        {
            InterfaceObj = svrNI.GetUtils(UtilsType.BrightnessUtils);
            SetOnBrightnessListener();
            LogTool.Log("创建BrightnessUtils");
        }
    }

    public void SetOnBrightnessListener()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        BrightnessListener listener = new BrightnessListener(this);

        if (InterfaceObj != null)
            InterfaceObj.Call("setListener", listener);
#endif
    }

    public int GetMaxBrightness()
    {
        if (InterfaceObj != null)
            return InterfaceObj.Call<int>("getMaxBrightness");
        else
            return 0;
    }

    public int GetCurrentBrightness()
    {
        if (InterfaceObj != null)
        {
            int bright = InterfaceObj.Call<int>("getCurrentBrightness");
            Svr.SvrLog.Log("getCurrentBrightness:"+ bright);
            return bright;
        }
        else
            return 0;
    }

    public void SetBrightness(int brightness)
    {
        Svr.SvrLog.Log("SetBrightness:" + brightness);
        if (InterfaceObj != null)
            InterfaceObj.Call("setBrightness", brightness);
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

    public void SetBrightnessChangedEvent(int brightness)
    {
        if (OnBrightnessChangedCallback != null)
            OnBrightnessChangedCallback(brightness);
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    public sealed class BrightnessListener : AndroidJavaProxy
    {
        private BrightnessUtils oTGContent;
        public BrightnessListener(BrightnessUtils oTG) : base("com.ssnwt.vr.androidmanager.BrightnessUtils$Listener")
        {
            oTGContent = oTG;
        }

        public void onBrightnessChanged(int brightness)
        {
            oTGContent.SetBrightnessChangedEvent(brightness);
        }
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SVR;

public class HomeButtonListener : MonoBehaviour
{
    public bool isCanBackHome = true;
    public Action HomeButtonCallback;
    public Action BackButtonCallback;

    public static HomeButtonListener Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        OnDeviceHomeButtonListener();
    }

    private void Update()
    {
//#if UNITY_EDITOR
        bool State = Input.GetKeyUp(KeyCode.Escape);
        //if (GvrControllerInput.GetControllerState(SvrControllerState.GvrController).connectionState == GvrConnectionState.Connected)
        //{
        //    State = State || GvrControllerInput.HomeButtonDown;
        //}
        //else
        //    State = false;

        if (State)
        {
            if (BackButtonCallback != null && isCanBackHome)
                BackButtonCallback();
        }
//#endif
    }

    //private void OnEnable()
    //{
    //    OnBackButtonClick += Instance_OnHomeButtonClick;
    //}

    //private void OnDisable()
    //{
    //    OnBackButtonClick -= Instance_OnHomeButtonClick;
    //}

    private bool Instance_OnHomeButtonClick()
    {
        MainThreadQueue.ExecuteQueue.Enqueue(() =>
        {
            if (HomeButtonCallback != null && isCanBackHome)
                HomeButtonCallback();
        });

        return true;
    }

    void OnDeviceHomeButtonListener()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        DeviceHomeListener listener = new DeviceHomeListener(this);
        if (jo != null)
            jo.Call("setOnDeviceHomeListener", listener);
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    public sealed class DeviceHomeListener : AndroidJavaProxy
    {
        private HomeButtonListener listener;
        public DeviceHomeListener(HomeButtonListener home) : base("com.ssnwt.vr.common.SSNWTActivity$OnDeviceHomeListener")
        {
            listener = home;
        }

        public void onHomeClick()
        {
            listener.Instance_OnHomeButtonClick();
        }
    }
#endif
}


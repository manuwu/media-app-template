/*
 * Author:李传礼
 * DateTime:2018.4.23
 * Description:原生接口
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UtilsType {
    ApkUtils, AppUtils, BatteryUtils,
    WifiUtils, VolumeUtils, BrightnessUtils,
    DateTimeUtils, WFDUtils, FotaUtils,
    BluetoothUtils, DeviceUtils
}

public class SvrNativeInterface : SingletonPure<SvrNativeInterface>
{
    public AppUtils AppUtils;
    public ApkUtils ApkUtils;
    public BatteryUtils BatteryUtils;
    public WifiUtils WifiUtils;
    public VolumeUtils VolumeUtils;
    public BrightnessUtils BrightnessUtils;
    public DateTimeUtils DateTimeUtils;
    public WFDUtils WFDUtils;
    public FotaUtils FotaUtils;
    public BluetoothUtils BluetoothUtils;
    public DeviceUtils DeviceUtils;

    bool IsInit;
    AndroidJavaObject AndroidInterface; 

    public SvrNativeInterface()
    {
        InitAndroidInterface();

        AppUtils = new AppUtils(this);
        ApkUtils = new ApkUtils(this);
        BatteryUtils = new BatteryUtils(this);
        WifiUtils = new WifiUtils(this);
        VolumeUtils = new VolumeUtils(this);
        BrightnessUtils = new BrightnessUtils(this);
        DateTimeUtils = new DateTimeUtils(this);
        WFDUtils = new WFDUtils(this);
        FotaUtils = new FotaUtils(this);
        BluetoothUtils = new BluetoothUtils(this);
        DeviceUtils = new DeviceUtils(this);
    }

    ~SvrNativeInterface()
    {
        Release();
    }

    void InitAndroidInterface()
    {
        if (IsInit)
            return;

        IsInit = true;
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass AndroidInterfaceClass = new AndroidJavaClass("com.ssnwt.vr.androidmanager.AndroidInterface");
        if (AndroidInterfaceClass == null)
            return;

        AndroidInterface = AndroidInterfaceClass.CallStatic<AndroidJavaObject>("getInstance");
        if (AndroidInterface == null)
            return;

        AndroidInterface.Call("init", GlobalAppManage.GetJApplication());
        LogTool.Log("AndroidInterface通过jApplication初始化");
#endif
    }

    void Release()
    {
        if (AndroidInterface != null)
            AndroidInterface.Call("release");
        IsInit = false;
    }

    public AndroidJavaObject GetUtils(UtilsType utilsType)
    {
        AndroidJavaObject utils = null;

        if (utilsType == UtilsType.ApkUtils)
        {
            if (AndroidInterface != null)
                utils = AndroidInterface.Call<AndroidJavaObject>("getApkUtils");
        }
        else if (utilsType == UtilsType.AppUtils)
        {
            if (AndroidInterface != null)
                utils = AndroidInterface.Call<AndroidJavaObject>("getAppUtils");
        }
        else if (utilsType == UtilsType.BatteryUtils)
        {
            if (AndroidInterface != null)
                utils = AndroidInterface.Call<AndroidJavaObject>("getBatteryUtils");
        }
        else if (utilsType == UtilsType.WifiUtils)
        {
            if (AndroidInterface != null)
                utils = AndroidInterface.Call<AndroidJavaObject>("getWifiUtils");
        }
        else if (utilsType == UtilsType.VolumeUtils)
        {
            if (AndroidInterface != null)
                utils = AndroidInterface.Call<AndroidJavaObject>("getVolumeUtils");
        }
        else if (utilsType == UtilsType.BrightnessUtils)
        {
            if (AndroidInterface != null)
                utils = AndroidInterface.Call<AndroidJavaObject>("getBrightnessUtils");
        }
        else if (utilsType == UtilsType.DateTimeUtils)
        {
            if (AndroidInterface != null)
                utils = AndroidInterface.Call<AndroidJavaObject>("getTimeUtils");
        }
        else if (utilsType == UtilsType.WFDUtils)
        {
            if (AndroidInterface != null)
                utils = AndroidInterface.Call<AndroidJavaObject>("getWFDUtils");
        }
        else if (utilsType == UtilsType.FotaUtils)
        {
            if (AndroidInterface != null)
                utils = AndroidInterface.Call<AndroidJavaObject>("getFotaUtils");
        }
        else if(utilsType == UtilsType.BluetoothUtils)
        {
            if (AndroidInterface != null)
                utils = AndroidInterface.Call<AndroidJavaObject>("getBluetoothUtils");
        }
        else if (utilsType == UtilsType.DeviceUtils)
        {
            if (AndroidInterface != null)
                utils = AndroidInterface.Call<AndroidJavaObject>("getDeviceUtils");
        }
        else
            return null;

        return utils;
    }
}

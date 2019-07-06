using UnityEngine;
using System.Collections.Generic;
using System.IO;

public enum LauncherEvent
{
    EnterSettings, EnterLibrary, LaunchRecentApp, LaunchPre_install, ClickBanner_1, ClickBanner_2, ClickBanner_3, //Home/Library
    EnterHome, ClickDelete, ClickCancelDelete, ConfirmDelete, LaunchApp, //Library
    EnterWiFi, EnterAdjust, EnterMoreSetting, EnterUpdate, EnterCast, BackHome, //SettingList
    ClickOnBrightnessBar, ClickOnSettingVolumeBar, ShiftController, Casting //Setting-Adjust
}

public enum MediaCenterEvent
{
    TabPictures, TabVideos, TabApks, SortList, ClickMore, ConfirmDeleteMedia, //MediaList
    VideoPlay, ClickOnTimeline, ClickOnVideoVolumeBar, ClickVideoSetting, ZoomInVideo, ZoomOutVideo, NextVideo, PreviousVideo, //VideoPlay
    PictureView, ZoomInPicture, ZoomOutPicture, NextPicture, PreviousPicture, SwipeNextPicture, SwipePreviousPicture, ClickPictureSetting //PictureView
}

public enum WelcomeEvent
{
    DoneHowToWare, DoneButtonGuide, DoneRecenterGuide, DoneCursorGuide, ExitGuide
}

/// <summary>
/// 将键值对(string，string)转换为Android Hasmap类型
/// </summary>
public class Common : SingletonPure<Common>
{
    // private AndroidJavaObject _map;
    // public AndroidJavaObject Map
    // {
    //     get
    //     {
    //         if (_map != null)
    //             return _map;
    //         else
    //         {
    //             _map = new AndroidJavaObject("com.ssnwt.vr.common.SSNWTMap");
    //             return _map;
    //         }
    //     }
    // }

    public void SetAttributes(string key, string value)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Map.Call("set", key, value);
#endif
    }

    public AndroidJavaObject GetAttributes()
    {
// #if UNITY_ANDROID && !UNITY_EDITOR
//         return Map.Call<AndroidJavaObject>("get");
// #else
        return null;
// #endif
    }

    public void ClearAttributes()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Map.Call("clear");
#endif
    }
}

/// <summary>
/// 百度移动统计
/// </summary>
public class Statistics : SingletonPure<Statistics> {

    private AndroidJavaClass _javaClass;
    // public AndroidJavaClass JavaClass
    // {
    //     get
    //     {
    //         if (_javaClass != null)
    //             return _javaClass;
    //         else
    //         {
    //             _javaClass = new AndroidJavaClass("com.baidu.mobstat.StatService");
    //             return _javaClass;
    //         }
    //     }
    // }

    private AndroidJavaObject _context;
    public AndroidJavaObject Context
    {
        get
        {
            if (_context != null)
                return _context;
            else
                return GetContext();
        }
    }

    AndroidJavaObject GetContext()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        _context = jo.Call<AndroidJavaObject>("getApplicationContext");

        return _context;
    }

    /// <summary>
    /// 应用第一个页面调用，会上传上一次的事件
    /// </summary>
    public void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // JavaClass.CallStatic("start", Context);
#endif
    }
    
    public void SetUserId(string userId)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // JavaClass.CallStatic("setUserId", Context, userId);
#endif
    }

    /// <summary>
    /// 仅针对测试，正式发布需要关闭，SetDebugOn(false)或者不使用
    /// </summary>
    /// <param name="debug"></param>
    public void SetDebugOn(bool debug)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // JavaClass.CallStatic("setDebugOn", debug);
#endif
    }

    /// <summary>
    /// 仅针对测试，获得设备id，web端添加测试机
    /// </summary>
    public void GetTestDeviceId()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // string filePath = GlobalVariable.SdcardPath + "/Alarms/DeviceId.log";
        // if (File.Exists(filePath))
        //     return;

        // string testDeviceId = JavaClass.CallStatic<string>("getTestDeviceId", Context);
        // string sdkVersion = JavaClass.CallStatic<string>("getSdkVersion");
        // string appKey = JavaClass.CallStatic<string>("getAppKey", Context);

        // string str = "{\n\tDeviceId:" + testDeviceId + ", \n\tVersion:" + sdkVersion + ", \n\tAppKey:" + appKey + "\n}";

        // byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
        // Stream sw = File.Create(filePath);
        // sw.Write(buffer, 0, buffer.Length);
        // sw.Close();
#endif
    }
    
    /// <summary>
    /// 埋点事件记录
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="eventLabel">Event Name</param>
    /// <param name="acc">计数，default 1，可以不传</param>
    /// <param name="attributes">参数键值对</param>
    public void OnEvent(object eventId, string eventLabel, int acc = 1, AndroidJavaObject attributes = null)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // JavaClass.CallStatic("onEvent", Context, eventId.ToString(), eventLabel, acc, attributes);
#endif
    }
}

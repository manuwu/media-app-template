using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 这是所有模块的主入口
/// Created by kevin.xie on 2018/5/24 0024.
/// </summary>

public class MainCommon
{
    public static event Action<int> OnNewIntentEvent;
    private static AndroidJavaObject mMainCommon;
    public static List<string> APPLIST = new List<string>();
    static MainCommon()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        mMainCommon = new AndroidJavaObject("skyworth.com.commonlib.MainCommon");
        CurrentActivity.Call("SetNewIntentInferface",new NewIntentInferface());
#endif
        APPLIST.Add("com.htc.viveport.store");
        APPLIST.Add("com.ssnwt.svrwelcom");

    }
    public sealed class NewIntentInferface : AndroidJavaProxy
    {
        public NewIntentInferface() : base("com.ssnwt.vr.common.BaseActivity$NewIntentInferface")
        {
        }
        public void OnNewIntent(int type)
        {
            MainThreadQueue.ExecuteQueue.Enqueue(()=> 
            {
                if (OnNewIntentEvent != null) OnNewIntentEvent(type);
            });
            
        }
    }
    public static AndroidJavaObject CurrentActivity
    {
        get
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return javaClass.GetStatic<AndroidJavaObject>("currentActivity");
#else
            return null;
#endif
        }
    }
    public static string LastName
    {
        get
        {
#if UNITY_EDITOR
            return null;
#else
            AndroidJavaObject lastName = CurrentActivity.Get<AndroidJavaObject>("mLastName");
            if (lastName != null)
            {
                return lastName.Call<string>("getPackageName");
            }
            else
            {
                return null;
            }
#endif
        }
    }
    public static void RunWelCome()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CurrentActivity.Call("RunWelCome");
#endif
    }
    public static bool HandlePairied
    {
        get
        {
#if UNITY_EDITOR
            return false;
#else
            return CurrentActivity.Call<bool>("HandlePairied");
#endif
        }
    }

    public static void setmGetKeyFlage(bool flage)
    {
#if UNITY_EDITOR
#else
        CurrentActivity.Call("setmGetKeyFlage", flage);
#endif
    }
    public static void StartSearch()
    {
#if UNITY_EDITOR
#else
        CurrentActivity.Call("sendHandle",2);
#endif
    }
    public static void StopSearch()
    {
#if UNITY_EDITOR
#else
        CurrentActivity.Call("sendHandle", 3);
#endif
    }
    public static void DisConnect()
    {

#if UNITY_EDITOR
#else
        CurrentActivity.Call("sendHandle", 1);
#endif
    }
    public static int GetKeyEvent()
    {
#if UNITY_EDITOR
        return -1;
#else
        return CurrentActivity.Call<int>("GetKeyEvent");
#endif
    }
    public static void CleanTop()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CurrentActivity.Call("CleanTop");
#endif
    }
    public static void Reboot()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CurrentActivity.Call("Reboot");
#endif
    }
    public static void ResetAotuRun()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CurrentActivity.Call("ResetAotuRun");
#endif
    }
    public static void StartHome()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CurrentActivity.Call("StartHome");
#endif
    }
    public static void ReturnToDefaultHome()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CurrentActivity.Call("ReturnToDefaultHome");
#endif
    }
    public static void CleanCache()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CurrentActivity.Call("CleanCache");
#endif
    }
    public static void StartFileManager()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CurrentActivity.Call("StartFileManager");
#endif
    }
    public static void ShutDown()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CurrentActivity.Call("ShutDown");
#endif
    }
    public static string GetAotuPackage()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return CurrentActivity.Call<string>("GetAotuPackage");
#endif
        return "";
    }
    public static void SetAotuRunApp(string package,string classname)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CurrentActivity.Call("SetAotuRunApp", package, classname);
#endif
    }
    private static AndroidJavaObject getVivportConnect()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return mMainCommon.Call<AndroidJavaObject>("getVivportConnect");
#endif
        return null;
    }
    private static AndroidJavaObject getOTGContent()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return mMainCommon.Call<AndroidJavaObject>("getOTGContent");
#endif
        return null;
    }
    private static AndroidJavaObject getLauncherApp()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return mMainCommon.Call<AndroidJavaObject>("getLauncherApp");
#endif
        return null;
    }

    private static LauncherApp launcherApp;


    public static bool IsVivportConnect { get; set; }

    public static LauncherApp LauncherApp
    {
        get
        {
            if (launcherApp == null)
                launcherApp = new LauncherApp(getLauncherApp());
            return launcherApp;
        }
    }
}

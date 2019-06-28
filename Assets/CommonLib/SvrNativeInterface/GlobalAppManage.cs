using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AppProcessInfo
{
    public string packageName;
    public string className;
    public AppProcessInfo()
    {
        this.packageName = "";
        this.className = "";
    }
}

public class GlobalAppManage {

    static bool IsInitCurActivity = false;
    static bool IsInitIntentObject = false;
    static AndroidJavaObject CurActivity;
    static AndroidJavaObject JApplication;
    static AndroidJavaObject IntentObject;//BackHome使用
    static AndroidJavaObject InstanceObject;

    static void InitCurrentActivity()
    {
        IsInitCurActivity = true;

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        if (unityPlayer != null)
            CurActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
#endif
    }

    static void InitIntentObject()
    {
        IsInitIntentObject = true;

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");//Reference of AndroidJavaClass class for intent
        IntentObject = new AndroidJavaObject("android.content.Intent");//Reference of AndroidJavaObject class for intent

        //call setAction method of the Intent object created
        IntentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_MAIN"));
        IntentObject.Call<AndroidJavaObject>("addCategory", intentClass.GetStatic<string>("CATEGORY_HOME"));
        IntentObject.Call<AndroidJavaObject>("setFlags", intentClass.GetStatic<int>("FLAG_ACTIVITY_NEW_TASK"));
#endif
    }

    public static AndroidJavaObject GetJApplication()//修改为静态函数用于测试
    {
        if (!IsInitCurActivity)
            InitCurrentActivity();

#if UNITY_ANDROID && !UNITY_EDITOR
        if (JApplication != null)
            return JApplication;
        else
        {
            if (CurActivity != null)
            {
                JApplication = CurActivity.Call<AndroidJavaObject>("getApplication");
                return JApplication;
            }
            else
                return null;
        }
#else
        return null;
#endif
    }

    /// <summary>
    /// 结束当前app
    /// </summary>
    public static void FinishSelfApp()
    {
        if (!IsInitCurActivity)
            InitCurrentActivity();

#if UNITY_ANDROID && !UNITY_EDITOR
        if (CurActivity != null)
            CurActivity.Call("finish");
#endif
    }

    /// <summary>
    /// 结束当前app
    /// </summary>
    public static void StopApp()
    {
        if (!IsInitCurActivity)
            InitCurrentActivity();

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass AppTaskManager = new AndroidJavaClass("com.ssnwt.vr.common.AppTaskManager");
        if (AppTaskManager == null)
            return;

        AndroidJavaObject AndroidInterface = AppTaskManager.CallStatic<AndroidJavaObject>("getInstance");
        if (AndroidInterface == null)
            return;

        AndroidInterface.Call("stopApp", CurActivity);
        LogTool.Log("AndroidInterface通过jApplication初始化, stopApp");
#endif
    }

    /// <summary>
    /// 重启当前app
    /// </summary>
    public static void RestartApp()
    {
        if (!IsInitCurActivity)
            InitCurrentActivity();

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass AppTaskManager = new AndroidJavaClass("com.ssnwt.vr.common.AppTaskManager");
        if (AppTaskManager == null)
            return;

        AndroidJavaObject AndroidInterface = AppTaskManager.CallStatic<AndroidJavaObject>("getInstance");
        if (AndroidInterface == null)
            return;

        AndroidInterface.Call("restartApp", CurActivity);
        LogTool.Log("AndroidInterface通过jApplication初始化, RestartApp");
#endif
    }

    /// <summary>
    /// 获得堆栈中app数量
    /// </summary>
    /// <returns></returns>
    public static int GetTaskSize()
    {
        if (!IsInitCurActivity)
            InitCurrentActivity();

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass AppTaskManager = new AndroidJavaClass("com.ssnwt.vr.common.AppTaskManager");
        if (AppTaskManager == null)
            return 0;

        AndroidJavaObject AndroidInterface = AppTaskManager.CallStatic<AndroidJavaObject>("getInstance");
        if (AndroidInterface == null)
            return 0;

        int taskSize = AndroidInterface.Call<int>("getTaskSize", CurActivity);
        LogTool.Log("AndroidInterface通过jApplication初始化, GetTaskSize");
        return taskSize;
#else
        return 0;
#endif
    }

    /// <summary>
    /// 从堆栈打开排序第二位的app，即上一个app
    /// 堆栈中app数量需要 >= 2
    /// </summary>
    /// <returns>打开成功或失败</returns>
    public static bool StartLastApp()
    {
        if (!IsInitCurActivity)
            InitCurrentActivity();

        bool isSuccess = false;
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass AppTaskManager = new AndroidJavaClass("com.ssnwt.vr.common.AppTaskManager");
        if (AppTaskManager == null)
            return false;

        AndroidJavaObject AndroidInterface = AppTaskManager.CallStatic<AndroidJavaObject>("getInstance");
        if (AndroidInterface == null)
            return false;

       isSuccess = AndroidInterface.Call<bool>("startLastApp", CurActivity);
        LogTool.Log("AndroidInterface通过jApplication初始化, StartLastApp");
#endif
        return isSuccess;
    }

    /// <summary>
    /// 返回launcher
    /// </summary>
    public static void BackHome()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!IsInitCurActivity)
            InitCurrentActivity();

        if (!IsInitIntentObject)
            InitIntentObject();

        if(CurActivity != null && IntentObject != null)
        {  
            LogTool.Log("BackHome");
            CurActivity.Call ("startActivity", IntentObject);
        }
#endif
    }

    static AppProcessInfo getAPPInfo(AndroidJavaObject infoObj)
    {
        AppProcessInfo appInfo = new AppProcessInfo()
        {
            packageName = infoObj.Call<string>("getPackageName"),
            className = infoObj.Call<string>("getClassName"),
        };
        return appInfo;
    }

    private static AndroidJavaObject getInstance()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        if (InstanceObject==null)
        {
            AndroidJavaClass AppTaskManager = new AndroidJavaClass("com.ssnwt.vr.common.AppTaskManager");
            if (AppTaskManager == null)
                return null;
            InstanceObject = AppTaskManager.CallStatic<AndroidJavaObject>("getInstance");
        }
#endif
        return InstanceObject;
    }

    public static List<AppProcessInfo> GetAllRunningProcess()
    {
        List<AppProcessInfo> appInfoList = new List<AppProcessInfo>();
        if (getInstance() != null)
        {
            AndroidJavaObject infos = getInstance().Call<AndroidJavaObject>("getRunningProcess", GetJApplication());
            int count = infos.Call<int>("size");
            for (int i = 0; i < count; i++)
            {
                AppProcessInfo appInfo = new AppProcessInfo();
                if (infos != null)
                {
                    appInfo = getAPPInfo(infos.Call<AndroidJavaObject>("get", i));
                    Debug.Log("getAPPInfo = " + appInfo.packageName + ", " + appInfo.className);
                    appInfoList.Add(appInfo);
                }
            }
        }
        return appInfoList;
    }

    public static void KillProcess(string pkg)
    {
        if (getInstance() != null)
            getInstance().Call("killProcess",GetJApplication(),pkg);
    }

    public static void ClearMemory()
    {
        if (getInstance() != null)
            getInstance().Call("clearMemory", GetJApplication());
    }

}

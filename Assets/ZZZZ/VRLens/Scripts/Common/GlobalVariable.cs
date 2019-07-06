/*
 * Author:李传礼 / 黄秋燕 Shemi
 * DateTime:2017.11.20
 * Description:全局变量
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public enum SortType { Latest, Earliest, Name, Size, Time, Id }
public enum MediaType { Video, Picture, Audio, Apk, App, Edit }
public enum StereoType
{
    ST2D, ST3D_LR, ST3D_TB,
    ST180_2D, ST180_LR, ST180_TB,
    ST360_2D, ST360_LR, ST360_TB
}

public enum ScanResultType { All, Add, Delete }

public class GlobalVariable
{
    #region 动态库接口
#if UNITY_ANDROID
    private const string dllName = "svr_plugin_media_scan";
    [DllImport(dllName)]
    private static extern bool Delete(int mediaType, string path, bool allowCB);
#endif
    #endregion

    #region 用户信息
    public static string UserNameKey = "GLOBAL_USER_INFOS_NAME";
    public static string UserCollectionVideosKey = "GLOBAL_USER_INFOS_COLLECTION_VIDEOS";
    public static string UserHistoricalRecordKey = "GLOBAL_USER_INFOS_HISTORICAL_RECORDS";
    #endregion

    #region Others
    public static string SdcardPath = "/storage/emulated/0";
    public static string LocalPath = Application.persistentDataPath;
    public static float AnimationSpendTime = 0.15f;

    public static void SetIntentListener(string objName, string fucName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (unityPlayer != null)
            {
                AndroidJavaObject CurActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                if (CurActivity != null)
                    CurActivity.Call("setOnNewIntentListener", objName, fucName);
            }
        }
        catch (Exception ee)
        {
            Debug.LogError("SetIntentListener exception:" + ee);
        }
#endif
    }

    /// <summary>
    /// 启动另一个app，当前app进入后台时才回发送回调
    /// </summary>
    /// <param name="objName"></param>
    /// <param name="fucName"></param>
    public static void SetEnterBackgroundListener(string objName, string fucName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (unityPlayer != null)
            {
                AndroidJavaObject CurActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                if (CurActivity != null)
                    CurActivity.Call("setEnterBackgroundListener", objName, fucName);
            }
        }
        catch(Exception ee)
        {
            Debug.LogError("setEnterBackgroundListener exception:" + ee);
        }
#endif
    }

    public static bool GetIsScreenOn()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (unityPlayer != null)
            {
                AndroidJavaObject CurActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                if (CurActivity != null)
                    return CurActivity.Call<bool>("isScreenOn");
                else
                    return false;
            }
            else return false;
        }
        catch(Exception ee)
        {
            Debug.LogError("GetIsScreenOn exception:" + ee);
            return false;
        }
#else
        return true;
#endif
    }

    public static string MBToShowGBStr(float BValue)
    {
        float KBValue = BValue / 1024;
        if (KBValue <= 1)
        {
            if (IsPositiveInteger(BValue))
                return string.Format("{0} B", BValue);
            else
                return string.Format("{0:0.00} B", BValue);
        }
        float MBValue = KBValue / 1024;
        if(MBValue <= 1)
        {
            if (IsPositiveInteger(KBValue))
                return string.Format("{0} KB", KBValue);
            else
                return string.Format("{0:0.00} KB", KBValue);
        }
        else if(1 < MBValue && MBValue <= 1024)
        {
            if (IsPositiveInteger(MBValue))
                return string.Format("{0} MB", MBValue);
            else
                return string.Format("{0:0.00} MB", MBValue);
        }
        else
        {
            float GBValue = MBValue / 1024;
            GBValue = Mathf.Clamp(GBValue, 0.1f, 99);
            if (IsPositiveInteger(GBValue))
                return string.Format("{0} GB", GBValue);
            else
                return string.Format("{0:#0.00} GB", GBValue);
        }
    }

    /// <summary>
    /// 是否为整数
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    private static bool IsPositiveInteger(float m)
    {
        if ((m - (int)m) > 0 || (m - (int)m) < 0)
            return false;
        else
            return true;
    }

    public static bool TimeCompare(string leftTime, string rightTime, out int result)
    {
        result = 0;
        DateTime time1;
        if (!DateTime.TryParse(leftTime, out time1))
            return false;
        DateTime time2;
        if (!DateTime.TryParse(rightTime, out time2))
            return false;

        result = time1.CompareTo(time2);
        return true;
    }

    /// <summary>
    /// 计算字符串在指定text控件中的长度
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static int CalculateLengthOfText(string message, Text tex)
    {
        int totalLength = 0;
        Font myFont = tex.font;
        myFont.RequestCharactersInTexture(message, tex.fontSize, tex.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();
        char[] arr = message.ToCharArray();
        foreach (char c in arr)
        {
            myFont.GetCharacterInfo(c, out characterInfo, tex.fontSize);
            totalLength += characterInfo.advance;
        }

        return totalLength;
    }

    /// <summary>
    /// 网络是否可用
    /// </summary>
    /// <returns>true:可用;false:不可用</returns>
    public static bool IsInternetReachability()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                return false;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                return true;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                return true;
            default:
                return false;
        }
    }
    #endregion

    #region Media
    public static int ApkPagePerIconCount = 15; //Apk页面默认个数
    public static int AppPagePerIconCount = 15; //App页面默认个数
    public static int VideoPagePerIconCount = 9; //Video页面默认个数
    public static int ImagePagePerIconCount = 9; //Image页面默认个数
    public static int HistoricalPagePerIconCount = 6; //历史记录页面默认个数
    public static int LiveAppPagePerIconCount = 9; //在线商店的在线app个数
    public static int LiveVideoPagePerIconCount = 12; //在线视频的在线视频个数

    /// <summary>
    /// 全局模式选择器
    /// </summary>
    public static MediaType CurMediaType = MediaType.Video; //默认开启Video选择

    public static string SCENE_MODEL_KEY = "SCENE_MODEL_KEY";
    public static string SCREEN_SIZE_TYPE_KEY = "SCREEN_SIZE_TYPE_KEY";
    public static string SCENE_DRIVE_MODEL_KEY = "SCENE_DRIVE_MODEL_KEY";

    public static string StereoTypeToShowStr(StereoType stereoType)
    {
        string showStr;
        switch (stereoType)
        {
            case StereoType.ST2D:
                showStr = "2D";
                break;
            case StereoType.ST3D_LR:
                showStr = "3DLR";
                break;
            case StereoType.ST3D_TB:
                showStr = "3DTB";
                break;
            case StereoType.ST180_2D:
                showStr = "180°2D";
                break;
            case StereoType.ST180_LR:
                showStr = "180°LR";
                break;
            case StereoType.ST180_TB:
                showStr = "180°TB";
                break;
            case StereoType.ST360_2D:
                showStr = "360°2D";
                break;
            case StereoType.ST360_LR:
                showStr = "360°LR";
                break;
            case StereoType.ST360_TB:
                showStr = "360°TB";
                break;
            default:
                showStr = "Unknow";
                break;
        }

        return showStr;
    }

    public static bool List_IsEditMode = false;

    /// <summary>
    /// 删除单个文件，没有回调函数，默认成功
    /// </summary>
    /// <param name="type">媒体类型</param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool DeleteFileWithPath(MediaType type, string path)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (path == null || path == string.Empty)
            return false;
        string pathJson = "{\"array\":[\"" + path + "\"]}";
        bool isSuccess = Delete((int)type, pathJson, false);

        return isSuccess;
#else
        return false;
#endif
    }

    /// <summary>
    /// 删除多个文件，传入路径数组，需要回调函数返回删除结果
    /// </summary>
    /// <param name="type"></param>
    /// <param name="path">路径数组</param>
    public static bool DeleteFilesWithPaths(MediaType type, string[] path)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        string pathArr = JsonArray<string>.ConvertToJson(path);
        if (pathArr == null || pathArr == string.Empty)
            return false;
        string pathJson = "{\"array\":" + pathArr + "}";
        if (pathJson == "{\"array\":[]}")
            return false;
        LogTool.Log("pathJson = " + pathJson);
        bool isSuccess = Delete((int)type, pathJson, true);
        return isSuccess;
#else
        return false;
#endif
    }

    public static void SetSceneModel(SceneModel model)
    {
        //if (CurMediaType == MediaType.Video)
            PlayerPrefs.SetInt(SCENE_MODEL_KEY, (int)model);
    }

    public static SceneModel GetSceneModel()
    {
        //if (CurMediaType == MediaType.Video)
        //{
            if (PlayerPrefs.HasKey(SCENE_MODEL_KEY))
                return (SceneModel)PlayerPrefs.GetInt(SCENE_MODEL_KEY);
            else
            {
                SetSceneModel(SceneModel.IMAXTheater);
                return SceneModel.IMAXTheater;
            }
        //}
        //else
        //    return SceneModel.IMAXTheater;
    }

    public static void SetDriveSceneModel(DriveSceneModel model)
    {
        PlayerPrefs.SetInt(SCENE_DRIVE_MODEL_KEY, (int)model);
    }

    public static DriveSceneModel GetDriveSceneModel()
    {
        if (PlayerPrefs.HasKey(SCENE_DRIVE_MODEL_KEY))
            return (DriveSceneModel)PlayerPrefs.GetInt(SCENE_DRIVE_MODEL_KEY);
        else
        {
            SetDriveSceneModel(DriveSceneModel.Playboy);
            return DriveSceneModel.Playboy;
        }
    }

    public static void SetScreenSizeType(ScreenSizeType type)
    {
        PlayerPrefs.SetInt(SCREEN_SIZE_TYPE_KEY, (int)type);
    }

    public static ScreenSizeType GetScreenSizeType()
    {
        if (PlayerPrefs.HasKey(SCREEN_SIZE_TYPE_KEY))
            return (ScreenSizeType)PlayerPrefs.GetInt(SCREEN_SIZE_TYPE_KEY);
        else
        {
            SetScreenSizeType(ScreenSizeType.Cinema);
            return ScreenSizeType.Cinema;
        }
    }


    //    public static void ChooseVideo(LiveVideoIcon apkIcon)
    //    {
    //        LiveVideoInfo live = apkIcon.GetVideoInfoByThisVideoIcon();
    //        if (live.sourceType == 1 || live.sourceType == 2)
    //        {
    //            PlayerDataControl.GetInstance().SetVideoVidCid(live.vid, live.cid, live.VideoName, (int)live.StereoType);
    //        }
    //        else if (live.sourceType == 0)
    //        {
    //            JVideoDescriptionInfo jVideo = new JVideoDescriptionInfo(-1, live.VideoName, live.Uri, live.Uri, 0, 0, (int)live.StereoType, 0, 0, System.DateTime.Now, null, null);
    //            PlayerDataControl.GetInstance().SetJVideoDscpInfoByLiveUrl(jVideo);
    //        }
    //#if UNITY_ANDROID && !UNITY_EDITOR
    //        if (Svr.SvrSetting.IsVR9Device)
    //            SceneLoaderAsync.GetInstance().LoadScene("Cinema_Environment_VR9");
    //        else
    //            SceneLoaderAsync.GetInstance().LoadScene("Cinema_Environment_Master");
    //#else
    //        SceneLoaderAsync.GetInstance().LoadScene("Cinema_Environment_VR9");
    //#endif
    //    }
    #endregion

    #region UpperPageControl
    public static bool IsHomeButtonBackLauncher = false;
    //public static List<LauncherPage> LastLauncherPage = new List<LauncherPage>();
    //public static LauncherPage CurLauncherPage = LauncherPage.Home;
    public static int UpperLiveVideoPage_CategoryKey = -1;
    public static int UpperLiveVideoPage_CurrentPageIndex = 0;

    //public static void AddLastLauncherPage(LauncherPage page)
    //{
    //    if (LastLauncherPage.Count <= 0 || (LastLauncherPage.Count > 0 && LastLauncherPage[LastLauncherPage.Count - 1] != page))
    //        LastLauncherPage.Add(page);
    //}
    #endregion
}

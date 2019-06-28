using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class LogTool : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private const string dllName = "unity_log";
    [DllImport(dllName)]
    private static extern void LOGD(string log);
    [DllImport(dllName)]
    private static extern void LOGE(string error);
    [DllImport(dllName)]
    private static extern void LOGD_TAG(string tag, string log);
    [DllImport(dllName)]
    private static extern void LOGE_TAG(string tag, string error);

#endif

    public static void Log(string str)
    {
        Debug.Log(str);
#if UNITY_ANDROID && !UNITY_EDITOR
        LOGD(str);
#endif
    }

    public static void Log(string tag, string log)
    {
        Debug.Log(log);
#if UNITY_ANDROID && !UNITY_EDITOR
        LOGD_TAG(tag, log);
#endif
    }

    public static void Error(string str)
    {
        Debug.LogError(str);
#if UNITY_ANDROID && !UNITY_EDITOR
        LOGE(str);
#endif
    }

    public static void Error(string tag, string log)
    {
        Debug.LogError(log);
#if UNITY_ANDROID && !UNITY_EDITOR
        LOGE_TAG(tag, log);
#endif
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class LauncherApp
{
    private AndroidJavaObject mLauncherApp;
    public LauncherApp(AndroidJavaObject launcherapp)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        mLauncherApp = launcherapp;
#endif
    }

    public  void StartAppByPackage(string pkname)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        mLauncherApp.Call("StartAppByPackage", pkname);
#endif
    }

    public  string[] GetRecentlyApps()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        string strs = mLauncherApp.Call<string>("GetRecentlyApps");
        Debug.Log("GetRecentlyApps:"+strs);
        string[] pknames = strs.Split(',');
        return pknames;
#endif
        return new string[0];
    }

    public  void WhenUnInstall(string pkname)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        mLauncherApp.Call("WhenUnInstall", pkname);
#endif
    }

    public SystemLanguage GetLanguage()
    {
        SystemLanguage systemLanguage = SystemLanguage.Chinese;
#if UNITY_ANDROID && !UNITY_EDITOR
        systemLanguage =  (SystemLanguage)mLauncherApp.Call<int>("GetLanguage");
#endif
        return systemLanguage;
    }
}


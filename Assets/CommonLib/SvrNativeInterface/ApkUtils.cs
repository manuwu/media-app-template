/*
 * Author:李传礼
 * DateTime:2018.4.23
 * Description:Apk工具
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApkUtils
{
    AndroidJavaObject InterfaceObj;

    public ApkUtils(SvrNativeInterface svrNI)
    {
        if(svrNI != null)
            InterfaceObj = svrNI.GetUtils(UtilsType.ApkUtils);

        LogTool.Log("创建ApkUtils");
    }

    public void SetOnOperateEvent(string gameObjectName, string functionName)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("setPackageListener", gameObjectName, functionName);
    }

    public string GetPackageNameByPath(string path)
    {
        if (InterfaceObj != null)
            return InterfaceObj.Call<string>("getPackageName", path);
        else
            return null;
    }

    public string GetAppNameByPath(string path)
    {
        if (InterfaceObj != null)
            return InterfaceObj.Call<string>("getAppName", path);
        else
            return null;
    }

    public void InstallApk(string apkPath)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("installApk", apkPath);
    }

    public void UninstallApk(string packageName)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("uninstallApk", packageName);
    }
}

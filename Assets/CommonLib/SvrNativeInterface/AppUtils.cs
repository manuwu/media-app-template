/*
 * Author:李传礼
 * DateTime:2018.4.23
 * Description:App工具
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppUtils
{
    AndroidJavaObject InterfaceObj;

    public AppUtils(SvrNativeInterface svrNI)
    {
        if (svrNI != null)
            InterfaceObj = svrNI.GetUtils(UtilsType.AppUtils);

        LogTool.Log("创建AppUtils");
    }

    public void SetOnComputeAppCountEvent(string gameObjectName, string functionName)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("setAppCountListener", gameObjectName, functionName);
    }

    public void ScanApps()
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("scanApps");
    }

    public void StartApp(string packageName)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("startApp", packageName);
    }

    /// <summary>
    /// 默认方式启动app
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="className"></param>
    public void StartApp(string packageName, string className)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("startApp", packageName, className);
    }

    /// <summary>
    /// 发送Intent方式打开app
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="className"></param>
    /// <param name="uri">只能传送地址，Intent接收方判断条件-PATH，可以为空，为空时判断jsonInfo是否为空</param>
    /// <param name="type">媒体类型，Intent接收方判断条件-FileType</param>
    /// <param name="title">媒体名称，可以为空。title为空时，Intent接收方根据uri截取名称</param>
    public void StartApp(string packageName, string className, string uri, string type, string title)
    {
        StartApp(packageName, className, uri, type, title, null);
        //if (InterfaceObj != null)
        //    InterfaceObj.Call("startApp", packageName, className, uri, type, title);
    }

    /// <summary>
    /// 发送Intent方式打开app
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="className"></param>
    /// <param name="uri">只能传送地址，Intent接收方判断条件-PATH，可以为空，为空时判断jsonInfo是否为空</param>
    /// <param name="type">媒体类型，Intent接收方判断条件-FileType</param>
    /// <param name="title">媒体名称，可以为空。title为空时，Intent接收方根据uri截取名称</param>
    /// <param name="jsonInfo">完整的媒体json数据</param>
    public void StartApp(string packageName, string className, string uri, string type, string title, string jsonInfo)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("startApp", packageName, className, type, title, uri, jsonInfo);
    }

    public byte[] GetIcon(string packageName, string className)
    {
        byte[] data = null;

        if (InterfaceObj != null)
            data = InterfaceObj.Call<byte[]>("getIcon", packageName, className);

        return data;
    }

    public string GetAppInfos(int index, int count)
    {
        string appInfos = "";

        if (InterfaceObj != null)
            appInfos = InterfaceObj.Call<string>("getAppInfos", index, count);

        return appInfos;
    }

    /// <summary>
    /// 判断app是否安装
    /// </summary>
    /// <param name="pkg"></param>
    /// <returns></returns>
    public bool CheckAppExist(string pkg)
    {
        bool isExist = false;

        if (InterfaceObj != null)
            isExist = InterfaceObj.Call<bool>("checkAppExist", pkg);

        return isExist;
    }

    /// <summary>
    /// 判断app是否是系统级应用
    /// </summary>
    /// <param name="pkg"></param>
    /// <returns></returns>
    public bool IsSystemApp(string pkg)
    {
        bool isExist = false;

        if (InterfaceObj != null)
            isExist = InterfaceObj.Call<bool>("isSystemApp", pkg);

        return isExist;
    }

    public int GetVersionCode(string pkg)
    {
        int versionCode = -1;
        if (InterfaceObj != null)
            versionCode = InterfaceObj.Call<int>("getVersionCode", pkg);

        return versionCode;
    }
}

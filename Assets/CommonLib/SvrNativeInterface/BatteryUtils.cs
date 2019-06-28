/*
 * Author:李传礼
 * DateTime:2018.4.23
 * Description:电池工具
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryUtils
{
    AndroidJavaObject InterfaceObj;

    public BatteryUtils(SvrNativeInterface svrNI)
    {
        if (svrNI != null)
            InterfaceObj = svrNI.GetUtils(UtilsType.BatteryUtils);

        LogTool.Log("创建BatteryUtils");
    }

    public int GetCurrentBattery()
    {
        int batteryValue = 0;

        if (InterfaceObj != null)
            batteryValue = InterfaceObj.Call<int>("getCurrentBattery");

        return batteryValue;
    }

    public int GetMaxBattery()
    {
        int batteryValue = 0;

        if (InterfaceObj != null)
            batteryValue = InterfaceObj.Call<int>("getMaxBattery");

        return batteryValue;
    }

    public int GetBatteryStatus()
    {
        int batteryStatus = 0;

        if (InterfaceObj != null)
            batteryStatus = InterfaceObj.Call<int>("getBatteryStatus");

        return batteryStatus;
    }
}

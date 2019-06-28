/*
 * 获得系统时间
 * 黄秋燕 Shemi
 * 2018-7-27
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DateTimeUtils : MonoBehaviour {
    AndroidJavaObject InterfaceObj;

    public DateTimeUtils(SvrNativeInterface svrNI)
    {
        if (svrNI != null)
            InterfaceObj = svrNI.GetUtils(UtilsType.DateTimeUtils);

        LogTool.Log("创建DateTimeUtils");
    }

    public DateTime GetCurrentDateTime()
    {
        if (InterfaceObj != null)
        {
            string timeValue = InterfaceObj.Call<string>("getCurrTime");
            return DateTime.Parse(timeValue);
        }
        else
            return DateTime.Now;
    }
}

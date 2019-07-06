using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utils  {
    /// <summary>
    /// 多余字符省略号处理
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
    public static string SetTextWithEllipsis( string txt,int startIndex,int endIndex)
    {
        if (txt.Length < endIndex)
            return txt;
        txt = txt.Substring(startIndex, endIndex);
        return string.Format("{0}...",txt);
    }

    /// <summary>
    /// 在sourse获取startstr 和endstr中间的字符串
    /// </summary>
    /// <param name="sourse"></param>
    /// <param name="startstr"></param>
    /// <param name="endstr"></param>
    /// <returns></returns>
    public static string MidStrEx(string sourse, string startstr, string endstr)
    {
        string result = string.Empty;
        int startindex, endindex;
        try
        {
            startindex = sourse.IndexOf(startstr);
            if (startindex == -1)
                return result;
            string tmpstr = sourse.Substring(startindex + startstr.Length);
            endindex = tmpstr.IndexOf(endstr);
            if (endindex == -1)
                return result;
            result = tmpstr.Remove(endindex);
        }
        catch (Exception ex)
        {

        }
        return result;
    }

    /// <summary>
    /// 时间戳Timestamp转换成日期   今天  昨天
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public static string SetTimeType(string timeStamp,bool isNeedAllTime=false)
    {

        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(string.Format("{0}0000000", timeStamp));
        TimeSpan toNow = new TimeSpan(lTime);
        DateTime targetDt = dtStart.Add(toNow);
        if (isNeedAllTime)
            return dtStart.Add(toNow).ToString("yyyy年MM月dd日 HH:mm");
        else
        {
            long ms = GetMilliseconds(DateTime.Now)-GetMilliseconds(dtStart.Add(toNow));
            if (ms <= 86400000)
                return dtStart.Add(toNow).ToString("HH:mm");
            else if (ms > 86400000 && ms < 172800000)
                return "昨天";
            else
                return dtStart.Add(toNow).ToString("MM月dd日");
        }
    }

    /// <summary>
    /// 时间转换为毫秒
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static long GetMilliseconds(DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (long)(time- startTime).TotalMilliseconds;
    }

    /// <summary>
    /// 获取当前本地时间戳
    /// </summary>
    /// <returns></returns>      
    public static long GetCurrentTimeUnix()
    {
        TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
        long t = (long)cha.TotalSeconds;
        return t;
    }

    /// <summary>
    /// 首行缩进2字符
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static string IndentString(string msg)
    {
        return string.Format("\u3000\u3000{0}", msg);
    }

    /// <summary>
    /// 网络可用
    /// </summary>
    public static bool NetAvailable
    {
        get
        {
            return Application.internetReachability == NetworkReachability.NotReachable;
        }
    }
}

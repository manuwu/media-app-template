/*
 * 2018-4-11
 * 黄秋燕 Shemi
 * 视频格式本地持久化存储管理
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoFormatDictionaryDetector : SingletonPure<VideoFormatDictionaryDetector>
{
    private static string PLAYERKEYWORD = "VIDEO_FORMATKEY_DICTIONARY";
    Dictionary<string, int> VideoFormatDic = new Dictionary<string, int>();
    bool isInint = false;

    public void GetVideoFormatTypeDic()
    {
        if (isInint) return;
        isInint = true;

        if (PlayerPrefs.HasKey(PLAYERKEYWORD) &&
            PlayerPrefs.GetString(PLAYERKEYWORD) != null && PlayerPrefs.GetString(PLAYERKEYWORD) != "")
        {
            string videoFormatDicString = PlayerPrefs.GetString(PLAYERKEYWORD);
            VideoFormatDic = StringToDictionary(videoFormatDicString);
        }
        else if (!PlayerPrefs.HasKey(PLAYERKEYWORD) && VideoFormatDic == null)
            VideoFormatDic = new Dictionary<string, int>();
    }

    // when app quit and pause
    public void SaveVideoFormatTypeDic()
    {
        GetVideoFormatTypeDic();
        if (VideoFormatDic.Count == 0)
            return;

        string videoFormatDicString = DictionaryListToString(VideoFormatDic);

        if (videoFormatDicString != "" && videoFormatDicString != null)
            PlayerPrefs.SetString(PLAYERKEYWORD, videoFormatDicString);
    }

    public bool HasVideoFormatOrNotByVideoId(string videoId)
    {
        GetVideoFormatTypeDic();
        if (VideoFormatDic.ContainsKey(videoId))
            return true;
        else
            return false;
    }

    public void SetVideoFormatTypeByVideoId(string videoId, int format)
    {
        GetVideoFormatTypeDic();
        if (HasVideoFormatOrNotByVideoId(videoId))
            VideoFormatDic[videoId] = format;
        else
            VideoFormatDic.Add(videoId, format);

        SaveVideoFormatTypeDic();
    }

    public int GetVideoFormatTypeByVideoId(string videoId)
    {
        GetVideoFormatTypeDic();
        if (VideoFormatDic.ContainsKey(videoId))
            return VideoFormatDic[videoId];
        else
            return 0;
    }

    public void DeleteFormatKeyByVideoId(string videoId)
    {
        GetVideoFormatTypeDic();
        if (VideoFormatDic.Count < 0)
            return;

        if (VideoFormatDic.ContainsKey(videoId))
            VideoFormatDic.Remove(videoId);
    }

    Dictionary<string, int> StringToDictionary(string value)
    {
        Dictionary<string, int> dic = new Dictionary<string, int>();

        if (value.Length < 1)
            return dic;
        string[] dicStrs = value.Split('|');
        foreach (string str in dicStrs)
        {
            string[] strs = str.Split('=');
            dic.Add(strs[0], int.Parse(strs[1]));
        }
        return dic;
    }

    string DictionaryListToString(Dictionary<string, int> dicInfo)
    {
        if (dicInfo.Count == 0)
        {
            return "";
        }
        string str = "";

        foreach (string key in dicInfo.Keys)
        {
            str += (key + "=" + dicInfo[key]);
            str += "|";
        }
        str = str.Substring(0, str.Length - 1);

        return str;
    }
}

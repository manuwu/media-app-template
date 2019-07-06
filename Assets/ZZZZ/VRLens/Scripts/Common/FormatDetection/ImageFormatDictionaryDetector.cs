/*
 * 2018-4-11
 * 黄秋燕 Shemi
 * 视频格式本地持久化存储管理
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageFormatDictionaryDetector : SingletonPure<ImageFormatDictionaryDetector>
{
    private static string PLAYERKEYWORD = "IMAGE_FORMATKEY_DICTIONARY";
    Dictionary<int, int> ImageFormatDic = new Dictionary<int, int>();
    bool isInint = false;

    public void GetImageFormatTypeDic()
    {
        if (isInint) return;
        isInint = true;

        if (PlayerPrefs.HasKey(PLAYERKEYWORD) &&
            PlayerPrefs.GetString(PLAYERKEYWORD) != null && PlayerPrefs.GetString(PLAYERKEYWORD) != "")
        {
            string panoramaFormatDicString = PlayerPrefs.GetString(PLAYERKEYWORD);
            ImageFormatDic = StringToDictionary(panoramaFormatDicString);
        }
        else
            ImageFormatDic = new Dictionary<int, int>();
    }

    // when app quit and pause
    public void SaveImageFormatTypeDic()
    {
        GetImageFormatTypeDic();
        if (ImageFormatDic.Count == 0)
            return;

        string panoramaFormatDicString = DictionaryListToString(ImageFormatDic);

        if (panoramaFormatDicString != "" && panoramaFormatDicString != null)
            PlayerPrefs.SetString(PLAYERKEYWORD, panoramaFormatDicString);
    }

    public bool HasImageFormatOrNotByImageId(int panoramaId)
    {
        GetImageFormatTypeDic();
        if (ImageFormatDic.ContainsKey(panoramaId))
            return true;
        else
            return false;
    }

    public void SetImageFormatTypeByImageId(int panoramaId, int format)
    {
        GetImageFormatTypeDic();
        if (HasImageFormatOrNotByImageId(panoramaId))
            ImageFormatDic[panoramaId] = format;
        else
            ImageFormatDic.Add(panoramaId, format);

        SaveImageFormatTypeDic();
    }

    public int GetImageFormatTypeByImageId(int panoramaId)
    {
        GetImageFormatTypeDic();
        if (ImageFormatDic.ContainsKey(panoramaId))
            return ImageFormatDic[panoramaId];
        else
            return 0;
    }

    public void DeleteFormatKeyByImageId(int panoramaId)
    {
        GetImageFormatTypeDic();
        if (ImageFormatDic.Count < 0)
            return;

        if (ImageFormatDic.ContainsKey(panoramaId))
            ImageFormatDic.Remove(panoramaId);
    }

    Dictionary<int, int> StringToDictionary(string value)
    {
        Dictionary<int, int> dic = new Dictionary<int, int>();

        if (value.Length < 1)
            return dic;
        string[] dicStrs = value.Split('|');
        foreach (string str in dicStrs)
        {
            string[] strs = str.Split('=');
            dic.Add(int.Parse(strs[0]), int.Parse(strs[1]));
        }
        return dic;
    }

    string DictionaryListToString(Dictionary<int, int> dicInfo)
    {
        if (dicInfo.Count == 0)
        {
            return "";
        }
        string str = "";

        foreach (int key in dicInfo.Keys)
        {
            str += (key + "=" + dicInfo[key]);
            str += "|";
        }
        str = str.Substring(0, str.Length - 1);

        return str;
    }
}

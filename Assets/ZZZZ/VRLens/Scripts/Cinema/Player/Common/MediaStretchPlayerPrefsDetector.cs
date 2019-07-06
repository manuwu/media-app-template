using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediaStretchPlayerPrefsDetector : SingletonPure<MediaStretchPlayerPrefsDetector>
{
    public string MediaId = string.Empty;

    public void SetMediaId(string mediaId)
    {
        MediaId = mediaId;
    }

    public void ResetMediaId()
    {
        MediaId = string.Empty;
    }

    public void SetMediaStretchKey(bool isOpen)
    {
        if (MediaId != string.Empty)
            PlayerPrefs.SetString(MediaId, isOpen.ToString());
    }

    public bool GetMediaStretchKey()
    {
        if (MediaId == string.Empty)
            return false;

        string key = "true";
        if (PlayerPrefs.HasKey(MediaId))
            key = PlayerPrefs.GetString(MediaId);

        bool isOpenStretch = key.Contains("True") || key.Contains("true") ? true : false;
        return isOpenStretch;
    }
}

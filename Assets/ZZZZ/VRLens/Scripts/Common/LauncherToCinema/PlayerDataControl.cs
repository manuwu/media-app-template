using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayingURLMode
{
    KTTV, Local, LiveUrl
}

public class PlayerDataControl : SingletonMB<PlayerDataControl>
{
    public PlayingURLMode CurPlayingMode = PlayingURLMode.LiveUrl;

    public VideoPlayManage VideoPlayManage;
    public ImagePlayManage ImagePlayManage;
    List<int> VideoIdSortList = new List<int>();//videoId对应序号
    public Action StopPlayCallBack;
    public Action InterruptPlayer;
    private void Awake()
    {
        if (VideoPlayManage == null)
            VideoPlayManage = new VideoPlayManage();
        if (ImagePlayManage == null)
            ImagePlayManage = new ImagePlayManage();
    }

    public void SetCurPlayIndex(int playIndex)
    {
        VideoPlayManage.SetCurPlayIndex(playIndex);
    }

    public int GetCurPlayIndex()
    {
        return VideoPlayManage.GetCurPlayIndex();
    }

    public int GetVideoIndex(int videoId)
    {
        for (int i = 0; i < VideoIdSortList.Count; i++)
        {
            if (VideoIdSortList[i] == videoId)
            {
                return i;
            }
        }

        return -1;
    }

    #region Local Video
    Dictionary<int, JVideoDescriptionInfo> VideoIdToJVideoDscpInfoDic = new Dictionary<int, JVideoDescriptionInfo>();

    public void SetVideoDescriptionInfo(List<int> idList, Dictionary<int, JVideoDescriptionInfo> descrip)
    {
        if (VideoPlayManage == null)
            VideoPlayManage = new VideoPlayManage();

        VideoIdSortList = idList;
        VideoIdToJVideoDscpInfoDic = descrip;
        CurPlayingMode = PlayingURLMode.Local;

        VideoPlayManage.SetVideoTotalCount(idList.Count);
    }

    public JVideoDescriptionInfo GetJVideoDscpInfoByIndex(int index)
    {
        if (index < 0 || index > VideoIdSortList.Count)
        {
            return null;
        }
        else
        {
            int id = VideoIdSortList[index];
            if (VideoIdToJVideoDscpInfoDic.ContainsKey(id))
                return VideoIdToJVideoDscpInfoDic[id];
            else
                return null;
        }
    }

    public void ClearVideoDscpInfo()
    {
        VideoIdSortList.Clear();
        VideoIdToJVideoDscpInfoDic.Clear();
    }
    #endregion

    #region KTTV Player
    string _vid = string.Empty;
    string _cid = string.Empty;
    string _name = string.Empty;
    int _stereoType = 9;

    public void SetVideoVidCid(string vid, string cid, string name, int stereoType)
    {
        if (VideoPlayManage == null)
            VideoPlayManage = new VideoPlayManage();

        _vid = vid;
        _cid = cid;
        _name = name;
        _stereoType = stereoType;
        CurPlayingMode = PlayingURLMode.KTTV;

        VideoPlayManage.SetVideoTotalCount(0);
    }

    public Dictionary<string, string> GetKKTVVideoVidCid()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("vid", _vid);
        dic.Add("cid", _cid);
        dic.Add("name", _name);
        dic.Add("stereoType", _stereoType.ToString());

        return dic;
    }
    #endregion

    #region LiveUrl
    JVideoDescriptionInfo _liveUrlInfo = new JVideoDescriptionInfo();
    public void SetJVideoDscpInfoByLiveUrl(JVideoDescriptionInfo liveUrlInfo)
    {
        CurPlayingMode = PlayingURLMode.LiveUrl;
        _liveUrlInfo = liveUrlInfo;
    }

    public JVideoDescriptionInfo GetJVideoDscpInfoByLiveUrl()
    {
        return _liveUrlInfo;
    }
    #endregion
}

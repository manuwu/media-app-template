using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JMediaInfo
{
    public int id;
    public string name;
    public string path; //路径(可能改变)
    public string uri; //相对存储路径
    public int size; // MB
    public string createTime;//2018-01-17 HH:mm:ss
    public string thumbnailPath; // 缩略图路径
}

[Serializable]
public class JVideoDescriptionInfo : JMediaInfo
{
    public long time;
    public int stereoType;//立体类型
    public int width;
    public int height;
    public string recognitionImagePath; // 格式识别的图片
    public string vid; //KKTV使用
    public string cid; //KKTV使用
    public bool live;  //是否是直播模式（true 播放器不做缓存）

    public JVideoDescriptionInfo()
    {
        this.id = -1;
        this.name = string.Empty;
        this.path = string.Empty;
        this.uri = string.Empty;
        this.size = -1;
        this.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.thumbnailPath = string.Empty;

        this.time = 0;
        this.stereoType = 9;
        this.width = 0;
        this.height = 0;
        this.recognitionImagePath = string.Empty;
        this.vid = string.Empty;
        this.cid = string.Empty;
        this.live = false;
    }

    public JVideoDescriptionInfo
        (int id, string videoName, string path, string uri, long time, int size, int stereoType, int width, int height, DateTime createTime, string thumbnailPath, string recognitionImagePath)
    {
        this.id = id;
        this.name = videoName;
        this.path = path;
        this.uri = uri;
        this.size = size;
        this.createTime = createTime.ToString("yyyy-MM-dd HH:mm:ss");
        this.thumbnailPath = thumbnailPath;

        this.time = time;
        this.stereoType = stereoType;
        this.width = width;
        this.height = height;
        this.recognitionImagePath = recognitionImagePath;
        this.live= false;
    }
}

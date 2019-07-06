using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class JImageDescriptionInfo : JMediaInfo
{
    public int stereoType;//立体类型
    public int width;
    public int height;
    public string recognitionImagePath; // 格式识别的图片

    public JImageDescriptionInfo
        (int id, string videoName, string path, string uri, int size, int stereoType, int width, int height, DateTime createTime, string thumbnailPath, string recognitionImagePath)
    {
        this.id = id;
        this.name = videoName;
        this.path = path;
        this.uri = uri;
        this.size = size;
        this.createTime = createTime.ToString("yyyy-MM-dd HH:mm:ss");
        this.recognitionImagePath = recognitionImagePath;
        this.thumbnailPath = thumbnailPath;

        this.stereoType = stereoType;
        this.width = width;
        this.height = height;
    }
}

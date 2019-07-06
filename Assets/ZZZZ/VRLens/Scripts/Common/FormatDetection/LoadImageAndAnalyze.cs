/*
 * 2018-4-11
 * 黄秋燕 Shemi
 * 图片格式分析管理
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Daydream.MediaAppTemplate;
using System;

public class LoadImageAndAnalyze : MonoBehaviour
{
    object lockd = new object();

    public IEnumerator StartAnalyzeImageThread(int mediaId, string texturePath)
    {
        // Wait a frame.
        yield return new WaitForEndOfFrame();
        lock (lockd)
        {
            //读取文件
            FileStream fs = new FileStream(texturePath, FileMode.Open, FileAccess.Read);
            int byteLength = (int)fs.Length;
            byte[] imgBytes = new byte[byteLength];
            fs.Read(imgBytes, 0, byteLength);
            fs.Close();
            fs.Dispose();
            //转化为Texture2D
            Texture2D frameTexture = new Texture2D(2, 2);
            frameTexture.LoadImage(imgBytes);

            yield return new WaitForEndOfFrame();
            //ImageBasedProjectionDetector.GetInstance().AnalyzeImage(mediaId, texturePath, frameTexture, OnFormatDetected);
            Destroy(frameTexture);
            frameTexture = null;
            yield return null;
        }
    }



    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            VideoFormatDictionaryDetector.GetInstance().SaveVideoFormatTypeDic();
            ImageFormatDictionaryDetector.GetInstance().SaveImageFormatTypeDic();
        }
    }

    private void OnApplicationQuit()
    {
        VideoFormatDictionaryDetector.GetInstance().SaveVideoFormatTypeDic();
        ImageFormatDictionaryDetector.GetInstance().SaveImageFormatTypeDic();
    }
}

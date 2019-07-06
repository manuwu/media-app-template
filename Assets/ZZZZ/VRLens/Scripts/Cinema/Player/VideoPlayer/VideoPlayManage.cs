/*
 * Author:李传礼
 * DateTime:2017.12.28
 * Description:视频播放管理
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoPlayManage
{
    public static LoopType CurLoopType;
    int VideoTotalCount;
    int CurVideoIndex;

    public VideoPlayManage()
    {
        CurLoopType = LoopType.ListLoop;
        VideoTotalCount = 0;
        CurVideoIndex = 0;
    }

    public void SetLoopType(LoopType loopType)
    {
        CurLoopType = loopType;
    }

    public void SetVideoTotalCount(int count)
    {
        VideoTotalCount = count;
    }

    public void SetCurPlayIndex(int index)
    {
        CurVideoIndex = index;
    }

    public int GetCurPlayIndex()
    {
        return CurVideoIndex;
    }

    public int GetPreviousVideoIndex()
    {
        if (VideoTotalCount == 0)
            return -1;

        CurVideoIndex--;
        if (CurVideoIndex < 0)
            CurVideoIndex = VideoTotalCount - 1;

        return CurVideoIndex;
    }

    public int GetNextVideoIndex()
    {
        if (VideoTotalCount == 0)
            return -1;

        CurVideoIndex++;
        if (CurVideoIndex > VideoTotalCount - 1)
            CurVideoIndex = 0;

        return CurVideoIndex;
    }
}

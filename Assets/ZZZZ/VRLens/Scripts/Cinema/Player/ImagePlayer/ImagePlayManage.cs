using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePlayManage
{
    int ImageTotalCount;
    int CurImageIndex;

    public ImagePlayManage()
    {
        ImageTotalCount = 0;
        CurImageIndex = 0;
    }

    public void SetImageTotalCount(int count)
    {
        ImageTotalCount = count;
    }

    public void SetCurPlayIndex(int index)
    {
        CurImageIndex = index;
    }

    public int GetCurPlayIndex()
    {
        return CurImageIndex;
    }

    public int GetPreviousImageIndex()
    {
        if (ImageTotalCount == 0)
            return -1;

        CurImageIndex--;
        if (CurImageIndex < 0)
            CurImageIndex = ImageTotalCount - 1;

        return CurImageIndex;
    }

    public int GetNextImageIndex()
    {
        if (ImageTotalCount == 0)
            return -1;

        CurImageIndex++;
        if (CurImageIndex > ImageTotalCount - 1)
            CurImageIndex = 0;

        return CurImageIndex;
    }

    /// <summary>
    /// 不改变当前index情况下获取上一个对象的index
    /// </summary>
    /// <returns></returns>
    public int GetPreviousImageIndexNoChangeCur()
    {
        if (ImageTotalCount == 0)
            return -1;

        int curIndex = CurImageIndex;
        curIndex--;
        if (curIndex < 0)
            curIndex = ImageTotalCount - 1;

        return curIndex;
    }

    public int GetNextImageIndexNoChangeCur()
    {
        if (ImageTotalCount == 0)
            return -1;

        int curIndex = CurImageIndex;
        curIndex++;
        if (curIndex > ImageTotalCount - 1)
            curIndex = 0;

        return curIndex;
    }
}

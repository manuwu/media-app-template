/*
 * Author:李传礼
 * DateTime:2018.03.26
 * Description:点排条
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DotsBar : MonoBehaviour
{
    bool IsInit = false;
    MixedButton[] Dots;
    [HideInInspector]
    public int MaxDotNum;
    int TotalDotNum;
    int CurSelectIndex;
    Dictionary<MixedButton, int> DotToIndexDic;
    MixedButton OldSelectDot;

    public Action<int, float> SelectDotBarCallback;//DotIndex DotsBar的t

	void Start ()
    {
        if (!IsInit)
            Init();
    }

    public void Init()
    {
        IsInit = true;

        Dots = this.GetComponentsInChildren<MixedButton>();
        MaxDotNum = Dots.Length;
        TotalDotNum = 0;
        CurSelectIndex = 0;
        DotToIndexDic = new Dictionary<MixedButton, int>();
        OldSelectDot = null;

        for (int i = 0; i < MaxDotNum; i++)
        {
            Dots[i].gameObject.SetActive(false);
            DotToIndexDic[Dots[i]] = i;
            Dots[i].ClickBtnCallback += SelectDot;//123深圳展会屏蔽 按钮射线接触也关掉了
        }
    }

    //设置点数
    public void SetDotNum(int dotNum)
    {
        if (!IsInit)
            Init();

        if (dotNum < 0)
            dotNum = 0;
        if (dotNum > MaxDotNum)
            dotNum = MaxDotNum;

        TotalDotNum = dotNum;

        for (int i = 0; i < MaxDotNum; i++)
        {
            if (i == CurSelectIndex)
            {
                Dots[i].SetSelected(true);
                OldSelectDot = Dots[i];
            }
            else
                Dots[i].SetSelected(false);

            if (dotNum > 1 && i < dotNum)
                Dots[i].gameObject.SetActive(true);
            else
                Dots[i].gameObject.SetActive(false);
        }
    }

    void SelectDot(MixedButton dot)
    {
        if (OldSelectDot != null)
            OldSelectDot.SetSelected(false);

        dot.SetSelected(true);
        OldSelectDot = dot;

        CurSelectIndex = DotToIndexDic[dot];
        float factor;
        if (TotalDotNum > 1)
            factor = (float)CurSelectIndex / (TotalDotNum - 1);
        else
            factor = 0;

        if (SelectDotBarCallback != null)
            SelectDotBarCallback(CurSelectIndex, factor);
    }

    public void SelectPreviousDot()
    {
        if(CurSelectIndex > 0)
        {
            CurSelectIndex--;

            if (OldSelectDot != null)
                OldSelectDot.SetSelected(false);

            Dots[CurSelectIndex].SetSelected(true);
            OldSelectDot = Dots[CurSelectIndex];
        }
    }

    public void SelectNextDot()
    {
        if (CurSelectIndex < TotalDotNum - 1)
        {
            CurSelectIndex++;

            if (OldSelectDot != null)
                OldSelectDot.SetSelected(false);

            Dots[CurSelectIndex].SetSelected(true);
            OldSelectDot = Dots[CurSelectIndex];
        }
    }
}

/*
 * Author:李传礼
 * DateTime:2017.11.13
 * Description:滚动展示面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class RollingShowPanel : VRInputMessageBase
{
    public RectTransform[] Panels;//面板数组（所有）
    public Transform[] PresentPositions;//展示位置数组
    public AnimationCurve AnimCurve;
    public int FPToPPI;//第一个面板对应的展示位置索引（从左往右），可以为负数（为负数则表示超出第一个索引的编号）
    public int StayCount;//停留个数（至少一个）

    //索引计算
    int PanelCount;
    [HideInInspector]
    public int PresentPosCount;

    //边界效果
    AnimationCurve BorderAnimCurve;

    //回调
    public Action<GameObject> ClickPanelCallback;

    void OnEnable()
    {
        Init();
    }

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            RollPanel(-3);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            RollPanel(3);
    }

    protected void Init()
    {
        if (Panels.Length == 0 || PresentPositions.Length == 0)
        {
            Debug.Log("展示面板和位置数组为空");
            return;
        }

        PanelCount = Panels.Length;
        PresentPosCount = PresentPositions.Length;
        BorderAnimCurve = new AnimationCurve();
        BorderAnimCurve.AddKey(new Keyframe(0, 0, 0, 0));
        BorderAnimCurve.AddKey(new Keyframe(0.5f, 0.2f, 0, 0));
        BorderAnimCurve.AddKey(new Keyframe(1, 0, 0, 0));

        //纠正参数
        if (StayCount < 1)
            StayCount = 1;
        else if (StayCount > PresentPosCount)
            StayCount = PresentPosCount;

        if (FPToPPI < -PanelCount + StayCount)
            FPToPPI = -PanelCount + StayCount;
        if (FPToPPI > PresentPosCount - StayCount)
            FPToPPI = PresentPosCount - StayCount;

        //初始化界面位置
        for (int i = 0; i < PanelCount; i++)
        {
            int presentPosIndex = FPToPPI + i;

            //有效
            if (presentPosIndex >= 0 && presentPosIndex <= PresentPosCount - 1)
            {
                Panels[i].position = PresentPositions[presentPosIndex].position;
                Panels[i].eulerAngles = PresentPositions[presentPosIndex].eulerAngles;
                Panels[i].localScale = PresentPositions[presentPosIndex].localScale;
            }
            else if (presentPosIndex < 0)
            {
                Panels[i].position = PresentPositions[0].position;
                Panels[i].eulerAngles = PresentPositions[0].eulerAngles;
                Panels[i].localScale = PresentPositions[0].localScale;
            }
            else if (presentPosIndex > PresentPosCount - 1)
            {
                Panels[i].position = PresentPositions[PresentPosCount - 1].position;
                Panels[i].eulerAngles = PresentPositions[PresentPosCount - 1].eulerAngles;
                Panels[i].localScale = PresentPositions[PresentPosCount - 1].localScale;
            }

            AutoSortPanel(0);

            EventTriggerListener.Get(Panels[i].gameObject).OnPtClick += ClickPanel;
        }
    }

    //滚动面板
    public void RollPanel(int step)
    {
        //到达尽头,至少有一个留在展示区
        if ((FPToPPI <= -PanelCount + StayCount && step < 0) || (FPToPPI >= PresentPosCount - StayCount && step > 0))
        {
            BorderEffect(step);
            return;
        }

        AutoSortPanel(step);

        int newFPToPPI = FPToPPI + step;
        //纠正参数
        if (newFPToPPI < -PanelCount + StayCount)
            newFPToPPI = -PanelCount + StayCount;
        if (newFPToPPI > PresentPosCount - StayCount)
            newFPToPPI = PresentPosCount - StayCount;

        //滚动
        for (int i = 0; i < PanelCount; i++)
        {
            int presentPosIndex = FPToPPI + i;
            int newPresentPosIndex = newFPToPPI + i;

            if (presentPosIndex < 0)
                presentPosIndex = 0;
            else if (presentPosIndex > PresentPosCount - 1)
                presentPosIndex = PresentPosCount - 1;
            if (newPresentPosIndex < 0)
                newPresentPosIndex = 0;
            else if (newPresentPosIndex > PresentPosCount - 1)
                newPresentPosIndex = PresentPosCount - 1;

            //起点或者终点在展示区
            if ((presentPosIndex >= 0 && presentPosIndex <= PresentPosCount - 1) || (newPresentPosIndex >= 0 && newPresentPosIndex <= PresentPosCount - 1))
                MotionAestheticsScrp.AccTranslating(Panels[i].gameObject, PresentPositions[presentPosIndex], PresentPositions[newPresentPosIndex], GlobalVariable.AnimationSpendTime, AnimCurve);
        }
        FPToPPI = newFPToPPI;
    }

    //边界效果
    void BorderEffect(int step)
    {
        for (int i = 0; i < PanelCount; i++)
        {
            int presentPosIndex = FPToPPI + i;
            if (presentPosIndex >= 0 && presentPosIndex <= PresentPosCount - 1)
            {
                int index;
                if (step < 0)
                    index = 0;
                else if (step > 0)
                    index = PresentPosCount - 1;
                else
                    break;
                MotionAestheticsScrp.AccTranslating(Panels[i].gameObject, PresentPositions[presentPosIndex], PresentPositions[index], GlobalVariable.AnimationSpendTime, BorderAnimCurve);
            }
        }
    }

    //自动排列层次
    void AutoSortPanel(int step)
    {
        int midPPI = PresentPosCount / 2;
        int newFPToPPI = FPToPPI + step;
        //纠正参数
        if (newFPToPPI < -PanelCount + StayCount)
            newFPToPPI = -PanelCount + StayCount;
        if (newFPToPPI > PresentPosCount - StayCount)
            newFPToPPI = PresentPosCount - StayCount;

        for (int i = 0; i < PanelCount; i++)
        {
            int newPresentPosIndex = newFPToPPI + i;

            if (newPresentPosIndex == midPPI)
                Panels[i].SetSiblingIndex(PanelCount);//加上位置gameObject所占的1位
            else if(newPresentPosIndex < midPPI)
                Panels[i].SetSiblingIndex(i + 1);//加上位置组的所占1位
            else if(newPresentPosIndex > midPPI)
                Panels[i].SetSiblingIndex(PanelCount - (newPresentPosIndex - midPPI));//加上位置组的所占1位
        }
    }

    //消息响应
    void ClickPanel(GameObject go)
    {
        if (ClickPanelCallback != null)
            ClickPanelCallback(go);
    }

    public override void TouchPadVector2(Vector2 vector)
    {
        if (vector.magnitude < 0.5f)
            return;

        float dir = Vector2.Dot(vector, Vector2.right);
        if (dir < 0)
            RollPanel(-3);
        else if (dir > 0)
            RollPanel(3);
    }
}

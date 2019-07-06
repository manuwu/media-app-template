/*
 * Author:李传礼
 * DateTime:2018.02.26
 * Description:卡片展示面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsShowPanel : MonoBehaviour
{
    public Transform[] Panels;
    public Transform[] AnchorPoints;
    public int StayCount;//停留个数
    public int FPToAPI;//First Panel To AnchorPoints Index（从左往右）可以为负数（为负数则表示超出第一个索引的编号）
    bool IsError;//参数是否错误
    Vector3 LeftAlbumDir;
    Vector3 RightAlbumDir;
    bool IsCreateMotionUnitList;
    List<MotionUnit> MotionUnitList;

    void Start ()
    {
        Init();
    }

    void Init()
    {
        if (Panels == null || AnchorPoints == null)
        {
            IsError = true;
            return;
        }
        else
            IsError = false;

        BorderCheck();

        LeftAlbumDir = Vector3.one;
        RightAlbumDir = Vector3.one;
        IsCreateMotionUnitList = false;
        MotionUnitList = new List<MotionUnit>();
    }

    //参数边界检查
    void BorderCheck()
    {
        //规范停留个数，停留个数不能高于锚点个数，然后不能高于Panel个数
        if (StayCount > AnchorPoints.Length)
            StayCount = AnchorPoints.Length;
        if (StayCount > Panels.Length)
            StayCount = Panels.Length;

        //计算FPToAPI边界
        int fpMinAPI = StayCount - Panels.Length;//第一面板最小锚点停留序号
        int fpMaxAPI = AnchorPoints.Length - StayCount;//第一面板最大锚点停留序号
        //规范First Panel 锚点序号
        if (FPToAPI < fpMinAPI)
            FPToAPI = fpMinAPI;
        else if (FPToAPI > fpMaxAPI)
            FPToAPI = fpMaxAPI;
    }

    public void Move(int step, float t)
    {
        if (IsError || step == 0)
            return;

        if(!IsCreateMotionUnitList)
        {
            IsCreateMotionUnitList = true;

            int fromPanelIndex, toPanelIndex;
            ComputeMotionPanelIndexSection(ref step, out fromPanelIndex, out toPanelIndex);
            for (int i = fromPanelIndex; i <= toPanelIndex; i++)
            {
                MotionUnit motionUnit = CreateMotionUnit(i, step);
                if (motionUnit != null)
                    MotionUnitList.Add(motionUnit);
            }
        }
        else
        {
            foreach(MotionUnit mu in MotionUnitList)
            {
                mu.Motion(t);
            }

            if (t == 1)
            {
                IsCreateMotionUnitList = false;
                MotionUnitList.Clear();
            }
        }
    }

    //运算运动时，panel要动用的序号范围，-1为无效
    void ComputeMotionPanelIndexSection(ref int step, out int fromIndex, out int toIndex)
    {
        StepCheck(ref step);

        if (step > 0)//序号加，从右至左移动
        {
            fromIndex = 0 - FPToAPI;
            if (fromIndex < 0)
                fromIndex = 0;

            toIndex = fromIndex + (AnchorPoints.Length - 1) + step;//先算出理想条件下的toIndex，再在下句做边界判断
            if (toIndex > Panels.Length - 1)
                toIndex = Panels.Length - 1;
        }
        else if(step < 0)//从左至右
        {
            int lpToAPI = FPToAPI + (Panels.Length - 1);//Last Panel To AnchorPoints Index
            if (lpToAPI >= AnchorPoints.Length - 1)//是否超出锚点边界
                toIndex = (AnchorPoints.Length - 1) - FPToAPI;
            else
                toIndex = Panels.Length - 1;

            fromIndex = toIndex - (AnchorPoints.Length - 1) + step;//先算出理想条件下的fromIndex，再在下句做边界判断
            if (fromIndex < 0)
                fromIndex = 0;
        }
        else
        {
            fromIndex = -1;
            toIndex = -1;
        }
    }

    //步数检测
    void StepCheck(ref int step)
    {
        if(step > 0)
        {
            int lpToAPI = FPToAPI + (Panels.Length - 1);//Last Panel To AnchorPoints Index
            int canMoveStep = lpToAPI - (StayCount - 1);
            if (step > canMoveStep)
                step = canMoveStep;
        }
        else if(step < 0)
        {
            int canMoveStep = (AnchorPoints.Length - StayCount) - FPToAPI;
            if (Mathf.Abs(step) > canMoveStep)
                step = -canMoveStep;
        }
    }

    MotionUnit CreateMotionUnit(int panelIndex, int step)
    {
        Transform panel = Panels[panelIndex];
        int fromAPI, toAPI;
        if (step >= 0)
        {
            fromAPI = FPToAPI + panelIndex;
            AnchorPointIndexCheck(ref fromAPI);
            toAPI = fromAPI + step;
            AnchorPointIndexCheck(ref toAPI);
        }
        else
        {
            toAPI = FPToAPI + panelIndex;
            AnchorPointIndexCheck(ref toAPI);
            fromAPI = toAPI + step;
            AnchorPointIndexCheck(ref fromAPI);
        }

        int movePointCount = toAPI - fromAPI;
        if (movePointCount <= 0)
            return null;
        else
        {
            Transform[] points = new Transform[movePointCount];
            for(int i = fromAPI; i <= toAPI; i++)
            {
                points[i - fromAPI] = AnchorPoints[i];
            }

            return new MotionUnit(panel, points);
        }
    }

    //锚点数组序号检测，可用性判断
    void AnchorPointIndexCheck(ref int api)
    {
        if (api < 0)
            api = 0;
        else if (api > AnchorPoints.Length - 1)
            api = AnchorPoints.Length - 1;
    }
}

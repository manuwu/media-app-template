/*
 * Author:李传礼
 * DateTime:2017.12.16
 * Description:凝视事件（没控制器的替代操作）
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeEvent : SingletonMB<GazeEvent>
{
    [HideInInspector]
    public bool IsApply;//是否生效
    float StartTime;
    float SpaceTime;
    [HideInInspector]
    public bool ClickButtonDown;

    void Awake ()
    {
        StartTime = -1;
        SpaceTime = 3;
        ClickButtonDown = false;
    }

	void Update ()
    {
        if (ClickButtonDown)//下一帧还原值
        {
            ClickButtonDown = false;
            //StartTime = -1;//一直注视同一物体，避免多次触发事件
            StartTime = -2;
        }

        if (StartTime > 0)
        {
            if (Time.time - StartTime >= SpaceTime)
                ClickButtonDown = true;
        }

        if (ClickButtonDown)
            Debug.Log("GazeEvent点击事件");
    }

    //开始计时
    public void BeginTimekeeping()
    {
        if (!IsApply)
            return;

        if(StartTime == -1)
            StartTime = Time.time;
    }

    public void StopTimekeeping()
    {
        if (!IsApply)
            return;

        StartTime = -1;
        ClickButtonDown = false;
    }
}

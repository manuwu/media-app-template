/*
 * Author:李传礼 / 黄秋燕  Shemi
 * DateTime:2017.12.5
 * Description:影院
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cinema : MonoBehaviour
{
    public SvrVideoPlayer VideoPlayer;
    private static Cinema mInstant;
    public static Cinema Instant { get { return mInstant; } }
    
    public GameObject CinemaUI;
    public static GvrHead GvrHead;
    public static bool IsPlayMode = true;//播放模式
    /// <summary>
    /// 是否播放 true:播放 false:暂停
    /// </summary>
    public static bool IsPlayOrPauseVideoPlayer;
    public static bool IsPointerEnterVideoPlayerUI;
    public static bool IsPlayEndWhenKTTVModel;
    /// <summary>
    /// 是否处于视角调整
    /// </summary>
    public static bool IsInFocus;
    /// <summary>
    /// 是否处于锁定视角
    /// </summary>
    public static bool IsInLockAngle;
    /// <summary>
    /// 是否第一次锁定视角，为了避免点击“锁定”按钮时触发UI回正消息
    /// </summary>
    public static bool IsFirstInLockAngle;

    Vector3 OldUIDir;
    [HideInInspector]
    public Vector3 CameraToUIPositionVector;
    [HideInInspector]
    public Vector3 CameraToPlayerPositionVector;

    public Action RecenterCallback;
    public Action ResetUIDirCallback;
    public Action FocusCompleteCallback;
    public Action SwitchVideoPlayerUIVisionCallback;

    private void Awake()
    {
        if (mInstant != null) DestroyImmediate(this);
            mInstant = this;

        LogTool.Log("开始查找Env 和 UI");
        //CinemaUI = GameObject.FindGameObjectWithTag("CinemaUI");
        GvrHead = Camera.main.gameObject.GetComponent<GvrHead>();
        IsPointerEnterVideoPlayerUI = false;
        IsPlayEndWhenKTTVModel = false;
        IsInFocus = false;
        IsInLockAngle = false;
        IsFirstInLockAngle = false;
    }

    public void Init()
    {
        //外部类的初始化
        ResetVariable();

        CameraToUIPositionVector = CinemaUI.transform.position - Camera.main.transform.position;
        CameraToPlayerPositionVector = PlayerGameobjectControl.Instance.QuadScreen.transform.parent.position - Camera.main.transform.position;

        GlobalRunningFunction.Instance.InputOperate.RecenteredCallback += Recentered;
        GlobalRunningFunction.Instance.InputOperate.ResetUIDirCallback += UIRecenterControl;
    }

    private void OnDestroy()
    {
        GlobalRunningFunction.Instance.InputOperate.RecenteredCallback -= Recentered;
        GlobalRunningFunction.Instance.InputOperate.ResetUIDirCallback -= UIRecenterControl;
    }

    void BackHome()
    {
        GlobalAppManage.BackHome();//123临时版本注释掉 上海
    }

    public void Recentered(Vector3 dir)
    {
        if (IsInLockAngle) return;

        Debug.Log("Recenter");
        if (RecenterCallback != null)
            RecenterCallback();
    }

    public void UIRecenterControl(Vector3 dir)
    {
        if (IsInLockAngle && IsFirstInLockAngle)
        {
            IsFirstInLockAngle = false;
            return;
        }
        if (!IsInFocus)//重置UI位置，跟随鼠标点击
        {
            Vector3 yDir = PreDefScrp.ComputeProjection(Vector3.up, dir);
            float angle = Vector3.Angle(OldUIDir, yDir);
            //Debug.Log("角度" + angle);
            if (!IsPointerEnterVideoPlayerUI)
            {
                if (angle <= 10)
                {
                    if (SwitchVideoPlayerUIVisionCallback != null)
                        SwitchVideoPlayerUIVisionCallback();
                }
                else
                {
                    if (ResetUIDirCallback != null)
                        ResetUIDirCallback();
                }

                OldUIDir = yDir;
            }
            else
                Debug.Log("已经进入VideoPlayerUI，所以没法动态切换显影");
        }
        else //视角回正
        {
            if (!IsPointerEnterVideoPlayerUI && FocusCompleteCallback != null)
                FocusCompleteCallback();
            else
                Debug.Log("已经进入VideoPlayerUI，所以没法视角调整");
        }
    }

    public void ResetVariable()
    {
        IsPointerEnterVideoPlayerUI = false;
    }
}

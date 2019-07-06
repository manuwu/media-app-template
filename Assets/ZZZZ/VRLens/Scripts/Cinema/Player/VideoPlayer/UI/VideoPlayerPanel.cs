/*
 * Author:李传礼 / 黄秋燕 Shemi
 * DateTime:2017.12.04
 * Description:视频播放面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate long Long_VoidDelegate();

public class VideoPlayerPanel : VRInputMessageBase
{
    private Renderer BendQuadScreen;
    public VideoControlPanel VideoCtrlPanel;

    JVideoDescriptionInfo CurJVideoDscpInfo;
    //JImageDescriptionInfo CurJImageDscpInfo;
    bool IsKeepShowUI;
    bool IsShowUIForAutoMode;//对于自动模式时，是否在显示
    bool OldIsMonoShow;
    bool UIIsShow; //UI是否显示

    bool TPAndVCPUIIsShow;
    bool IsEnterPlayer; //true时才能手势缩放播放器
    bool IsEnterUI; //true时不能切换图片
    bool IsShowLoading; //true时不能切换图片
    bool IsChangeSize; //true时不能切换图片
    float OldScreenSize;
    Vector3 QuadScreenMinSize;
    Vector3 QuadScreenMaxSize;
    Vector3 QuadScreenStandardSize;

    public Action<StereoType> ChangeStereoTypeCallback;
    public Action<bool> PointerEnterUICallback;
    public Long_VoidDelegate GetVideoCurrentTimePtr;

    //Picture action for change previous/next picture
    public Action UIRecenterCallback;
    public Action ChangeToPreviousPictureCallback;
    public Action ChangeToNextPictureCallback;
    public Action<bool> StretchingPictureCallback;
    public Action ShowUICallback;

    public void ResetVariable()
    {
        CurJVideoDscpInfo = null;
        //CurJImageDscpInfo = null;
        IsKeepShowUI = false;
        IsShowUIForAutoMode = true;
        OldIsMonoShow = true;
        OldScreenSize = 0;
        IsEnterPlayer = false;
        IsShowLoading = false;
        TPAndVCPUIIsShow = true;
        IsChangeSize = false;
        UIIsShow = false;

        QuadScreenMinSize = new Vector3(17.91354f, 10.087324f, 1);
        QuadScreenMaxSize = CinemaSettings.GetInstance().ImaxQuadScreenScale;
        QuadScreenStandardSize = new Vector3(21.91031f, 12.337952f, 1);

        GlobalRunningFunction.Instance.ShowControllerRayLine();
        StopAutoHideUI();
        VideoCtrlPanel.Hide();
    }

    public void Init()
    {
        if (BendQuadScreen == null)
            BendQuadScreen = PlayerGameobjectControl.Instance.QuadScreen;

        ResetVariable();
        VideoCtrlPanel.Init();

        VideoCtrlPanel.SettingsPanel.StereoTypePanel.ChangeStereoTypeCallback = ChangeStereoType;
        //VideoCtrlPanel.ClickBackBtnCallback += BackToLocalList;
        VideoCtrlPanel.SettingsPanel.StereoTypePanel.ShowStereoTypePanelCallback = ShowStereoTypePanel;
        VideoCtrlPanel.SettingsPanel.ScreenSizePanel.ChangeScreenSizeTypeCallback = ChangeScreenSize;
        VideoCtrlPanel.VolumePanel.PointerEnterUICallback = PointerEnterUI;
        VideoCtrlPanel.SettingsPanel.PointerEnterUICallback = PointerEnterUI;
        VideoCtrlPanel.SettingsPanel.StereoTypePanel.PointerEnterUICallback = PointerEnterUI;
        VideoCtrlPanel.SettingsPanel.ScreenSizePanel.PointerEnterUICallback = PointerEnterUI;
        VideoCtrlPanel.SettingsPanel.SceneChangePanel.PointerEnterUICallback = PointerEnterUI;
        VideoCtrlPanel.SettingsPanel.LoopModePanel.PointerEnterUICallback = PointerEnterUI;
        VideoCtrlPanel.PointerEnterUICallback = PointerEnterUI;
    }

    void ShowStereoTypePanel()
    {
        if (CurJVideoDscpInfo != null)
        {
            Statistics.GetInstance().OnEvent(MediaCenterEvent.ClickVideoSetting, "点击视频设置控件");
            VideoCtrlPanel.SettingsPanel.StereoTypePanel.Show((StereoType)CurJVideoDscpInfo.stereoType);//根据信息显示当前类型
        }
    }

    void ChangeScreenSize()
    {
        ScreenSizeType sizeType = GlobalVariable.GetScreenSizeType();
        switch (sizeType)
        {
            case ScreenSizeType.Cinema:
                BendQuadScreen.transform.localScale = QuadScreenMaxSize;
                BendQuadScreen.transform.localPosition = CinemaSettings.GetInstance().ImaxQuadScreenPosition;
                break;
            case ScreenSizeType.Standard:
                BendQuadScreen.transform.localScale = QuadScreenStandardSize;
                BendQuadScreen.transform.localPosition = new Vector3(0, 0.75f, 19.1f);
                break;
            case ScreenSizeType.MINI:
                BendQuadScreen.transform.localScale = QuadScreenMinSize;
                BendQuadScreen.transform.localPosition = new Vector3(0, -0.34f, 19.1f);
                break;
            default:
                break;
        }
    }

    void ChangeStereoType(StereoType stereoType)
    {
        if (CurJVideoDscpInfo == null)
            return;

        CurJVideoDscpInfo.stereoType = (int)stereoType;

        if (ChangeStereoTypeCallback != null)
            ChangeStereoTypeCallback(stereoType);
    }
    
    void BackToLocalList()
    {
        PointerEnterUI(false);
    }

    public void PointerEnterUI(bool isEnter)
    {
        IsEnterUI = isEnter;

        if (PointerEnterUICallback != null)
            PointerEnterUICallback(isEnter);
    }

    public void Show()
    {
        Debug.Log("显示播放界面");
        //this.transform.localScale = Vector3.one;
        this.gameObject.SetActive(true);
        UIIsShow = true;
        VideoCtrlPanel.Show();

#if UNITY_ANDROID
        AutoHideUI();
#endif
    }

    public void Hide()
    {
        ResetUI();
        RestUIControl();
        this.gameObject.SetActive(false);
    }

    public void SetVideoPlayerState(JVideoDescriptionInfo jVdi)
    {
        CurJVideoDscpInfo = jVdi;

        VideoCtrlPanel.SetVideoName(jVdi.name);
        VideoCtrlPanel.SetVideoCurrentTime(0);
        VideoCtrlPanel.SetPlayMode(true);
    }

    //public void SetImagePlayerState(JImageDescriptionInfo jImg)
    //{
    //    CurJImageDscpInfo = jImg;

    //    BendQuadScreen.transform.localScale = QuadScreenStandardSize;

    //    VideoCtrlPanel.SetVideoName(jImg.name);
    //}

    public void PlayVideo(bool isManual = false)
    {
        if (!isManual)
            VideoCtrlPanel.SetPlayMode(true);//播放按钮变成播放模式
    }

    void ResetUI()
    {
        VideoCtrlPanel.ResetUI();
    }

    public void KeepShowUI()
    {
        Debug.Log("持续显示VPUI");
        IsKeepShowUI = true;
        IsShowUIForAutoMode = true;
        UIIsShow = true;
        StopAutoHideUI();
        
        VideoCtrlPanel.Show();

        TPAndVCPUIIsShow = true;
        GlobalRunningFunction.Instance.ShowControllerRayLine();
    }

    public void ResetControlValue()
    {
        IsKeepShowUI = false;

        if (!BendQuadScreen.gameObject.activeInHierarchy)
            IsEnterPlayer = false;

        Cinema.IsPointerEnterVideoPlayerUI = IsEnterUI;
    }

    /// <summary>
    /// Change player transform and scale
    /// </summary>
    public void ChangePlayerUI()
    {
        if (BendQuadScreen == null)
            BendQuadScreen = PlayerGameobjectControl.Instance.QuadScreen;
        SceneModel curSceneModel = GlobalVariable.GetSceneModel();
        if (curSceneModel == SceneModel.Default || curSceneModel == SceneModel.StarringNight)
        {
            ChangeScreenSize();
            //BendQuadScreen.transform.localPosition = new Vector3(0, -0.06f, 9.3f);
        }
        else if (curSceneModel == SceneModel.IMAXTheater)
        {
            BendQuadScreen.transform.localScale = CinemaSettings.GetInstance().ImaxQuadScreenScale;
            BendQuadScreen.transform.localPosition = CinemaSettings.GetInstance().ImaxQuadScreenPosition;
        }
        else if (curSceneModel == SceneModel.Drive)
        {
            CinemaSettings.GetInstance().DriveModelQuadScreenTrans();
        }
    }

    public void SwitchUIVision()
    {
        if (IsKeepShowUI)
            return;

        //if (IsEnterPlayer && TPAndVCPUIIsShow)
        //    return;

        if (!IsShowUIForAutoMode)
            ShowUI();
        else
            HideUI();
    }

    public void ShowUI()
    {
        Debug.Log("显示VPUI");
        IsKeepShowUI = false;
        IsShowUIForAutoMode = true;
        UIIsShow = true;
        //场景出现
        if (Cinema.IsInLockAngle && ShowUICallback != null)
            ShowUICallback();
        Cinema.IsInLockAngle = false;
        VideoCtrlPanel.Show();

        TPAndVCPUIIsShow = true;
        GlobalRunningFunction.Instance.ShowControllerRayLine();
        AutoHideUI();
    }

    public void HideUI()
    {
        Debug.Log("隐藏VPUI");
        //if (!Cinema.IsPlayMode)
        //    return;
        StopAutoHideUI();
        IsKeepShowUI = false;
        IsShowUIForAutoMode = false;
        Cinema.IsPointerEnterVideoPlayerUI = false;
        UIIsShow = false;
        VideoCtrlPanel.Hide();

        TPAndVCPUIIsShow = false;
        GlobalRunningFunction.Instance.HideControllerRayLine();
    }

    void StopAutoHideUI()
    {
        if (IsInvoking("HideUI"))
            CancelInvoke("HideUI");
    }

    public void AutoHideUI()
    {
        if (IsEnterPlayer && TPAndVCPUIIsShow)
            return;

        Debug.Log("开始自动隐藏VPUI");
        IsKeepShowUI = false;
        StopAutoHideUI();
        Invoke("HideUI",5);
    }

    public void SetLoadingObjActive(bool isShow)
    {
        IsShowLoading = isShow;
    }

    public void RestUIControl()
    {
        Debug.Log("重置VPUI控制");
        IsKeepShowUI = false;
        IsShowUIForAutoMode = true;
        StopAutoHideUI();
        
        UIIsShow = true;
        VideoCtrlPanel.Show();

        TPAndVCPUIIsShow = true;
        GlobalRunningFunction.Instance.ShowControllerRayLine();
    }

    public void SetVideoFormatTypeWhenPlayLoop(int stereoType)
    {
        if (!CinemaPanel.IsPlayLoop)
            return;

        VideoFormatDictionaryDetector.GetInstance().SetVideoFormatTypeByVideoId(CurJVideoDscpInfo.id.ToString(), (int)stereoType);
    }

    public bool IsUIShowing()
    {
        return UIIsShow;
    }

    private void OnDestroy()
    {
        BendQuadScreen = null;
        VideoCtrlPanel = null;
    }
}

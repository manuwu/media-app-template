/*
 * Author:李传礼 / 黄秋燕 Shemi
 * DateTime:2017.12.14 / 2018-8-6
 * Description:视频控制面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using IVR.Language;

public class VideoControlPanel : MonoBehaviour
{
    public BackButton BackBtn;
    public Button PlayBtn;
    public Button PauseBtn;
    //public Button PreviousBtn;
    public Button NextBtn;
    public Button VolumeBtn;
    public Button SettingBtn;
    public Button FocusBtn;
    public Button LockBtn;
    public Text VideoNameText;
    public Image[] VolumeImages;

    public PlayProgressBarPanel PlayPBPanel;
    public VolumePanel VolumePanel;
    public VideoSettingsPanel SettingsPanel;

    public Sprite PlayBtnHovrSprite;
    public Sprite PlayBtnNorSprite;
    public Sprite PauseBtnHovrSprite;
    public Sprite PauseBtnNorSprite;

    [SerializeField]
    private Sprite FocusHoverSprite;
    [SerializeField]
    private Sprite FocusNorSprite;
    [SerializeField]
    private Sprite LockNorSprite;
    [SerializeField]
    private Sprite LockBtnHovrSprite;
    [SerializeField]
    private Sprite LockBtnNorSprite;

    private Image PlayBtnChildImage;
    private Image PauseBtnChildImage;

    bool IsLive = false;
    /// <summary>
    /// 是否hover了视角调节按钮
    /// </summary>
    bool IsEnterFocus = false;
    bool IsShow;
    /// <summary>
    /// 锁定视角按钮是否可用
    /// </summary>
    bool IsEnableLockBtn = false;


    public Action ClickBackBtnCallback;
    public Action ClickPlayBtnAndReplayCallback;
    public Action<bool> ClickPlayBtnCallback;
    public Action ClickPauseBtnCallback;
    public Action ClickPreviousBtnCallback;
    public Action ClickNextBtnCallback;
    public Action<bool> PointerEnterUICallback;
    public Action<float> VolumeValueChangedByUICallback;
    public Action ClickLockBtnCallback;

    public void Init()
    {
        IsShow = false;
        SettingsPanel.Init();
        VolumePanel.Init();
        PlayPBPanel.Init();

        IsLive = false;
        PlayBtnChildImage = PlayBtn.transform.GetChild(0).gameObject.GetComponent<Image>();
        PauseBtnChildImage = PauseBtn.transform.GetChild(0).gameObject.GetComponent<Image>();

        EventTriggerListener.Get(BackBtn.gameObject).OnPtClick = ClickBackBtn;
        EventTriggerListener.Get(PlayBtn.gameObject).OnPtClick = PlayOrPause;
        EventTriggerListener.Get(PauseBtn.gameObject).OnPtClick = PlayOrPause;
        EventTriggerListener.Get(NextBtn.gameObject).OnPtClick = ClickNextBtn;
        EventTriggerListener.Get(VolumeBtn.gameObject).OnPtClick = ClickVolumeBtn;
        EventTriggerListener.Get(SettingBtn.gameObject).OnPtClick = ClickSettingBtn;
        EventTriggerListener.Get(FocusBtn.gameObject).OnPtClick = ClickFocusBtn;
        EventTriggerListener.Get(LockBtn.gameObject).OnPtClick = ClickLockBtn;
        VolumePanel.ChangVolumePercentCallback = VolumeValueChanged;

        EventTriggerListener.Get(this.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(this.gameObject).OnPtExit = OnPointerExit;
        EventTriggerListener.Get(BackBtn.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(BackBtn.gameObject).OnPtExit = OnPointerExit;
        EventTriggerListener.Get(PlayBtn.gameObject).OnPtEnter = OnPointerEnterPlay;
        EventTriggerListener.Get(PlayBtn.gameObject).OnPtExit = OnPointerExitPlay;
        EventTriggerListener.Get(PauseBtn.gameObject).OnPtEnter = OnPointerEnterPause;
        EventTriggerListener.Get(PauseBtn.gameObject).OnPtExit = OnPointerExitPause;
        EventTriggerListener.Get(NextBtn.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(NextBtn.gameObject).OnPtExit = OnPointerExit;
        EventTriggerListener.Get(VolumeBtn.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(VolumeBtn.gameObject).OnPtExit = OnPointerExit;
        EventTriggerListener.Get(VolumePanel.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(VolumePanel.gameObject).OnPtExit = OnPointerExit;
        EventTriggerListener.Get(PlayPBPanel.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(PlayPBPanel.gameObject).OnPtExit = OnPointerExit;
        EventTriggerListener.Get(SettingBtn.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(SettingBtn.gameObject).OnPtExit = OnPointerExit;
        EventTriggerListener.Get(FocusBtn.gameObject).OnPtEnter = OnPointerEnterFocusBtn;
        EventTriggerListener.Get(FocusBtn.gameObject).OnPtExit = OnPointerExitFocusBtn;
        EventTriggerListener.Get(LockBtn.gameObject).OnPtEnter = OnPointerEnterLockBtn;
        EventTriggerListener.Get(LockBtn.gameObject).OnPtExit = OnPointerExitLockBtn;

        PlayerBtnIsReplayTextWhenKTTV(false);
        SetLockBtnInteractable(IsEnableLockBtn);
    }

    public void PlayerBtnIsReplayTextWhenKTTV(bool isRestart)
    {
        //if (isRestart)
        //    PlayBtn.transform.GetChild(1).GetComponent<Text>().SetTextByKey("Cinema.SvrVideoPlayer.VideoControlPanel.PlayBtn.RePlay.Text");
        //else
        //    PlayBtn.transform.GetChild(1).GetComponent<Text>().SetTextByKey("Cinema.SvrVideoPlayer.VideoControlPanel.PlayBtn.Text");
    }

    /// <summary>
    /// 播放/暂停按钮状态控制
    /// </summary>
    /// <param name="isOnline">true:disable按钮, false:enable</param>
    public void PlayBtnControl(bool isOnline)
    {
        //if (IsLive == isOnline) return;

        //IsLive = isOnline;
        if (isOnline) //直播
        {
            PlayBtn.interactable = false;
            PlayBtn.GetComponent<Image>().raycastTarget = false;
            PauseBtn.interactable = false;
            PauseBtn.GetComponent<Image>().raycastTarget = false;
            PlayPBPanel.CurrentTimeText.gameObject.SetActive(false);
            PlayPBPanel.TotalTimeText.gameObject.SetActive(false);
            PlayPBPanel.PlayPBSlider.gameObject.SetActive(false);
            PlayPBPanel.PlayPBSliderBg.raycastTarget = false;
            PlayBtnChildImage.color = new Color(PlayBtnChildImage.color.r, PlayBtnChildImage.color.g, PlayBtnChildImage.color.b, 0.3f);
            PauseBtnChildImage.color = new Color(PauseBtnChildImage.color.r, PauseBtnChildImage.color.g, PauseBtnChildImage.color.b, 0.3f);
        }
        else //点播或本地
        {
            PlayBtn.interactable = true;
            PlayBtn.GetComponent<Image>().raycastTarget = true;
            PauseBtn.interactable = true;
            PauseBtn.GetComponent<Image>().raycastTarget = true;
            PlayPBPanel.CurrentTimeText.gameObject.SetActive(true);
            PlayPBPanel.TotalTimeText.gameObject.SetActive(true);
            PlayPBPanel.PlayPBSlider.gameObject.SetActive(true);
            PlayPBPanel.PlayPBSliderBg.raycastTarget = true;
            PlayBtnChildImage.color = new Color(PlayBtnChildImage.color.r, PlayBtnChildImage.color.g, PlayBtnChildImage.color.b, 1);
            PauseBtnChildImage.color = new Color(PauseBtnChildImage.color.r, PauseBtnChildImage.color.g, PauseBtnChildImage.color.b, 1);
        }
    }

    public void SetPlayMode(bool isPlay)
    {
        if(isPlay)
        {
            PlayBtn.gameObject.SetActive(false);
            PauseBtn.gameObject.SetActive(true);
            PauseBtn.gameObject.transform.DOScale(Vector3.one, GlobalVariable.AnimationSpendTime);
        }
        else
        {
            PlayBtn.gameObject.SetActive(true);
            PlayBtn.gameObject.transform.DOScale(Vector3.one, GlobalVariable.AnimationSpendTime);
            PauseBtn.gameObject.SetActive(false);
        }
    }

    public void SetVideoName(string name)
    {
        //VideoNameText.Init(name);
        VideoNameText.text = name;
    }
    
    void ClickBackBtn(GameObject go)
    {
        if (ClickBackBtnCallback != null)
            ClickBackBtnCallback();
    }

    void PlayOrPause(GameObject go)
    {
        if (IsLive) return;

        VolumePanel.Hide();
        SettingsPanel.Hide();
        if (DOTween.IsTweening("PlayOrPause"))
            DOTween.Kill("PlayOrPause");
        go.transform.DOScale(Vector3.one * 0.8f, GlobalVariable.AnimationSpendTime)
            .SetId("PlayOrPause")
            .OnComplete(() => MoveEnd(go));
    }

    void MoveEnd(GameObject go)
    {
        go.SetActive(false);
        if (go == PlayBtn.gameObject)
        {
            Cinema.IsPlayOrPauseVideoPlayer = true;
            PauseBtn.gameObject.transform.DOScale(Vector3.one, GlobalVariable.AnimationSpendTime);

            if (Cinema.IsPlayEndWhenKTTVModel)
            {
                if (ClickPlayBtnAndReplayCallback != null)
                    ClickPlayBtnAndReplayCallback();
            }
            else
            {
                if (ClickPlayBtnCallback != null)
                    ClickPlayBtnCallback(true);
            }
            PauseBtn.gameObject.SetActive(true);
        }
        else
        {
            Cinema.IsPlayOrPauseVideoPlayer = false;
            PlayBtn.gameObject.transform.DOScale(Vector3.one, GlobalVariable.AnimationSpendTime);

            if (ClickPauseBtnCallback != null)
                ClickPauseBtnCallback();
            PlayBtn.gameObject.SetActive(true);
        }
    }

    void ClickPreviousBtn(GameObject go)
    {
        VolumePanel.Hide();
        SettingsPanel.Hide();
        Statistics.GetInstance().OnEvent(MediaCenterEvent.PreviousVideo, "上一个视频");

        if (ClickPreviousBtnCallback != null)
            ClickPreviousBtnCallback();
    }

    void ClickNextBtn(GameObject go)
    {
        VolumePanel.Hide();
        SettingsPanel.Hide();
        Statistics.GetInstance().OnEvent(MediaCenterEvent.NextVideo, "下一个视频");

        if (ClickNextBtnCallback != null)
            ClickNextBtnCallback();
    }

    void ClickVolumeBtn(GameObject go)
    {
        SettingsPanel.Hide();
        VolumePanel.SetVolumePanelStatus();
    }

    void ClickSettingBtn(GameObject go)
    {
        VolumePanel.Hide();
        SettingsPanel.SetSettingsPanelStatus();
    }

    void ClickFocusBtn(GameObject go)
    {
        Cinema.IsInFocus = !Cinema.IsInFocus;
        SettingsPanel.Hide();
        VolumePanel.Hide();

        if (!Cinema.IsInFocus)
        {
            CinemaGlobalToastCanvasControl.GetInstance().GlobalToast.Hide();
            if (!IsEnterFocus)
                OnPointerExitFocusBtn(go);
        }
        else
            CinemaGlobalToastCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.VideoControlPanel.FocusBtn.Toast", -1);
    }

    public void FocusComplete()
    {
        ClickFocusBtn(FocusBtn.gameObject);
        CinemaGlobalToastCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.Recentered.Toast");
    }

    void ClickLockBtn(GameObject go)
    {
        Cinema.IsFirstInLockAngle = true;
        Cinema.IsInLockAngle = !Cinema.IsInLockAngle;
        go.transform.GetChild(0).GetComponent<Image>().sprite = LockNorSprite;
        go.transform.GetChild(1).gameObject.SetActive(false);
        SettingsPanel.Hide();
        VolumePanel.Hide();

        if (ClickLockBtnCallback != null)
            ClickLockBtnCallback();
    }

    void VolumeValueChanged(float value, bool IsVoluntary)
    {
        if (value <= 0)
            VolumeIsMute();
        else
            VolumeNotMute();

        if (IsVoluntary) //by UI
        {
            if (VolumeValueChangedByUICallback != null)
                VolumeValueChangedByUICallback(value);
        }
    }

    public void VolumeIsMute() // 静音
    {
        VolumeImages[0].gameObject.SetActive(false);
        VolumeImages[1].gameObject.SetActive(true);
    }

    public void VolumeNotMute()
    {
        VolumeImages[0].gameObject.SetActive(true);
        VolumeImages[1].gameObject.SetActive(false);
    }

    void OnPointerEnter(GameObject go)
    {
        if (PointerEnterUICallback != null)
            PointerEnterUICallback(true);
    }

    void OnPointerEnterPlay(GameObject go)
    {
        if (IsLive) return;
        go.transform.GetChild(0).GetComponent<Image>().sprite = PlayBtnHovrSprite;
        go.transform.GetChild(1).gameObject.SetActive(true);
        OnPointerEnter(go);
    }

    void OnPointerExitPlay(GameObject go)
    {
        if (IsLive) return;
        go.transform.GetChild(0).GetComponent<Image>().sprite = PlayBtnNorSprite;
        go.transform.GetChild(1).gameObject.SetActive(false);
        OnPointerExit(go);
    }

    void OnPointerEnterPause(GameObject go)
    {
        if (IsLive) return;
        go.transform.GetChild(0).GetComponent<Image>().sprite = PauseBtnHovrSprite;
        go.transform.GetChild(1).gameObject.SetActive(true);
        OnPointerEnter(go);
    }

    void OnPointerExitPause(GameObject go)
    {
        if (IsLive) return;
        go.transform.GetChild(0).GetComponent<Image>().sprite = PauseBtnNorSprite;
        go.transform.GetChild(1).gameObject.SetActive(false);
        OnPointerExit(go);
    }

    void OnPointerEnterFocusBtn(GameObject go)
    {
        IsEnterFocus = true;
        go.transform.GetChild(0).GetComponent<Image>().sprite = FocusHoverSprite;
        OnPointerEnter(go);
    }
    
    void OnPointerExitFocusBtn(GameObject go)
    {
        IsEnterFocus = false;
        if (Cinema.IsInFocus) return;

        go.transform.GetChild(0).GetComponent<Image>().sprite = FocusNorSprite;
        OnPointerExit(go);
    }

    void OnPointerEnterLockBtn(GameObject go)
    {
        if (!IsEnableLockBtn) return;
        go.transform.GetChild(0).GetComponent<Image>().sprite = LockBtnHovrSprite;
        go.transform.GetChild(1).gameObject.SetActive(true);
        OnPointerEnter(go);
    }

    void OnPointerExitLockBtn(GameObject go)
    {
        if (!IsEnableLockBtn) return;
        go.transform.GetChild(0).GetComponent<Image>().sprite = LockBtnNorSprite;
        go.transform.GetChild(1).gameObject.SetActive(false);
        OnPointerExit(go);
    }

    void OnPointerExit(GameObject go)
    {
        if (PointerEnterUICallback != null)
            PointerEnterUICallback(false);
    }

    public void SetVideoTotalTime(long time)
    {
        PlayPBPanel.SetTotalTime(time);
    }

    public void SetVideoCurrentTime(long time)
    {
        PlayPBPanel.SetCurrentTime(time);
    }

    public void SetLockBtnInteractable(bool isEnable)
    {
        IsEnableLockBtn = isEnable;
        LockBtn.interactable = isEnable;
        LockBtn.GetComponent<Image>().raycastTarget = isEnable;
        Image LockBtnChildImage = LockBtn.transform.GetChild(0).gameObject.GetComponent<Image>();

        if (!isEnable)
        {
            LockBtnChildImage.sprite = LockBtnNorSprite;
            LockBtnChildImage.color = new Color(LockBtnChildImage.color.r, LockBtnChildImage.color.g, LockBtnChildImage.color.b, 0.3f);
        }
        else
            LockBtnChildImage.color = new Color(LockBtnChildImage.color.r, LockBtnChildImage.color.g, LockBtnChildImage.color.b, 1);
    }

    public void Show()
    {
        if (IsShow)
            return;

        IsShow = true;
        gameObject.SetActive(true);
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV 
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            NextBtn.interactable = false;
            NextBtn.GetComponent<Image>().raycastTarget = false;
            Image NextBtnChildImage = NextBtn.transform.GetChild(0).gameObject.GetComponent<Image>();
            NextBtnChildImage.color = new Color(NextBtnChildImage.color.r, NextBtnChildImage.color.g, NextBtnChildImage.color.b, 0.3f);
        }
    }

    public void Hide()
    {
        if (!IsShow)
            return;

        IsShow = false;
        VolumePanel.Hide();
        SettingsPanel.Hide();
        gameObject.SetActive(false);
    }

    public void ResetUI()
    {
        SetVideoCurrentTime(0);
        SetPlayMode(false);
        VolumePanel.Hide();
        SettingsPanel.Hide();
    }

    private void OnDestroy()
    {
        BackBtn = null;
        PlayBtn = null;
        PauseBtn = null;

        NextBtn = null;
        VolumeBtn = null;
        FocusBtn = null;
        SettingBtn = null;
        VideoNameText = null;
        VolumeImages = null;

        PlayPBPanel = null;
        VolumePanel = null;
        SettingsPanel = null;

        PlayBtnHovrSprite = null;
        PlayBtnNorSprite = null;
        PauseBtnHovrSprite = null;
        PauseBtnNorSprite = null;

        PlayBtnChildImage = null;
        PauseBtnChildImage = null;

        ClickBackBtnCallback = null;
        ClickPlayBtnAndReplayCallback = null;
        ClickPlayBtnCallback = null;
        ClickPauseBtnCallback = null;
        ClickPreviousBtnCallback = null;
        ClickNextBtnCallback = null;
        PointerEnterUICallback = null;
        VolumeValueChangedByUICallback = null;
    }
}

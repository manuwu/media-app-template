/*
 * Author:李传礼 / 黄秋燕 Shemi
 * DateTime:2017.12.05
 * Description:影院面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CinemaPanel : MonoBehaviour
{
    public Cinema Cinema;
    public VideoPlayerPanel VideoPlayerPanel;

    AndroidJavaObject AndroidMediaScanInterface;
    AndroidJavaObject AndroidPlayerInterface;
    JVideoDescriptionInfo CurJVideoInfo;
    JImageDescriptionInfo CurJImageInfo;

    bool isInit;
    float from = 0;
    float to = 1;
    float step = -20;
    bool Focus = true;
    bool Pause = false;
    private static bool mpreOEM = false;
    private static bool moem = true;
    public static bool IsOEM
    {
        get
        {
            return moem;
        }
    }
    public static UnityEvent OnOEMEvent = new UnityEvent();

    private static bool lastLoop = false;
    private static bool playLoop = false;
    public static bool IsPlayLoop
    {
        get
        {
            return playLoop;
        }
    }
    public static UnityEvent OnPlayLoopEvent = new UnityEvent();

    bool IsBackLauncher = false;
    bool isEnterAdFirst = false;
    bool IsChangeDefinitionModel = false; //是否为切换清晰度
    DefinitionModel CurDefn = DefinitionModel.UNKOWN;

    void Awake()
    {
        if (PlayerDataControl.GetInstance().CurPlayingMode != PlayingURLMode.KTTV)
            InitCinemaNativeInterface("playermanager");

        AnalizePlayMode();
    }

    void Start()
    {
        Statistics.GetInstance().Start();

        CurJVideoInfo = null;
        CurJImageInfo = null;
        //ClearMovieSubtitle();

        Init();

        StartPlayNewVideo();
        if (Svr.SvrSetting.IsVR9Device)
            Application.targetFrameRate = 30;
    }

    private void OnEnable()
    {
        StartPlayNewVideo();
    }

    private void Update()
    {
        if (Cinema.IsInLockAngle && !VideoPlayerPanel.IsUIShowing())
        {
            UpdateUIAngleDir();
        }
    }

    private void StartPlayNewVideo()
    {
        PlayingURLMode curPlayingMode = PlayerDataControl.GetInstance().CurPlayingMode;
        if ((curPlayingMode == PlayingURLMode.KTTV || curPlayingMode == PlayingURLMode.LiveUrl)
            && !GlobalVariable.IsInternetReachability())
        {
            ShowErrorMessageAndBackList(ExceptionEvent.EXCEPTION_NETWORK_ERROR, "Cinema.SvrVideoPlayer.OnExceptionEvent.Video.NETWORK_ERROR");
            return;
        }

        Cinema.IsPlayEndWhenKTTVModel = false;
        if (curPlayingMode == PlayingURLMode.Local)
            PlayNewVideo(PlayerDataControl.GetInstance().GetCurPlayIndex());
        else if (curPlayingMode == PlayingURLMode.KTTV)
            PlayNewVideo(PlayerDataControl.GetInstance().GetKKTVVideoVidCid());
        else if (curPlayingMode == PlayingURLMode.LiveUrl)
            PlayNewVideo(PlayerDataControl.GetInstance().GetJVideoDscpInfoByLiveUrl());
    }

    private void Init()
    {
        if (isInit)
            return;

        isInit = true;

        Cinema.Init();
        VideoPlayerPanel.Init();
        BindEvents();
    }

    private void ReleaseInterface()
    {
        if (PlayerDataControl.GetInstance().CurPlayingMode != PlayingURLMode.KTTV)
        {
            if (AndroidPlayerInterface != null)
                AndroidPlayerInterface.Call("release");
        }
    }

    void InitCinemaNativeInterface(string packageName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass nativeInterfaceClass = new AndroidJavaClass(string.Format("com.ssnwt.vr.{0}.jni.NativeInterface", packageName));
        if (nativeInterfaceClass == null)
            return;
        AndroidJavaObject AndroidInterface = nativeInterfaceClass.CallStatic<AndroidJavaObject>("getInstance");
        if (AndroidInterface == null)
            return;

        AndroidInterface.Call("init", GlobalAppManage.GetJApplication());

        switch (packageName)
        {
            case "mediascan":
                AndroidMediaScanInterface = AndroidInterface;
                break;
            case "playermanager":
                AndroidPlayerInterface = AndroidInterface;
                break;
        }
        LogTool.Log("AndroidInterface通过jApplication初始化, InitCinemaNativeInterface");
#endif
    }

    void BindEvents()
    {
        Cinema.RecenterCallback = Recenter;
        Cinema.ResetUIDirCallback = ShowVideoPlayerUI;
        Cinema.SwitchVideoPlayerUIVisionCallback = SwitchVideoPlayerUIVision;
        Cinema.FocusCompleteCallback = FocusComplete;
        Cinema.VideoPlayer.OnReady = VideoReadyComplete;
        Cinema.VideoPlayer.OnEnd = VideoPlayComplete;
        Cinema.VideoPlayer.OnVolumeChange = VideoVolumeChangedEvent;
        Cinema.VideoPlayer.OnVideoError = ShowErrorMessageAndBackList;
        Cinema.VideoPlayer.OnProgressChange = UpdatePlayingProgress;
        Cinema.VideoPlayer.ScreenSizeBtnStatusControlCallback += VideoSettingsUIControl;
        //Cinema.VideoPlayer.ScreenSizeBtnStatusControlCallback += SubtitleUIControl;
        Cinema.VideoPlayer.TensileBtnStatusControlCallback = TensileBtnStatusControl;
        Cinema.VideoPlayer.FinishIntent = BackLocalMediaPanel;
        Cinema.VideoPlayer.SetSubtitle = SetMovieSubtitle;
        Cinema.VideoPlayer.ReleaseInterfaceCallback = ReleaseInterface;
        Cinema.VideoPlayer.DMR_PlayVideoCallback = PlayVideo;
        Cinema.VideoPlayer.DMR_PauseVideoCallback = DmrPauseVideo;
        Cinema.VideoPlayer.ChangeSceneStyleCallback = ChangeSceneStyle;
        Cinema.VideoPlayer.AdRemainingTimeCallback = EnableOrEndAd;
        Cinema.VideoPlayer.DefinitionListCallback = GetDefinitionList;
        Cinema.VideoPlayer.VideoDurationCallback = UpdateVideoTotalTime;
        Cinema.VideoPlayer.OnBufferProgressChange = OnBufferProgress;
        Cinema.VideoPlayer.OnBufferStart = OnBufferStartShowLoading;
        Cinema.VideoPlayer.OnBufferFinish = OnBufferFinishHideLoading;

        VideoPlayerPanel.ChangeStereoTypeCallback = ChangeStereoType;
        VideoPlayerPanel.PointerEnterUICallback = PointerEnterVideoPlayerUI;
        VideoPlayerPanel.GetVideoCurrentTimePtr = Cinema.VideoPlayer.GetCurrentPosition;
        VideoPlayerPanel.UIRecenterCallback = RecenterControlUI;

        VideoPlayerPanel.VideoCtrlPanel.ClickBackBtnCallback += BackLocalMediaPanel;
        VideoPlayerPanel.VideoCtrlPanel.ClickPlayBtnCallback = PlayVideo;
        VideoPlayerPanel.VideoCtrlPanel.ClickPlayBtnAndReplayCallback = StartPlayNewVideo;
        VideoPlayerPanel.VideoCtrlPanel.ClickPauseBtnCallback = PauseVideo;
        VideoPlayerPanel.VideoCtrlPanel.ClickPreviousBtnCallback = PlayPreviousVideo;
        VideoPlayerPanel.VideoCtrlPanel.ClickNextBtnCallback = PlayNextVideo;
        VideoPlayerPanel.VideoCtrlPanel.PlayPBPanel.SeekToTimeCallback = SeekToTime;
        VideoPlayerPanel.VideoCtrlPanel.VolumeValueChangedByUICallback = VolumeValueChanged;
        VideoPlayerPanel.VideoCtrlPanel.PlayPBPanel.SliderCheckStatusCallback = SliderCheckStatus;
        VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.SceneChangePanel.ChangeSceneStyleCallback = ChangeSceneStyle;
        VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.LoopModePanel.ChangeLoopTypeCallback = SelectLoopType;
		VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.LoopModePanel.ChangeAutoPlayCallback = SetPlayMode;
        VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.StereoTypePanel.TensileSwitchCallback = StretchingPicture;
        VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.DefinitionPanel.ChangeDefinitionModelCallback += ChangeDefinitionModel;
        VideoPlayerPanel.VideoCtrlPanel.ClickLockBtnCallback = LockAngleEvent;
        VideoPlayerPanel.ShowUICallback = LoadSceneWhenShowUI;

        //HomeButtonListener.Instance.HomeButtonCallback += HomeButtonBack;
        //HomeButtonListener.Instance.BackButtonCallback += BackButtonBack;
        PlayerDataControl.GetInstance().InterruptPlayer += BackLocalMediaPanel;
    }

    void Recenter()
    {
        UpdateUIAngleDir();
        VideoPlayerPanel.SwitchUIVision();
    }

    void ShowVideoPlayerUI()
    {
        VideoPlayerPanel.ShowUI();
        VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.Hide();
        ResetUIDir();
    }

    void SwitchVideoPlayerUIVision()
    {
        VideoPlayerPanel.SwitchUIVision();
        ResetUIDir();
    }

    void FocusComplete()
    {
        UpdateUIAngleDir();

        VideoPlayerPanel.VideoCtrlPanel.FocusComplete();
        VideoPlayerPanel.AutoHideUI();
    }

    void LockAngleEvent()
    {
        if (Cinema.IsInLockAngle)
        {
            CinemaGlobalToastCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.LockAngle.Quit.Toast");
            Cinema.IsPointerEnterVideoPlayerUI = false;

            VideoPlayerPanel.HideUI();
            UpdateUIAngleDir();
            //场景消失
            //CinemaMaterialSetting.GetInstance().UnLoadRender();
            //if (CinemaMaterialSetting.GetInstance().ImaxPurple.activeInHierarchy)
            //    CinemaMaterialSetting.GetInstance().ImaxPurple.SetActive(false);
            //if (CinemaMaterialSetting.GetInstance().DriveSceneBox.activeInHierarchy)
            //    CinemaMaterialSetting.GetInstance().DriveSceneBox.SetActive(false);
        }
    }

    void UpdateUIAngleDir()
    {
        Vector3 dir = Camera.main.transform.forward;
        Quaternion qua = Camera.main.transform.rotation;
        Cinema.CinemaUI.transform.forward = dir;

        Vector3 valueDir = Vector3.Cross(Vector3.forward, dir);
        float valueAngle = Vector3.Angle(Vector3.forward, dir);
        Vector3 cToUVector = PreDefScrp.RotateAroundAxis(Cinema.CameraToUIPositionVector, valueDir, valueAngle);
        Cinema.CinemaUI.transform.position = Camera.main.transform.position + cToUVector;
        Cinema.CinemaUI.transform.rotation = qua;

        Vector3 cToUVector2 = PreDefScrp.RotateAroundAxis(Cinema.CameraToPlayerPositionVector, valueDir, valueAngle);
        PlayerGameobjectControl.Instance.QuadScreen.transform.parent.forward = dir;
        PlayerGameobjectControl.Instance.QuadScreen.transform.parent.position = Camera.main.transform.position + cToUVector2;
        PlayerGameobjectControl.Instance.QuadScreen.transform.parent.rotation = qua;
        //CinemaMaterialSetting.GetInstance().ImaxPurple.transform.forward = dir;
        //CinemaMaterialSetting.GetInstance().ImaxPurple.transform.rotation = qua;
        //CinemaMaterialSetting.GetInstance().DriveSceneBox.transform.forward = dir;
        //CinemaMaterialSetting.GetInstance().DriveSceneBox.transform.rotation = qua;
    }

    void ResetUIDir()
    {
        Vector3 dir = Vector3.zero;
        Quaternion qua = Quaternion.identity;
        if (CurJVideoInfo.stereoType == (int)StereoType.ST2D || CurJVideoInfo.stereoType == (int)StereoType.ST3D_LR || CurJVideoInfo.stereoType == (int)StereoType.ST3D_TB)
        {
            dir = PlayerGameobjectControl.Instance.QuadScreen.transform.parent.forward;
            qua = PlayerGameobjectControl.Instance.QuadScreen.transform.parent.rotation;
        }
        else if (CurJVideoInfo.stereoType == (int)StereoType.ST180_2D || CurJVideoInfo.stereoType == (int)StereoType.ST180_LR
            || CurJVideoInfo.stereoType == (int)StereoType.ST180_TB || CurJVideoInfo.stereoType == (int)StereoType.ST360_2D
            || CurJVideoInfo.stereoType == (int)StereoType.ST360_LR || CurJVideoInfo.stereoType == (int)StereoType.ST360_TB)
        {
            dir = Camera.main.transform.forward;
            qua = Camera.main.transform.rotation;
        }

        Cinema.CinemaUI.transform.forward = dir;

        Vector3 valueDir = Vector3.Cross(Vector3.forward, dir);
        float valueAngle = Vector3.Angle(Vector3.forward, dir);
        Vector3 cToUVector = PreDefScrp.RotateAroundAxis(Cinema.CameraToUIPositionVector, valueDir, valueAngle);
        Cinema.CinemaUI.transform.position = Camera.main.transform.position + cToUVector;
        Cinema.CinemaUI.transform.rotation = qua;
        Debug.Log("ResetUIDir");
    }

    void SetStereoTypeForUI(int stereoType = -1)
    {
        int type;
        if (stereoType == -1)
        {
            type = CurJVideoInfo.stereoType;
            if (type > 8)
                type = 6;

            if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local)
            {
                if (VideoFormatDictionaryDetector.GetInstance().HasVideoFormatOrNotByVideoId(CurJVideoInfo.id.ToString()))
                    type = VideoFormatDictionaryDetector.GetInstance().GetVideoFormatTypeByVideoId(CurJVideoInfo.id.ToString());
            }
            else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            {
                string id = string.Format("{0}-{1}", CurJVideoInfo.vid, CurJVideoInfo.cid);
                if (VideoFormatDictionaryDetector.GetInstance().HasVideoFormatOrNotByVideoId(id))
                    type = VideoFormatDictionaryDetector.GetInstance().GetVideoFormatTypeByVideoId(id);
            }
            else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
            {
                if (VideoFormatDictionaryDetector.GetInstance().HasVideoFormatOrNotByVideoId(CurJVideoInfo.uri))
                    type = VideoFormatDictionaryDetector.GetInstance().GetVideoFormatTypeByVideoId(CurJVideoInfo.uri);
            }
        }
        else
            type = stereoType;

        CurJVideoInfo.stereoType = type;
        LogTool.Log("播放格式：" + (StereoType)type);
        Cinema.VideoPlayer.SetPlayMode((StereoType)type);
        ImaxPurpleControlByStereoType((StereoType)type);
        VideoPlayerPanel.ShowUI();
        WindowStereoTypeUIReset();
    }

    void EnableOrEndAd(long remainingTime)
    {
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
        {
            if (remainingTime > 0 && !isEnterAdFirst)
            {
                isEnterAdFirst = true;
                VideoPlayerPanel.VideoCtrlPanel.PlayBtnControl(true);
                VideoPlayerPanel.VideoCtrlPanel.PlayPBPanel.EnableOrDisableSlider(true);
            }
            else if (remainingTime <= 0 && isEnterAdFirst)
            {
                isEnterAdFirst = false;
                VideoPlayerPanel.VideoCtrlPanel.PlayBtnControl(false);
                VideoPlayerPanel.VideoCtrlPanel.PlayPBPanel.EnableOrDisableSlider(false);
            }
        }
    }

    //void VideoPreAdStart(bool isEnterAd)
    //{
    //    if (CurJVideoInfo != null)
    //    {
    //        SetStereoTypeForUI(0);
    //    }
    //    else
    //        Invoke("BackLocalMediaPanel", 1);
    //}
    
    void GetDefinitionList(List<MifengPlayer.DefnInfo> list, MifengPlayer.DefnInfo current)
    {
        //if (!current.defn.Equals("fhd") && !VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.DefinitionPanel.IsFirstSetDefn())
        //    ChangeDefinitionModel(DefinitionModel.FHD, true, false);
        //else
            VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.DefinitionPanel.SetDefnList(list, current);
        ChangeDefinitionSuccessEvent();
    }

    void VideoReadyComplete()
    {
        long totalTime = Cinema.VideoPlayer.GetVideoDuration();
        VideoPlayerPanel.VideoCtrlPanel.SetVideoTotalTime(totalTime);
        LogTool.Log("CurJVideoInfo = " + CurJVideoInfo.name + " " + CurJVideoInfo.stereoType);

        if (CurJVideoInfo != null)
        {
            SetStereoTypeForUI();
            RecordVideoPlayEvent(CurJVideoInfo);
            VideoPlayerPanel.VideoCtrlPanel.PlayBtnControl(false);
            VideoPlayerPanel.VideoCtrlPanel.PlayPBPanel.ShowOrHideSlider(true);
            VideoPlayerPanel.VideoCtrlPanel.PlayPBPanel.EnableOrDisableSlider(false);
        }
        else
            Invoke("BackLocalMediaPanel", 1);
    }

    void UpdateVideoTotalTime(long time)
    {
        long totalTime = 0;
        if (time != 0)
            totalTime = time;
        else
            totalTime = Cinema.VideoPlayer.GetVideoDuration();

        VideoPlayerPanel.VideoCtrlPanel.SetVideoTotalTime(totalTime);
        isEnterAdFirst = false;
        VideoPlayerPanel.VideoCtrlPanel.PlayBtnControl(false);
        VideoPlayerPanel.VideoCtrlPanel.PlayPBPanel.EnableOrDisableSlider(false);
    }

    /// <summary>
    /// 缓冲进度
    /// </summary>
    /// <param name="bufferPercent">缓冲百分比</param>
    /// <param name="downloadSpeed">缓冲速度（带单位）</param>
    private void OnBufferProgress(float bufferPercent, string downloadSpeed)
    {
        //CinemaCustomizeCanvasControl.GetInstance().SetDownloadSpeed(downloadSpeed);
    }

    void OnBufferStartShowLoading()
    {
        if (IsInvoking("BufferLoading"))
            CancelInvoke("BufferLoading");
        if (IsInvoking("BufferHidLoading"))
            CancelInvoke("BufferHidLoading");

        Invoke("BufferLoading", 0.7f);
    }

    private void ChooseCinemaCnvasTrans()
    {
        if (CurJVideoInfo == null || (CurJVideoInfo.stereoType < 0 || CurJVideoInfo.stereoType > 8)) return;
        if (CurJVideoInfo.stereoType == (int)StereoType.ST2D || CurJVideoInfo.stereoType == (int)StereoType.ST3D_LR || CurJVideoInfo.stereoType == (int)StereoType.ST3D_TB)
        {
            CinemaTipsCanvasControl.GetInstance().transform.parent = PlayerGameobjectControl.Instance.QuadScreen.transform;
            CinemaTipsCanvasControl.GetInstance().transform.localRotation = Quaternion.identity;

            if (GlobalVariable.GetSceneModel() == SceneModel.Default
            || GlobalVariable.GetSceneModel() == SceneModel.StarringNight
            || GlobalVariable.GetSceneModel() == SceneModel.IMAXTheater)
            {
                CinemaTipsCanvasControl.GetInstance().transform.localPosition = new Vector3(0, -0.07f, -17);
                CinemaTipsCanvasControl.GetInstance().transform.localScale = new Vector3(0.0003087959f, 0.0005483741f, 0.008f);
            }
            else if (GlobalVariable.GetSceneModel() == SceneModel.Drive)
            {
                DriveSceneModel driveModel = GlobalVariable.GetDriveSceneModel();
                switch (driveModel)
                {
                    case DriveSceneModel.Karting:
                        CinemaTipsCanvasControl.GetInstance().transform.localPosition = new Vector3(-0.52f, -0.4f, -42);
                        CinemaTipsCanvasControl.GetInstance().transform.localScale = new Vector3(0.0002222223f, 0.0003960396f, 0.008f);
                        break;
                    case DriveSceneModel.King:
                        CinemaTipsCanvasControl.GetInstance().transform.localPosition = new Vector3(0, -0.4f, -42);
                        CinemaTipsCanvasControl.GetInstance().transform.localScale = new Vector3(0.0001369315f, 0.0002769398f, 0.008f);
                        break;
                    case DriveSceneModel.Rattletrap:
                        CinemaTipsCanvasControl.GetInstance().transform.localPosition = new Vector3(0.478f, -0.418f, -42);
                        CinemaTipsCanvasControl.GetInstance().transform.localScale = new Vector3(0.0003001876f, 0.0005333334f, 0.008f);
                        break;
                    default: //DriveSceneModel.Playboy
                        CinemaTipsCanvasControl.GetInstance().transform.localPosition = new Vector3(0, -0.4f, -42);
                        CinemaTipsCanvasControl.GetInstance().transform.localScale = new Vector3(0.0002191781f, 0.0003809524f, 0.008f);
                        break;
                }
            }
        }
        else
        {
            CinemaTipsCanvasControl.GetInstance().CinemaCanvasNormalTrans();
            CinemaCustomizeCanvasControl.GetInstance().SetDownloadSpeed("0 B/s");
        }
    }

    void BufferLoading()
    {
        //ChooseCinemaCnvasTrans();
        //CinemaCustomizeCanvasControl.GetInstance().LoadingPanel.SetActive(true);
        //Invoke("BufferHidLoading", 20f);
    }

    void BufferHidLoading()
    {
        //CinemaCustomizeCanvasControl.GetInstance().LoadingPanel.SetActive(false);
        //CinemaCustomizeCanvasControl.GetInstance().SetDownloadSpeed("0 B/s");
    }

    void OnBufferFinishHideLoading()
    {
        //if (IsInvoking("BufferLoading"))
        //    CancelInvoke("BufferLoading");
        //if (IsInvoking("BufferHidLoading"))
        //    CancelInvoke("BufferHidLoading");

        //BufferHidLoading();
    }

    void ChangeDefinitionSuccessEvent()
    {
        if (IsChangeDefinitionModel) 
        {
            IsChangeDefinitionModel = false;
            ChooseCinemaCnvasTrans();
            switch (CurDefn)
            {
                case DefinitionModel.DEFINITION_4K:
                    CinemaTipsCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.ChangeDefinitionSuccess", "4K");
                    break;
                case DefinitionModel.DEFINITION_1080P:
                    CinemaTipsCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.ChangeDefinitionSuccess", "1080P");
                    break;
                case DefinitionModel.DEFINITION_720P:
                    CinemaTipsCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.ChangeDefinitionSuccess", "720P");
                    break;
                default:
                    CinemaTipsCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.ChangeDefinitionSuccess", "720P");
                    break;
            }
        }
    }

    void VideoPlayComplete()
    {
        VideoPlayerPanel.VideoCtrlPanel.SetPlayMode(false);

        //shemi
        //if (GlobalVariable.IsIntent)
        //{
        //    BackLocalMediaPanel();
        //    return;
        //}
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local)
        {
            if (VideoPlayManage.CurLoopType == LoopType.SinglePlay)
            {
                ;
            }
            else if (VideoPlayManage.CurLoopType == LoopType.AutoReplay)
            {
                PlayVideo();
            }
            else if (VideoPlayManage.CurLoopType == LoopType.ListLoop)
            {
                PlayNextVideo();
            }
        }
        else
        {
            //Cinema.IsPlayEndWhenKTTVModel = true;
            //VideoPlayerPanel.VideoCtrlPanel.PlayerBtnIsReplayTextWhenKTTV(true);
            VideoPlayerPanel.VideoCtrlPanel.PlayBtnControl(true);
            VideoPlayerPanel.VideoCtrlPanel.PlayPBPanel.ShowOrHideSlider(false);
            VideoPlayerPanel.VideoCtrlPanel.PlayPBPanel.EnableOrDisableSlider(true);
            CinemaTipsCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.SvrVideoPlayer.OnExceptionEvent.Video.PlayerComplete", -1);
        }
        //else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV
        //    || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        //    BackLocalMediaPanel();
    }

    void RecordVideoPlayEvent(JVideoDescriptionInfo video)
    {
        Common.GetInstance().SetAttributes("视频类型", StereoTypeToWebStereoString((StereoType)video.stereoType));
        int secound = (int)(video.time / 1000);
        Common.GetInstance().SetAttributes("视频时长", string.Format("{0}", secound));
        Statistics.GetInstance().OnEvent(MediaCenterEvent.VideoPlay, "视频播放", 1, Common.GetInstance().GetAttributes());
        Common.GetInstance().ClearAttributes();
    }

    string StereoTypeToWebStereoString(StereoType stereoType)
    {
        string webStereoString = string.Empty;
        switch (stereoType)
        {
            case StereoType.ST2D:
                webStereoString = "Window-2D";
                break;
            case StereoType.ST3D_LR:
                webStereoString = "Window-3D-SS";
                break;
            case StereoType.ST3D_TB:
                webStereoString = "Window-3D-UD";
                break;
            case StereoType.ST360_2D:
                webStereoString = "360-2D";
                break;
            case StereoType.ST360_LR:
                webStereoString = "360-3D-SS";
                break;
            case StereoType.ST360_TB:
                webStereoString = "360-3D-UD";
                break;
            case StereoType.ST180_2D:
                webStereoString = "180-2D";
                break;
            case StereoType.ST180_LR:
                webStereoString = "180-3D-SS";
                break;
            case StereoType.ST180_TB:
                webStereoString = "180-3D-UD";
                break;
        }

        return webStereoString;
    }

    void PointerEnterVideoPlayerUI(bool isEnter)
    {
        //if (Cinema.IsPlayMode)
        {
            if (isEnter)
                VideoPlayerPanel.KeepShowUI();
            else
                VideoPlayerPanel.AutoHideUI();
        }

        Cinema.IsPointerEnterVideoPlayerUI = isEnter;
    }

    void ShowErrorMessageAndBackList(ExceptionEvent eventId, string errMessage)
    {
        Debug.LogFormat("ShowErrorMessageAndBackList {0}:{1}", eventId.ToString(), errMessage);

        if (!IsChangeDefinitionModel)
        {
            if (eventId == ExceptionEvent.PATH_ERROR || eventId == ExceptionEvent.NOT_SUPPORT_FORMAT
                || eventId == ExceptionEvent.NOT_SUPPORT_SIZE || eventId == ExceptionEvent.OTHER)
            {
                CinemaTipsCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey(errMessage, -1);
                Invoke("BackLocalMediaPanel", 4);
            }
            else
                CinemaTipsCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey(errMessage);
        }
        else
        {
            //切换出错
            CinemaTipsCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.ChangeDefinitionFailed");
            Invoke("BackLocalMediaPanel", 4);
        }
    }

    void HomeButtonBack()
    {
        GlobalVariable.IsHomeButtonBackLauncher = true;
        BackLocalMediaPanel();
    }

    void BackButtonBack()
    {
        GlobalVariable.IsHomeButtonBackLauncher = false;
        BackLocalMediaPanel();
    }

    void BackLocalMediaPanel()
    {
        Cinema.GvrHead.trackPosition = true;
        isEnterAdFirst = false;
        Cinema.VideoPlayer.Stop();
        //Cinema.VideoPlayer.Release();
        PlayerDataControl.GetInstance().ClearVideoDscpInfo();
        CinemaTipsCanvasControl.GetInstance().GlobalToast.Hide();
        //CinemaGlobalToastCanvasControl.GetInstance().GlobalToast.Hide();
        //OnBufferFinishHideLoading();
        MediaStretchPlayerPrefsDetector.GetInstance().ResetMediaId();
        //GlobalRunningFunction.Instance.Subtitle.transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform.parent;
        //GlobalRunningFunction.Instance.ShowControllerRayLine();
        //Cinema.ResetSphereScreenDir(); //reset 360 model's dir
        Cinema.VideoPlayer.ClearVideoCache();
        if (PlayerDataControl.GetInstance().StopPlayCallBack != null)
            PlayerDataControl.GetInstance().StopPlayCallBack();
    }

    void SetMovieSubtitle(string title)
    {
        //if (title != null)
        //{
        //    GlobalRunningFunction.Instance.Subtitle.SubtitleFirstText.text = title;
        //}
        //else
        //    ClearMovieSubtitle();
    }

    /// <summary>
    /// 清空字幕
    /// </summary>
    void ClearMovieSubtitle()
    {
        //GlobalRunningFunction.Instance.Subtitle.SubtitleFirstText.text = string.Empty;
        //GlobalRunningFunction.Instance.Subtitle.SubtitleSecondText.text = string.Empty;
    }

    void UpdatePlayingProgress(int curTime)
    {
        VideoPlayerPanel.VideoCtrlPanel.SetVideoCurrentTime(curTime);
    }

    void PlayVideo(bool isManual = false)
    {
        Cinema.VideoPlayer.Play();
        VideoPlayerPanel.PlayVideo(isManual);
    }

    void PauseVideo()
    {
        Cinema.VideoPlayer.Pause();
    }

    void DmrPauseVideo()
    {
        PauseVideo();
        VideoPlayerPanel.VideoCtrlPanel.SetPlayMode(false);
    }

    void PlayPreviousVideo()
    {
        int index = PlayerDataControl.GetInstance().VideoPlayManage.GetPreviousVideoIndex();
        if (index == -1)
            return;

        Cinema.VideoPlayer.Stop();
        PlayNewVideo(index);
    }

    void PlayNextVideo()
    {
        int index = PlayerDataControl.GetInstance().VideoPlayManage.GetNextVideoIndex();
        if (index == -1)
            return;

        Cinema.VideoPlayer.Stop();
        PlayNewVideo(index);
    }

    void SeekToTime(long time)
    {
        Cinema.VideoPlayer.SeekToTime(time);
    }

    void SelectLoopType(bool isLoop)
    {
        LoopType loopType = VideoPlayManage.CurLoopType;
        if (isLoop)
            loopType = LoopType.SinglePlay;
        else
            loopType = LoopType.ListLoop;

        PlayerDataControl.GetInstance().VideoPlayManage.SetLoopType(loopType);
        Cinema.VideoPlayer.SetLoop(isLoop);
    }

    void ChangeStereoType(StereoType stereoType)
    {
        if (/*!GlobalVariable.IsIntent && */!IsPlayLoop)
        {
            if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local)
                VideoFormatDictionaryDetector.GetInstance().SetVideoFormatTypeByVideoId(CurJVideoInfo.id.ToString(), (int)stereoType);
            else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
                VideoFormatDictionaryDetector.GetInstance().SetVideoFormatTypeByVideoId
                    (string.Format("{0}-{1}", CurJVideoInfo.vid, CurJVideoInfo.cid), (int)stereoType);
            else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
                VideoFormatDictionaryDetector.GetInstance().SetVideoFormatTypeByVideoId(CurJVideoInfo.uri, (int)stereoType);
        }
        else if (IsPlayLoop)
        {
            VideoPlayerPanel.SetVideoFormatTypeWhenPlayLoop((int)stereoType);
        }

        if (CurJVideoInfo != null)
            CurJVideoInfo.stereoType = (int)stereoType;
        Cinema.VideoPlayer.SetPlayMode(stereoType);
        ImaxPurpleControlByStereoType(stereoType);
        WindowStereoTypeUIReset();
    }

    private void WindowStereoTypeUIReset()
    {
        if (CurJVideoInfo.stereoType == (int)StereoType.ST2D || CurJVideoInfo.stereoType == (int)StereoType.ST3D_LR || CurJVideoInfo.stereoType == (int)StereoType.ST3D_TB)
        {
            Vector3 dir = PlayerGameobjectControl.Instance.QuadScreen.transform.parent.forward;
            Quaternion qua = PlayerGameobjectControl.Instance.QuadScreen.transform.parent.rotation;
            Cinema.CinemaUI.transform.forward = dir;

            Vector3 valueDir = Vector3.Cross(Vector3.forward, dir);
            float valueAngle = Vector3.Angle(Vector3.forward, dir);
            Vector3 cToUVector = PreDefScrp.RotateAroundAxis(Cinema.CameraToUIPositionVector, valueDir, valueAngle);
            Cinema.CinemaUI.transform.position = Camera.main.transform.position + cToUVector;
            Cinema.CinemaUI.transform.rotation = qua;
        }
    }

    /// <summary>
    /// DMR调用播放
    /// </summary>
    /// <param name="jVdi"></param>
    public void PlayNewVideo(JVideoDescriptionInfo jVdi)
    {
        CurJVideoInfo = null;
        if (jVdi == null)
            return;

        //Cinema.IsPlayMode = true;
        Cinema.IsPlayOrPauseVideoPlayer = true;
        Cinema.VideoPlayer.Stop();
        PlayNewVideoBase(jVdi);

        PlayVideoAfterAnalyze(-1, jVdi);
    }

    /// <summary>
    /// KKTV调用播放
    /// </summary>
    /// <param name="dic"></param>
    void PlayNewVideo(Dictionary<string, string> dic)
    {
        Cinema.IsPlayOrPauseVideoPlayer = true;
        CurJVideoInfo = null;

        JVideoDescriptionInfo jdi = new JVideoDescriptionInfo();
        jdi.vid = dic["vid"];
        jdi.cid = dic["cid"];
        jdi.name = dic["name"];
        jdi.stereoType = int.Parse(dic["stereoType"]);

        PlayNewVideoBase(jdi);
        Cinema.VideoPlayer.PreparedPlayVideo(dic);
    }

    /// <summary>
    /// 本地MediaPlayer调用播放
    /// </summary>
    /// <param name="videoIndex"></param>
    void PlayNewVideo(int videoIndex)
    {
        Cinema.IsPlayOrPauseVideoPlayer = true;
        CurJVideoInfo = null;
        JVideoDescriptionInfo jVdi = PlayerDataControl.GetInstance().GetJVideoDscpInfoByIndex(videoIndex);
        if (jVdi == null)
            return;

        //shemi
        //LocalVideosPanel.VideosViewPanel.ResetCurVideoId(jVdi.id);
        PlayNewVideoBase(jVdi);

        //没有识别过立体格式&&底层没有传送格式&&格式识别的图片不是null
        if ((jVdi.stereoType < 0 || jVdi.stereoType > 8)
            && !VideoFormatDictionaryDetector.GetInstance().HasVideoFormatOrNotByVideoId(jVdi.id.ToString())
            && jVdi.recognitionImagePath != null)
            StartCoroutine(/*Cinema.VideoManage.MediaManage.*/GlobalRunningFunction.Instance.LoadImageAndAnalyze.StartAnalyzeImageThread(jVdi.id, jVdi.recognitionImagePath));
        else
            PlayVideoAfterAnalyze(videoIndex, null);
    }

    void PlayNewVideoBase(JVideoDescriptionInfo jVdi)
    {
        if (IsInvoking("BackLocalMediaPanel"))
            CancelInvoke("BackLocalMediaPanel");

        CurJVideoInfo = jVdi;
        VideoPlayerPanel.SetVideoPlayerState(jVdi);
        VideoPlayerPanel.VideoCtrlPanel.PlayBtnControl(true);
    }

    void PlayVideoAfterAnalyze(int playIndex, JVideoDescriptionInfo jVideo)
    {
        CurJVideoInfo = null;
        JVideoDescriptionInfo jVdi;
        if (jVideo != null)
            jVdi = jVideo;
        else
            jVdi = PlayerDataControl.GetInstance().GetJVideoDscpInfoByIndex(playIndex);

        if (jVdi == null)
            return;
        CurJVideoInfo = jVdi;
        LogTool.Log("播放索引：" + jVdi.uri + " 播放名称：" + jVdi.name);
        Cinema.VideoPlayer.PreparedPlayVideo(jVdi);
    }

    void SliderCheckStatus(bool isPause)
    {
        VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.Hide();
        if (isPause)
            PauseVideo();
        else
            PlayVideo();
    }

    void VolumeValueChanged(float f)
    {
        VideoPlayerPanel.ShowUI();
        Cinema.VideoPlayer.SetVolumePercent(f);
    }

    void VideoVolumeChangedEvent(float f)
    {
        VideoPlayerPanel.VideoCtrlPanel.VolumePanel.ChangeVolumeByDevice(f);
    }

    void RecenterControlUI()
    {
        Cinema.UIRecenterControl(Camera.main.transform.forward);
    }

    /// <summary>
    /// 判断立体格式时，同时判断Setting UI上"屏幕尺寸"&"场景选择"按钮的可选性
    /// </summary>
    /// <param name="stereoType"></param>
    void VideoSettingsUIControl(StereoType stereoType)
    {
        if (stereoType == StereoType.ST2D || stereoType == StereoType.ST3D_LR || stereoType == StereoType.ST3D_TB)
        {
            if (GlobalVariable.GetSceneModel() == SceneModel.Default || GlobalVariable.GetSceneModel() == SceneModel.StarringNight)
                VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.ScreenSizeBtnStatusControl(true);
            else if (GlobalVariable.GetSceneModel() == SceneModel.IMAXTheater || GlobalVariable.GetSceneModel() == SceneModel.Drive)
                VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.ScreenSizeBtnStatusControl(false);

            VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.SceneChangeBtnStatusControl(true);
        }
        else if (stereoType == StereoType.ST180_2D || stereoType == StereoType.ST180_LR || stereoType == StereoType.ST180_TB ||
            stereoType == StereoType.ST360_2D || stereoType == StereoType.ST360_LR || stereoType == StereoType.ST360_TB)
        {
            VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.ScreenSizeBtnStatusControl(false);
            VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.SceneChangeBtnStatusControl(false);
        }
    }

    /// <summary>
    /// 根据播放视频的立体格式控制字幕Subtitle的跟随显示方式
    /// 平面视频/180°视频字幕不跟随摄像机；360°视频字幕跟随摄像机
    /// 需要打开回调关系，现已屏蔽此功能
    /// </summary>
    /// <param name="stereoType"></param>
    void SubtitleUIControl(StereoType stereoType)
    {
        switch (stereoType)
        {
            case StereoType.ST2D:
            case StereoType.ST3D_LR:
            case StereoType.ST3D_TB:
                GlobalRunningFunction.Instance.Subtitle.transform.parent = VideoPlayerPanel.gameObject.transform;
                GlobalRunningFunction.Instance.Subtitle.transform.localPosition = new Vector3(0, 300, 0);
                break;
            case StereoType.ST180_2D:
            case StereoType.ST180_LR:
            case StereoType.ST180_TB:
                GlobalRunningFunction.Instance.Subtitle.transform.parent = VideoPlayerPanel.gameObject.transform;
                GlobalRunningFunction.Instance.Subtitle.transform.localPosition = new Vector3(0, 100, 0);
                break;
            case StereoType.ST360_2D:
            case StereoType.ST360_LR:
            case StereoType.ST360_TB:
                GlobalRunningFunction.Instance.Subtitle.transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform.parent;
                GlobalRunningFunction.Instance.Subtitle.transform.localPosition = new Vector3(0, 0.4f, 9f);
                break;
        }

        GlobalRunningFunction.Instance.Subtitle.transform.SetAsFirstSibling();
        GlobalRunningFunction.Instance.Subtitle.transform.forward = GlobalRunningFunction.Instance.Subtitle.transform.parent.forward;
        GlobalRunningFunction.Instance.Subtitle.transform.localRotation = new Quaternion(0, 0, 0, 0);
        ChooseCinemaCnvasTrans();
    }

    void TensileBtnStatusControl(StereoType stereoType)
    {
        if (!isInit)
            Init();

        VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.StereoTypePanel.TensileBtnControl(stereoType);
    }

    void StretchingPicture(bool isOpen)
    {
        MediaStretchPlayerPrefsDetector.GetInstance().SetMediaStretchKey(isOpen);
        Cinema.VideoPlayer.SetStretchingPicture(isOpen);
    }

    private void LoadSceneWhenShowUI()
    {
        if (CurJVideoInfo != null)
            ImaxPurpleControlByStereoType((StereoType)CurJVideoInfo.stereoType);
        else
            ImaxPurpleControlByStereoType(StereoType.ST2D);
    }

    public void ChangeSceneStyle(bool isInteractable, bool isStretch)
    {
        Cinema.VideoPlayer.TensileSetStretchingPicture(isStretch);
        VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.ScreenSizeBtnStatusControl(isInteractable);
        LoadSceneWhenShowUI();
        ChooseCinemaCnvasTrans();
    }

    void ChangeSkyboxStyle()
    {
        //if (GlobalVariable.GetSceneModel() == SceneModel.Default)
        //{
        //    if (CinemaMaterialSetting.GetInstance().DefaultMat != null /*&& RenderSettings.skybox != CinemaMaterialSetting.Instance.DefaultMat*/)
        //        CinemaMaterialSetting.GetInstance().LoadRender(CinemaMaterialSetting.GetInstance().DefaultMat);
        //    else
        //        CinemaMaterialSetting.GetInstance().SetDefaultScene();
        //}
        //else if (GlobalVariable.GetSceneModel() == SceneModel.StarringNight)
        //{
        //    if (CinemaMaterialSetting.GetInstance().StarringMat != null/* && RenderSettings.skybox != CinemaMaterialSetting.Instance.StarringMat*/)
        //        CinemaMaterialSetting.GetInstance().LoadRender(CinemaMaterialSetting.GetInstance().StarringMat);
        //    else
        //        CinemaMaterialSetting.GetInstance().SetStarringNightScene();
        //}
        //else if (GlobalVariable.GetSceneModel() == SceneModel.IMAXTheater)
        //{
        //    CinemaMaterialSetting.GetInstance().SetIMAXTheaterScene();
        //    CinemaMaterialSetting.GetInstance().UnLoadRender();
        //}
        //else if (GlobalVariable.GetSceneModel() == SceneModel.Drive)
        //{
        //    DriveSceneModel driveModel = GlobalVariable.GetDriveSceneModel();
        //    switch (driveModel)
        //    {
        //        case DriveSceneModel.Karting:
        //            CinemaMaterialSetting.GetInstance().SetDrive_KartingScene();
        //            break;
        //        case DriveSceneModel.King:
        //            CinemaMaterialSetting.GetInstance().SetDrive_KingScene();
        //            break;
        //        case DriveSceneModel.Rattletrap:
        //            CinemaMaterialSetting.GetInstance().SetDrive_RattletrapScene();
        //            break;
        //        default: //DriveSceneModel.Playboy
        //            CinemaMaterialSetting.GetInstance().SetDrive_PlayboyScene();
        //            break;
        //    }
        //    CinemaMaterialSetting.GetInstance().UnLoadRender();
        //}
    }

    void ImaxPurpleControlByStereoType(StereoType stereoType)
    {
        //ChangeSkyboxStyle();
        //if (stereoType == StereoType.ST2D || stereoType == StereoType.ST3D_LR || stereoType == StereoType.ST3D_TB)
        //{
        //    VideoPlayerPanel.VideoCtrlPanel.SetLockBtnInteractable(true);
        //    if (GlobalVariable.GetSceneModel() == SceneModel.Default || GlobalVariable.GetSceneModel() == SceneModel.StarringNight)
        //    {
        //        if (CinemaMaterialSetting.GetInstance().ImaxPurple.activeInHierarchy)
        //            CinemaMaterialSetting.GetInstance().ImaxPurple.SetActive(false);
        //        if (CinemaMaterialSetting.GetInstance().DriveSceneBox.activeInHierarchy)
        //            CinemaMaterialSetting.GetInstance().DriveSceneBox.SetActive(false);
        //    }
        //    else if (GlobalVariable.GetSceneModel() == SceneModel.IMAXTheater)
        //    {
        //        if (!CinemaMaterialSetting.GetInstance().ImaxPurple.activeInHierarchy)
        //            CinemaMaterialSetting.GetInstance().ImaxPurple.SetActive(true);
        //        if (CinemaMaterialSetting.GetInstance().DriveSceneBox.activeInHierarchy)
        //            CinemaMaterialSetting.GetInstance().DriveSceneBox.SetActive(false);
        //        CinemaMaterialSetting.GetInstance().UnLoadRender();
        //    }
        //    else if (GlobalVariable.GetSceneModel() == SceneModel.Drive)
        //    {
        //        VideoPlayerPanel.VideoCtrlPanel.SetLockBtnInteractable(false);
        //        if (CinemaMaterialSetting.GetInstance().ImaxPurple.activeInHierarchy)
        //            CinemaMaterialSetting.GetInstance().ImaxPurple.SetActive(false);
        //        if (!CinemaMaterialSetting.GetInstance().DriveSceneBox.activeInHierarchy)
        //            CinemaMaterialSetting.GetInstance().DriveSceneBox.SetActive(true);
        //        CinemaMaterialSetting.GetInstance().UnLoadRender();
        //    }
        //}
        //else if (stereoType == StereoType.ST180_2D || stereoType == StereoType.ST180_LR || stereoType == StereoType.ST180_TB ||
        //    stereoType == StereoType.ST360_2D || stereoType == StereoType.ST360_LR || stereoType == StereoType.ST360_TB)
        //{
        //    VideoPlayerPanel.VideoCtrlPanel.SetLockBtnInteractable(false);
        //    if (CinemaMaterialSetting.GetInstance().ImaxPurple.activeInHierarchy)
        //        CinemaMaterialSetting.GetInstance().ImaxPurple.SetActive(false);
        //    if (CinemaMaterialSetting.GetInstance().DriveSceneBox.activeInHierarchy)
        //        CinemaMaterialSetting.GetInstance().DriveSceneBox.SetActive(false);
        //}
        VideoPlayerPanel.ChangePlayerUI();
    }

    private void ChangeDefinitionModel(DefinitionModel definitionModel, bool IsChangeSDK, bool IsNeedToast = true)
    {
        //切换中
        if (CurDefn == definitionModel) return;
        CurDefn = definitionModel;
        if (IsChangeSDK)
        {
            if (IsNeedToast)
            {
                ChooseCinemaCnvasTrans();
                switch (definitionModel)
                {
                    case DefinitionModel.DEFINITION_4K:
                        CinemaTipsCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.StartChangeDefinition", "4K");
                        break;
                    case DefinitionModel.DEFINITION_1080P:
                        CinemaTipsCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.StartChangeDefinition", "1080P");
                        break;
                    case DefinitionModel.DEFINITION_720P:
                        CinemaTipsCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.StartChangeDefinition", "720P");
                        break;
                    default:
                        CinemaTipsCanvasControl.GetInstance().GlobalToast.ShowToastByXMLLanguageKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.StartChangeDefinition", "720P");
                        break;
                }
                IsChangeDefinitionModel = true;
            }

            Cinema.VideoPlayer.SetDefinitionModel(VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.DefinitionPanel.GetDefnInfoWithModel(definitionModel));
        }
    }

    private void OnApplicationPause(bool pause)
    {
        Pause = pause;
        AnalizePlayMode();
        //AnalizeIntent();
    }

    private void OnApplicationFocus(bool focus)
    {
        Focus = focus;
    }

    private void AnalizePlayMode()
    {
#if SVR_USE_GAZE
        string type = SystemProperties.get("persist.svr.video_player_type", "list");
        Debug.Log("playloop:" + type);
        playLoop = type == "playloop";

        if (playLoop != lastLoop)
        {
            if (OnPlayLoopEvent != null)
                OnPlayLoopEvent.Invoke();

            lastLoop = playLoop;
            Cinema.IsPlayMode = true;
        }
#endif
    }

    public void SetPlayMode(bool isOpen)
    {
#if SVR_USE_GAZE
        string type;
        if(isOpen)
            type = "playloop";
        else
            type = "list";

        playLoop = isOpen;

        SystemProperties.set("persist.svr.video_player_type", type);
        AnalizePlayMode();
#endif
    }

    private void OnDestroy()
    {
        //HomeButtonListener.Instance.HomeButtonCallback -= HomeButtonBack;
        //HomeButtonListener.Instance.BackButtonCallback -= BackButtonBack;

        Cinema.VideoPlayer.ScreenSizeBtnStatusControlCallback -= VideoSettingsUIControl;
        //Cinema.VideoPlayer.ScreenSizeBtnStatusControlCallback -= SubtitleUIControl;
        VideoPlayerPanel.VideoCtrlPanel.ClickBackBtnCallback -= BackLocalMediaPanel;
        VideoPlayerPanel.VideoCtrlPanel.SettingsPanel.DefinitionPanel.ChangeDefinitionModelCallback -= ChangeDefinitionModel;
        PlayerDataControl.GetInstance().InterruptPlayer -= BackLocalMediaPanel;

        if (Svr.SvrSetting.IsVR9Device)
            Application.targetFrameRate = 72;
        
        Cinema = null;
        VideoPlayerPanel = null;
    }

}

/*
 * Author:李传礼 / 黄秋燕 Shemi
 * DateTime:2017.12.08
 * Description:视频播放
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;

public enum VideoPlayerState{ Idle, Preparing, Buffering, Ready, Play, Pause, Ended }//播放器状态
public enum RenderCommand { InitializePlayer, UpdateVideo, FreeTexture }//渲染命令
public enum VideoEvent//视频事件
{
    VIDEO_READY = 0, VIDEO_ENDED,
    VIDEO_BUFFER_PROGRESS = 2, VIDEO_BUFFER_START, VIDEO_BUFFER_FINISH, //缓冲节点，缓冲开始，缓冲结束
    VIDEO_PLAYING_PROGRESS = 5, VIDEO_TEXTURE_CREATED,
    VIDEO_DMR_PLAY = 7, VIDEO_DMR_PAUSE, VIDEO_DMR_STOP, VIDEO_DMR_SEEK, //DMR请求
    VIDEO_UPDATE_SUBTITLE = 11, //字幕
    VIDEO_KTTV_DURATION = 201,VIDEO_KTTV_ENTER_AD_COUNT_DOWN, VIDEO_KTTV_AD_REMAINING_TIME,
    VIDEO_SIZE_EVENT = 300
}
public enum ExceptionEvent
{
    PATH_ERROR, NOT_SUPPORT_FORMAT, NOT_SUPPORT_SIZE,
    OTHER, EXCEPTION_NOT_BEST_SIZE, EXCEPTION_NETWORK_ERROR = 5, EXCEPTION_NOT_GOOD_SIZE,
    KTTV_IPLIMIT, KTTV_LOGIN, KTTV_PLAYER_ERROR
}//异常事件

public class SvrVideoPlayer : MonoBehaviour
{
    #region 动态库接口
#if UNITY_ANDROID
    private const string dllName = "svr_plugin_player";
    [DllImport(dllName)]
    private static extern void InitEnvironment();
    [DllImport(dllName)]
    private static extern void ReleaseEnvironment();
    [DllImport(dllName)]
    private static extern IntPtr GetRenderEventFunc();
    [DllImport(dllName)]
    private static extern int GetExternalSurfaceTextureId(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern int GetVideoMatrix(IntPtr videoPlayerPtr, float[] videoMatrix, int size);
    [DllImport(dllName)]
    private static extern IntPtr CreateVideoPlayer();//only click icon do once
    [DllImport(dllName)]
    private static extern void DestroyVideoPlayer(IntPtr videoPlayerPtr);//back list do it
    [DllImport(dllName)]
    private static extern int GetVideoPlayerEventBase(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern IntPtr SetDataSource(IntPtr videoPlayerPtr, string svrVideoInfo); //used before play video every times
    [DllImport(dllName)]
    private static extern int GetSupportResolutions(IntPtr videoPlayerPtr, int[] resolutions, int size);//传入数组大小至少为3，返回个数小于等于数组个数
    [DllImport(dllName)]
    private static extern void SetInitialResolution(IntPtr videoPlayerPtr, int initialResolution);
    [DllImport(dllName)]
    private static extern int GetPlayerState(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern int GetWidth(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern int GetHeight(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern bool PlayVideo(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern bool PauseVideo(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern bool StopVideo(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern void ResetPlayer(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern long GetDuration(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern long GetCurrentPosition(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern void SetCurrentPosition(IntPtr videoPlayerPtr, long pos);
    [DllImport(dllName)]
    private static extern int GetMaxVolume(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern int GetCurrentVolume(IntPtr videoPlayerPtr);
    [DllImport(dllName)]
    private static extern void SetCurrentVolume(IntPtr videoPlayerPtr, int value);
    [DllImport(dllName)]
    private static extern void SetOnVideoEventCallback(IntPtr videoPlayerPtr, Action<IntPtr, int, int, string> this_EventId_callback, IntPtr callback_arg);
    [DllImport(dllName)]
    private static extern void SetOnExceptionEventCallback(IntPtr videoPlayerPtr, Action<IntPtr, int, int, string> this_EventId_Message_callback, IntPtr callback_arg);
    [DllImport(dllName)]
    private static extern void SetLoop(IntPtr videoPlayerPtr, bool isLoop);
    [DllImport(dllName)]
    /// Note: This is now obsolete. Use SetOnVolumeChangedEvent2(IntPtr, Action) instead.
    private static extern void SetOnVolumeChangedEvent(Action this_EventId_VolumeChanged_callback);
    [DllImport(dllName)]
    private static extern void SetOnVolumeChangedEvent2(IntPtr videoPlayerPtr, Action<IntPtr> volume_changed_callback, IntPtr callback_arg);
    [DllImport(dllName)]
    private static extern void SetSubtitleSource(IntPtr videoPlayerPtr, string path);
    [DllImport(dllName)]
    private static extern void ClearCache();

    private const string dllName2 = "svr_plugin_media_scan";
    [DllImport(dllName2)]
    private static extern void SetOnImageCompressedEvent(Action<string> image_compressed_callback);
#endif
    #endregion

    [HideInInspector]
    public Renderer QuadScreen;
    [HideInInspector]
    public Renderer HemisphereScreen;
    [HideInInspector]
    public Renderer SphereScreen;
    Renderer VideoScreen;
#if UNITY_ANDROID
    private MifengPlayer mPlayer;
    /// <summary>
    /// main video player
    /// </summary>
    IntPtr VideoPlayerPtr;//播放器
    /// <summary>
    /// The rendering event function at the Native.
    /// </summary>
    IntPtr RenderEventFunction;//Native端的渲染事件函数
    int VideoPlayerEventBase;//播放器的事件起始值
    int SurfaceTextureId;//图像Id
    float[] VideoMatrixRaw;//视频矩阵原始数据
    Matrix4x4 VideoMatrix;//视频矩阵
    int VideoMatrixPropertyId;//视频矩阵属性Id
#endif
    int MaxVolume;//最大音量
    int VideoWidth;//视频宽
    int VideoHeight;//视频高
    /// <summary>
    /// If the video is ready
    /// </summary>
    bool IsVideoReady;//视频是否准备完毕
    /// <summary>
    /// Whatever the video is ready or not, 
    /// the video button has been clicked and the video is ready to play automatically. 
    /// </summary>
    bool ReadyPlay;//不管视频是否准备完毕已经点击播放按钮，视频准备好就自动播放
    /// <summary>
    /// To make sure create video player for once.
    /// </summary>
    bool IsInit;
    /// <summary>
    /// 是否需要释放图片
    /// </summary>
    bool IsNeedFreeTexeture;
    StereoType m_StereoType;
    int TimeCount;
    Texture MainTexture;
    bool IsRelased;
    bool CanUpdateProgress;

    private Action<IntPtr, int, int, string> VideoEventPtr;
    private Action<IntPtr, int, int, string> ExceptionEventPtr;
    private Action<IntPtr> VolumeChangePtr;

    #region public delegate
    public delegate void VideoEnd();
    public delegate void VideoReady();
    public delegate void VideoVolumeChange(float volumePercent);
    public delegate void VideoBufferProgressChange(float bufferPercent, string downloadSpeed);
    public delegate void VideoBufferStart();
    public delegate void VideoBufferFinish();
    public delegate void VideoProgressChange(int time);
    public delegate void VideoError(ExceptionEvent errorCode, string errMessage);

    public VideoEnd OnEnd;
    public VideoReady OnReady;
    public VideoVolumeChange OnVolumeChange;
    public VideoBufferProgressChange OnBufferProgressChange;
    public VideoBufferStart OnBufferStart;
    public VideoBufferFinish OnBufferFinish;
    public VideoProgressChange OnProgressChange;
    public VideoError OnVideoError;
    #endregion

    public Material ImageStereoInside;
    public Material VideoUnlitForMultiViewMaterial;//360 video for singlepass
    public Material VideoMaterial;//360 video for non-singlepass

    public Action<StereoType> ScreenSizeBtnStatusControlCallback;
    public Action<StereoType> TensileBtnStatusControlCallback;
    public Action FinishIntent;
    public Action<string> SetSubtitle;
    public Action ReleaseInterfaceCallback;
    public Action<bool> DMR_PlayVideoCallback;
    public Action DMR_PauseVideoCallback;
    public Action<bool, bool> ChangeSceneStyleCallback;
    public Action<long> AdRemainingTimeCallback;
    public Action<long> VideoDurationCallback;
    public Action<List<MifengPlayer.DefnInfo>, MifengPlayer.DefnInfo> DefinitionListCallback;

    public Action StopPlayCallBack;
    private void Awake ()
    {
        InitEnvironment();
        Init();
    }

    private void Update ()
    {
#if UNITY_ANDROID
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
            UpdateVideoRenderTexture();
        else
        {
            if (IsVideoReady)
            mPlayer.updateTexture();
        }
#endif

        if (Input.GetKeyDown(KeyCode.P))
            OnVideoEvent(VideoEvent.VIDEO_BUFFER_START, 0);
        if (Input.GetKeyDown(KeyCode.S))
            OnVideoEvent(VideoEvent.VIDEO_BUFFER_FINISH, 0);
        if (Input.GetKeyDown(KeyCode.W))
            OnVideoEventBufferingProgress(VideoEvent.VIDEO_BUFFER_PROGRESS, 0, 122);
    }

    private void Init()
    {
        MainTexture = null;
        VideoPlayerPtr = IntPtr.Zero;
        VideoEventPtr = new Action<IntPtr, int, int, string>(OnVideoEvent);
        ExceptionEventPtr = new Action<IntPtr, int, int, string>(OnExceptionEvent);
        VolumeChangePtr = new Action<IntPtr>(OnVolumeChangedEvent);

        QuadScreen = PlayerGameobjectControl.Instance.QuadScreen;
        HemisphereScreen = PlayerGameobjectControl.Instance.HemisphereScreen;
        SphereScreen = PlayerGameobjectControl.Instance.SphereScreen;
        VideoScreen = QuadScreen;

        MaxVolume = 0;
        VideoWidth = 0;
        VideoHeight = 0;
        TimeCount = 0;
        IsVideoReady = false;
        ReadyPlay = false;
        IsInit = false;
        IsNeedFreeTexeture = false;
        SurfaceTextureId = -1;
        CanUpdateProgress = true;

        CreatVideoPlayer();
    }

#if UNITY_ANDROID
    //执行播放器事件
    private void IssuePlayerEvent(int play, RenderCommand renderCmd)
    {
        if (RenderEventFunction != IntPtr.Zero)
        {
            GL.IssuePluginEvent(RenderEventFunction, play + (int)renderCmd);
        }
    }

    //更新纹理
    private void UpdateVideoRenderTexture()
    {
        if (VideoPlayerPtr != IntPtr.Zero && IsVideoReady)
        {
            IssuePlayerEvent(VideoPlayerEventBase, RenderCommand.UpdateVideo); //更新视频帧
            GetVideoMatrix(VideoPlayerPtr, VideoMatrixRaw, 16);//获取旋转矩阵
            VideoMatrix = GvrMathHelpers.ConvertFloatArrayToMatrix(VideoMatrixRaw); //矩阵转换
            VideoScreen.sharedMaterial.SetMatrix(VideoMatrixPropertyId, VideoMatrix); //设置矩阵
        }
    }

    private void UpdateMatrix(float[] matrix)
    {
        MainThreadQueue.ExecuteQueue.Enqueue(() => 
        {
            if (matrix == null) return;
            VideoScreen.sharedMaterial.SetMatrix(VideoMatrixPropertyId, ConvertFloatArrayToMatrix(matrix)); //设置矩阵
        });
    }

    private Matrix4x4 ConvertFloatArrayToMatrix(float[] floatArray)
    {
        Matrix4x4 result = new Matrix4x4();

        if (floatArray == null || floatArray.Length != 16)
        {
            throw new System.ArgumentException("floatArray must not be null and have a length of 16.");
        }

        result[0, 0] = floatArray[0];
        result[1, 0] = floatArray[1];
        result[2, 0] = floatArray[2];
        result[3, 0] = floatArray[3];
        result[0, 1] = floatArray[4];
        result[1, 1] = floatArray[5];
        result[2, 1] = floatArray[6];
        result[3, 1] = floatArray[7];
        result[0, 2] = floatArray[8];
        result[1, 2] = floatArray[9];
        result[2, 2] = floatArray[10];
        result[3, 2] = floatArray[11];
        result[0, 3] = floatArray[12];
        result[1, 3] = floatArray[13];
        result[2, 3] = floatArray[14];
        result[3, 3] = floatArray[15];

        return result;
    }
#endif

    public void ClearVideoCache()
    {
        ClearCache();
    }

    /// <summary>
    /// Set-up shader value for stereo mode
    /// </summary>
    public void SetPlayMode(/*bool isChangePlayMode, */StereoType stereoType)
    {
        m_StereoType = stereoType;
        if (stereoType == StereoType.ST180_2D || stereoType == StereoType.ST180_LR || stereoType == StereoType.ST180_TB)
        {
            //Cinema.GvrHead.trackPosition = true;
            QuadScreen.gameObject.SetActive(false);
            HemisphereScreen.gameObject.SetActive(true);
            SphereScreen.gameObject.SetActive(false);

            VideoScreen = HemisphereScreen;
            //RenderSettings.skybox = null;
            Camera.main.clearFlags = CameraClearFlags.Nothing;
        }
        else if (stereoType == StereoType.ST360_2D || stereoType == StereoType.ST360_LR || stereoType == StereoType.ST360_TB)
        {
            //Cinema.GvrHead.trackPosition = false;
            QuadScreen.gameObject.SetActive(false);
            HemisphereScreen.gameObject.SetActive(false);
            SphereScreen.gameObject.SetActive(true);

            VideoScreen = SphereScreen;
            //RenderSettings.skybox = null;
            Camera.main.clearFlags = CameraClearFlags.Nothing;
        }
        else/* (stereoType == StereoType.ST2D || stereoType == StereoType.ST3D_LR || stereoType == StereoType.ST3D_TB)*/
        {
            SceneModel sceneModel = GlobalVariable.GetSceneModel();
            bool isInteractable = false;
            switch(sceneModel)
            {
                case SceneModel.Default:
                case SceneModel.StarringNight:
                    isInteractable = true;
                    break;
            }
            Camera.main.clearFlags = CameraClearFlags.Skybox;
            if (ChangeSceneStyleCallback != null)
                ChangeSceneStyleCallback(isInteractable, true);

            //Cinema.GvrHead.trackPosition = true;
            QuadScreen.gameObject.SetActive(true);
            HemisphereScreen.gameObject.SetActive(false);
            SphereScreen.gameObject.SetActive(false);

            VideoScreen = QuadScreen;
        }

        if (Gvr.Internal.BaseVRDevice.IsSupported && VideoScreen.sharedMaterial != VideoUnlitForMultiViewMaterial)
            VideoScreen.sharedMaterial = VideoUnlitForMultiViewMaterial;
        else if (!Gvr.Internal.BaseVRDevice.IsSupported && VideoScreen.sharedMaterial != VideoMaterial)
            VideoScreen.sharedMaterial = VideoMaterial;

        if (VideoScreen != null && VideoScreen.sharedMaterial != null && VideoScreen.sharedMaterial.mainTexture == null && MainTexture != null)
            VideoScreen.sharedMaterial.mainTexture = MainTexture;

        //ChangeSceneType(false);

        VideoScreen.sharedMaterial.SetFloat("_StereoMode", 0);
        VideoScreen.sharedMaterial.DisableKeyword("_STEREOMODE_LEFTRIGHT");
        VideoScreen.sharedMaterial.DisableKeyword("_STEREOMODE_TOPBOTTOM");

        int i = (int)stereoType % 3;
        if (i == 1)
        {
            VideoScreen.sharedMaterial.EnableKeyword("_STEREOMODE_LEFTRIGHT");
            VideoScreen.sharedMaterial.SetFloat("_StereoMode", 2);
        }
        else if (i == 2)
        {
            VideoScreen.sharedMaterial.EnableKeyword("_STEREOMODE_TOPBOTTOM");
            VideoScreen.sharedMaterial.SetFloat("_StereoMode", 1);

        }

        if (ScreenSizeBtnStatusControlCallback != null)
            ScreenSizeBtnStatusControlCallback(m_StereoType);
        TensileSetStretchingPicture(true);
    }

    /// <summary>
    /// 画面拉伸与拉伸按钮控制
    /// </summary>
    public void TensileSetStretchingPicture(bool isStretch)
    {
        if (isStretch)
        {
            SetStretchingPicture(MediaStretchPlayerPrefsDetector.GetInstance().GetMediaStretchKey());
            if (TensileBtnStatusControlCallback != null)
                TensileBtnStatusControlCallback(m_StereoType);
        }
    }

    public void PreparedPlayVideo(Dictionary<string, string> dic)
    {
        if (mPlayer == null)
            mPlayer = new MifengPlayer();

        MediaStretchPlayerPrefsDetector.GetInstance().SetMediaId(string.Format("{0}-{1}", dic["vid"], dic["cid"]));
        mPlayer.setDataSource(dic["vid"], dic["cid"]);
        IsNeedFreeTexeture = true;
        LogTool.Log("播放索引：" + dic["cid"] +"-"+ dic["vid"] + " 播放名称：" + dic["name"] +" 播放类型："+ dic["stereoType"]);

        VideoWidth = 0;
        VideoHeight = 0;
        LogTool.Log("SetDataSource success");
    }

    /// <summary>
    /// Major role is set data source
    /// </summary>
    /// <param name="svrVideoInfo">BaseVideoInfo data</param>
    public void PreparedPlayVideo(JVideoDescriptionInfo svrVideoInfo)
    {
#if UNITY_ANDROID
        if (VideoPlayerPtr == IntPtr.Zero)
        {
            Debug.Log("VideoPlayerPtr:为IntPtr.Zero");
            return;
        }

        MediaStretchPlayerPrefsDetector.GetInstance().SetMediaId(svrVideoInfo.id.ToString());
        string json = JsonUtility.ToJson(svrVideoInfo);
        Debug.Log("svrVideoInfo json:" + json);
        IsVideoReady = false;

        VideoPlayerPtr = SetDataSource(VideoPlayerPtr, json);
        IsNeedFreeTexeture = true;
        IssuePlayerEvent(VideoPlayerEventBase, RenderCommand.InitializePlayer);

        //shemi:: 之后需要更换为扫描本地字幕，扫描回调中再设置字幕
        //string path = svrVideoInfo.uri;
        //path = MediaManage.GetPath((int)MediaType.Video ,path);
        //path = path.Substring(0, path.LastIndexOf("."));
        //SetSubtitleSource(VideoPlayerPtr, string.Format("{0}.srt", path));

        VideoWidth = svrVideoInfo.width;
        VideoHeight = svrVideoInfo.height;
        Debug.Log("VideoScreenWidth:" + VideoWidth + " VideoScreenHeight:" + VideoHeight);
        LogTool.Log("SetDataSource success");
#endif
    }

    public void ResetPlayerTextureDefault()
    {
        if (VideoScreen != null && VideoScreen.sharedMaterial != null && VideoScreen.sharedMaterial.mainTexture != null)
        {
            lock (VideoScreen.sharedMaterial.mainTexture)
            {
                if (VideoScreen != null && VideoScreen.sharedMaterial != null && VideoScreen.sharedMaterial.mainTexture != null)
                    Destroy(VideoScreen.sharedMaterial.mainTexture);
            }
        }
    }

    #region Event
#if UNITY_ANDROID
    private void OnVideoEvent(VideoEvent eventId, int code)
    {
        OnVideoEvent(IntPtr.Zero, (int)eventId, code, "");
    }

    private void OnVideoEventBufferingProgress(VideoEvent eventId, int progress, int speed)
    {
        OnVideoEvent(IntPtr.Zero, (int)eventId, progress, speed.ToString());
    }

    private void OnVideoEvent(IntPtr cbdata, int eventId, int code, string message)
    {
        if ((PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl) && cbdata == IntPtr.Zero)
            return;

        try
        {
            MainThreadQueue.ExecuteQueue.Enqueue(() => {
                if (eventId == (int)VideoEvent.VIDEO_READY)
                {
#if UNITY_ANDROID
                    if ((PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
                        || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl) && VideoPlayerPtr == IntPtr.Zero)
                        return;

                    LogTool.Log("视频准备完毕");
                    VideoWidth = GetVideoWidth();
                    VideoHeight = GetVideoHeight();
                    LogTool.Log("VideoWidth = " + VideoWidth + ", VideoHeight = "+ VideoHeight);
                    if (OnReady != null)//set UI total time
                        OnReady();

                    IsVideoReady = true;
                    if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
                        || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
                        MaxVolume = GetMaxVolume(VideoPlayerPtr);
                    else
                        MaxVolume = mPlayer.getMaxVolume();

                    //reset volume UI
                    VolumeChangedComplete();

                    if (Cinema.IsPlayOrPauseVideoPlayer)
                        Play();

                    // Set loop type
                    //shemi
                    //if (!GlobalVariable.IsIntent)
                    if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
                        || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
                    {
                        bool isLoop = false;
                        if (VideoPlayManage.CurLoopType == LoopType.ListLoop)
                            isLoop = false;
                        else if (VideoPlayManage.CurLoopType == LoopType.SinglePlay)
                            isLoop = true;
                        SetLoop(isLoop);
                    }
#endif
                }
                else if (eventId == (int)VideoEvent.VIDEO_ENDED)
                {
                    LogTool.Log("视频播放完毕");
                    if (OnEnd != null)
                        OnEnd();
                }
                else if (eventId == (int)VideoEvent.VIDEO_BUFFER_PROGRESS)
                {
                    int speed = int.Parse(message);
                    if (OnBufferProgressChange != null)
                        OnBufferProgressChange(code, string.Format("{0}/s", GlobalVariable.MBToShowGBStr(speed)));
                }
                else if (eventId == (int)VideoEvent.VIDEO_BUFFER_START)
                {
                    CanUpdateProgress = false;
                    if (OnBufferStart != null)
                        OnBufferStart();
                }
                else if (eventId == (int)VideoEvent.VIDEO_BUFFER_FINISH)
                {
                    CanUpdateProgress = true;
                    if (OnBufferFinish != null)
                        OnBufferFinish();
                }
                else if (eventId == (int)VideoEvent.VIDEO_PLAYING_PROGRESS)
                {
                    // 每0.5s发送事件，但是每1s才更新UI
                    if (CanUpdateProgress)
                    {
                        if (++TimeCount % 2 == 0)
                            return;

                        if (OnProgressChange != null)
                            OnProgressChange(code);
                    }
                }
                else if (eventId == (int)VideoEvent.VIDEO_TEXTURE_CREATED)
                {
                    SurfaceTextureId = code;

                    Texture2D texture = Texture2D.CreateExternalTexture(4, 4, TextureFormat.ARGB32, false, false, new System.IntPtr(SurfaceTextureId));
                    MainTexture = texture;
                }
                else if (eventId == (int)VideoEvent.VIDEO_DMR_PLAY)
                {
                    //Play();
                    if (DMR_PlayVideoCallback != null)
                        DMR_PlayVideoCallback(false);
                }
                else if (eventId == (int)VideoEvent.VIDEO_DMR_PAUSE)
                {
                    //Pause();
                    if (DMR_PauseVideoCallback != null)
                        DMR_PauseVideoCallback();
                }
                else if (eventId == (int)VideoEvent.VIDEO_DMR_STOP)
                {
                    if (FinishIntent != null)
                        FinishIntent();
                }
                else if (eventId == (int)VideoEvent.VIDEO_DMR_SEEK)
                {
                    SeekToTime(code);
                }
                else if (eventId == (int)VideoEvent.VIDEO_UPDATE_SUBTITLE)
                {
                    //更新字幕
                    if (SetSubtitle != null)
                        SetSubtitle(message);
                }
                else if (eventId == (int)VideoEvent.VIDEO_KTTV_DURATION)
                {
                    //返回预览
                    if (VideoDurationCallback != null)
                        VideoDurationCallback(code);
                }
                else if (eventId == (int)VideoEvent.VIDEO_KTTV_AD_REMAINING_TIME)
                {
                    //广告剩余时间（enable进度条）
                    if (AdRemainingTimeCallback != null)
                        AdRemainingTimeCallback(code);
                }
            });
        }
        catch (InvalidCastException e)
        {
            Debug.LogError("GC Handle pointed to unexpected type: videoPlayer.Expected " + typeof(SvrVideoPlayer));
            throw e;
        }
    }

    private void OnExceptionEvent(ExceptionEvent errorCode, string errMessage)
    {
        OnExceptionEvent(IntPtr.Zero, (int)errorCode, -1, errMessage);
    }

    private void OnExceptionEvent(IntPtr cbdata, int eventId, int percent, string message)
    {
        try
        {
            Debug.Log("Exception event message：" + message);
            if (eventId == (int)ExceptionEvent.PATH_ERROR)
            {
                MainThreadQueue.ExecuteQueue.Enqueue(() => VideoExceptionEvent(eventId, "Cinema.SvrVideoPlayer.OnExceptionEvent.Video.PathError"));
            }
            else if (eventId == (int)ExceptionEvent.NOT_SUPPORT_FORMAT || eventId == (int)ExceptionEvent.NOT_SUPPORT_SIZE)
            {
                MainThreadQueue.ExecuteQueue.Enqueue(() => VideoExceptionEvent(eventId, "Cinema.SvrVideoPlayer.OnExceptionEvent.Video.NOT_SUPPORT"));
            }
            else if (eventId == (int)ExceptionEvent.OTHER)
            {
                MainThreadQueue.ExecuteQueue.Enqueue(() => VideoExceptionEvent(eventId, "Cinema.SvrVideoPlayer.OnExceptionEvent.Video.OtherError"));
            }
            else if (eventId == (int)ExceptionEvent.EXCEPTION_NOT_BEST_SIZE || eventId == (int)ExceptionEvent.EXCEPTION_NOT_GOOD_SIZE)
            {
                MainThreadQueue.ExecuteQueue.Enqueue(() => VideoExceptionEvent(eventId, "Cinema.SvrVideoPlayer.OnExceptionEvent.Video.NotBestSize"));
            }
            else if (eventId == (int)ExceptionEvent.EXCEPTION_NETWORK_ERROR)
            {
                MainThreadQueue.ExecuteQueue.Enqueue(() => VideoExceptionEvent(eventId, "Cinema.SvrVideoPlayer.OnExceptionEvent.Video.NETWORK_ERROR"));
            }
            else if (eventId == (int)ExceptionEvent.KTTV_IPLIMIT)
            {
                MainThreadQueue.ExecuteQueue.Enqueue(() => VideoExceptionEvent(eventId, "Cinema.SvrVideoPlayer.OnExceptionEvent.Video.KTTV_IPLIMIT"));
            }
            else if (eventId == (int)ExceptionEvent.KTTV_LOGIN)
            {
                MainThreadQueue.ExecuteQueue.Enqueue(() => VideoExceptionEvent(eventId, "Cinema.SvrVideoPlayer.OnExceptionEvent.Video.KTTV_LOGIN"));
            }
            else if (eventId == (int)ExceptionEvent.KTTV_PLAYER_ERROR)
            {
                MainThreadQueue.ExecuteQueue.Enqueue(() => VideoExceptionEvent(eventId, "Cinema.SvrVideoPlayer.OnExceptionEvent.Video.KTTV_PLAYER_ERROR"));
            }
        }
        catch (InvalidCastException e)
        {
            Debug.LogError("GC Handle pointed to unexpected type: videoPlayer.Expected " + typeof(SvrVideoPlayer));
            throw e;
        }
    }

    void OnVolumeChangedEvent()
    {
        OnVolumeChangedEvent(IntPtr.Zero);
    }

    void OnVolumeChangedEvent(IntPtr cbdata)
    {
        MainThreadQueue.ExecuteQueue.Enqueue(() => VolumeChangedComplete());
    }

    void OnDefnEvent(List<MifengPlayer.DefnInfo> list, MifengPlayer.DefnInfo current)
    {
        if (list != null)
        {
            MainThreadQueue.ExecuteQueue.Enqueue(() =>
          {
            if (list == null || current == null) return;
              if (DefinitionListCallback != null)
                  DefinitionListCallback(list, current);
          });
        }
    }

    void OnVieoSizeEvent(int w, int h)
    {
        VideoWidth = w;
        VideoHeight = h;
        SetStretchingPicture(MediaStretchPlayerPrefsDetector.GetInstance().GetMediaStretchKey());
    }
#endif
    #endregion

    private void VolumeChangedComplete()
    {
        float currentVolumePer = GetCurrentVolumePercent();
        if (OnVolumeChange != null)
            OnVolumeChange(currentVolumePer);
    }

    private void VideoExceptionEvent(int eventId, string errMessage)
    {
        if (OnVideoError != null)
            OnVideoError((ExceptionEvent)eventId, errMessage);
    }

    /// <summary>
    /// Before playing video you need create video player first.
    /// Just do once.
    /// </summary>
    public void CreatVideoPlayer()
    {
        if (IsInit)
            return;
#if UNITY_ANDROID && !UNITY_EDITOR
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local 
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            VideoPlayerPtr = CreateVideoPlayer();
            SetOnVolumeChangedEvent2(VideoPlayerPtr, VolumeChangePtr, ToIntPtr(this));
            SetOnVideoEventCallback(VideoPlayerPtr, VideoEventPtr, ToIntPtr(this));
            SetOnExceptionEventCallback(VideoPlayerPtr, ExceptionEventPtr, ToIntPtr(this));

            RenderEventFunction = GetRenderEventFunc();
            VideoPlayerEventBase = GetVideoPlayerEventBase(VideoPlayerPtr);

            VideoMatrixRaw = new float[16];
            VideoMatrix = Matrix4x4.identity;
        }
        else if(PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
        {
            if (mPlayer == null)
                mPlayer = new MifengPlayer();
            mPlayer.OnVideoEvent = OnVideoEvent;
            mPlayer.OnDefnEvent = OnDefnEvent;
            mPlayer.OnVideoError = OnExceptionEvent;
            mPlayer.OnVolumeChangedEvent = OnVolumeChangedEvent;
            mPlayer.onUpdateMatrix = UpdateMatrix;
            mPlayer.OnVieoSize = OnVieoSizeEvent;
            mPlayer.OnBufferingProgress = OnVideoEventBufferingProgress;
            mPlayer.createTexture();
        }
        VideoMatrixPropertyId = Shader.PropertyToID("video_matrix");
#endif
        IsInit = true;
    }

    /// <summary>
    /// Need to release resources before exiting the application 
    /// </summary>
    public void Release()
    {
#if UNITY_ANDROID
        if (IsRelased) return;
        IsRelased = true;

        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local 
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayerPtr == IntPtr.Zero)
                return;
            DestroyVideoPlayer(VideoPlayerPtr);
            VideoPlayerPtr = IntPtr.Zero;
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            mPlayer.release();

        IsVideoReady = false;
        ReadyPlay = false;
        IsInit = false;
        mPlayer = null;
        ResetPlayerTextureDefault();
#endif
    }

    public void SetStretchingPicture(bool isOpen)
    {
        ChangeVideoWH(VideoWidth, VideoHeight, m_StereoType, isOpen);
    }

    void ChangeVideoWH(float w, float h, StereoType m_VideoType, bool isOpen)
    {
        if (m_VideoType == StereoType.ST2D || m_VideoType == StereoType.ST3D_LR || m_VideoType == StereoType.ST3D_TB)
        {
            float video_wh = 1.0f;
            video_wh = getScale(w, h, m_VideoType, isOpen);
            LogTool.Log("video_width:" + w + ",video_height:" + h + ",getScale:" + video_wh);
            video_wh = (VideoScreen.transform.localScale.x / VideoScreen.transform.localScale.y) / video_wh;
            LogTool.Log("_Offset:" + video_wh);
            VideoScreen.sharedMaterial.SetFloat("_Offset", video_wh);
        }
        else
        {
            LogTool.Log("_Offset:1");
            VideoScreen.sharedMaterial.SetFloat("_Offset", 1);
        }
    }

    float getScale(float w, float h, StereoType m_VideoType, bool isOpen)
    {
        if (w != 0 && h != 0)
        {
            if (m_StereoType != StereoType.ST2D && m_StereoType != StereoType.ST3D_LR && m_StereoType != StereoType.ST3D_TB)
                isOpen = false;

            switch (m_VideoType)
            {
                case StereoType.ST3D_LR:
                    if (isOpen)
                        return w / h;
                    else
                        return (w / 2.0f) / h;
                case StereoType.ST3D_TB:
                    if (isOpen)
                        return w / h;
                    else
                        return (2 * w) / h;
                case StereoType.ST2D:
                    return w / h;
                default:
                    return 1;
            }
        }
        else
            return 16.0f / 9.0f;
    }

    public void Play()
    {
#if UNITY_ANDROID
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayerPtr == IntPtr.Zero)
                return;
            if (IsVideoReady)
                PlayVideo(VideoPlayerPtr);
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            mPlayer.playVideo();

        ReadyPlay = true;
#endif
    }

    public void Pause()
    {
#if UNITY_ANDROID
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayerPtr == IntPtr.Zero)
                return;

            PauseVideo(VideoPlayerPtr);
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            mPlayer.pauseVideo();

        ReadyPlay = false;
#endif
    }

    public void Stop()
    {
#if UNITY_ANDROID
        ReadyPlay = false;
        IsVideoReady = false;

        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayerPtr == IntPtr.Zero)
                return;

            StopVideo(VideoPlayerPtr);
            ResetPlayer(VideoPlayerPtr);
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            mPlayer.stopVideo();

        if (IsNeedFreeTexeture)
        {
            if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
                IssuePlayerEvent(SurfaceTextureId * 100, RenderCommand.FreeTexture);
            else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
                mPlayer.releaseTexture();

            IsNeedFreeTexeture = false;
            ResetPlayerTextureDefault();
        }
#endif

        ////发送null回调是为了清空字幕
        //if (SetSubtitle != null)
        //    SetSubtitle(null);
    }

    public void SeekToTime(long ms)
    {
#if UNITY_ANDROID
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayerPtr == IntPtr.Zero)
                return;

            SetCurrentPosition(VideoPlayerPtr, ms);
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            mPlayer.seekTo(ms);

        LogTool.Log("Seek to " + ms);
#endif
    }

    /// <summary>
    /// Set single loop
    /// </summary>
    /// <param name="isLoop"></param>
    public void SetLoop(bool isLoop)
    {
#if UNITY_ANDROID
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            return;

        if (VideoPlayerPtr == IntPtr.Zero)
            return;
         
        SetLoop(VideoPlayerPtr, isLoop);
#endif
    }

    public int GetMaxVolume()
    {
#if UNITY_ANDROID
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayerPtr == IntPtr.Zero)
                return 0;

            return GetMaxVolume(VideoPlayerPtr);
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            return mPlayer.getMaxVolume();
        else
            return 0;
#else
        return 0;
#endif
    }

    public int GetCurrentVolume()
    {
#if UNITY_ANDROID
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayerPtr == IntPtr.Zero)
                return 0;

            return GetCurrentVolume(VideoPlayerPtr);
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            return mPlayer.getCurrentVolume();
        else
            return 0;
#else
        return 0;
#endif
    }

    /// <summary>
    /// Convert the volume value to a percentage value
    /// </summary>
    /// <returns>percentage volume value</returns>
    public float GetCurrentVolumePercent()
    {
        try
        {
#if UNITY_ANDROID
            if ((PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
                || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl) && VideoPlayerPtr == IntPtr.Zero)
                return 0;

            int CurrentVolume = GetCurrentVolume();
            int maxVolume = GetMaxVolume();
            float CurrentVolumePercent = 0;
            if (maxVolume != 0)
                CurrentVolumePercent = (float)CurrentVolume / maxVolume;
            return CurrentVolumePercent;
#endif
        }
        catch (Exception e)
        {
            Debug.Log("GetCurrentVolumePercent have exception" + e.Message);
            return 0.5f;
        }
    }

    public long GetVideoDuration()
    {
#if UNITY_ANDROID
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayerPtr == IntPtr.Zero)
                return 0;

            return GetDuration(VideoPlayerPtr);
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            return mPlayer.getVideoDuration();
        else
            return 0;
#else
        return 0;
#endif
    }

    public long GetCurrentPosition()
    {
#if UNITY_ANDROID
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayerPtr == IntPtr.Zero)
                return 0;

            return GetCurrentPosition(VideoPlayerPtr);
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            return mPlayer.getCurrentVideoTime();
        else
            return 0;
#else
        return 0;
#endif
    }

    public int GetVideoWidth()
    {
#if UNITY_ANDROID
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayerPtr == IntPtr.Zero)
                return 0;

            return GetWidth(VideoPlayerPtr);
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            return mPlayer.getVideoWidth();
        else
            return 0;
#else
        return 0;
#endif
    }

    public int GetVideoHeight()
    {
#if UNITY_ANDROID
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayerPtr == IntPtr.Zero)
                return 0;

            return GetHeight(VideoPlayerPtr);
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            return mPlayer.getVideoHeight();
        else
            return 0;
#else
        return 0;
#endif
    }

    /// <summary>
    /// Get current video player states.
    /// </summary>
    /// <returns>player states</returns>
    public VideoPlayerState GetPlayerState()
    {
#if UNITY_ANDROID
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            return VideoPlayerState.Idle;

        if (VideoPlayerPtr == IntPtr.Zero)
            return VideoPlayerState.Idle;

        return (VideoPlayerState)GetPlayerState(VideoPlayerPtr);
#else
        return VideoPlayerState.Idle;
#endif
    }

    public void SetVolumePercent(float percent)
    {
#if UNITY_ANDROID
        if (percent < 0)
            percent = 0;
        else if (percent > 1)
            percent = 1;

        int value = Mathf.FloorToInt(MaxVolume * percent);
        if (percent != 0 && value == 0)
            value = 1;
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayerPtr == IntPtr.Zero)
                return;
            SetCurrentVolume(VideoPlayerPtr, value);
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
            mPlayer.setCurrentVolume(value);
#endif
    }

    public void SetDefinitionModel(MifengPlayer.DefnInfo info)
    {
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
        {
            mPlayer.switchDefinition(info);
        }
    }
    
    private static IntPtr ToIntPtr(System.Object obj)
    {
        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            GCHandle handle = GCHandle.Alloc(obj);
            return GCHandle.ToIntPtr(handle);
        }
        else
            return IntPtr.Zero;
    }

    private void OnApplicationPause(bool pause)
    {
#if UNITY_ANDROID
        if ((PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl) && VideoPlayerPtr == IntPtr.Zero)
            return;

        if (pause && ReadyPlay)
        {
            Pause();
            ReadyPlay = true;
        }
        else if (ReadyPlay)
            Play();
#endif
    }

    //    private void OnApplicationQuit()
    //    {
    //#if UNITY_ANDROID
    //        if (VideoPlayerPtr != IntPtr.Zero)
    //        {
    //            Release();

    //            if (ReleaseInterfaceCallback != null)
    //                ReleaseInterfaceCallback();
    //        }
    //#endif
    //    }

    private void OnApplicationQuit()
    {
        ReleaseEnvironment();
    }

    private void OnDestroy()
    {
        Release();
        if (ReleaseInterfaceCallback != null)
            ReleaseInterfaceCallback();

        ImageStereoInside = null;
        VideoUnlitForMultiViewMaterial = null;
        VideoMaterial = null;
        MainTexture = null;
        QuadScreen = null;
        HemisphereScreen = null;
        SphereScreen = null;
        VideoScreen = null;
        mPlayer = null;
    }
}

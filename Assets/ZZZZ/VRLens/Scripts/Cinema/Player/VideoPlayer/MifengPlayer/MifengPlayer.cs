using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public enum RenderCommand2 { CreateTexture, ReleaseTexture, UpdateTexture }
public enum DefinitionModel
{
    UNKOWN,
    DEFINITION_STANDARD = 1,//标清
    DEFINITION_HIGH = 2,//高清
    DEFINITION_720P = 4,DEFINITION_1080P, DEFINITION_4K =10
}

public class MifengPlayer
{
    private const string TAG = "UnityPlayer";
    #region dll
#if UNITY_ANDROID
    private const string dllName = "svr_plugin_player_renderer";
    [DllImport(dllName)]
    private static extern IntPtr GetRenderEventFunc();
#endif
    #endregion

    private AndroidJavaObject mCurrentActivity;
    private AndroidJavaObject mKTTVPlayer;

    //public delegate void OnTextureCreated(int textureId);
    public delegate void OnUpdateMatrix(float[] matrix);
    //public OnTextureCreated onTextureCreated;
    public OnUpdateMatrix onUpdateMatrix;

    public delegate void VieoEvent(VideoEvent videoEvent, int code);
    public VieoEvent OnVideoEvent;
    public delegate void VolumeChangedEvent();
    public VolumeChangedEvent OnVolumeChangedEvent;
    public delegate void DefnEvent(List<DefnInfo> list, DefnInfo current);
    public DefnEvent OnDefnEvent;
    public delegate void VideoError(ExceptionEvent errorCode, string errMessage);
    public VideoError OnVideoError;
    public delegate void VieoSize(int w, int h);
    public VieoSize OnVieoSize;
    public delegate void BufferingProgress(VideoEvent videoEvent, int progress, int speed);
    public BufferingProgress OnBufferingProgress;

    public MifengPlayer()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
        if (PlayerPrefs.HasKey(GlobalVariable.UserNameKey))
        {
            string[] key = PlayerPrefs.GetString(GlobalVariable.UserNameKey).Split(':');
            Debug.Log("Is login & user id:" + key[2]);
            mKTTVPlayer = new AndroidJavaObject ("com.ssnwt.player.Player", activity, key[2]);
        }
        else
            mKTTVPlayer = new AndroidJavaObject ("com.ssnwt.player.Player", activity);
        mKTTVPlayer.Call ("setListener", new PlayerEventListener (this));
#endif
    }

    public void setDataSource(string vid, string cid)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        LogTool.Log(TAG, string.Format("Player-{0}-{1}", vid, cid));
        mKTTVPlayer.Call ("setDataSource", cid, vid, 0);
#endif
    }

    public void setDataSource(string vid, string cid, long startTime)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        LogTool.Log(TAG, string.Format ("Player-{0}-{1}-{2}", vid, cid, startTime));
        mKTTVPlayer.Call("setDataSource", cid, vid, (int)startTime);
#endif
    }

    private void renderEvent(RenderCommand2 eventCode)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GL.IssuePluginEvent(GetRenderEventFunc(), (int)eventCode);
#endif
    }

    public void createTexture()
    {
        renderEvent(RenderCommand2.CreateTexture);
    }
    public void releaseTexture()
    {
        renderEvent(RenderCommand2.ReleaseTexture);
    }
    public void updateTexture()
    {
        renderEvent(RenderCommand2.UpdateTexture);
    }

    public void onVideoEvent(VideoEvent videoEvent, int code)
    {
        if (OnVideoEvent != null)
            OnVideoEvent(videoEvent, code);
    }

    public void onVolumeChangedEvent()
    {
        if (OnVolumeChangedEvent != null)
            OnVolumeChangedEvent();
    }

    public void onBufferingProgress(VideoEvent videoEvent, int progress, int speed)
    {
        if (OnBufferingProgress != null)
            OnBufferingProgress(videoEvent, progress, speed);
    }

    public void playVideo()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        mKTTVPlayer.Call("playVideo");
#endif
    }

    public long getCurrentVideoTime()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return mKTTVPlayer.Call<long>("getCurrentVideoTime");
#else
        return 0;
#endif
    }

    public long getVideoDuration()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return mKTTVPlayer.Call<long>("getVideoDuration");
#else
        return 0;
#endif
    }

    public bool isPlaying()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return mKTTVPlayer.Call<bool>("isPlaying");
#else
        return false;
#endif
    }

    public void pauseVideo()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        mKTTVPlayer.Call("pauseVideo");
#endif
    }

    public void stopVideo()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        mKTTVPlayer.Call("stopVideo");
#endif
    }

    public void release()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        mKTTVPlayer.Call("release");
#endif
    }

    public void seekTo(long ms)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        mKTTVPlayer.Call("seekTo", ms);
#endif
    }

    public int getCurrentVolume()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return mKTTVPlayer.Call<int>("getCurrentVolume");
#else
        return 0;
#endif
    }

    public int getMaxVolume()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return mKTTVPlayer.Call<int>("getMaxVolume");
#else
        return 0;
#endif
    }

    public int getVideoHeight()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return mKTTVPlayer.Call<int>("getVideoHeight");
#else
        return 0;
#endif
    }

    public int getVideoWidth()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return mKTTVPlayer.Call<int>("getVideoWidth");
#else
        return 0;
#endif
    }

    public void setCurrentVolume(int volume)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        mKTTVPlayer.Call("setCurrentVolume", volume);
#endif
    }

    public void switchDefinition(DefnInfo defn)
    {
        mKTTVPlayer.Call("switchDefinition", defn.obj);
    }

    public class DefnInfo
    {
        public DefinitionModel defn;
        public int audioType; //音轨标识;0-NORMAL(普通) 1-DOLBY(杜比)
        public int codecType; //流编码属性;0-H264(H264 码流) 1-H211(H265 码流)
        public int dynamicRangeType; //HDR 属性;0-SDR(SDK 码流) 1-DOLBY_VISION(DOLBY_VISION 码流) 2-HDR10(HDR10 码流)
        public int ctrlType; //VIP 属性;-1-CTRL_NONE(普通码流) 0-CTRL_VIP(VIP 用户可看) 1-CTRL_LOGIN(登录用户可看)
        public int benefitType; //当前用户对影片享有的权益;0-CAN_PLAY(可播) 1-CAN_NOT_PLAY(不可播) 2-PREVIEW(预览)
        public AndroidJavaObject obj;

        public DefnInfo()
        {
            this.defn = DefinitionModel.DEFINITION_720P;
            this.audioType = 0;
            this.codecType = 0;
            this.dynamicRangeType = 0;
            this.ctrlType = 0;
            this.benefitType = 0;
            this.obj = null;
        }
    }
   
    public DefnInfo getDefnJInfo(AndroidJavaObject defnJInfoObj)
    {
        DefnInfo defnInfo = new DefnInfo()
        {
            defn = (DefinitionModel)defnJInfoObj.Call<int>("getDefinition"),
            audioType = defnJInfoObj.Call<int>("getAudioType"),
            codecType = defnJInfoObj.Call<int>("getCodecType"),
            dynamicRangeType = defnJInfoObj.Call<int>("getDynamicRangeType"),
            ctrlType = defnJInfoObj.Call<int>("getCtrlType"),
            benefitType = defnJInfoObj.Call<int>("getBenefitType"),
            obj = defnJInfoObj,
        };
        return defnInfo;
    }

    public List<DefnInfo> GetDefnJInfoList(AndroidJavaObject infos)
    {
        List<DefnInfo> defnJInfoObjList = new List<DefnInfo>();
#if UNITY_ANDROID && !UNITY_EDITOR
        int count = infos.Call<int>("size");
        for (int i = 0; i<count;i++)
        {
            DefnInfo defnInfo = new DefnInfo();
            if (infos != null)
            {
                defnInfo = getDefnJInfo(infos.Call<AndroidJavaObject>("get", i));
                Debug.Log("GetDefnJInfo defnInfo = " + defnInfo.defn + ", "+ defnInfo.benefitType);
                defnJInfoObjList.Add(defnInfo);
            }
        }
        
        return defnJInfoObjList;
#else
        return null;
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    public sealed class PlayerEventListener : AndroidJavaProxy
    {
        private MifengPlayer mPlayer;
        private float[] mSTMatrix;
        public PlayerEventListener(MifengPlayer player) : base("com.ssnwt.player.PlayerEventListener")
        {
            mPlayer = player;
            mSTMatrix = new float[16];
        }
        public void onTextureCreated(int texture)
        {
            mPlayer.onVideoEvent(VideoEvent.VIDEO_TEXTURE_CREATED, texture);
        }
        public void onUpdateMatrix(AndroidJavaObject matrix)
        {
            LogTool.Log(TAG, "onUpdateMatrix");
            int length = matrix.Call<int>("size");
            if (length == 16)
            {
                for (int i = 0; i < length; i++)
                {
                    AndroidJavaObject value = matrix.Call<AndroidJavaObject>("get", i);
                    mSTMatrix[i] = value.Call<float>("floatValue");
                }
                mPlayer.onUpdateMatrix(mSTMatrix);
            }
        }

        public void onVideoPrepared()
        {
            LogTool.Log(TAG, "onVideoPrepared");
            //// 获取宽高。进度条enable
            mPlayer.onVideoEvent(VideoEvent.VIDEO_READY, -1);
        }

        public void onVideoCompleted()
        {
            LogTool.Log(TAG, "onVideoCompleted");
            mPlayer.onVideoEvent(VideoEvent.VIDEO_ENDED, -1);
        }

        public void onUpdatePosition(long position)
        {
            //LogTool.Log(TAG, string.Format("onUpdatePosition-{0}", position));
            mPlayer.onVideoEvent(VideoEvent.VIDEO_PLAYING_PROGRESS, (int)position);
        }

        /// <summary>
        /// 清晰度回调
        /// </summary>
        /// <param name="infos">所有清晰度列表</param>
        /// <param name="currDef">当前清晰度</param>
        public void onDefinition(AndroidJavaObject infos, AndroidJavaObject currDef)
        {
            Debug.Log("onDefinition");
            List<DefnInfo> list = mPlayer.GetDefnJInfoList(infos);

            DefnInfo current = mPlayer.getDefnJInfo(currDef);
            Debug.Log("onDefinition current defn = " + current.defn + ", benefitType = "+ current.benefitType);
            mPlayer.OnDefnEvent(list, current);
        }

        /// <summary>
        /// 缓冲
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="speed"></param>
        public void onBuffering(int percent, int speed)
        {
            LogTool.Log(TAG, string.Format("onBuffering-{0}-{1}", percent, speed));
            mPlayer.onBufferingProgress(VideoEvent.VIDEO_BUFFER_PROGRESS, percent, speed);
        }

        public void onBufferingBegin()
        {
            LogTool.Log(TAG, "onBufferingBegin");
            mPlayer.onVideoEvent(VideoEvent.VIDEO_BUFFER_START, -1);
        }

        public void onBufferingEnd()
        {
            LogTool.Log(TAG, "onBufferingEnd");
            mPlayer.onVideoEvent(VideoEvent.VIDEO_BUFFER_FINISH, -1);
        }

        /// <summary>
        /// 播放错误信息
        /// 初始化播放视频断网时，底层直接推送123-103网络错误信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="what"></param>
        public void onError(int model, int code, string serverCode)
        {
            ExceptionEvent errorCode = ExceptionEvent.KTTV_PLAYER_ERROR;
            string errMessage = string.Format("{0}-{1}-{2}", model, code, serverCode);
            LogTool.Log(TAG, errMessage);
            if (isMatchIpLimit(code.ToString()) || isMatchIpLimit(serverCode))
                //版权原因，无法观看
                errorCode = ExceptionEvent.KTTV_IPLIMIT;
            //else if (isMatchLogin(code.ToString()) || isMatchLogin(serverCode))
            //    //登录设备达到上限
            //    errorCode = ExceptionEvent.KTTV_LOGIN;
            else if (isMatchNetwork(code.ToString()) || !GlobalVariable.IsInternetReachability())
                errorCode = ExceptionEvent.EXCEPTION_NETWORK_ERROR;
            else
                //播放错误
                errorCode = ExceptionEvent.KTTV_PLAYER_ERROR;

            mPlayer.OnVideoError(errorCode, errMessage);
        }

        private static string[] NETWORK_ERROR_CODE = { "-50", "501", "502", "3101", "3102",
                "3201", "3202", "4011", "4012", "5001", "2001", "3001", "4001", "6001", "1001" };
        private static string[] SAME_TIME_LOGIN_MAX_CODE = { "A10001", "Q00501", "A02603",
                "504_Q00501", "A10002"};
        private static string[] IP_LIMIT_CODE = { "E200003", "104_109", "1003_A00000-109", "A00111" };
        public bool isMatchNetwork(string code)
        {
            return isMatch(code, NETWORK_ERROR_CODE);
        }

        public bool isMatchLogin(string code)
        {
            return isMatch(code, SAME_TIME_LOGIN_MAX_CODE);
        }
        public bool isMatchIpLimit(string code)
        {
            return isMatch(code, IP_LIMIT_CODE);
        }
        private bool isMatch(string code, string[] errors)
        {
            if (code == null || code == string.Empty || errors == null || errors.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < errors.Length; i++)
            {
                string error = errors[i];
                code = code.ToUpper();
                error = error.ToUpper();
                if (code.Equals(error))
                {
                    return true;
                }
            }
            return false;
        }

        public void onVideoSize(int width, int height)
        {
            LogTool.Log(TAG, string.Format("onVideoSize-{0}-{1}", width, height));
            mPlayer.OnVieoSize(width, height);
        }

        /// <summary>
        /// 总时长
        /// </summary>
        /// <param name="previewTime">总时长</param>
        public void onDuration(long previewTime)
        {
            LogTool.Log(TAG, string.Format("onDuration-{0}", previewTime));
            mPlayer.onVideoEvent(VideoEvent.VIDEO_KTTV_DURATION, (int)previewTime);
        }

        /// <summary>
        /// 收费视频预览
        /// </summary>
        /// <param name="previewTime">总时长</param>
        public void onPreviewTime(long previewTime)
        {
            //LogTool.Log(TAG, string.Format("onPreviewTime-{0}", previewTime));
            //mPlayer.onVideoEvent(VideoEvent.VIDEO_KTTV_DURATION, (int)previewTime);
        }

        /// <summary>
        /// 广告剩余时间（enable进度条）
        /// </summary>
        /// <param name="remainingTime"></param>
        public void onAdRemainingTime(long remainingTime)
        {
            LogTool.Log(TAG, string.Format("onAdRemainingTime-{0}", remainingTime));
            mPlayer.onVideoEvent(VideoEvent.VIDEO_KTTV_AD_REMAINING_TIME, (int)remainingTime);
        }

        public void onVolumeChanged(int volume)
        {
            LogTool.Log(TAG, string.Format("onVolumeChanged-{0}", volume));
            mPlayer.onVolumeChangedEvent();
        }
    }
#endif
}

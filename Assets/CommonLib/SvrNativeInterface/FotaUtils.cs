using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using SVR;
using Svr;

public class FotaUtils
{
    AndroidJavaObject InterfaceObj;

    public FotaUtils(SvrNativeInterface svrNI)
    {
        if (svrNI != null)
        {
            InterfaceObj = svrNI.GetUtils(UtilsType.FotaUtils);
            SetOnFotaEvent();
            LogTool.Log("创建FotaUtils");
        }
    }

    public void SetOnFotaEvent(/*FotaListener listener*/)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        FotaListener listener = new FotaListener(this);
        if (InterfaceObj != null)
            InterfaceObj.Call("setListener", listener);
#endif
    }

    public void CheckUpdate()
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("checkUpdate");
    }

    public void GoToDownload()
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("goToDownload");
    }

    public bool IsDownloading()
    {
        bool isDownloading = false;

        if (InterfaceObj != null)
            isDownloading = InterfaceObj.Call<bool>("isDownloading");

        return isDownloading;
    }

    public bool IsDownloadingFinished()
    {
        bool isDownloadingFinished = false;

        if (InterfaceObj != null)
            isDownloadingFinished = InterfaceObj.Call<bool>("isDownloadingFinished");

        return isDownloadingFinished;
    }

    public bool HasNewVersion()
    {
        if (InterfaceObj != null)
            return InterfaceObj.Call<bool>("hasNewVersion");
        else
            return true;
    }

    public UpgradeInfo GetUpgradInfo()
    {
        UpgradeInfo upgrade = new UpgradeInfo();
        AndroidJavaObject upgradeInfoObj = InterfaceObj.Call<AndroidJavaObject>("getUpgradeInfo");
        if (upgradeInfoObj != null)
            upgrade = getUpdateInfo(upgradeInfoObj);
        else
            Debug.LogError("upgradeInfoObj == null");
        return upgrade;
    }

    public void installPackage()
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("installPackage");
    }

    public void Resume()
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("resume");
    }

    public void Pause()
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("pause");
    }

    public enum DownLoadError
    {
        SPACE_NOT_ENOUGH = 1,
        NET_ERROR = 2
    }

    public class UpgradeInfo
    {
        public String pkgversion;
        public String pkgcndesc;
        public String pkgendesc;
        public int pkgsize;
        public bool isforceupgrade;
    }

    public enum MessageType
    {
        None,
        VersionCallBack,
        ErrorCallBack,
        FinishCallBack
    }

    private Queue<Message> otgMessageQueue = new Queue<Message>();

    public void LoopMessage()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {
            Message tt = new Message((int)MessageType.VersionCallBack);
            //message.SetObject("updateinfo", oTGContent.mUpgradeInfo);
            tt.SetBool("newversion", true);
            tt.SetBool("isforceupgrade", true);
            tt.SetString("desc", "这是更新信息");
            tt.SetString("version", "100006");
            otgMessageQueue.Enqueue(tt);
        }

        if (otgMessageQueue.Count == 0) return;
        Message message = otgMessageQueue.Peek();
        switch ((MessageType)message.messageType)
        {
            case MessageType.VersionCallBack:
                if (OnVersionCallback != null)
                {
                    VersionCallStruct callStruct = new VersionCallStruct()
                    {
                        newversion = message.GetBool("newversion"),
                        version = message.GetString("version"),
                        desc = message.GetString("desc"),
                        isforceupgrade = message.GetBool("isforceupgrade")
                    };
                    OnVersionCallback(callStruct);
                    otgMessageQueue.Dequeue();
                }
                break;
            case MessageType.ErrorCallBack:
                DownLoadError downLoadError = (DownLoadError)message.GetInt("error");
                if (OnDownLoadError != null)
                {
                    OnDownLoadError(downLoadError);
                    otgMessageQueue.Dequeue();
                }

                break;
            case MessageType.FinishCallBack:
                if (OnDownLoadCompleted != null)
                {
                    OnDownLoadCompleted();
                    otgMessageQueue.Dequeue();
                }
                break;
            default:
                break;
        }



    }

#if UNITY_ANDROID && !UNITY_EDITOR
    public sealed class FotaListener : AndroidJavaProxy
    {
        private FotaUtils oTGContent;
        public FotaListener(FotaUtils oTG) : base("com.ssnwt.vr.androidmanager.FotaUtils$FotaListener")
        {
            oTGContent = oTG;
        }

        public void onVersion(bool isNew, bool force, string description, string pkgVersion)
        {
            //if (oTGContent.OnVersionCallback != null) oTGContent.OnVersionCallback(isNew);
            Debug.Log("OnVersion:" + isNew);
            //string version = string.Empty;
            //if (pkgVersion != null)
            //    version = pkgVersion.Call<string>("toString");
            //string des = string.Empty;
            //if (description != null)
            //    des = description.Call<string>("toString");

            Debug.Log("pkgVersion = "+ pkgVersion + ", description = "+ description);
            //oTGContent.HaveNewVersion = isNew;
            //oTGContent.DispathVersionCallBack(isNew);
            //if (isNew)
            //{
            //    oTGContent.mUpgradeInfo = oTGContent.getUpdateInfo(upgradeInfoObj);
            //}

            Message message = new Message((int)MessageType.VersionCallBack);
            //message.SetObject("updateinfo", oTGContent.mUpgradeInfo);
            message.SetBool("newversion", isNew);
            message.SetBool("isforceupgrade", force/* oTGContent.mOTGContent.GetStatic<bool>("misforceupgrade")*/);
            message.SetString("desc", description/*oTGContent.mOTGContent.GetStatic<string>("mDescription")*/);
            message.SetString("version", pkgVersion/*oTGContent.mOTGContent.GetStatic<string>("mVersion")*/);
            oTGContent.otgMessageQueue.Enqueue(message);


        }

        public void onDownloadProgress(int progress)
        {
            oTGContent.DownLoadProgress = progress * 0.01f;
        }
        public void onDownloadError(int code)
        {

            //DownLoadError downLoadError = (DownLoadError)code;
            //if (oTGContent.OnDownLoadError != null)
            //    oTGContent.OnDownLoadError(downLoadError);

            Message message = new Message((int)MessageType.ErrorCallBack);
            message.SetInt("error", code);
            oTGContent.otgMessageQueue.Enqueue(message);
        }
        public void onDownloadFinish()
        {
            //if (oTGContent.OnDownLoadCompleted != null) oTGContent.OnDownLoadCompleted();
            Message message = new Message((int)MessageType.FinishCallBack);
            oTGContent.otgMessageQueue.Enqueue(message);
        }
    }
#endif
    /// <summary>
    /// 下载出现错误
    /// </summary>
    public UnityAction<DownLoadError> OnDownLoadError;
    /// <summary>
    /// Fota下载完成
    /// </summary>
    public UnityAction OnDownLoadCompleted;
    public struct VersionCallStruct
    {
        public bool newversion;
        public string version;
        public string desc;
        public bool isforceupgrade;
    }
    /// <summary>
    /// 收到新版本的回调
    /// </summary>
    public event UnityAction<VersionCallStruct> OnVersionCallback;
    public bool HaveNewVersion { get; private set; }
    public float DownLoadProgress { get; private set; }
    private UpgradeInfo mUpgradeInfo;
    public UpgradeInfo MUpgradeInfo
    {
        get
        {
            if (mUpgradeInfo == null)
            {
                mUpgradeInfo = GetUpgradInfo();
            }

            return mUpgradeInfo;
        }
    }

    private UpgradeInfo getUpdateInfo(AndroidJavaObject upgradeInfoObj)
    {

        UpgradeInfo upgradeInfo = new UpgradeInfo()
        {
            pkgversion = upgradeInfoObj.Call<string>("getPkgversion"),
            pkgsize = upgradeInfoObj.Call<int>("getPkgsize"),
            pkgcndesc = upgradeInfoObj.Call<string>("getPkgcndesc"),
            pkgendesc = upgradeInfoObj.Call<string>("getPkgendesc"),
            isforceupgrade = upgradeInfoObj.Call<int>("getForceupgrade") == 1

        };
        return upgradeInfo;
    }

    public String getCurrentVersion()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass buidlClass = new AndroidJavaClass("android.os.Build");
        return buidlClass.GetStatic<string>("DISPLAY");
#endif
        return "SVR 1.0";
    }
}

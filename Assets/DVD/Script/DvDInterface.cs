using Daydream.MediaAppTemplate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DvDInterface : SingletonMB<DvDInterface>
{
    bool IsInitCurActivity = false;
     AndroidJavaObject CurActivity;
     AndroidJavaObject JApplication;
     AndroidJavaObject InstanceObject;
     AndroidJavaObject ConnectObj;
     public AndroidJavaObject FileObj;
     AndroidJavaClass ArrayClass;
    List<DVDDirectoryInfo> m_dVDdirectoryInfos=new List<DVDDirectoryInfo>();
    List<DVDFileInfo> m_dVDFileInfos = new List<DVDFileInfo>();

    [SerializeField]
    private GameObject grantedPrefab;

    private GameObject grantedObj;
    public Action DiskReadyCallBack;
    private void Awake()
    {
        InitCurrentActivity();
        SetDVDListener();
        ArrayClass = new AndroidJavaClass("java.lang.reflect.Array");
        FileObj = new AndroidJavaObject("com.ssnwt.dvd.DVDFile");
        Connect();
    }

     void InitCurrentActivity()
    {
        IsInitCurActivity = true;
        //#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        if (unityPlayer != null)
            CurActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
//#endif
    }

    private  AndroidJavaObject getInstance()
    {
//#if UNITY_ANDROID && !UNITY_EDITOR

        if (InstanceObject==null)
        {
            AndroidJavaClass AppTaskManager = new AndroidJavaClass("com.ssnwt.dvd.DVDManagerService");
            if (AppTaskManager == null)
                return null;
            InstanceObject = AppTaskManager.GetStatic<AndroidJavaObject>("Singleton");
        }
//#endif
        return InstanceObject;
    }

    public  AndroidJavaObject GetJApplication()//修改为静态函数用于测试
    {
        if (!IsInitCurActivity)
            InitCurrentActivity();

#if UNITY_ANDROID && !UNITY_EDITOR
        if (JApplication != null)
            return JApplication;
        else
        {
            if (CurActivity != null)
            {
                JApplication = CurActivity.Call<AndroidJavaObject>("getApplication");
                return JApplication;
            }
            else
                return null;
        }
#else
        return null;
#endif
    }

    public  void Connect()
    {
        if (getInstance() != null)
        {
            getInstance().Call("connect");
        }
    }

     void SetDVDListener()
    {
        if(getInstance()!=null)
        {
            getInstance().Call("addConnectedListener", new ConnectionListener(this));
            getInstance().Call("addDiskStateListener", new DiskStateListener(this));
            getInstance().Call("addDoorStateListener", new DoorStateListener(this));
        }
    }

    DVDDirectoryInfo GetDVDFileInfo(AndroidJavaObject obj)
    {
        DVDDirectoryInfo fileInfo = new DVDDirectoryInfo()
        {
            path= obj.Call<string>("getUrl"),
            name = obj.Call<string>("getName"),
            isDir = obj.Call<bool>("isDir"),
            extension = obj.Call<string>("getType"),
            javaObj = obj,
        };
        return fileInfo;
    }

    void onDiskFind()
    {
        Debug.Log("unity+onDiskFind");
    }

    void onDiskReady()
    {
        Debug.Log("unity+onDiskReady");
        //m_dVDFileInfos=GetFileList();
        MainThreadQueue.ExecuteQueue.Enqueue(() =>
        {
            if (grantedObj == null)
                grantedObj = GameObject.Instantiate(grantedPrefab);
            if (DiskReadyCallBack != null)
                DiskReadyCallBack();
        });
    }

    public sealed class DiskStateListener:AndroidJavaProxy
    {
        private DvDInterface mDvdTest;
        public DiskStateListener(DvDInterface dvdTest) : base("com.ssnwt.dvd.OnDiskStateChangeListener")
        {
            mDvdTest = dvdTest;
        }

        public void onDiskFind()
        {
            mDvdTest.onDiskFind();
        }

        public void onDiskReady()
        {
            mDvdTest.onDiskReady();
        }
    }

    public sealed class DoorStateListener : AndroidJavaProxy
    {
        private DvDInterface mDvdTest;
        public DoorStateListener(DvDInterface dvdTest) : base("com.ssnwt.dvd.OnDoorStateChangeListener")
        {
            mDvdTest = dvdTest;
        }

        public void onLock()
        {
            Debug.Log("java+onLock");
        }

        public void onUnLock()
        {
            Debug.Log("java+onUnLock");
        }
    }

    public sealed class ConnectionListener : AndroidJavaProxy
    {
        private DvDInterface mDvdTest;
        public ConnectionListener(DvDInterface dvdTest) : base("com.ssnwt.dvd.ConnectionListener")
        {
            mDvdTest = dvdTest;
        }

        public void onError()
        {
            Debug.Log("java+onError");
        }

        public void onSuccess()
        {
            Debug.Log("java+onSuccess");
        }

        public void onTimeout()
        {
            Debug.Log("java+onTimeout");
        }
    }

    public List<DVDDirectoryInfo> GetFileList()
    {
        List<DVDDirectoryInfo> dvdFile = new List<DVDDirectoryInfo>();
        if (FileObj != null)
        {
            AndroidJavaObject infos= FileObj.Call<AndroidJavaObject>("listFiles");
            int count = ArrayClass.CallStatic<int>("getLength",infos);
            for (int i = 0; i < count; i++)
            {
                DVDDirectoryInfo fileInfo = new DVDDirectoryInfo();
                if (infos!=null)
                {
                    fileInfo = GetDVDFileInfo(ArrayClass.CallStatic<AndroidJavaObject>("get", infos, i));
                    Debug.Log("getFileInfo = " + fileInfo.path + ", " + fileInfo.name+","+ fileInfo.extension);
                    dvdFile.Add(fileInfo);
                }
            }
        }
        return dvdFile;
    }

    public DVDDirectoryInfo[] GetFileList(AndroidJavaObject fileObj)
    {
        List<DVDDirectoryInfo> dvdFile = new List<DVDDirectoryInfo>();
        if (fileObj != null)
        {
            AndroidJavaObject infos = fileObj.Call<AndroidJavaObject>("listFiles");
            int count = ArrayClass.CallStatic<int>("getLength", infos);
            for (int i = 0; i < count; i++)
            {
                DVDDirectoryInfo fileInfo = new DVDDirectoryInfo();
                if (infos != null)
                {
                    fileInfo = GetDVDFileInfo(ArrayClass.CallStatic<AndroidJavaObject>("get", infos, i));
                    Debug.Log("fileInfo.name " + fileInfo.name+ "fileInfo.extension" + fileInfo.extension+ "fileInfo.isDir"+fileInfo.isDir);
                    dvdFile.Add(fileInfo);
                }
            }
        }
        return dvdFile.ToArray();
    }

    public void AnalyzeDirectorys()
    {
        List< DVDDirectoryInfo> dctArray = GetFileList();
        m_dVDdirectoryInfos.Clear();
        m_dVDFileInfos.Clear();
        for (int i = 0; i < dctArray.Count; i++)
        {
            if (dctArray[i].isDir)
            {
                m_dVDdirectoryInfos.Add(dctArray[i]);
            }
            else
            {
                DVDFileInfo file = new DVDFileInfo();
                file.fileName = dctArray[i].name;
                file.fileUrl = dctArray[i].path;
                file.isDir = dctArray[i].isDir;
                file.extension = dctArray[i].extension;
                file.javaObj = dctArray[i].javaObj;
                m_dVDFileInfos.Add(file);
            }
        }
    }

    public DVDDirectoryInfo[] GetDirectorys()
    {
        return m_dVDdirectoryInfos.ToArray();
    }

    public DVDFileInfo[] GetFiles()
    {
        return m_dVDFileInfos.ToArray();
    }
}

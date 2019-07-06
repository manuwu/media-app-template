using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpHelper : SingletonMB<HttpHelper>
{
    AndroidJavaObject InterfaceObj;

    #region delegate
    public delegate void HttpError(string tag, int code);
    public HttpError OnError;
    public delegate void HttpSuccess(string tag, string message);
    public HttpSuccess OnSuccess;
    #endregion

    void Start ()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        InterfaceObj = _getInstance();
#endif
        if (InterfaceObj != null)
            SetListener();
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject _getInstance()
    {
        AndroidJavaClass AppTaskManager = new AndroidJavaClass("com.ssnwt.vr.download.HttpHelper");
        if (AppTaskManager == null)
            return null;

        AndroidJavaObject AndroidInterface = AppTaskManager.CallStatic<AndroidJavaObject>("getInstance");
        if (AndroidInterface == null)
            return null;
        else
            return AndroidInterface;
    }
#endif

    private void SetListener()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        HttpListener listener = new HttpListener(this);
        if (InterfaceObj != null)
            InterfaceObj.Call("setHttpListener", listener);
#endif
    }

    /// <summary>
    /// Get请求
    /// </summary>
    /// <param name="tag">对象标志</param>
    /// <param name="uri">完整地址</param>
    public void RequestGet(string tag, string uri)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
        {
            Debug.Log("RequestGet: " + tag + " " + uri);
            InterfaceObj.Call("requestGet", tag, uri);
        }
#endif
    }

    /// <summary>
    /// Post请求
    /// </summary>
    /// <param name="tag">对象标志</param>
    /// <param name="uri">接口地址</param>
    /// <param name="post">post参数</param>
    public void RequestPost(string tag, string uri, string post)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (InterfaceObj != null)
        {
            Debug.Log("RequestPost: " + tag + " " + uri + " "+post);
            InterfaceObj.Call("requestPost", tag, uri, post);
        }
#endif
    }

    #region Listener
    public void onError(string tag, int code)
    {
        MainThreadQueue.ExecuteQueue.Enqueue(() =>
        {
            if (OnError != null)
                OnError(tag, code);
        });
    }

    public void onSuccess(string tag, string message)
    {
        MainThreadQueue.ExecuteQueue.Enqueue(() =>
        {
            if (OnSuccess != null)
            OnSuccess(tag, message);
        });
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    public sealed class HttpListener : AndroidJavaProxy
    {
        private HttpHelper HttpHelper;
        public HttpListener(HttpHelper helper) : base("com.ssnwt.vr.download.HttpListener")
        {
            HttpHelper = helper;
        }

        public void onError(string tag, int code)
        {
            Debug.Log("HttpHelper onError: " + tag + " " + code);
            HttpHelper.onError(tag, code);
        }

        public void onSuccess(string tag, string message)
        {
            Debug.Log("HttpHelper onSuccess: " + tag + " " + message);
            HttpHelper.onSuccess(tag, message);
        }
    }
#endif
    #endregion
}

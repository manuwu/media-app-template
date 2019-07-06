using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class WWWPostAndGet
{
    public static IEnumerator POST(string url, Dictionary<string, string> post, Action<bool, string> requestCompleteCallback)
    {
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<string, string> post_arg in post)
            form.AddField(post_arg.Key, post_arg.Value);

        WWW www = new WWW(url, form);
        Debug.Log("WWWPostAndGet.POST(" + www.url + ")");
        www.threadPriority = UnityEngine.ThreadPriority.High;
        yield return www;

        if (www.error != null)
        {
            Debug.Log("WWWPostAndGet.POST error :"+ www.error);
            yield return new WaitForSeconds(3);
            www = new WWW(url, form);
            Debug.Log("WWWPostAndGet.POST2(" + www.url + ")");
            www.threadPriority = UnityEngine.ThreadPriority.High;
            yield return www;

            if (www.error != null)
            {
                Debug.Log("WWWPostAndGet.POST2 error2 :" + www.error);
                yield return new WaitForSeconds(3);
                www = new WWW(url, form);
                Debug.Log("WWWPostAndGet.POST3(" + www.url + ")");
                www.threadPriority = UnityEngine.ThreadPriority.High;
                yield return www;

                if (www.error != null)
                {
                    Debug.Log("WWWPostAndGet.POST3 error3 :" + www.error);
                    yield return new WaitForSeconds(3);
                    www = new WWW(url, form);
                    Debug.Log("WWWPostAndGet.POST4(" + www.url + ")");
                    www.threadPriority = UnityEngine.ThreadPriority.High;
                    yield return www;

                    if (www.error != null)
                    {
                        Debug.Log("WWWPostAndGet.POST4 error4 :" + www.error);
                        MainThreadQueue.ExecuteQueue.Enqueue(() => CenterToastPanel.Instance.ShowToast("Cinema.SvrVideoPlayer.OnExceptionEvent.Video.NETWORK_ERROR"));
                        if (requestCompleteCallback != null)
                            requestCompleteCallback(false, "error :" + www.error);
                    }
                    else
                    {
                        if (requestCompleteCallback != null)
                            requestCompleteCallback(true, www.text);
                    }
                }
                else
                {
                    if (requestCompleteCallback != null)
                        requestCompleteCallback(true, www.text);
                }
            }
            else
            {
                if (requestCompleteCallback != null)
                    requestCompleteCallback(true, www.text);
            }
        }
        else
        {
            if (requestCompleteCallback != null)
                requestCompleteCallback(true, www.text);
        }

        www.Dispose();
        www = null;
    }

    public static IEnumerator GET(string url, Dictionary<string, string> get, Action<bool, string> requestCompleteCallback)
    {
        string Parameters;
        bool first;
        if (get.Count > 0)
        {
            first = true;
            Parameters = "?";
            foreach (KeyValuePair<string, string> post_arg in get)
            {
                if (first)
                    first = false;
                else
                    Parameters += "&";

                Parameters += post_arg.Key + "=" + post_arg.Value;
            }
        }
        else
            Parameters = "";
        url += Parameters;
        WWW www = new WWW(url);
        Debug.Log("WWWPostAndGet.GET(" + www.url + ")");
        www.threadPriority = UnityEngine.ThreadPriority.High;
        yield return www;

        if (www.error != null)
        {
            Debug.Log("WWWPostAndGet.GET error :" + www.error);
            yield return new WaitForSeconds(3);
            www = new WWW(url);
            Debug.Log("WWWPostAndGet.GET2(" + www.url + ")");
            www.threadPriority = UnityEngine.ThreadPriority.High;
            yield return www;

            if (www.error != null)
            {
                Debug.Log("WWWPostAndGet.GET2 error2 :" + www.error);
                yield return new WaitForSeconds(3);
                www = new WWW(url);
                Debug.Log("WWWPostAndGet.GET3(" + www.url + ")");
                www.threadPriority = UnityEngine.ThreadPriority.High;
                yield return www;

                if (www.error != null)
                {
                    Debug.Log("WWWPostAndGet.GET3 error3 :" + www.error);
                    yield return new WaitForSeconds(3);
                    www = new WWW(url);
                    Debug.Log("WWWPostAndGet.GET4(" + www.url + ")");
                    www.threadPriority = UnityEngine.ThreadPriority.High;
                    yield return www;

                    if (www.error != null)
                    {
                        Debug.Log("WWWPostAndGet.GET4 error4 :" + www.error);
                        MainThreadQueue.ExecuteQueue.Enqueue(() => CenterToastPanel.Instance.ShowToast("Cinema.SvrVideoPlayer.OnExceptionEvent.Video.NETWORK_ERROR"));
                        if (requestCompleteCallback != null)
                            requestCompleteCallback(false, "error :" + www.error);
                    }
                    else
                    {
                        if (requestCompleteCallback != null)
                            requestCompleteCallback(true, www.text);
                    }
                }
                else
                {
                    if (requestCompleteCallback != null)
                        requestCompleteCallback(true, www.text);
                }
            }
            else
            {
                if (requestCompleteCallback != null)
                    requestCompleteCallback(true, www.text);
            }
        }
        else
        {
            if (requestCompleteCallback != null)
                requestCompleteCallback(true, www.text);
        }

        www.Dispose();
        www = null;
    }  
}

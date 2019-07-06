using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IVR.Language;

public class CenterToastPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject ToastPanel;
    [SerializeField]
    private Text text;

    private static CenterToastPanel mInstance = null;
    public static CenterToastPanel Instance
    {
        get
        {
            return mInstance;
        }
    }

    private void Awake()
    {
        mInstance = this;
    }

    public void ShowToast(string xmlString)
    {
        MainThreadQueue.ExecuteQueue.Enqueue(() =>
        {
            //text.SetTextByKey(xmlString);
            ToastPanel.SetActive(true);

            if (IsInvoking("HideToast"))
                CancelInvoke("HideToast");
            Invoke("HideToast", 2);
        });
    }

    public void ShowToast(string xmlString, params object[] args)
    {
        MainThreadQueue.ExecuteQueue.Enqueue(() =>
        {
            //text.SetTextByKey(xmlString, args);
            ToastPanel.SetActive(true);
            if (IsInvoking("HideToast"))
                CancelInvoke("HideToast");
            Invoke("HideToast", 2);
        });
    }

    public void HideToast()
    {
        text.text = string.Empty;
        ToastPanel.SetActive(false);
    }
}

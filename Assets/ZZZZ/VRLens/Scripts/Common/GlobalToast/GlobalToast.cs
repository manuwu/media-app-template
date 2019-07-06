/*
 * 黄秋燕 Shemi
 * 全局Toast提示
 * 2018-7-5
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IVR.Language;

public class GlobalToast : MonoBehaviour
{
    public GameObject Toast;
    public Text HintText;

    /* 跟随摄像机旋转，抖动严重，屏蔽
    bool isShow;
    Vector3 CameraToUIVector;

    private void Start()
    {
        isShow = false;
        CameraToUIVector = Toast.transform.position - Camera.main.transform.position;
    }

    private void Update()
    {
        if (isShow)
            UpdateUI(Camera.main.transform.forward);
    }

    void UpdateUI(Vector3 dir)
    {
        Vector3 valueDir = Vector3.Cross(Vector3.forward, dir);
        float valueAngle = Vector3.Angle(Vector3.forward, dir);

        Vector3 cToUVector = PreDefScrp.RotateAroundAxis(CameraToUIVector, valueDir, valueAngle);

        Toast.transform.forward = dir;
        Toast.transform.position = Camera.main.transform.position + cToUVector;
    }
    */

    /// <summary>
    /// 显示全局提示
    /// </summary>
    /// <param name="str">must be xml language key</param>
    /// <param name="hideTime">多少秒以后隐藏toast</param>
    public void ShowToastByXMLLanguageKey(string str, float hideTime = 3)
    {
        if (IsInvoking("Hide"))
            CancelInvoke("Hide");

        Debug.Log("ShowToast str = "+ str);
        //HintText.SetTextByKey(str);
        Toast.SetActive(true);
        
        if (hideTime >= 0)
            Invoke("Hide", hideTime);
    }

    public void ShowToastByXMLLanguageKey(string str, params object[] args)
    {
        if (IsInvoking("Hide"))
            CancelInvoke("Hide");

        Debug.Log("ShowToast str = " + str);
        //HintText.SetTextByKey(str, args);
        Toast.SetActive(true);

        Invoke("Hide", 3);
    }

    /// <summary>
    /// 显示全局提示
    /// </summary>
    /// <param name="str">传入即输出</param>
    /// <param name="hideTime">多少秒以后隐藏toast</param>
    public void ShowToast(string str, float hideTime = 3)
    {
        if (IsInvoking("Hide"))
            CancelInvoke("Hide");

        Debug.Log("ShowToast str = " + str);
        HintText.text = str;
        Toast.SetActive(true);

        if (hideTime >= 0)
            Invoke("Hide", hideTime);
    }

    public void Hide()
    {
        HintText.text = string.Empty;
        Toast.SetActive(false);
    }
}

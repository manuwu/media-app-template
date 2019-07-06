/*
 * Author:李传礼
 * DateTime:2017.12.16
 * Description:影院顶层面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TopPanel : MonoBehaviour
{
    public Text VideoNameText;
    public Button BackBtn;
    public MixedButton SettingBtn;

    public Action ClickBackBtnCallback;
    public Action ClickSettingBtnCallback;
    public Action<bool> PointerEnterUICallback;

    void Start ()
    {
        Init();
    }

    void Init()
    {
        EventTriggerListener.Get(BackBtn.gameObject).OnPtClick = ClickBackBtn;
        EventTriggerListener.Get(SettingBtn.gameObject).OnPtClick = ClickSettingBtn;

        EventTriggerListener.Get(this.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(this.gameObject).OnPtExit = OnPointerExit;
    }

    public void SetVideoName(string name)
    {
        VideoNameText.text = name;
    }

    void ClickBackBtn(GameObject go)
    {
        if (ClickBackBtnCallback != null)
            ClickBackBtnCallback();
    }

    void ClickSettingBtn(GameObject go)
    {
        if (ClickSettingBtnCallback != null)
            ClickSettingBtnCallback();
    }

    public void Show()
    {
        this.transform.localScale = Vector3.one;
    }

    public void Hide()
    {
        this.transform.localScale = Vector3.zero;
    }

    void OnPointerEnter(GameObject go)
    {
        if (PointerEnterUICallback != null)
            PointerEnterUICallback(true);
    }

    void OnPointerExit(GameObject go)
    {
        if (PointerEnterUICallback != null)
            PointerEnterUICallback(false);
    }
}

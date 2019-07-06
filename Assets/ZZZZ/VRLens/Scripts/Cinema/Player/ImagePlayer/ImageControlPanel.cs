
 /* 2018-5-22
 * 黄秋燕 Shemi
 * 图片播放器的控制面板
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ImageControlPanel : MonoBehaviour
{
    [SerializeField]
    private BackButton BackBtn;
    [SerializeField]
    private Button PreviousBtn;
    [SerializeField]
    private Button NextBtn;
    [SerializeField]
    private Button SettingBtn;
    [SerializeField]
    private Text NameText;
    [SerializeField]
    private Text TotalCountText;
    [SerializeField]
    private Text CurIndexText;
    public ImageSettingsPanel SettingsPanel;

    [HideInInspector]
    public RawImage PreviousBtnImage;
    [HideInInspector]
    public RawImage NextBtnImage;

    public Action ClickBackBtnCallback;
    public Action ClickPreviousBtnCallback;
    public Action ClickNextBtnCallback;
    public Action<bool> PointerEnterUICallback;

    public void Init()
    {
        SettingsPanel.Init();

        PreviousBtnImage = PreviousBtn.GetComponentInChildren<RawImage>();
        NextBtnImage = NextBtn.GetComponentInChildren<RawImage>();

        EventTriggerListener.Get(BackBtn.gameObject).OnPtClick = ClickBackBtn;
        EventTriggerListener.Get(PreviousBtn.gameObject).OnPtClick = ClickPreviousBtn;
        EventTriggerListener.Get(NextBtn.gameObject).OnPtClick = ClickNextBtn;
        EventTriggerListener.Get(SettingBtn.gameObject).OnPtClick = ClickSettingBtn;

        //EventTriggerListener.Get(this.gameObject).OnPtEnter += OnPointerEnter;
        //EventTriggerListener.Get(this.gameObject).OnPtExit += OnPointerExit;
        EventTriggerListener.Get(BackBtn.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(BackBtn.gameObject).OnPtExit = OnPointerExit;
        EventTriggerListener.Get(PreviousBtn.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(PreviousBtn.gameObject).OnPtExit = OnPointerExit;
        EventTriggerListener.Get(NextBtn.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(NextBtn.gameObject).OnPtExit = OnPointerExit;
        EventTriggerListener.Get(SettingBtn.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(SettingBtn.gameObject).OnPtExit = OnPointerExit;
    }

    void ClickBackBtn(GameObject go)
    {
        if (ClickBackBtnCallback != null)
            ClickBackBtnCallback();
    }

    void ClickPreviousBtn(GameObject go)
    {
        SettingsPanel.Hide();
        Statistics.GetInstance().OnEvent(MediaCenterEvent.PreviousPicture, "点击上一个图片");

        if (ClickPreviousBtnCallback != null)
            ClickPreviousBtnCallback();
    }

    void ClickNextBtn(GameObject go)
    {
        SettingsPanel.Hide();
        Statistics.GetInstance().OnEvent(MediaCenterEvent.NextPicture, "点击下一个图片");

        if (ClickNextBtnCallback != null)
            ClickNextBtnCallback();
    }

    void ClickSettingBtn(GameObject go)
    {
        SettingsPanel.SetSettingsPanelStatus();
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

    public void UpdateImageName(string name)
    {
        NameText.text = name;
    }

    public void UpdateImageTotalCount(int count)
    {
        TotalCountText.text = count.ToString();
    }

    public void UpdateCurImageIndex(int index)
    {
        CurIndexText.text = index.ToString();
    }

    public void Show()
    {
        //this.transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        SettingsPanel.Hide();
        //this.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        BackBtn = null;
        PreviousBtn = null;
        NextBtn = null;
        SettingBtn = null;
        NameText = null;
        TotalCountText = null;
        CurIndexText = null;
        SettingsPanel = null;
        PreviousBtnImage = null;
        NextBtnImage = null;
        ClickBackBtnCallback = null;
        ClickPreviousBtnCallback = null;
        ClickNextBtnCallback = null;
        PointerEnterUICallback = null;
    }
}

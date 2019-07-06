/*
 * Author:李传礼 / 黄秋燕 Shemi
 * DateTime:2017.12.14
 * Description:音量面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class VolumePanel : MonoBehaviour
{
    public Slider VolumeSlider;
    public Text VolumePercentText;
    public Button VolumeAddBtn;
    public Button VolumeDecreaseBtn;

    Vector3 HoverPointStartPos;
    Vector3 HoverPointShowVector;
    bool IsEnter;
    bool IsVoluntary; // 是否物理按键主动
    bool IsMute;//是否静音
    bool IsChangedVolume; // 是否已经修改过一次
    bool IsShow;

    int MaxVolume;
    float VolumeStep;
    float OldSliderValue;

    RectTransform rectTrans; //slider rect
    Vector3 localPos; //slider position

    public Action<float,bool> ChangVolumePercentCallback;
    public Action<bool> PointerEnterUICallback;

    public void Init ()
    {
        IsEnter = false;
        IsVoluntary = false;
        IsMute = false;
        IsChangedVolume = false;
        IsShow = false;

        MaxVolume = 1;
        VolumeStep = 1.0f / 15.0f;
        OldSliderValue = 0;

        rectTrans = VolumeSlider.GetComponent<RectTransform>();
        localPos = VolumeSlider.transform.localPosition - VolumeSlider.transform.right * rectTrans.rect.width / 2;

        VolumeSlider.onValueChanged.AddListener(ChangeVolume);
        
        EventTriggerListener.Get(this.gameObject).OnPtEnter = OnPointerEnterPanel;
        EventTriggerListener.Get(this.gameObject).OnPtExit = OnPointerExitPanel;
        EventTriggerListener.Get(VolumeSlider.gameObject).OnPtEnter = OnPointerEnterPanel;
        EventTriggerListener.Get(VolumeSlider.gameObject).OnPtExit = OnPointerExitPanel;
        EventTriggerListener.Get(VolumeAddBtn.gameObject).OnPtClick = VolumeAdd;
        EventTriggerListener.Get(VolumeDecreaseBtn.gameObject).OnPtClick = VolumeDecrease;
    }

    void OnPointerEnterPanel(GameObject go)
    {
        if (IsEnter)
            return;

        IsEnter = true;

        if (PointerEnterUICallback != null)
            PointerEnterUICallback(true);
    }

    void OnPointerExitPanel(GameObject go)
    {
        if (!IsEnter)
            return;

        IsEnter = false;

        if (PointerEnterUICallback != null)
            PointerEnterUICallback(false);
    }
    
    void VolumeAdd(GameObject go)
    {
        OldSliderValue = VolumeSlider.value;
        float t = OldSliderValue + VolumeStep;
        if (t > MaxVolume)
            t = MaxVolume;
        ChangeVolume(t);
    }

    void VolumeDecrease(GameObject go)
    {
        OldSliderValue = VolumeSlider.value;
        float t = OldSliderValue - VolumeStep;
        if (t < 0)
            t = 0;
        ChangeVolume(t);
    }

    public void SetVolumePanelStatus()
    {
        if (IsShow)
            Hide();
        else
            Show();
    }

    public void ChangeVolume(float f)
    {
        if (IsChangedVolume)
        {
            IsChangedVolume = false;
            return;
        }

        IsVoluntary = true;

        SetCurrentVolume(f);

        Statistics.GetInstance().OnEvent(MediaCenterEvent.ClickOnVideoVolumeBar, "点击音量条");
    }

    public void ChangeVolumeByDevice(float f)
    {
        IsVoluntary = false;

        SetCurrentVolume(f);
    }

    //0-100
    void SetCurrentVolume(float volume)
    {
        if (volume < 0)
            volume = 0;
        else if (volume > MaxVolume)
            volume = MaxVolume;

        float value = volume / MaxVolume;
        IsChangedVolume = true;
        VolumeSlider.value = value; 
        int valuePercent = (int)(value * 100);
        VolumePercentText.text = valuePercent + "%";

        if (value == 0)
            IsMute = true;
        else
            IsMute = false;

        IsChangedVolume = false;
        if (ChangVolumePercentCallback != null)
            ChangVolumePercentCallback(value, IsVoluntary);
    }
    
    public void Show()
    {
        if (IsShow)
            return;

        IsShow = true;
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (!IsShow)
            return;

        IsShow = false;

        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        VolumeSlider = null;
        VolumePercentText = null;
        VolumeAddBtn = null;
        VolumeDecreaseBtn = null;
    }
}

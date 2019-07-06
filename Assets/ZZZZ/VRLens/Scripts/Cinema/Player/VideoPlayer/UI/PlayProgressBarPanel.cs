/*
 * Author:李传礼
 * DateTime:2017.12.14
 * Description:播放进度条面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
#if UD_I3VR
using i3vr;
#endif

public class PlayProgressBarPanel : MonoBehaviour
{
    public Text CurrentTimeText;
    public Text TotalTimeText;
    public Image PlayPBSliderBg;
    public Slider PlayPBSlider;
    public GameObject PlayPBDot;
    Text CurHoverText;

    Vector3 HoverPointStartPos;
    Vector3 HoverPointShowVector;
    bool IsEnter;
    bool IsPBDotShow;
    bool IsEnterAd;

    bool IsVoluntary;//是否为主动改变值
    long TotalTime;//毫秒
    float CurHoverValue;

    RectTransform rectTrans; //slider rect
    Vector3 localPos; //slider position

    public Action<long> SeekToTimeCallback;
    public Action<bool> SliderCheckStatusCallback;

    public void Init()
    {
        IsEnter = false;
        IsPBDotShow = false;
        IsEnterAd = false;
        IsVoluntary = false;
        TotalTime = 0;
        CurHoverValue = 0;

        CurHoverText = PlayPBDot.GetComponentInChildren<Text>();
        rectTrans = PlayPBSlider.GetComponent<RectTransform>();
        localPos = PlayPBSlider.transform.localPosition - PlayPBSlider.transform.right * rectTrans.rect.width / 2;

        PlayPBSlider.onValueChanged.AddListener(ValueChanged);

        EventTriggerListener.Get(PlayPBSliderBg.gameObject).OnPtDown = ClickPlayPBSlider;
        EventTriggerListener.Get(PlayPBSliderBg.gameObject).OnPtUp = OnPointerUpSlider;
        EventTriggerListener.Get(PlayPBSliderBg.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(PlayPBSliderBg.gameObject).OnPtExit = OnPointerExit;
        EventTriggerListener.Get(PlayPBSlider.gameObject).OnPtUp = OnPointerUpSlider;
    }

    void Update()
    {
        if (IsEnter)
            HoverPointFollow();
    }

    void ComputeHoverPointInfo()
    {
        // before recentered real-time update slider position
        HoverPointStartPos = PlayPBSlider.transform.TransformPoint(localPos);
        HoverPointShowVector = PlayPBSlider.transform.TransformVector(Vector3.right * rectTrans.rect.width);
    }

    public void SetTotalTime(long totalTime)
    {
        LogTool.Log("设置总时间：" + totalTime);
        TotalTime = totalTime;

        int seconds = (int)(totalTime / 1000);
        TotalTimeText.text = PreDefScrp.SecondsToHMS(seconds);
    }

    void ValueChanged(float f)
    {
        if (IsVoluntary)
        {
            IsVoluntary = false;
            return;
        }

        if (SliderCheckStatusCallback != null)
            SliderCheckStatusCallback(true);

        long times = (long)(f * TotalTime);
        if (SeekToTimeCallback != null)
            SeekToTimeCallback(times);

        int seconds = (int)(times / 1000);
        CurrentTimeText.text = PreDefScrp.SecondsToHMS(seconds);

        Statistics.GetInstance().OnEvent(MediaCenterEvent.ClickOnTimeline, "点击进度条");
    }

    public void SetCurrentTime(long currentTime)
    {
        int seconds = (int)(currentTime / 1000);
        CurrentTimeText.text = PreDefScrp.SecondsToHMS(seconds);

        float t = 0;
        if (TotalTime != 0)
            t = (float)currentTime / TotalTime;

        IsVoluntary = true;
        PlayPBSlider.value = t;
    }

    void ClickPlayPBSlider(GameObject go)
    {
        PlayPBSlider.value = CurHoverValue;
        //ValueChanged(CurHoverValue);
    }

    public void ShowOrHideSlider(bool isShow)
    {
            this.gameObject.SetActive(isShow);
    }

    public void EnableOrDisableSlider(bool isEnterAd)
    {
        //if (IsEnterAd == isEnterAd) return;
        //IsEnterAd = isEnterAd;

        if (isEnterAd)
            PlayPBSliderBg.GetComponent<Image>().raycastTarget = false;
        else
            PlayPBSliderBg.GetComponent<Image>().raycastTarget = true;
    }

    void OnPointerEnter(GameObject go)
    {
        IsEnter = true;
        ComputeHoverPointInfo();
    }

    void OnPointerExit(GameObject go)
    {
        IsEnter = false;
        ShowOrHidePBDot(false);
    }

    void OnPointerUpSlider(GameObject go)
    {
        if (SliderCheckStatusCallback != null)
            SliderCheckStatusCallback(false);
    }

    void HoverPointFollow()
    {
        Vector3 pos = GvrPointerInputModule.CurrentRaycastResult.worldPosition;
        Vector3 v = pos - HoverPointStartPos;
        float len = PreDefScrp.GetProjLen(v, HoverPointShowVector);
        if (len >= 0 && len < HoverPointShowVector.magnitude)
        {
            ShowOrHidePBDot(true);
            CurHoverValue = (HoverPointShowVector.normalized * len).magnitude / HoverPointShowVector.magnitude;
            long times = (long)(CurHoverValue * TotalTime);
            int seconds = (int)(times / 1000);
            CurHoverText.text = PreDefScrp.SecondsToHMS(seconds);
            PlayPBDot.transform.position = HoverPointStartPos + HoverPointShowVector.normalized * len;
            PlayPBDot.transform.localPosition = new Vector3(PlayPBDot.transform.localPosition.x, 0, PlayPBDot.transform.localPosition.z);
        }
        else
            ShowOrHidePBDot(false);
    }

    void ShowOrHidePBDot(bool isShow)
    {
        if (IsPBDotShow == isShow)
            return;

        if(isShow)
            PlayPBDot.gameObject.SetActive(true);
        else
            PlayPBDot.gameObject.SetActive(false);

        IsPBDotShow = isShow;
    }

    private void OnDestroy()
    {
        CurrentTimeText = null;
        TotalTimeText = null;
        PlayPBSliderBg = null;
        PlayPBSlider = null;
        PlayPBDot = null;
        CurHoverText = null;
        rectTrans = null;
        SeekToTimeCallback = null;
        SliderCheckStatusCallback = null;
    }
}

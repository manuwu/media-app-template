/*
 * 2018-8-17
 * 黄秋燕 Shemi
 * 视频循环模式面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum LoopType { SinglePlay, AutoReplay, ListLoop }

public class LoopModePanel : MonoBehaviour
{
    public MixedButton SingleCycleBtn;
    public MixedButton AutoPlayBtn;

    bool IsEnter;
    bool IsShow;
    bool SingleCycleBtnIsDisable;

    public Action<bool> PointerEnterUICallback;
    public Action<GameObject, bool> PointerEnterBtnCallback;
    public Action<bool> ChangeLoopTypeCallback;
    public Action<bool> ChangeAutoPlayCallback;

    public void Init()
    {
        IsEnter = false;
        IsShow = false;
        SingleCycleBtnIsDisable = false;

        SingleCycleBtn.SelectBtnCallback += SelectSingleCycleBtn;
        AutoPlayBtn.SelectBtnCallback += SelectAutoPlayBtn;

        EventTriggerListener.Get(this.gameObject).OnPtEnter = OnPointerEnterPanel;
        EventTriggerListener.Get(this.gameObject).OnPtExit = OnPointerExitPanel;
        EventTriggerListener.Get(SingleCycleBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(SingleCycleBtn.gameObject).OnPtExit = OnPointerExitBtn;
        EventTriggerListener.Get(AutoPlayBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(AutoPlayBtn.gameObject).OnPtExit = OnPointerExitBtn;

#if SVR_USE_GAZE
        AutoPlayBtn.gameObject.SetActive(true);
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(215, 215);
        this.transform.localPosition = new Vector3(111, 112.5f, 0);
        this.transform.GetChild(0).localPosition = new Vector3(-4.5f, 77.2f, 0);
        this.transform.GetChild(1).localPosition = new Vector3(-5, 13.7f, 0);
#else
        AutoPlayBtn.gameObject.SetActive(false);
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(215, 130);
        this.transform.localPosition = new Vector3(111, 72.2f, 0);
        this.transform.GetChild(0).localPosition = new Vector3(-4.5f, 42.7f, 0);
        this.transform.GetChild(1).localPosition = new Vector3(-5, -19f, 0);
#endif
    }

    void SelectSingleCycleBtn(MixedButton stb, bool isSelect)
    {
        if (SingleCycleBtnIsDisable) return;
        if (ChangeLoopTypeCallback != null)
            ChangeLoopTypeCallback(isSelect);
    }

    void SelectAutoPlayBtn(MixedButton btn, bool isSelect)
    {
        if (ChangeAutoPlayCallback != null)
            ChangeAutoPlayCallback(isSelect);
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

    void OnPointerEnterBtn(GameObject go)
    {
        if (SingleCycleBtnIsDisable) return;
        if (PointerEnterBtnCallback != null)
            PointerEnterBtnCallback(go, true);
    }

    void OnPointerExitBtn(GameObject go)
    {
        if (SingleCycleBtnIsDisable) return;
        if (PointerEnterBtnCallback != null)
            PointerEnterBtnCallback(go, false);
    }

    public void SetLoopModePanelStatus()
    {
        if (IsShow)
            Hide();
        else
            Show();
    }

    public void Show()
    {
        if (IsShow)
            return;

        IsShow = true;

        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            if (VideoPlayManage.CurLoopType == LoopType.SinglePlay)
                SingleCycleBtn.SetSelected(true);
            else
                SingleCycleBtn.SetSelected(false);
        }
        else if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.KTTV)
        {
            SingleCycleBtn.SetSelected(false);
            SingleCycleBtnIsDisable = true;
            Image SingleCycleBtnImage = SingleCycleBtn.GetComponent<Image>();
            SingleCycleBtnImage.raycastTarget = false;
            SingleCycleBtnImage.color = new Color(SingleCycleBtnImage.color.r, SingleCycleBtnImage.color.g, SingleCycleBtnImage.color.b, 0.3f);
            Text SingleCycleBtnText = SingleCycleBtn.GetComponentInChildren<Text>();
            SingleCycleBtnText.color = new Color(SingleCycleBtnText.color.r, SingleCycleBtnText.color.g, SingleCycleBtnText.color.b, 0.3f);
        }

        if (CinemaPanel.IsPlayLoop)
            AutoPlayBtn.SetSelected(true);
        else
            AutoPlayBtn.SetSelected(false);

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
        SingleCycleBtn = null;
        AutoPlayBtn = null;
        PointerEnterUICallback = null;
        PointerEnterBtnCallback = null;
        ChangeLoopTypeCallback = null;
        ChangeAutoPlayCallback = null;
    }
}

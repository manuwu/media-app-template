/*
 * 2018-8-6
 * 黄秋燕 Shemi
 * 屏幕尺寸管理面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum ScreenSizeType
{
    Cinema, Standard, MINI
}

public class ScreenSizePanel : MonoBehaviour {
    public MixedButton CinemaBtn;
    public MixedButton StandardBtn;
    public MixedButton MINIBtn;

    bool IsEnter;
    bool IsShow;

    public Action<bool> PointerEnterUICallback;
    public Action<GameObject, bool> PointerEnterBtnCallback;
    public Action ChangeScreenSizeTypeCallback;

    public void Init()
    {
        IsEnter = false;
        IsShow = false;

        CinemaBtn.SelectBtnCallback = SelectCinemaBtn;
        StandardBtn.SelectBtnCallback = SelectStandardBtn;
        MINIBtn.SelectBtnCallback = SelectIMINIBtn;

        EventTriggerListener.Get(this.gameObject).OnPtEnter = OnPointerEnterPanel;
        EventTriggerListener.Get(this.gameObject).OnPtExit = OnPointerExitPanel;
        EventTriggerListener.Get(CinemaBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(CinemaBtn.gameObject).OnPtExit = OnPointerExitBtn;
        EventTriggerListener.Get(StandardBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(StandardBtn.gameObject).OnPtExit = OnPointerExitBtn;
        EventTriggerListener.Get(MINIBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(MINIBtn.gameObject).OnPtExit = OnPointerExitBtn;
    }
    
    void SelectCinemaBtn(MixedButton stb, bool isSelect)
    {
        SetAllBtnUnselected();
        CinemaBtn.SetSelected(true);

        GlobalVariable.SetScreenSizeType(ScreenSizeType.Cinema);
        if (ChangeScreenSizeTypeCallback != null)
            ChangeScreenSizeTypeCallback();
    }

    void SelectStandardBtn(MixedButton stb, bool isSelect)
    {
        SetAllBtnUnselected();
        StandardBtn.SetSelected(true);

        GlobalVariable.SetScreenSizeType(ScreenSizeType.Standard);
        if (ChangeScreenSizeTypeCallback != null)
            ChangeScreenSizeTypeCallback();
    }

    void SelectIMINIBtn(MixedButton stb, bool isSelect)
    {
        SetAllBtnUnselected();
        MINIBtn.SetSelected(true);

        GlobalVariable.SetScreenSizeType(ScreenSizeType.MINI);
        if (ChangeScreenSizeTypeCallback != null)
            ChangeScreenSizeTypeCallback();
    }

    void SetAllBtnUnselected()
    {
        CinemaBtn.SetSelected(false);
        StandardBtn.SetSelected(false);
        MINIBtn.SetSelected(false);
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
        if (PointerEnterBtnCallback != null)
            PointerEnterBtnCallback(go, true);
    }

    void OnPointerExitBtn(GameObject go)
    {
        if (PointerEnterBtnCallback != null)
            PointerEnterBtnCallback(go, false);
    }

    public void SetScreenSizePanelStatus()
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

        SetAllBtnUnselected();
        ScreenSizeType sizeType = GlobalVariable.GetScreenSizeType();
        switch (sizeType)
        {
            case ScreenSizeType.Cinema:
                CinemaBtn.SetSelected(true);
                break;
            case ScreenSizeType.Standard:
                StandardBtn.SetSelected(true);
                break;
            case ScreenSizeType.MINI:
                MINIBtn.SetSelected(true);
                break;
            default:
                break;
        }

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
        CinemaBtn = null;
        StandardBtn = null;
        MINIBtn = null;
        PointerEnterUICallback = null;
        PointerEnterBtnCallback = null;
        ChangeScreenSizeTypeCallback = null;
    }
}

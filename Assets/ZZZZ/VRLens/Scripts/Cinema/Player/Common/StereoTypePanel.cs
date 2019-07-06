/*
 * Author:李传礼
 * DateTime:2017.12.21
 * Description:立体类型面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StereoTypePanel : VRInputMessageBase
{
    public MixedButton[] StereoTypeBtns;
    public MixedButton[] ProjectionTypeBtns;
    public MixedButton TensileBtn;

    Image TensileBtnImage;
    Text TensileBtnText;

    int STBIndex;//立体类型按钮索引
    int PTBIndex;//投影类型按钮索引
    bool IsEnter;
    bool IsShow;
    bool IsAllowEvent;

    public Action ShowStereoTypePanelCallback;
    public Action<StereoType> ChangeStereoTypeCallback;
    public Action<bool> PointerEnterUICallback;
    public Action<bool> TensileSwitchCallback;

    public void Init()
    {
        STBIndex = 0;
        PTBIndex = 0;
        IsEnter = false;
        IsShow = false;
        IsAllowEvent = true;

        TensileBtnImage = TensileBtn.GetComponent<Image>();
        TensileBtnText = TensileBtn.GetComponentInChildren<Text>();

        foreach (MixedButton btn in StereoTypeBtns)
        {
            btn.SelectBtnCallback = SelectSTB;
        }

        foreach (MixedButton btn in ProjectionTypeBtns)
        {
            btn.SelectBtnCallback = SelectPTB;
        }

        EventTriggerListener.Get(this.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(this.gameObject).OnPtExit = OnPointerExit;

        TensileBtn.SelectBtnCallback = ClickTensileBtn;
        TensileBtn.PointerEnterBtnCallback = OnPointerEnterBtn;
        TensileBtn.PointerExitBtnCallback = OnPointerExitBtn;
    }

    public void Show(StereoType stereoType)
    {
        StereoTypeBtns[STBIndex].SetSelected(false);
        ProjectionTypeBtns[PTBIndex].SetSelected(false);

        switch (stereoType)
        {
            case StereoType.ST2D:
                STBIndex = 0;
                PTBIndex = 0;
                break;
            case StereoType.ST180_2D:
                STBIndex = 0;
                PTBIndex = 1;
                break;
            case StereoType.ST360_2D:
                STBIndex = 0;
                PTBIndex = 2;
                break;
            case StereoType.ST3D_LR:
                STBIndex = 1;
                PTBIndex = 0;
                break;
            case StereoType.ST180_LR:
                STBIndex = 1;
                PTBIndex = 1;
                break;
            case StereoType.ST360_LR:
                STBIndex = 1;
                PTBIndex = 2;
                break;
            case StereoType.ST3D_TB:
                STBIndex = 2;
                PTBIndex = 0;
                break;
            case StereoType.ST180_TB:
                STBIndex = 2;
                PTBIndex = 1;
                break;
            case StereoType.ST360_TB:
                STBIndex = 2;
                PTBIndex = 2;
                break;
        }

        StereoTypeBtns[STBIndex].SetSelected(true);
        ProjectionTypeBtns[PTBIndex].SetSelected(true);

        TensileBtnControl(stereoType);
        Show();
    }

    void SelectSTB(MixedButton stb, bool isSelect)
    {
        if (stb != StereoTypeBtns[STBIndex])
            StereoTypeBtns[STBIndex].SetSelected(false);
        else
        {
            stb.SetSelected(true);
            return;
        }

        for (int i = 0; i < StereoTypeBtns.Length; i++)
        {
            if (StereoTypeBtns[i] == stb)
            {
                STBIndex = i;
                break;
            }
        }

        StereoType st = GetStereoType(STBIndex, PTBIndex);
        if (ChangeStereoTypeCallback != null)
            ChangeStereoTypeCallback(st);
    }

    void SelectPTB(MixedButton ptb, bool isSelect)
    {
        if (ptb != ProjectionTypeBtns[PTBIndex])
            ProjectionTypeBtns[PTBIndex].SetSelected(false);
        else
        {
            ptb.SetSelected(true);
            return;
        }

        for (int i = 0; i < ProjectionTypeBtns.Length; i++)
        {
            if (ProjectionTypeBtns[i] == ptb)
            {
                PTBIndex = i;
                break;
            }
        }

        StereoType st = GetStereoType(STBIndex, PTBIndex);
        if (ChangeStereoTypeCallback != null)
            ChangeStereoTypeCallback(st);
    }

    StereoType GetStereoType(int stbIndex, int ptbIndex)
    {
        if(stbIndex == 0)
        {
            if (ptbIndex == 0)
                return StereoType.ST2D;
            else if (ptbIndex == 1)
                return StereoType.ST180_2D;
            else
                return StereoType.ST360_2D;
        }
        else if( stbIndex == 1)
        {
            if (ptbIndex == 0)
                return StereoType.ST3D_LR;
            else if (ptbIndex == 1)
                return StereoType.ST180_LR;
            else
                return StereoType.ST360_LR;
        }
        else
        {
            if (ptbIndex == 0)
                return StereoType.ST3D_TB;
            else if (ptbIndex == 1)
                return StereoType.ST180_TB;
            else
                return StereoType.ST360_TB;
        }
    }

    void OnPointerEnter(GameObject go)
    {
        IsEnter = true;

        if (PointerEnterUICallback != null)
            PointerEnterUICallback(true);
    }

    void OnPointerExit(GameObject go)
    {
        if (!IsEnter)
            return;

        IsEnter = false;

        if (PointerEnterUICallback != null)
            PointerEnterUICallback(false);
    }

    void OnPointerEnterBtn(MixedButton stb)
    {
        if (!IsAllowEvent)
            return;

        OnPointerEnter(stb.gameObject);
    }

    void OnPointerExitBtn(MixedButton stb)
    {
        if (!IsAllowEvent)
            return;

        OnPointerExit(stb.gameObject);
    }

    void ClickTensileBtn(MixedButton stb, bool isSelect)
    {
        if (!IsAllowEvent)
            return;

        if (TensileSwitchCallback != null)
            TensileSwitchCallback(isSelect);
    }

    void TensileBtnSelectControl()
    {
        //if (GlobalVariable.GetSceneModel() == SceneModel.IMAXTheater)
        //{
        //    TensileBtn.SetSelected(false);
        //    return;
        //}

        TensileBtn.SetSelected(MediaStretchPlayerPrefsDetector.GetInstance().GetMediaStretchKey());
    }

    public void TensileBtnControl(StereoType stereoType)
    {
        //if (GlobalVariable.GetSceneModel() == SceneModel.Default || GlobalVariable.GetSceneModel() == SceneModel.StarringNight)
        {
            switch (stereoType)
            {
                //case StereoType.ST2D:
                case StereoType.ST3D_LR:
                case StereoType.ST3D_TB:
                    IsAllowEvent = true;
                    TensileBtnImage.raycastTarget = true;
                    TensileBtnText.color = new Color(130.0f / 255.0f, 136.0f / 255.0f, 142.0f / 255.0f, 1);
                    TensileBtnImage.color = Color.white;
                    break;
                default:
                    IsAllowEvent = false;
                    TensileBtnImage.raycastTarget = false;
                    TensileBtnText.color = new Color(130.0f / 255.0f, 136.0f / 255.0f, 142.0f / 255.0f, 0.3f);
                    TensileBtnImage.color = new Color(1, 1, 1, 0.3f);
                    break;
            }
        }
        //else if (GlobalVariable.GetSceneModel() == SceneModel.IMAXTheater)
        //{
        //    IsAllowEvent = false;
        //    TensileBtnImage.raycastTarget = false;
        //    TensileBtnText.color = new Color(130.0f / 255.0f, 136.0f / 255.0f, 142.0f / 255.0f, 0.3f);
        //    TensileBtnImage.color = new Color(1, 1, 1, 0.3f);
        //}
    }

    public void SetStereoTypePanelStatus()
    {
        if (IsShow)
            Hide();
        else
        {
            //Show();
            TensileBtnSelectControl();

            if (ShowStereoTypePanelCallback != null)
                ShowStereoTypePanelCallback();
        }
    }

    void Show()
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
        StereoTypeBtns = null;
        ProjectionTypeBtns = null;
        TensileBtn = null;
        TensileBtnImage = null;
        TensileBtnText = null;
        ShowStereoTypePanelCallback = null;
        ChangeStereoTypeCallback = null;
        PointerEnterUICallback = null;
        TensileSwitchCallback = null;
    }
}

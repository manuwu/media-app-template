/*
 * 2018-8-8
 * 黄秋燕 Shemi
 * 图片设置管理面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ImageSettingsPanel : MonoBehaviour {
    //public ImageStereoTypePanel StereoTypePanel;

    Image TensileBtnImage;

    bool IsShow;

    public Action<bool> PointerEnterUICallback;

    public void Init()
    {
        //StereoTypePanel.Init();

        IsShow = false;

        EventTriggerListener.Get(this.gameObject).OnPtEnter += OnPointerEnterPanel;
        EventTriggerListener.Get(this.gameObject).OnPtExit += OnPointerExitPanel;
    }

    void OnPointerEnterPanel(GameObject go)
    {
        if (PointerEnterUICallback != null)
            PointerEnterUICallback(true);
    }

    void OnPointerExitPanel(GameObject go)
    {
        if (PointerEnterUICallback != null)
            PointerEnterUICallback(false);
    }

    public void SetSettingsPanelStatus()
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
        //StereoTypePanel.SetStereoTypePanelStatus();
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (!IsShow)
            return;

        IsShow = false;

        //StereoTypePanel.SetStereoTypePanelStatus();
        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        //StereoTypePanel = null;
        TensileBtnImage = null;
        PointerEnterUICallback = null;
    }
}

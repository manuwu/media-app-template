/*
 * 2018-8-6
 * 黄秋燕 Shemi
 * 场景选择管理面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum SceneModel
{
    Default, StarringNight, IMAXTheater,
    //汽车影院(默认Drive_Playboy)
    Drive
}

public enum DriveSceneModel { Karting, King, Playboy, Rattletrap }

public class SceneChangePanel : MonoBehaviour {
    public MixedButton DefaultBtn;
    public MixedButton ImaxTheaterBtn;
    public MixedButton StarrySkyBtn;
    public MixedButton DriveBtn;

    bool IsEnter;
    bool IsShow;

    public Action<bool> PointerEnterUICallback;
    public Action<GameObject, bool> PointerEnterBtnCallback;
    public Action<bool, bool> ChangeSceneStyleCallback;

    public void Init()
    {
        IsEnter = false;
        IsShow = false;

        DefaultBtn.SelectBtnCallback = SelectDefaultScene;
        StarrySkyBtn.SelectBtnCallback = SelectStarrySkyScene;
        ImaxTheaterBtn.SelectBtnCallback = SelectImaxScene;
        DriveBtn.SelectBtnCallback = SelectDriveScene;

        EventTriggerListener.Get(this.gameObject).OnPtEnter = OnPointerEnterPanel;
        EventTriggerListener.Get(this.gameObject).OnPtExit = OnPointerExitPanel;
        EventTriggerListener.Get(DefaultBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(DefaultBtn.gameObject).OnPtExit = OnPointerExitBtn;
        EventTriggerListener.Get(StarrySkyBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(StarrySkyBtn.gameObject).OnPtExit = OnPointerExitBtn;
        EventTriggerListener.Get(ImaxTheaterBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(ImaxTheaterBtn.gameObject).OnPtExit = OnPointerExitBtn;
        EventTriggerListener.Get(DriveBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(DriveBtn.gameObject).OnPtExit = OnPointerExitBtn;
    }

    void SelectDefaultScene(MixedButton stb, bool isSelect)
    {
        GlobalVariable.SetSceneModel(SceneModel.Default);
        SetBtnStatus();
        if (ChangeSceneStyleCallback != null)
            ChangeSceneStyleCallback(true, true);
    }

    void SelectStarrySkyScene(MixedButton stb, bool isSelect)
    {
        GlobalVariable.SetSceneModel(SceneModel.StarringNight);
        SetBtnStatus();
        if (ChangeSceneStyleCallback != null)
            ChangeSceneStyleCallback(true, true);
    }

    void SelectImaxScene(MixedButton stb, bool isSelect)
    {
        GlobalVariable.SetSceneModel(SceneModel.IMAXTheater);
        SetBtnStatus();
        if (ChangeSceneStyleCallback != null)
            ChangeSceneStyleCallback(false, true);
    }

    void SelectDriveScene(MixedButton stb, bool isSelect)
    {
        GlobalVariable.SetSceneModel(SceneModel.Drive);
        SetBtnStatus();
        if (ChangeSceneStyleCallback != null)
            ChangeSceneStyleCallback(false, true);
    }

    private void SetBtnStatus()
    {
        SceneModel sceneModel = GlobalVariable.GetSceneModel();
        if (sceneModel == SceneModel.Default)
        {
            DefaultBtn.SetSelected(true);
            StarrySkyBtn.SetSelected(false);
            ImaxTheaterBtn.SetSelected(false);
            DriveBtn.SetSelected(false);
        }
        else if (sceneModel == SceneModel.StarringNight)
        {
            StarrySkyBtn.SetSelected(true);
            DefaultBtn.SetSelected(false);
            ImaxTheaterBtn.SetSelected(false);
            DriveBtn.SetSelected(false);
        }
        else if (sceneModel == SceneModel.IMAXTheater)
        {
            ImaxTheaterBtn.SetSelected(true);
            StarrySkyBtn.SetSelected(false);
            DefaultBtn.SetSelected(false);
            DriveBtn.SetSelected(false);
        }
        else if (sceneModel == SceneModel.Drive)
        {
            DriveBtn.SetSelected(true);
            ImaxTheaterBtn.SetSelected(false);
            StarrySkyBtn.SetSelected(false);
            DefaultBtn.SetSelected(false);
        }
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

    public void SetSceneChangePanelStatus()
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
        SetBtnStatus();

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
        DefaultBtn = null;
        ImaxTheaterBtn = null;
        DriveBtn = null;
        StarrySkyBtn = null;
        PointerEnterUICallback = null;
        PointerEnterBtnCallback = null;
        ChangeSceneStyleCallback = null;
    }
}

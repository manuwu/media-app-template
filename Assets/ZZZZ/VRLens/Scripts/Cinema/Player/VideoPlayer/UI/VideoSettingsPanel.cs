/*
 * 2018-8-6
 * 黄秋燕 Shemi
 * 视频设置管理面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using IVR.Language;

public class VideoSettingsPanel : MonoBehaviour {
    public ScreenSizePanel ScreenSizePanel;
    public StereoTypePanel StereoTypePanel;
    public SceneChangePanel SceneChangePanel;
    public LoopModePanel LoopModePanel;
    public DefinitionPanel DefinitionPanel;

    public Button ScreenSizeBtn;
    public Button StereoTypeBtn;
    public Button SceneChangeBtn;
    public Button LoopModeBtn;
    public Button DefinitionBtn;

    public Color NormalColor;
    public Color HoverColor;
    public Color DisableColor;
    public Image ScreenSizeBtnChildImage;
    public Image SceneChangeBtnChildImage;
    public Sprite ScreenSizeBtnHoverSpr;
    public Sprite ScreenSizeBtnNormalSpr;
    public Sprite SceneChangeBtnHoverSpr;
    public Sprite SceneChangeBtnNormalSpr;

    bool IsEnter;
    bool IsShow;
    bool ScreenSizeBtnIsInteractable;
    bool SceneChangeBtnIsInteractable;

    public Action<bool> PointerEnterUICallback;

    public void Init()
    {
        ScreenSizePanel.Init();
        StereoTypePanel.Init();
        SceneChangePanel.Init();
        LoopModePanel.Init();
        DefinitionPanel.Init();

        IsEnter = false;
        IsShow = false;
        ScreenSizeBtnIsInteractable = true;
        SceneChangeBtnIsInteractable = true;

        EventTriggerListener.Get(this.gameObject).OnPtEnter = OnPointerEnterUI;
        EventTriggerListener.Get(this.gameObject).OnPtExit = OnPointerExitUI;
        EventTriggerListener.Get(ScreenSizeBtn.gameObject).OnPtEnter = OnPointerEnterScreenSizeBtn;
        EventTriggerListener.Get(ScreenSizeBtn.gameObject).OnPtExit = OnPointerExitScreenSizeBtn;
        EventTriggerListener.Get(StereoTypeBtn.gameObject).OnPtEnter = OnPointerEnterUI;
        EventTriggerListener.Get(StereoTypeBtn.gameObject).OnPtExit = OnPointerExitUI;
        EventTriggerListener.Get(SceneChangeBtn.gameObject).OnPtEnter = OnPointerEnterSceneChangeBtn;
        EventTriggerListener.Get(SceneChangeBtn.gameObject).OnPtExit = OnPointerExitSceneChangeBtn;
        EventTriggerListener.Get(LoopModeBtn.gameObject).OnPtEnter = OnPointerEnterUI;
        EventTriggerListener.Get(LoopModeBtn.gameObject).OnPtExit = OnPointerExitUI;
        EventTriggerListener.Get(DefinitionBtn.gameObject).OnPtEnter = OnPointerEnterUI;
        EventTriggerListener.Get(DefinitionBtn.gameObject).OnPtExit = OnPointerExitUI;

        EventTriggerListener.Get(ScreenSizeBtn.gameObject).OnPtClick = ClickScreenSizeBtn;
        EventTriggerListener.Get(StereoTypeBtn.gameObject).OnPtClick = ClickStereoTypeBtn;
        EventTriggerListener.Get(SceneChangeBtn.gameObject).OnPtClick = ClickSceneChangeBtn;
        EventTriggerListener.Get(LoopModeBtn.gameObject).OnPtClick = ClickLoopModeBtn;
        EventTriggerListener.Get(DefinitionBtn.gameObject).OnPtClick = ClickDefinitionBtn;

        SceneChangePanel.PointerEnterBtnCallback = PointerEnterBtn;
        ScreenSizePanel.PointerEnterBtnCallback = PointerEnterBtn;
        LoopModePanel.PointerEnterBtnCallback = PointerEnterBtn;
        DefinitionPanel.PointerEnterBtnCallback = PointerEnterBtn;
        DefinitionPanel.ChangeDefinitionModelCallback += ChangeDefinitionModel;

        if (PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.Local
            || PlayerDataControl.GetInstance().CurPlayingMode == PlayingURLMode.LiveUrl)
        {
            LoopModeBtn.gameObject.SetActive(true);
            DefinitionBtn.gameObject.SetActive(false);
        }
        else
        {
            LoopModeBtn.gameObject.SetActive(false);
            DefinitionBtn.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        DefinitionPanel.ChangeDefinitionModelCallback -= ChangeDefinitionModel;     
        ScreenSizeBtn = null;
        StereoTypeBtn = null;
        SceneChangeBtn = null;
        LoopModeBtn = null;
        DefinitionBtn = null;
        ScreenSizeBtnChildImage = null;
        SceneChangeBtnChildImage = null;
        ScreenSizeBtnHoverSpr = null;
        ScreenSizeBtnNormalSpr = null;
        SceneChangeBtnHoverSpr = null;
        SceneChangeBtnNormalSpr = null;
    }

    void OnPointerEnterUI(GameObject go)
    {
        if (PointerEnterUICallback != null)
            PointerEnterUICallback(true);
    }

    void OnPointerExitUI(GameObject go)
    {
        if (PointerEnterUICallback != null)
            PointerEnterUICallback(false);
    }

    void PointerEnterBtn(GameObject go, bool isEnter)
    {
        if (isEnter)
            OnPointerEnterUI(go);
        else
            OnPointerExitUI(go);
    }

    void OnPointerEnterScreenSizeBtn(GameObject go)
    {
        if (ScreenSizeBtnIsInteractable)
        {
            OnPointerEnterUI(go);
            ScreenSizeBtnChildImage.sprite = ScreenSizeBtnHoverSpr;
            ScreenSizeBtn.GetComponentInChildren<Text>().color = HoverColor;
        }
        else
            OnPointerEnterUI(go);
    }

    void OnPointerExitScreenSizeBtn(GameObject go)
    {
        if (ScreenSizeBtnIsInteractable)
        {
            OnPointerExitUI(go);
            ScreenSizeBtnChildImage.sprite = ScreenSizeBtnNormalSpr;
            ScreenSizeBtn.GetComponentInChildren<Text>().color = NormalColor;
        }
        else
            OnPointerExitUI(go);
    }

    void OnPointerEnterSceneChangeBtn(GameObject go)
    {
        if (SceneChangeBtnIsInteractable)
        {
            OnPointerEnterUI(go);
            SceneChangeBtnChildImage.sprite = SceneChangeBtnHoverSpr;
            SceneChangeBtn.GetComponentInChildren<Text>().color = HoverColor;
        }
        else
            OnPointerEnterUI(go);
    }

    void OnPointerExitSceneChangeBtn(GameObject go)
    {
        if (SceneChangeBtnIsInteractable)
        {
            OnPointerExitUI(go);
            SceneChangeBtnChildImage.sprite = SceneChangeBtnNormalSpr;
            SceneChangeBtn.GetComponentInChildren<Text>().color = NormalColor;
        }
        else
            OnPointerExitUI(go);
    }

    void ChangeDefinitionModel(DefinitionModel definitionModel, bool IsChangeSDK, bool IsNeedToast = true)
    {
        //switch (definitionModel)
        //{
        //    case DefinitionModel.DEFINITION_4K:
        //        DefinitionBtn.GetComponentInChildren<Text>().SetTextByKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.AutoBtn.Text", "4K");
        //        break;
        //    case DefinitionModel.DEFINITION_1080P:
        //        DefinitionBtn.GetComponentInChildren<Text>().SetTextByKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.AutoBtn.Text", "1080P");
        //        break;
        //    case DefinitionModel.DEFINITION_720P:
        //        DefinitionBtn.GetComponentInChildren<Text>().SetTextByKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.AutoBtn.Text", "720P");
        //        break;
        //    case DefinitionModel.UNKOWN:
        //        DefinitionBtn.GetComponentInChildren<Text>().SetTextByKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.Title.Text");
        //        break;
        //    default:
        //        DefinitionBtn.GetComponentInChildren<Text>().SetTextByKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.AutoBtn.Text", "720P");
        //        break;
        //}
    }

    public void ScreenSizeBtnStatusControl(bool isInteractable)
    {
        ScreenSizeBtnIsInteractable = isInteractable;

        if (isInteractable)
        {
            ScreenSizeBtn.interactable = true;
            ScreenSizeBtnChildImage.color = Color.white;
            ScreenSizeBtn.GetComponentInChildren<Text>().color = NormalColor;
        }
        else
        {
            ScreenSizeBtn.interactable = false;
            ScreenSizeBtnChildImage.color = new Color(1, 1, 1, 0.3f);
            ScreenSizeBtn.GetComponentInChildren<Text>().color = DisableColor;
        }
    }

    public void SceneChangeBtnStatusControl(bool isInteractable)
    {
        SceneChangeBtnIsInteractable = isInteractable;

        if (isInteractable)
        {
            SceneChangeBtn.interactable = true;
            SceneChangeBtnChildImage.color = Color.white;
            SceneChangeBtn.GetComponentInChildren<Text>().color = NormalColor;
        }
        else
        {
            SceneChangeBtn.interactable = false;
            SceneChangeBtnChildImage.color = new Color(1, 1, 1, 0.3f);
            SceneChangeBtn.GetComponentInChildren<Text>().color = DisableColor;
        }
    }

    void ClickScreenSizeBtn(GameObject go)
    {
        if (ScreenSizeBtnIsInteractable)
        {
            StereoTypePanel.Hide();
            SceneChangePanel.Hide();
            LoopModePanel.Hide();
            DefinitionPanel.Hide();
            ScreenSizePanel.SetScreenSizePanelStatus();
        }
    }

    void ClickStereoTypeBtn(GameObject go)
    {
        ScreenSizePanel.Hide();
        SceneChangePanel.Hide();
        LoopModePanel.Hide();
        DefinitionPanel.Hide();
        StereoTypePanel.SetStereoTypePanelStatus();
    }

    void ClickSceneChangeBtn(GameObject go)
    {
        if (SceneChangeBtnIsInteractable)
        {
            ScreenSizePanel.Hide();
            StereoTypePanel.Hide();
            LoopModePanel.Hide();
            DefinitionPanel.Hide();
            SceneChangePanel.SetSceneChangePanelStatus();
        }
    }

    void ClickLoopModeBtn(GameObject go)
    {
        ScreenSizePanel.Hide();
        StereoTypePanel.Hide();
        SceneChangePanel.Hide();
        DefinitionPanel.Hide();
        LoopModePanel.SetLoopModePanelStatus();
    }

    void ClickDefinitionBtn(GameObject go)
    {
        ScreenSizePanel.Hide();
        StereoTypePanel.Hide();
        SceneChangePanel.Hide();
        LoopModePanel.Hide();
        DefinitionPanel.SetDefinitionPanelStatus();
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
        ChangeDefinitionModel(DefinitionPanel.CurDefinitionModel, false);

        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (!IsShow)
            return;

        IsShow = false;

        ResetUI();
        this.gameObject.SetActive(false);
    }

    public void ResetUI()
    {
        ScreenSizePanel.Hide();
        StereoTypePanel.Hide();
        SceneChangePanel.Hide();
        LoopModePanel.Hide();
        DefinitionPanel.Hide();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using IVR.Language;

public class DefinitionPanel : MonoBehaviour
{
    public MixedButton FourKBtn; //4K
    public MixedButton FHDBtn; //蓝光 1080
    public MixedButton SHDBtn; //超清 720
    public MixedButton AutoBtn; //自动

    [HideInInspector]
    public DefinitionModel CurDefinitionModel; //用于UI显示当前选择的清晰度
    private DefinitionModel AutoDefinitionModel; //用于记录首次自动选择的清晰度
    private MixedButton OldDefnBtn;
    bool FourKBtnIsDisable = true;
    bool FHDBtnIsDisable = true;
    bool SHDBtnIsDisable = true;
    bool AutoBtnIsDisable = true;
    bool IsEnter;
    bool IsShow;

    private List<MifengPlayer.DefnInfo> CurrentDefnList;

    public Action<bool> PointerEnterUICallback;
    public Action<GameObject, bool> PointerEnterBtnCallback;
    /// <summary>
    /// 改变清晰度
    /// bool：是否改变底层清晰度？false：不改变底层，只改变UI；true：改变底层，同时改变UI
    /// </summary>
    public Action<DefinitionModel, bool, bool> ChangeDefinitionModelCallback;

    public void Init()
    {
        IsEnter = false;
        IsShow = false;
        CurDefinitionModel = DefinitionModel.UNKOWN;
        AutoDefinitionModel = DefinitionModel.UNKOWN;
        OldDefnBtn = AutoBtn;
        CurrentDefnList = new List<MifengPlayer.DefnInfo>();

        EventTriggerListener.Get(this.gameObject).OnPtEnter = OnPointerEnterPanel;
        EventTriggerListener.Get(this.gameObject).OnPtExit = OnPointerExitPanel;
        EventTriggerListener.Get(FourKBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(FourKBtn.gameObject).OnPtExit = OnPointerExitBtn;
        EventTriggerListener.Get(FHDBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(FHDBtn.gameObject).OnPtExit = OnPointerExitBtn;
        EventTriggerListener.Get(SHDBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(SHDBtn.gameObject).OnPtExit = OnPointerExitBtn;
        EventTriggerListener.Get(AutoBtn.gameObject).OnPtEnter = OnPointerEnterBtn;
        EventTriggerListener.Get(AutoBtn.gameObject).OnPtExit = OnPointerExitBtn;

        FourKBtn.SelectBtnCallback = SelectFourKBtn;
        FHDBtn.SelectBtnCallback = SelectFHDBtn;
        SHDBtn.SelectBtnCallback = SelectSHDBtn;
        AutoBtn.SelectBtnCallback = SelectAutoBtn;
    }

    public bool IsFirstSetDefn()
    {
        if (AutoDefinitionModel == DefinitionModel.UNKOWN)
            return true;
        else return false;
    }

    private void SetUIState()
    {
        if (FourKBtnIsDisable)
        {
            FourKBtn.enabled = false;
            FourKBtn.GetComponent<Image>().raycastTarget = false;
            Text FourKBtnText = FourKBtn.GetComponentInChildren<Text>();
            FourKBtnText.color = new Color(FourKBtnText.color.r, FourKBtnText.color.g, FourKBtnText.color.b, 0.3f);
        }
        else
        {
            FourKBtn.enabled = true;
            FourKBtn.GetComponent<Image>().raycastTarget = true;
            Text FourKBtnText = FourKBtn.GetComponentInChildren<Text>();
            FourKBtnText.color = new Color(FourKBtnText.color.r, FourKBtnText.color.g, FourKBtnText.color.b, 1);
        }
        if (FHDBtnIsDisable)
        {
            FHDBtn.enabled = false;
            FHDBtn.GetComponent<Image>().raycastTarget = false;
            Text FHDBtnText = FHDBtn.GetComponentInChildren<Text>();
            FHDBtnText.color = new Color(FHDBtnText.color.r, FHDBtnText.color.g, FHDBtnText.color.b, 0.3f);
        }
        else
        {
            FHDBtn.enabled = true;
            FHDBtn.GetComponent<Image>().raycastTarget = true;
            Text FHDBtnText = FHDBtn.GetComponentInChildren<Text>();
            FHDBtnText.color = new Color(FHDBtnText.color.r, FHDBtnText.color.g, FHDBtnText.color.b, 1);
        }
        if (SHDBtnIsDisable)
        {
            SHDBtn.enabled = false;
            SHDBtn.GetComponent<Image>().raycastTarget = false;
            Text SHDBtnText = SHDBtn.GetComponentInChildren<Text>();
            SHDBtnText.color = new Color(SHDBtnText.color.r, SHDBtnText.color.g, SHDBtnText.color.b, 0.3f);
        }
        else
        {
            SHDBtn.enabled = true;
            SHDBtn.GetComponent<Image>().raycastTarget = true;
            Text SHDBtnText = SHDBtn.GetComponentInChildren<Text>();
            SHDBtnText.color = new Color(SHDBtnText.color.r, SHDBtnText.color.g, SHDBtnText.color.b, 1);
        }
        if (AutoBtnIsDisable)
        {
            AutoBtn.enabled = false;
            AutoBtn.GetComponent<Image>().raycastTarget = false;
            Text AutoBtnText = AutoBtn.GetComponentInChildren<Text>();
            AutoBtnText.color = new Color(AutoBtnText.color.r, AutoBtnText.color.g, AutoBtnText.color.b, 0.3f);
        }
        else
        {
            AutoBtn.enabled = true;
            AutoBtn.GetComponent<Image>().raycastTarget = true;
            Text AutoBtnText = AutoBtn.GetComponentInChildren<Text>();
            AutoBtnText.color = new Color(AutoBtnText.color.r, AutoBtnText.color.g, AutoBtnText.color.b, 1);
        }

        OldDefnBtn.SetSelected(false);
        switch (CurDefinitionModel)
        {
            case DefinitionModel.UNKOWN:
                AutoBtn.SetSelected(true);
                OldDefnBtn = AutoBtn;
                break;
            case DefinitionModel.DEFINITION_1080P:
                FHDBtn.SetSelected(true);
                OldDefnBtn = FHDBtn;
                break;
            case DefinitionModel.DEFINITION_720P:
                SHDBtn.SetSelected(true);
                OldDefnBtn = SHDBtn;
                break;
            case DefinitionModel.DEFINITION_4K:
                FourKBtn.SetSelected(true);
                OldDefnBtn = FourKBtn;
                break;
            default:
                break;
        }

        //switch (AutoDefinitionModel)
        //{
            //case DefinitionModel.DEFINITION_4K:
            //    AutoBtn.GetComponentInChildren<Text>().SetTextByKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.AutoBtn.Text", "4K");
            //    break;
            //case DefinitionModel.DEFINITION_1080P:
            //    AutoBtn.GetComponentInChildren<Text>().SetTextByKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.AutoBtn.Text", "1080P");
            //    break;
            //case DefinitionModel.DEFINITION_720P:
            //    AutoBtn.GetComponentInChildren<Text>().SetTextByKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.AutoBtn.Text", "720P");
            //    break;
            //default:
            //    AutoBtn.GetComponentInChildren<Text>().SetTextByKey("Cinema.VideoPlayerPanel.VariablePanel.DefinitionPanel.AutoBtn.Text", "720P");
            //    break;
        //}
    }

    public void SetDefnList(List<MifengPlayer.DefnInfo> list, MifengPlayer.DefnInfo current)
    {
        SHDBtnIsDisable = true;
        FHDBtnIsDisable = true;
        FourKBtnIsDisable = true;
        CurrentDefnList.Clear();
        CurrentDefnList.AddRange(list);

        for (int i = 0; i < list.Count; i++)
        {
            DefinitionModel defn = list[i].defn;
            if (defn.Equals(DefinitionModel.DEFINITION_720P) && list[i].benefitType == 0)
                SHDBtnIsDisable = false;
            if (defn.Equals(DefinitionModel.DEFINITION_1080P) && list[i].benefitType == 0)
                FHDBtnIsDisable = false;
            if (defn.Equals(DefinitionModel.DEFINITION_4K) && list[i].benefitType == 0)
                FourKBtnIsDisable = false;
        }


        //if (current.defn != null || current.name != null)
        {
            AutoBtnIsDisable = false;
            if (AutoDefinitionModel != DefinitionModel.UNKOWN) return; //说明选择过自动
            switch (current.defn)
            {
                case DefinitionModel.DEFINITION_720P:
                    AutoDefinitionModel = DefinitionModel.DEFINITION_720P;
                    break;
                case DefinitionModel.DEFINITION_1080P:
                    AutoDefinitionModel = DefinitionModel.DEFINITION_1080P;
                    break;
                case DefinitionModel.DEFINITION_4K:
                    AutoDefinitionModel = DefinitionModel.DEFINITION_4K;
                    break;
                default:
                    AutoDefinitionModel = DefinitionModel.DEFINITION_720P;
                    break;
            }
        }
        SetUIState();

        if (ChangeDefinitionModelCallback != null)
            ChangeDefinitionModelCallback(AutoDefinitionModel, false, true);
    }

    public MifengPlayer.DefnInfo GetDefnInfoWithModel(DefinitionModel model)
    {
        if (model == DefinitionModel.UNKOWN) return null;

        foreach(MifengPlayer.DefnInfo info in CurrentDefnList)
        {
            if (info.defn == model) return info;
        }

        return null;
    }

    void SelectFourKBtn(MixedButton stb, bool isSelect)
    {
        if (FourKBtnIsDisable || OldDefnBtn == stb) return;
        OldDefnBtn.SetSelected(false);
        OldDefnBtn = stb;
        CurDefinitionModel = DefinitionModel.DEFINITION_4K;
        stb.SetSelected(true);
        if (ChangeDefinitionModelCallback != null)
            ChangeDefinitionModelCallback(DefinitionModel.DEFINITION_4K, true, true);
    }

    void SelectFHDBtn(MixedButton stb, bool isSelect)
    {
        if (FHDBtnIsDisable || OldDefnBtn == stb) return;
        OldDefnBtn.SetSelected(false);
        OldDefnBtn = stb;
        CurDefinitionModel = DefinitionModel.DEFINITION_1080P;
        stb.SetSelected(true);
        if (ChangeDefinitionModelCallback != null)
            ChangeDefinitionModelCallback(DefinitionModel.DEFINITION_1080P, true, true);
    }

    void SelectSHDBtn(MixedButton stb, bool isSelect)
    {
        if (SHDBtnIsDisable || OldDefnBtn == stb) return;
        OldDefnBtn.SetSelected(false);
        OldDefnBtn = stb;
        stb.SetSelected(true);
        CurDefinitionModel = DefinitionModel.DEFINITION_720P;
        if (ChangeDefinitionModelCallback != null)
            ChangeDefinitionModelCallback(DefinitionModel.DEFINITION_720P, true, true);
    }

    void SelectAutoBtn(MixedButton stb, bool isSelect)
    {
        if (AutoBtnIsDisable || OldDefnBtn == stb) return;
        OldDefnBtn.SetSelected(false);
        OldDefnBtn = stb;
        stb.SetSelected(true);
        CurDefinitionModel = DefinitionModel.UNKOWN;
        if (ChangeDefinitionModelCallback != null)
            ChangeDefinitionModelCallback(AutoDefinitionModel, true, true);
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

    public void SetDefinitionPanelStatus()
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
        SetUIState();

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
        FourKBtn = null;
        FHDBtn = null;
        SHDBtn = null;
        AutoBtn = null;
        OldDefnBtn = null;
        CurrentDefnList = null;
        PointerEnterUICallback = null;
        PointerEnterBtnCallback = null;
    }
}

/*
 * Author:李传礼
 * DateTime:2017.12.21
 * Description:复合按钮（具备Button和Toggle两种操作属性，同时兼备了无背景处理）
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public enum TransitionType { Color, Sprite, None }

[RequireComponent(typeof(Image))]
public class MixedButton : MonoBehaviour
{
    public bool IsToggle;//是否开启Toggle属性
    public bool AutoHideText;//是否自动隐藏文本
    public bool DisableHoverAfterSelected;//选择后是否禁用Hover
    public TransitionType Transition = TransitionType.Sprite;

    public Sprite TargetSprite;
    public Sprite HighlightedSprite;
    public Sprite PressedSprite;
    public Sprite DisabledSprite;
    public Sprite SelectedSprite;

    public Color NormalColor = new Color(1, 1, 1, 1);
    public Color HighlightedColor = new Color(0.96f, 0.96f, 0.96f, 1);
    public Color PressedColor = new Color(0.78f, 0.78f, 0.78f, 1);
    public Color DisabledColor = new Color(0.78f, 0.78f, 0.78f, 0.5f);
    public Color SelectedColor = new Color(0.35f, 0.35f, 0.35f, 0.5f);

    bool IsInit = false;
    Image GraphicTarget;
    Text NameText;
    Color OriginalTextAlphaZero;
    Color OriginalTextAlphaOne;
    Color OriginalColor;
    Color TransparentColor;

    float SpendTime;
    Sprite OldSprite;
    protected bool IsSelected;

    public Action<MixedButton> ClickBtnCallback;
    public Action<MixedButton, bool> SelectBtnCallback;
    public Action<MixedButton> PointerEnterBtnCallback;
    public Action<MixedButton> PointerExitBtnCallback;

    void Start()
    {
        if (!IsInit)
            Init();
    }

    void Init()
    {
        IsInit = true;

        GraphicTarget = this.GetComponent<Image>();
        Transform trans = this.transform.Find("Text");
        if (trans != null)
        {
            NameText = trans.GetComponent<Text>();
            OriginalTextAlphaOne = NameText.color;
            OriginalTextAlphaZero = new Color(OriginalTextAlphaOne.r, OriginalTextAlphaOne.g, OriginalTextAlphaOne.b, 0);
        }
        OriginalColor = GraphicTarget.color;
        TransparentColor = GraphicTarget.color - new Color(0, 0, 0, OriginalColor.a);
        OriginalColor = TransparentColor + new Color(0, 0, 0, 1);
        SpendTime = 0.3f;
        IsSelected = false;

        if (Transition == TransitionType.Sprite)
        {
            if (TargetSprite != null)
            {
                GraphicTarget.sprite = TargetSprite;
                GraphicTarget.color = OriginalColor;
            }
            else
                GraphicTarget.color = TransparentColor;
        }
        else if (Transition == TransitionType.Color)
        {
            if (TargetSprite != null)
                GraphicTarget.sprite = TargetSprite;
            GraphicTarget.color = NormalColor;
        }

        if (AutoHideText && NameText != null)
            NameText.color = OriginalTextAlphaZero;

        OldSprite = GraphicTarget.sprite;

        EventTriggerListener.Get(this.gameObject).OnPtEnter += Hover;
        EventTriggerListener.Get(this.gameObject).OnPtExit += Exit;
        EventTriggerListener.Get(this.gameObject).OnPtDown += Press;
        EventTriggerListener.Get(this.gameObject).OnPtUp += Release;
        EventTriggerListener.Get(this.gameObject).OnPtClick += Click;
    }

    public void ShowText(bool isShow)
    {
        if (!IsInit)
            Init();

        if (AutoHideText && NameText != null)
        {
            if (isShow)
                NameText.DOColor(OriginalTextAlphaOne, SpendTime);
            else
                NameText.DOColor(OriginalTextAlphaZero, SpendTime);
        }
    }

    protected void Hover(GameObject go)
    {
        if (PointerEnterBtnCallback != null)
            PointerEnterBtnCallback(this);

        if (AutoHideText && NameText != null)
            NameText.DOColor(OriginalTextAlphaOne, SpendTime);

        if (IsSelected && DisableHoverAfterSelected)
            return;

        if (Transition == TransitionType.Sprite)
            ChangeSprite(HighlightedSprite);
        else if (Transition == TransitionType.Color)
            ChangeColor(HighlightedColor);
    }

    protected void Exit(GameObject go)
    {
        if (!IsSelected)
        {
            if (Transition == TransitionType.Sprite)
                ChangeSprite(TargetSprite);
            else if (Transition == TransitionType.Color)
                ChangeColor(NormalColor);
        }
        else
        {
            if (Transition == TransitionType.Sprite)
                ChangeSprite(SelectedSprite);
            else if (Transition == TransitionType.Color)
                ChangeColor(SelectedColor);
        }

        if (AutoHideText && NameText != null)
            NameText.DOColor(OriginalTextAlphaZero, SpendTime);

        if (PointerExitBtnCallback != null)
            PointerExitBtnCallback(this);
    }

    void Press(GameObject go)
    {
        if (Transition == TransitionType.Sprite)
            ChangeSprite(PressedSprite);
        else if (Transition == TransitionType.Color)
            ChangeColor(PressedColor);
    }

    void Release(GameObject go)
    {
        if (!IsToggle)
        {
            if (Transition == TransitionType.Sprite)
                ChangeSprite(HighlightedSprite);
            else if (Transition == TransitionType.Color)
                ChangeColor(HighlightedColor);
        }
        else
        {
            if (!IsSelected)
            {
                if (Transition == TransitionType.Sprite)
                    ChangeSprite(SelectedSprite);
                else if (Transition == TransitionType.Color)
                    ChangeColor(SelectedColor);
            }
            else
            {
                if (Transition == TransitionType.Sprite)
                    ChangeSprite(TargetSprite);
                else if (Transition == TransitionType.Color)
                    ChangeColor(NormalColor);
            }

            IsSelected = !IsSelected;
        }

        if (SelectBtnCallback != null)
            SelectBtnCallback(this, IsSelected);
    }

    void Click(GameObject go)
    {
        if (ClickBtnCallback != null)
            ClickBtnCallback(this);
    }

    void ChangeSprite(Sprite sp)
    {
        if (OldSprite == sp)
            return;
        OldSprite = sp;
        
        if (GraphicTarget != null)
        {
            GraphicTarget.sprite = sp;
            if (sp != null)
                GraphicTarget.DOColor(OriginalColor, 0);
            else
                GraphicTarget.color = TransparentColor;
        }
    }

    void ChangeColor(Color c)
    {
        if (GraphicTarget != null)
        {
            GraphicTarget.DOColor(c, SpendTime);
        }
    }

    void OnEnable()
    {
        if (!IsToggle)
        {
            if (Transition == TransitionType.Sprite)
                ChangeSprite(TargetSprite);
            else if (Transition == TransitionType.Color)
                ChangeColor(NormalColor);
        }
        else
        {
            if (!IsSelected)
            {
                if (Transition == TransitionType.Sprite)
                    ChangeSprite(TargetSprite);
                else if (Transition == TransitionType.Color)
                    ChangeColor(NormalColor);
            }
            else
            {
                if (Transition == TransitionType.Sprite)
                    ChangeSprite(SelectedSprite);
                else if (Transition == TransitionType.Color)
                    ChangeColor(SelectedColor);
            }
        }
    }

    void OnDisable()
    {
        if (Transition == TransitionType.Sprite)
            ChangeSprite(DisabledSprite);
        else if (Transition == TransitionType.Color)
            ChangeColor(DisabledColor);
    }

    public void SetSelected(bool isSelected)
    {
        if (!IsInit)
            Init();

        if (!IsToggle)
            return;

        if (isSelected == IsSelected)
            return;

        if (!IsSelected)
        {
            if (Transition == TransitionType.Sprite)
                ChangeSprite(SelectedSprite);
            else if (Transition == TransitionType.Color)
                ChangeColor(SelectedColor);
        }
        else
        {
            if (Transition == TransitionType.Sprite)
                ChangeSprite(TargetSprite);
            else if (Transition == TransitionType.Color)
                ChangeColor(NormalColor);
        }

        IsSelected = isSelected;
    }

    public bool SelectedStatus()
    {
        if (!IsInit)
            Init();

        return IsSelected;
    }

    public void SetText(string str)
    {
        if (!IsInit)
            Init();

        if (NameText != null)
            NameText.text = str;
    }
}

/*
 * Author:李传礼
 * DateTime:2018.04.20
 * Description:滑动按钮
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public enum OffStatusPos { Up, Bottom, Left, Right }

public class SlipButton : MonoBehaviour
{
    public Image BtnImage;
    //public Color BtnOffColor;
    //public Color BtnOnColor;
    //public Color BgOffColor;
    //public Color BgOnColor;
    public bool SwitchStatus;
    public OffStatusPos OffStatusPos;
    //public AnimationCurve AnimCurve;

    bool IsInit = false;
    bool IsVoluntary;//是否为主动改变值
    Image BgImage;
    float SpendTime;
    Vector3 MoveFrom;//off状态
    Vector3 MoveTo;

    public Action<bool> SlipStatusCallback;

    void Start ()
    {
        if (!IsInit)
            Init();
    }

    void Init()
    {
        IsInit = true;

        IsVoluntary = false;
        BgImage = this.GetComponent<Image>();
        SpendTime = 0.3f;

        float hLength = (BgImage.rectTransform.rect.width - BtnImage.rectTransform.rect.width) / 2;
        float vLength = (BgImage.rectTransform.rect.height - BtnImage.rectTransform.rect.height) / 2;
        if (OffStatusPos == OffStatusPos.Up)
        {
            MoveFrom = Vector3.up * vLength;
            MoveTo = Vector3.down * vLength;
        }
        else if (OffStatusPos == OffStatusPos.Bottom)
        {
            MoveFrom = Vector3.down * vLength;
            MoveTo = Vector3.up * vLength;
        }
        else if (OffStatusPos == OffStatusPos.Left)
        {
            MoveFrom = Vector3.left * hLength;
            MoveTo = Vector3.right * hLength;
        }
        else
        {
            MoveFrom = Vector3.right * hLength;
            MoveTo = Vector3.left * hLength;
        }

        EventTriggerListener.Get(BtnImage.gameObject).OnPtEnter += OnPointerEnter;
        EventTriggerListener.Get(BtnImage.gameObject).OnPtExit += OnPointerExit;
        EventTriggerListener.Get(BtnImage.gameObject).OnPtClick += OnPointerClick;
    }

    public void SwitchSlipBtnStatus(bool isOpen)
    {
        SwitchStatus = isOpen;

        if (!SwitchStatus)
        {
            BtnImage.rectTransform.localPosition = MoveFrom;
            BgImage.GetComponent<ImageColoringManager>().OnNormalStatus();
        }
        else
        {
            BtnImage.rectTransform.localPosition = MoveTo;
            //BtnImage.color = BtnOnColor;
            //BgImage.color = BgOnColor;
            BgImage.GetComponent<ImageColoringManager>().OnSelectedStatus();
        }
    }

    void OnPointerEnter(GameObject go)
    {
        //MotionAestheticsScrp.AccLocalScaling(BtnImage.gameObject, Vector3.one, Vector3.one * 1.08f, SpendTime, AnimCurve);
        BtnImage.rectTransform.DOScale(Vector3.one * 1.08f, SpendTime);
    }

    void OnPointerExit(GameObject go)
    {
        //MotionAestheticsScrp.AccLocalScaling(BtnImage.gameObject, Vector3.one * 1.08f, Vector3.one, SpendTime, AnimCurve);
        BtnImage.rectTransform.DOScale(Vector3.one, SpendTime);
    }

    void OnPointerClick(GameObject go)
    {
        if (!SwitchStatus)
        {
            //MotionAestheticsScrp.AccLocalMoving(BtnImage.gameObject, MoveFrom, MoveTo, SpendTime, AnimCurve);
            BtnImage.rectTransform.DOLocalMove(MoveTo, SpendTime);
            //MotionAestheticsScrp.AccColorChanging(BtnImage.gameObject, BtnOffColor, BtnOnColor, SpendTime, AnimCurve);
            //MotionAestheticsScrp.AccColorChanging(BgImage.gameObject, BgOffColor, BgOnColor, SpendTime, AnimCurve);
            BgImage.GetComponent<ImageColoringManager>().OnSelectedStatus();
        }
        else
        {
            //MotionAestheticsScrp.AccLocalMoving(BtnImage.gameObject, MoveTo, MoveFrom, SpendTime, AnimCurve);
            BtnImage.rectTransform.DOLocalMove(MoveFrom, SpendTime);
            //MotionAestheticsScrp.AccColorChanging(BtnImage.gameObject, BtnImage.color, BtnOffColor, SpendTime, AnimCurve);
            //MotionAestheticsScrp.AccColorChanging(BgImage.gameObject, BgImage.color, BgOffColor, SpendTime, AnimCurve);
            BgImage.GetComponent<ImageColoringManager>().OnNormalStatus();
        }

        SwitchStatus = !SwitchStatus;

        if (!IsVoluntary)
        {
            if (SlipStatusCallback != null)
                SlipStatusCallback(SwitchStatus);
        }
        else
            IsVoluntary = false;
    }

    public void SetSwitchStatus(bool isOn)
    {
        if (!IsInit)
            Init();
        if (SwitchStatus == isOn)
            return;
        //if (this.gameObject.activeInHierarchy)
        if(true)
        {
            IsVoluntary = true;
            OnPointerClick(this.gameObject);
        }
        else
        {
            if (!isOn)
            {
                BtnImage.rectTransform.localPosition = MoveFrom;
                //BtnImage.GetComponent<ImageColoringManager>().OnNormalStatus();
                BgImage.GetComponent<ImageColoringManager>().OnNormalStatus();
            }
            else
            {
                BtnImage.rectTransform.localPosition = MoveTo;
                //BtnImage.color = BtnOnColor;
                //BgImage.color = BgOnColor;
                //BtnImage.GetComponent<ImageColoringManager>().OnSelectedStatus();
                BgImage.GetComponent<ImageColoringManager>().OnSelectedStatus();
            }
            SwitchStatus = isOn;
        }
    }
}

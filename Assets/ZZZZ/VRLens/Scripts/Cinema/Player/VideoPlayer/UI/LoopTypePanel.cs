/*
 * Author:李传礼
 * DateTime:2017.12.14
 * Description:循环类型面板
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class LoopTypePanel : MonoBehaviour
{
    public Button SinglePlayBtn;
    public Button AutoReplayBtn;
    public Button ListLoopBtn;
    RectTransform RectTrans;
    bool IsShow;

    Vector3 MoveStartPos;
    Vector3 MoveEndPos;

    public Action<LoopType> SelectLoopTypeCallback;

	void Start ()
    {
        Init();
    }
    
    void Init()
    {
        RectTrans = this.GetComponent<RectTransform>();
        MoveEndPos = this.transform.localPosition;
        MoveStartPos = MoveEndPos - new Vector3(0, RectTrans.sizeDelta.y / 2, 0);
        IsShow = false;

        EventTriggerListener.Get(SinglePlayBtn.gameObject).OnPtClick = SelectLoopType;
        EventTriggerListener.Get(AutoReplayBtn.gameObject).OnPtClick = SelectLoopType;
        EventTriggerListener.Get(ListLoopBtn.gameObject).OnPtClick = SelectLoopType;
        EventTriggerListener.Get(this.gameObject).OnPtExit = OnPointerExit;
    }

    void SelectLoopType(GameObject go)
    {
        LoopType loopType;
        if (go == SinglePlayBtn.gameObject)
            loopType = LoopType.SinglePlay;
        else if (go == AutoReplayBtn.gameObject)
            loopType = LoopType.AutoReplay;
        else
            loopType = LoopType.ListLoop;

        if (SelectLoopTypeCallback != null)
            SelectLoopTypeCallback(loopType);

        Hide();
    }

    void OnPointerExit(GameObject go)
    {
        Hide();
    }

    public void Show()
    {
        if (IsShow)
            return;

        IsShow = true;
        this.gameObject.transform.DOScale(Vector3.one, GlobalVariable.AnimationSpendTime);
        this.gameObject.transform.DOLocalMove(MoveEndPos, GlobalVariable.AnimationSpendTime);
    }

    public void Hide()
    {
        if (!IsShow)
            return;

        IsShow = false;
        this.gameObject.transform.DOScale(Vector3.zero, GlobalVariable.AnimationSpendTime);
        this.gameObject.transform.DOLocalMove(MoveStartPos, GlobalVariable.AnimationSpendTime);
    }
}

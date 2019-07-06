/*
 * Author:李传礼
 * DateTime:2018.01.26
 * Description:UI层次外壳
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDepthShell : MonoBehaviour
{
    public bool IsAutoCreateShell = false;
    bool IsApply = false;
    RectTransform ShellRectTrans;
    RectTransform ThisRectTrans;
    Transform FirstParent;
    Transform SecondParent;
    int OriginalOrderIndex;
    bool IsHideShell;

    Vector3 LocalPos;
    Vector3 LocalEuler;
    Vector3 LocalScale;

    void Start ()
    {
        if (IsAutoCreateShell)
        {
            if(!IsApply)
                Apply();
        }
    }
    
    void Init()
    {
        if (ShellRectTrans == null)
        {
            GameObject go = new GameObject("UIDepthShell");
            ShellRectTrans = go.AddComponent<RectTransform>();
        }
        if(ThisRectTrans == null)
            ThisRectTrans = this.GetComponent<RectTransform>();

        FirstParent = ThisRectTrans.parent;
        if (FirstParent != null)
            SecondParent = FirstParent.parent;
        else
            SecondParent = null;

        OriginalOrderIndex = ThisRectTrans.GetSiblingIndex();
        IsHideShell = true;
        LocalPos = Vector3.zero;
        LocalEuler = Vector3.zero;
        LocalScale = Vector3.one;
    }

    public void Apply()
    {
        IsApply = true;

        Init();
        PutInShell();
    }

    //放入壳
    void PutInShell()
    {
        IsHideShell = false;

        ShellRectTrans.SetParent(FirstParent);
        ShellRectTrans.SetSiblingIndex(OriginalOrderIndex);
        PreDefScrp.RectTransformDeepCopy(ThisRectTrans, ShellRectTrans);
        ShellRectTrans.localEulerAngles = Vector3.zero;
        ShellRectTrans.localScale = Vector3.one;

        ThisRectTrans.SetParent(ShellRectTrans);
        ThisRectTrans.localScale = Vector3.one;//如果ShellRectTrans的父类全局坐标为0，那this的缩放信息就丢失了
    }

    public void TopShow()
    {
        if (!IsApply)
            Apply();

        if (IsHideShell)
            PutInShell();

        SaveLocalTrans();
        ThisRectTrans.SetParent(SecondParent);
        ThisRectTrans.SetAsLastSibling();
    }

    public void SortBottom()
    {
        if (!IsApply)
            Apply();

        if (IsHideShell)
            PutInShell();

        SaveLocalTrans();
        ThisRectTrans.SetParent(SecondParent);
        ThisRectTrans.SetAsFirstSibling();
    }

    void SaveLocalTrans()
    {
        LocalPos = ThisRectTrans.localPosition;
        LocalEuler = ThisRectTrans.localEulerAngles;
        LocalScale = ThisRectTrans.localScale;
    }

    public void Recover()
    {
        if (!IsApply)
            Apply();

        if (IsHideShell)
            PutInShell();

        ThisRectTrans.SetParent(ShellRectTrans);
        ThisRectTrans.localPosition = LocalPos;
        ThisRectTrans.localEulerAngles = LocalEuler;
        ThisRectTrans.localScale = LocalScale;
    }

    public void HideShell()
    {
        if (!IsApply)
            Apply();

        if (ShellRectTrans == null)
            return;
        IsHideShell = true;

        ShellRectTrans.SetParent(SecondParent);
        ThisRectTrans.SetParent(FirstParent);
        ThisRectTrans.SetSiblingIndex(OriginalOrderIndex);

        ShellRectTrans.SetParent(ThisRectTrans);
        ShellRectTrans.localPosition = Vector3.zero;
        ShellRectTrans.localEulerAngles = Vector3.zero;
        ShellRectTrans.localScale = Vector3.one;
    }
}

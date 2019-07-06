/*
 * Author:李传礼
 * DateTime:2017.12.13
 * Description:UI层次控制
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDepth : MonoBehaviour
{
    public int Order;
    bool IsInit = false;
    Canvas Canvas;

    void Start ()
    {
        if (!IsInit)
            Init();
    }
    
    void Init()
    {
        Canvas = this.GetComponent<Canvas>();
        if (Canvas == null)
            Canvas = this.gameObject.AddComponent<Canvas>();

        GraphicRaycaster gphRaycaster = this.gameObject.GetComponent<GraphicRaycaster>();
        if (gphRaycaster == null)
            gphRaycaster = this.gameObject.AddComponent<GraphicRaycaster>();//这个一直都要存在(鼠标模式和Ximmerse)
#if UD_XIMMERSE
        gphRaycaster.enabled = true;
#elif UD_GVR
#if UNITY_ANDROID && !UNITY_EDITOR
        gphRaycaster.enabled = false;//只在GVR环境中，要把它false掉，不然影响手柄控制
#else
        gphRaycaster.enabled = true;
#endif
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        if (this.gameObject.GetComponent<GvrPointerGraphicRaycaster>() == null)
            this.gameObject.AddComponent<GvrPointerGraphicRaycaster>();
#endif

        Canvas.overrideSorting = true;
        Canvas.sortingOrder = Order;

        IsInit = true;
    }

    public void SetSorting(bool isSorting)
    {
        if (!IsInit)
            Init();
        Canvas.overrideSorting = isSorting;
    }

    public void SetOrder(int order)
    {
        if (!IsInit)
            Init();
        Order = order;

        bool curValue = Canvas.overrideSorting;
        Canvas.overrideSorting = true;
        Canvas.sortingOrder = Order;
        Canvas.overrideSorting = curValue;
    }
}

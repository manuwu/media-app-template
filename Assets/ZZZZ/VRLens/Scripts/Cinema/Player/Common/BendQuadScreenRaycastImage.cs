/*
 * 2018-5-18
 * 黄秋燕 Shemi
 * 平面视频Raycast图片，使平面播放器可以响应hover事件
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BendQuadScreenRaycastImage : MonoBehaviour {
    public Action<bool> PointerEnterUICallback;

    void Start()
    {
        EventTriggerListener.Get(this.gameObject).OnPtEnter = OnPointerEnter;
        EventTriggerListener.Get(this.gameObject).OnPtExit = OnPointerExit;
    }

    void OnPointerEnter(GameObject go)
    {
        if (PointerEnterUICallback != null)
            PointerEnterUICallback(true);
    }

    void OnPointerExit(GameObject go)
    {
        if (PointerEnterUICallback != null)
            PointerEnterUICallback(false);
    }
}

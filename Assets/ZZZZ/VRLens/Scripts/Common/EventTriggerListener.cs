/*
 * Author:wbq
 * Modify:李传礼
 * ModifyDateTime:2017.12.13
 * ModifyDescription:事件封装
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class EventTriggerListener : UIBehaviour, IMoveHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IDragHandler, IScrollHandler, IEventSystemHandler
{
    public Action<GameObject, Vector2> OnPtMove;
    public Action<GameObject> OnPtDown;
    public Action<GameObject> OnPtUp;
    public Action<GameObject> OnPtClick;
    public Action<GameObject> OnPtEnter;
    public Action<GameObject> OnPtExit;
    public Action<GameObject> OnPtSelect;
    public Action<GameObject> OnPtDeselect;
    public Action<GameObject, Vector2> OnPtDrag;
    public Action<GameObject, Vector2> OnPtScroll;

    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    public void OnMove(AxisEventData eventData)
    {
        if (OnPtMove != null)
            OnPtMove(gameObject, eventData.moveVector);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnPtDown != null)
            OnPtDown(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (OnPtUp != null)
            OnPtUp(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnPtClick != null)
            OnPtClick(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cinema.IsPointerEnterVideoPlayerUI = true;
        if (OnPtEnter != null)
            OnPtEnter(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cinema.IsPointerEnterVideoPlayerUI = false;
        if (OnPtExit != null)
            OnPtExit(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (OnPtSelect != null)
            OnPtSelect(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (OnPtDeselect != null)
            OnPtDeselect(gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OnPtDrag != null)
            OnPtDrag(gameObject, eventData.delta);
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (OnPtScroll != null)
            OnPtScroll(gameObject, eventData.scrollDelta);
    }
}

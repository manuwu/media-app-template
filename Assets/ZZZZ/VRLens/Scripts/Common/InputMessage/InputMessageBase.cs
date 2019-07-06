/*
 * Author:李传礼
 * DateTime:2017.7.19
 * Description:消息基类
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMessageBase : MonoBehaviour
{
	void Awake ()
    {
        InputMessageEngine.GetInstance().LMouseClickCallback += LeftMouseClick;
        InputMessageEngine.GetInstance().LMouseDbClickCallback += LeftMouseDbClick;
        InputMessageEngine.GetInstance().LMouseDragDeltaCallback += LeftMouseDragDelta;
        InputMessageEngine.GetInstance().LMouseDragScreenPosCallback += LeftMouseDragScreenPos;
        InputMessageEngine.GetInstance().LMouseUpCallback += LeftMouseUp;

        InputMessageEngine.GetInstance().RMouseClickCallback += RightMouseClick;
        InputMessageEngine.GetInstance().RMouseDbClickCallback += RightMouseDbClick;
        InputMessageEngine.GetInstance().RMouseDragDeltaCallback += RightMouseDragDelta;
        InputMessageEngine.GetInstance().RMouseDragScreenPosCallback += RightMouseDragScreenPos;
        InputMessageEngine.GetInstance().RMouseUpCallback += RightMouseUp;

        InputMessageEngine.GetInstance().MidMouseScrollCallback += MiddleMouseScroll;
        InputMessageEngine.GetInstance().MidMouseScrollEndCallback += MidMouseScrollEnd;
        InputMessageEngine.GetInstance().MouseEnterCallback += MouseHover;
        InputMessageEngine.GetInstance().MouseExitCallback += MouseExit;

        InputMessageEngine.GetInstance().SingleFingerTouchCallback += SingleFingerTouch;
        InputMessageEngine.GetInstance().SingleFingerDragDeltaCallback += SingleFingerDragDelta;
        InputMessageEngine.GetInstance().SingleFingerDragScreenPosCallback += SingleFingerDragScreenPos;
        InputMessageEngine.GetInstance().SingleFingerLiftCallback += SingleFingerLift;

        InputMessageEngine.GetInstance().DoubleFingerTouchCallback += DoubleFingerTouch;
        InputMessageEngine.GetInstance().DoubleFingerDragDeltaCallback += DoubleFingerDragDelta;
        InputMessageEngine.GetInstance().DoubleFingerDragScreenPosCallback += DoubleFingerDragScreenPos;
        InputMessageEngine.GetInstance().DoubleFingerZoomDeltaCallback += DoubleFingerZoomDelta;
        InputMessageEngine.GetInstance().DoubleFingerLiftCallback += DoubleFingerLift;
    }

    public virtual void LeftMouseClick(GameObject go, Vector3 point)
    {
    }

    public virtual void LeftMouseDbClick(GameObject go, Vector3 point)
    {
    }

    public virtual void LeftMouseDragDelta(Vector2 delta)
    {
    }

    public virtual void LeftMouseDragScreenPos(Vector3 screenPos)
    {
    }

    public virtual void LeftMouseUp()
    {
    }

    public virtual void RightMouseClick(GameObject go, Vector3 point)
    {
    }

    public virtual void RightMouseDbClick(GameObject go, Vector3 point)
    {
    }

    public virtual void RightMouseDragDelta(Vector2 delta)
    {
    }

    public virtual void RightMouseDragScreenPos(Vector3 screenPos)
    {
    }

    public virtual void RightMouseUp()
    {
    }

    public virtual void MiddleMouseScroll(float f)
    {
    }

    public virtual void MidMouseScrollEnd()
    {
    }

    public virtual void MouseHover(GameObject go)
    {
    }

    public virtual void MouseExit(GameObject go)
    {
    }

    public virtual void SingleFingerTouch(GameObject go, Vector3 point)
    {
    }

    public virtual void SingleFingerDragDelta(Vector2 delta)
    {
    }

    public virtual void SingleFingerDragScreenPos(Vector3 screenPos)
    {
    }

    public virtual void SingleFingerLift()
    {
    }

    public virtual void DoubleFingerTouch(GameObject go, Vector3 point)
    {
    }

    public virtual void DoubleFingerDragDelta(Vector2 delta)
    {
    }

    public virtual void DoubleFingerDragScreenPos(Vector3 screenPos)
    {
    }

    public virtual void DoubleFingerZoomDelta(float ratio)
    {
    }

    public virtual void DoubleFingerLift(InputMode inputMode)
    {
    }
}

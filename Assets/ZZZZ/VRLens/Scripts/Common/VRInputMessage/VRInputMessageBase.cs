/*
 * Author:李传礼
 * DateTime:2017.11.30
 * Description:VR输入消息基类
 */
using UnityEngine;

public class VRInputMessageBase : MonoBehaviour
{
    void Awake()
    {
        VRInputMessageEngine.GetInstance().AppButtonPressCallback += AppButtonPress;
        VRInputMessageEngine.GetInstance().HomeButtonUpCallback += HomeButtonUp;
        VRInputMessageEngine.GetInstance().HoumeButtonLongPressCallback += HoumeButtonLongPress;
        VRInputMessageEngine.GetInstance().RecenteredCallback += Recentered;
        VRInputMessageEngine.GetInstance().TouchPadPosCenteredCallback += TouchPadPosCentered;
        VRInputMessageEngine.GetInstance().TouchPadVector2Callback += TouchPadVector2;
        VRInputMessageEngine.GetInstance().TouchPadStretchVector2Callback += TouchPadStretchVector2;
        VRInputMessageEngine.GetInstance().TouchPadTouchCallback += TouchPadTouch;
        VRInputMessageEngine.GetInstance().TouchPadPressCallback += TouchPadPress;
        VRInputMessageEngine.GetInstance().TouchPadPressDirCallback += TouchPadPressDir;
        VRInputMessageEngine.GetInstance().PointerEnterCallback += PointerEnter;
        VRInputMessageEngine.GetInstance().PointerExitCallback += PointerExit;

        VRInputMessageEngine.GetInstance().TouchPadSlipUnifiedDirCallback += TouchPadSlipUnifiedDirSlip;
        VRInputMessageEngine.GetInstance().TouchPadEveryoneSlipUnifiedDirCallback += TouchPadEveryoneSlipUnifiedDirSlip;
        VRInputMessageEngine.GetInstance().TouchPadSlipUnifiedDirEndCallback += TouchPadSlipUnifiedDirEnd;

#if UD_XIMMERSE
        VRInputMessageEngine.GetInstance().TriggerPressCallback += TriggerPress;
        VRInputMessageEngine.GetInstance().TriggerPressDirCallback += TriggerPressDir;
        VRInputMessageEngine.GetInstance().TouchPadAnyClickCallback += TouchPadAnyClick;
#endif

#if UD_I3VR
        VRInputMessageEngine.GetInstance().TouchPadDirCallback += TouchPadDir;
#endif

    }

    public virtual void AppButtonPress(bool isDown)
    {
        //Debug.Log("AppButton是否被按下：" + isDown);
    }

    public virtual void HomeButtonUp()
    {
        //Debug.Log("HomeButtonUp");
    }

    public virtual void HoumeButtonLongPress(Vector3 dir)
    {
        //Debug.Log("长按HomeButton");
    }

    public virtual void Recentered(Vector3 dir)
    {
        //LogTool.Log("Recentered dir = " + dir);
    }

    public virtual void TouchPadPosCentered(Vector2 pos)
    {
        //LogTool.Log("TouchPad Pos：" + pos);
    }

    public virtual void TouchPadVector2(Vector2 vector)
    {
        //LogTool.Log("TouchPad Vector2：" + vector );
    }

    public virtual void TouchPadStretchVector2(Vector2 vector)
    {
        //LogTool.Log("TouchPad StretchVector2：" + vector);
    }

    public virtual void TouchPadTouch(bool isDown)
    {
        //Debug.Log("TouchPad是否被触摸：" + isDown);
    }

    public virtual void TouchPadPress(GameObject hitGO, bool isDown)
    {
        //if (hitGO != null)
        //    Debug.Log("射线击中：" + hitGO.name);

        //Debug.Log("TouchPad是否被按下：" + isDown);
    }

    public virtual void TouchPadPressDir(Vector3 dir, bool isDown)
    {
        //Debug.Log("手柄方向：" + dir + " TouchPad是否按下" + isDown);
    }

    public virtual void PointerEnter(GameObject go)
    {
        //Debug.Log("PointerEnter:" + go);
    }

    public virtual void PointerExit(GameObject go)
    {
        //Debug.Log("PointerExit:" + go);
    }

    public virtual void TouchPadSlipUnifiedDirSlip(GestureDirection gestureDir, float f)
    {
        //LogTool.Log("滑动中手势方向：" + gestureDir + " f:" + f);
    }

    public virtual void TouchPadEveryoneSlipUnifiedDirSlip(GestureDirection gestureDir, float f)
    {
        //LogTool.Log("滑动中手势方向：" + gestureDir + " f:" + f);
    }

    public virtual void TouchPadSlipUnifiedDirEnd(GestureDirection gestureDir)
    {
        //LogTool.Log("滑动结束手势方向：" + gestureDir);
    }

    public virtual void TriggerPress(GameObject hitGO, bool isDown)
    {
        //if (hitGO != null)
        //    Debug.Log("射线击中：" + hitGO.name);

        //Debug.Log("Trigger是否被按下：" + isDown);
    }

    public virtual void TriggerPressDir(Vector3 dir, bool isDown)
    {
        //Debug.Log("手柄方向：" + dir + " Trigger是否按下" + isDown);
    }

    public virtual void TouchPadAnyClick(GameObject go)
    {
        //Debug.Log("点击TouchPad按键");
    }

    public virtual void TouchPadDir(Vector2 dir)
    {
        //Debug.Log("TouchPad dir：" + dir);
    }
}

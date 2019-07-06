/*
 * Author:李传礼
 * DateTime:2017.7.19
 * Description:VR输入消息（手柄事件）
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Gvr.Internal;

public class VRInputMessageEngine : SingletonMB<VRInputMessageEngine>
{
    public Action<bool> AppButtonPressCallback;
    public Action HomeButtonUpCallback;
    public Action<Vector3> HoumeButtonLongPressCallback;
    public Action<Vector3> RecenteredCallback;
    public Action<Vector2> TouchPadPosCenteredCallback;//基于中心点的位置
    public Action<Vector2> TouchPadVector2Callback;//触摸向量(每帧向量)
    public Action<Vector2> TouchPadStretchVector2Callback;//触摸向量（从接触点作为起点算的向量）
    public Action<bool> TouchPadTouchCallback;//接触
    public Action<GameObject, bool> TouchPadPressCallback;//按下
    public Action<Vector3, bool> TouchPadPressDirCallback;//按下
    public Action<GameObject> PointerEnterCallback;
    public Action<GameObject> PointerExitCallback;

    //手势滑动
    public Action<GestureDirection, float> TouchPadSlipUnifiedDirCallback;//统一方向的滑动
    public Action<GestureDirection, float> TouchPadEveryoneSlipUnifiedDirCallback;//不统一方向的滑动
    public Action<GestureDirection> TouchPadSlipUnifiedDirEndCallback;//统一方向的滑动结束

    bool IsInit = false;
    bool IsTouchDown;
    Vector2 StartTouchPoint;
    Vector2 FirstTouchPoint;
    GameObject CurGO;//上一次射到的物体
    GameObject ButtonDownGO;//按下时的物体 click共用

    float HomeBtnDownTime;
    float TriggerDownTime;
    float SpaceTime;

    bool IsGesture;
    float GestureThreshold;//手势阀值
    GestureDirection CurGestureDir;
    GestureDirection CurGestureDirSlip;
    SvrControllerState CurGvrConnectionState;

    private void Awake()
    {
        GvrControllerInput.OnConterollerChanged += GvrControllerInput_OnStateChanged;
    }

    private void OnDestroy()
    {
        GvrControllerInput.OnConterollerChanged -= GvrControllerInput_OnStateChanged;
    }

    private void OnEnable()
    {
        GvrControllerInput_OnStateChanged(GvrControllerInput.SvrState, GvrControllerInput.SvrState);
    }

    private void GvrControllerInput_OnStateChanged(SvrControllerState state, SvrControllerState oldState)
    {
        CurGvrConnectionState = state;
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        EditorInputMessage();
#elif UNITY_ANDROID
            GvrInputMessage();

#endif
    }

    public void Init()
    {
        if (IsInit)
            return;

        IsInit = true;
        IsTouchDown = false;
        StartTouchPoint = Vector3.zero;
        FirstTouchPoint = Vector3.zero;
        CurGO = null;
        ButtonDownGO = null;

        HomeBtnDownTime = -1;
        TriggerDownTime = -1;
        SpaceTime = 1.2f;

        IsGesture = false;
        GestureThreshold = 0.1f;
        CurGestureDir = GestureDirection.Up;
        CurGestureDirSlip = GestureDirection.Up;
    }

    void EditorInputMessage()
    {
        //PC simulation by lichuanli
        RaycastHit hit;
        GameObject hitGO = null;
        Vector3 hitPos = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            hitGO = hit.collider.gameObject;
            hitPos = hit.point;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (TouchPadPressCallback != null)
                TouchPadPressCallback(hitGO, true);

            if (TouchPadPressDirCallback != null)
                TouchPadPressDirCallback(Camera.main.transform.forward, true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (TouchPadPressCallback != null)
                TouchPadPressCallback(hitGO, false);

            if (TouchPadPressDirCallback != null)
                TouchPadPressDirCallback(Camera.main.transform.forward, false);
        }

        if (Input.GetKeyDown("a"))
        {
            if (AppButtonPressCallback != null)
                AppButtonPressCallback(true);
        }
        else if (Input.GetKeyUp("a"))
        {
            if (AppButtonPressCallback != null)
                AppButtonPressCallback(false);
        }

        //if (Input.GetKeyDown("h"))
        //{
        //    HomeBtnDownTime = Time.time;
        //}
        //else if (Input.GetKeyUp("h"))
        //{
        //    if (HomeBtnDownTime == -1)//超过时间限制则忽略该事件
        //    {
        //        return;
        //    }

        //    HomeBtnDownTime = -1;

        //    if (HomeButtonUpCallback != null)
        //        HomeButtonUpCallback();
        //}

        //if (HomeBtnDownTime != -1)
        //{
        //    if (Time.time - HomeBtnDownTime >= SpaceTime)
        //    {
        //        HomeBtnDownTime = -1;
        //        if (HoumeButtonLongPressCallback != null)
        //            HoumeButtonLongPressCallback(Camera.main.transform.forward);
        //    }
        //}

        if (Input.GetMouseButtonDown(1))
        {
            IsTouchDown = true;
            StartTouchPoint = Input.mousePosition;
            FirstTouchPoint = Input.mousePosition;

            if (TouchPadTouchCallback != null)
                TouchPadTouchCallback(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            IsTouchDown = false;
            StartTouchPoint = Vector2.zero;
            FirstTouchPoint = Vector2.zero;

            if (TouchPadTouchCallback != null)
                TouchPadTouchCallback(false);

            if (IsGesture)
            {
                IsGesture = false;

                if (TouchPadSlipUnifiedDirEndCallback != null)
                    TouchPadSlipUnifiedDirEndCallback(CurGestureDir);
            }
        }

        if (IsTouchDown)
        {
            if (TouchPadVector2Callback != null)
                TouchPadVector2Callback(((Vector2)Input.mousePosition - StartTouchPoint) / Screen.width);

            Vector2 stretchVector2 = ((Vector2)Input.mousePosition - FirstTouchPoint) / Screen.width;
            if (TouchPadStretchVector2Callback != null)
                TouchPadStretchVector2Callback(stretchVector2);

            //手势判断
            if (!IsGesture && stretchVector2.sqrMagnitude > Mathf.Pow(GestureThreshold, 2))
            {
                IsGesture = true;
                CurGestureDir = PreDefScrp.ComputeGestureDirection(stretchVector2);
            }

            if (IsGesture)
            {
                float f = PreDefScrp.ComputeGestureDirectionLen(CurGestureDir, stretchVector2);
                if (TouchPadSlipUnifiedDirCallback != null)
                    TouchPadSlipUnifiedDirCallback(CurGestureDir, f);
            }

            StartTouchPoint = Input.mousePosition;
        }

        if (Input.GetKeyDown("r"))
        {
            if (RecenteredCallback != null)
                RecenteredCallback(Camera.main.transform.forward);
        }

        //if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        //{
        //    float x = Camera.main.transform.eulerAngles.x;
        //    float y = Camera.main.transform.eulerAngles.y;
        //    y += Input.GetAxis("Mouse X") * 90 * Time.deltaTime;
        //    x -= Input.GetAxis("Mouse Y") * 90 * Time.deltaTime;

        //    if (x > 90 && x < 180)
        //        x = 90;
        //    if (x < 270 && x > 180)
        //        x = 270;
        //    Camera.main.transform.rotation = Quaternion.Euler(x, y, 0);
        //}

        if (CurGO != hitGO)
        {
            //Debug.Log("CurGo:" + CurGO);
            //Debug.Log("hitGO:" + hitGO);
            if (PointerEnterCallback != null)
                PointerEnterCallback(hitGO);

            if (PointerExitCallback != null)
                PointerExitCallback(CurGO);

            //LogTool.Log("__________________________________");

            CurGO = hitGO;
            if (hitGO != null)
                Cinema.IsPointerEnterVideoPlayerUI = true;
            else
                Cinema.IsPointerEnterVideoPlayerUI = false;
        }
    }

    void GvrInputMessage()
    {
        //if (GvrPtInputModule == null)  
        //{
        //    LogTool.Log("警告：GvrPtInputModule为空");
        //    return;
        //}

        GameObject hitGO = GvrPointerInputModule.CurrentRaycastResult.gameObject;//新版本为GvrPointerInputModule.CurrentRaycastResult.gameObject

        if (GvrControllerInput.AppButtonDown)//新版本为GvrControllerInput
        {
            IsTouchDown = false;
            if (AppButtonPressCallback != null)
                AppButtonPressCallback(true);
        }
        else if (GvrControllerInput.AppButtonUp)
        {
            if (AppButtonPressCallback != null)
                AppButtonPressCallback(false);
        }

        if (GvrControllerInput.TriggerButtonDown)
        {
            IsTouchDown = false;
        }

        if (GvrControllerInput.HomeButtonDown)
        {
            //LogTool.Log("GVR HomeBtn Down");
            IsTouchDown = false;
            HomeBtnDownTime = Time.time;
        }

        if (HomeBtnDownTime != -1)
        {
            if (GvrControllerInput.HomeButtonState)//长按
            {
                if (Time.time - HomeBtnDownTime >= SpaceTime)
                {
                    LogTool.Log("GVR HomeBtn LongPress");
                    HomeBtnDownTime = -1;

                    if (HoumeButtonLongPressCallback != null)
                        HoumeButtonLongPressCallback(Camera.main.transform.forward);
                }
            }
            //else//这里松开
            //{
            //    LogTool.Log("GVR HomeBtn Up");
            //    if (HomeButtonUpCallback != null)
            //        HomeButtonUpCallback();

            //    HomeBtnDownTime = -1;
            //}
        }

		NoloRecenter();
        if (GvrControllerInput.Recentered)
        {
            if (RecenteredCallback != null)
                RecenteredCallback(Camera.main.transform.forward);
        }

        if (GvrControllerInput.IsTouching)
        {
            if (TouchPadPosCenteredCallback != null)
            {
                Vector2 touchPosVec = new Vector2(GvrControllerInput.TouchPos.x, -GvrControllerInput.TouchPos.y);
                Vector2 offset = new Vector2(-0.5f, 0.5f);
                touchPosVec = touchPosVec + offset;
                TouchPadPosCenteredCallback(touchPosVec * 2);//GvrControllerInput.TouchPosCentered
            }
        }

        if (GvrControllerInput.TouchDown)
        {
            IsTouchDown = true;
            StartTouchPoint = GvrControllerInput.TouchPos;
            FirstTouchPoint = GvrControllerInput.TouchPos;

            if (TouchPadTouchCallback != null)
                TouchPadTouchCallback(true);
        }
        else if (GvrControllerInput.TouchUp)
        {
            IsTouchDown = false;
            StartTouchPoint = Vector2.zero;
            FirstTouchPoint = Vector2.zero;

            if (TouchPadTouchCallback != null)
                TouchPadTouchCallback(false);

            if (IsGesture)
            {
                IsGesture = false;
                if (TouchPadSlipUnifiedDirEndCallback != null)
                    TouchPadSlipUnifiedDirEndCallback(CurGestureDir);
            }
        }

        if (IsTouchDown)
        {
            Vector3 oneFrameVector2 = GvrControllerInput.TouchPos - StartTouchPoint;
            StartTouchPoint = GvrControllerInput.TouchPos;
            oneFrameVector2 = new Vector2(oneFrameVector2.x, -oneFrameVector2.y);//y轴反向处理
            if (TouchPadVector2Callback != null)
                TouchPadVector2Callback(oneFrameVector2);

            Vector3 stretchVector2 = GvrControllerInput.TouchPos - FirstTouchPoint;
            stretchVector2 = new Vector2(stretchVector2.x, -stretchVector2.y);//y轴反向处理
            if (TouchPadStretchVector2Callback != null)
                TouchPadStretchVector2Callback(stretchVector2);

            //手势判断
            if (!IsGesture && stretchVector2.sqrMagnitude > Mathf.Pow(GestureThreshold, 2))
            {
                IsGesture = true;
                CurGestureDir = PreDefScrp.ComputeGestureDirection(stretchVector2);
            }

            if (IsGesture)
            {
                // firstPoint-by-point gesture
                float f = PreDefScrp.ComputeGestureDirectionLen(CurGestureDir, stretchVector2);
                if (TouchPadSlipUnifiedDirCallback != null)
                    TouchPadSlipUnifiedDirCallback(CurGestureDir, f);

                // everyPoint-by-point gesture
                CurGestureDirSlip = PreDefScrp.ComputeGestureDirection(oneFrameVector2);
                float t = PreDefScrp.ComputeGestureDirectionLen(CurGestureDirSlip, oneFrameVector2);
                if (TouchPadEveryoneSlipUnifiedDirCallback != null)
                    TouchPadEveryoneSlipUnifiedDirCallback(CurGestureDirSlip, t);
            }
        }

        if (CurGvrConnectionState == SvrControllerState.None)
        {
            // gaze版Recenter方法 Controller未连接的状态
            if (GvrPointerInputModule.Pointer.TriggerDown)
            {

                if (TouchPadPressCallback != null)
                    TouchPadPressCallback(hitGO, true);

                if (TouchPadPressDirCallback != null)
                    TouchPadPressDirCallback(Camera.main.transform.forward, true);

                TriggerDownTime = Time.time;
            }

            if (TriggerDownTime != -1)
            {
                if (Time.time - TriggerDownTime >= SpaceTime)//长按
                {
                    TriggerDownTime = -1;
                    //if (RecenteredCallback != null)
                    //    RecenteredCallback(Camera.main.transform.forward);
                }
                else
                {
                    if (GvrPointerInputModule.Pointer.TriggerUp)
                    {
                        TriggerDownTime = -1;
                        if (TouchPadPressCallback != null)
                            TouchPadPressCallback(hitGO, false);

                        if (TouchPadPressDirCallback != null)
                            TouchPadPressDirCallback(Camera.main.transform.forward, false);
                    }
                }
            }
            
        }
        else
        {
            if (GvrControllerInput.ClickButtonDown)
            {
                IsTouchDown = false;

                if (TouchPadPressCallback != null)
                    TouchPadPressCallback(hitGO, true);

                if (TouchPadPressDirCallback != null && hitGO == null)
                    TouchPadPressDirCallback(Camera.main.transform.forward, true);
            }
            else if (GvrControllerInput.ClickButtonUp)
            {
                if (TouchPadPressCallback != null)
                    TouchPadPressCallback(hitGO, false);

                if (TouchPadPressDirCallback != null && hitGO == null)
                    TouchPadPressDirCallback(Camera.main.transform.forward, false);
            }
        }

        if (CurGO != hitGO)
        {
            if (PointerExitCallback != null)
                PointerExitCallback(CurGO);

            if (PointerEnterCallback != null)
                PointerEnterCallback(hitGO);

            CurGO = hitGO;
            if (hitGO != null)
                Cinema.IsPointerEnterVideoPlayerUI = true;
             else
                Cinema.IsPointerEnterVideoPlayerUI = false;
        }
    }

    private int leftcontrollerRecenter_PreFrame = -1;
    private int rightcontrollerRecenter_PreFrame = -1;
    private int recenterSpacingFrame = 20;
    void NoloRecenter()
    {
        //leftcontroller double click system button
        if (GvrControllerInput.GetControllerState(SvrControllerState.NoloLeftContoller).homeButtonUp
            || GvrControllerInput.GetControllerState(SvrControllerState.NoloRightContoller).homeButtonUp)
        {
            if (Time.frameCount - leftcontrollerRecenter_PreFrame <= recenterSpacingFrame)
            {
                if (RecenteredCallback != null)
                    RecenteredCallback(Camera.main.transform.forward);
                leftcontrollerRecenter_PreFrame = -1;
            }
            else
            {
                leftcontrollerRecenter_PreFrame = Time.frameCount;
            }
        }

    }
}

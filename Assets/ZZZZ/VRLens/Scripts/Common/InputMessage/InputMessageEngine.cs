/*
 * Author:李传礼
 * DateTime:2017.7.19
 * Description:鼠标和触屏点击事件
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum InputMode {Drag, Zoom, Default}

sealed class InputMessageEngine : SingletonMB<InputMessageEngine>
{
    //鼠标点击事件
    public Action<GameObject, Vector3> LMouseClickCallback;
    public Action<GameObject, Vector3> LMouseDbClickCallback;
    public Action<Vector2> LMouseDragDeltaCallback;
    public Action<Vector3> LMouseDragScreenPosCallback;
    public Action LMouseUpCallback;

    public Action<GameObject, Vector3> RMouseClickCallback;
    public Action<GameObject, Vector3> RMouseDbClickCallback;
    public Action<Vector2> RMouseDragDeltaCallback;
    public Action<Vector3> RMouseDragScreenPosCallback;
    public Action RMouseUpCallback;

    public Action<float> MidMouseScrollCallback;
    public Action MidMouseScrollEndCallback;
    public Action<GameObject> MouseEnterCallback;
    public Action<GameObject> MouseExitCallback;

    //触屏点击事件
    public Action<GameObject, Vector3> SingleFingerTouchCallback;
    public Action<Vector2> SingleFingerDragDeltaCallback;
    public Action<Vector3> SingleFingerDragScreenPosCallback;
    public Action SingleFingerLiftCallback;//单指离开
    public Action<GameObject, Vector3> DoubleFingerTouchCallback;
    public Action<Vector2> DoubleFingerDragDeltaCallback;
    public Action<Vector3> DoubleFingerDragScreenPosCallback;
    public Action<float> DoubleFingerZoomDeltaCallback;//屏幕最小边百分比0-1
    public Action<InputMode> DoubleFingerLiftCallback;//双指离开

    [SerializeField]
    LayerMask CheckLayer;
    float SpaceTime;//时间间隔
    GameObject BtnClickGO; //单击的物体
    GameObject CurGO;//上一次射到的物体
    float LClickTime;//左键单击时间
    float RClickTime;//右键点击时间
    float LDragTime;
    float RDragTime;
    int LBtnClickCount;//左键点击次数
    int RBtnClickCount;//右键点击次数

    float LStartX, LEndX, LStartY, LEndY;//左键点击屏幕坐标
    float RStartX, REndX, RStartY, REndY;//右键点击屏幕坐标

    float LastRoll;//上一次中键滑动值

    float DoubleFingerDis;//两指之间距离
    float MinScreenSide;//最小屏幕边长
    InputMode CurInputMode;//当前模式
    int CurFingerMode;//当前手指模式
    float TouchTime;//手指第一次Touch时间

    Camera RayCamera;

    bool IsShowWarning;//是否显示警告

    void Awake()
    {
        SpaceTime = 0.3f;
        BtnClickGO = null;
        CurGO = null;
        LClickTime = 0;
        RClickTime = 0;
        LDragTime = -1;
        RDragTime = -1;
        LBtnClickCount = 0;
        RBtnClickCount = 0;

        LStartX = 0;
        LEndX = 0;
        LStartY = 0;
        LEndY = 0;
        RStartX = 0;
        REndX = 0;
        RStartY = 0;
        REndY = 0;

        LastRoll = 0;

        DoubleFingerDis = 0;
        MinScreenSide = PreDefScrp.GetMin(Screen.width, Screen.height);
        CurInputMode = InputMode.Default;
        CurFingerMode = 0;
        TouchTime = 0;

        Input.multiTouchEnabled = true;
        IsShowWarning = false;
    }

    void Update()
    {
        if (RayCamera != null)
        {
#if UNITY_STANDALONE_WIN
            InputMessageByMouse();
#elif UNITY_ANDROID || UNITY_IOS
            InputMessageByTouch();
#endif
        }
        else
        {
            if(!IsShowWarning)
            {
                Debug.LogWarning("-----------------------------------------------射线相机为空");
                IsShowWarning = true;
            }
        }
    }

    public void SetCamera(Camera camera)
    {
        RayCamera = camera;
    }

    public void ChangeCheckLayer(LayerMask checkLayer)
    {
        CheckLayer = checkLayer;
    }

    void InputMessageByMouse()
    {
        RaycastHit hit;
        GameObject hitGO = null;
        Vector3 hitPos = Vector3.zero;
        Ray ray = RayCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, CheckLayer.value))
        {
            hitGO = hit.collider.gameObject;
            hitPos = hit.point;
        }

        //左键点击
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - LClickTime < SpaceTime)
            {
                if (BtnClickGO == hitGO)
                    LBtnClickCount++;
                else
                {
                    BtnClickGO = hitGO;
                    LBtnClickCount = 1;
                }
            }
            else
            {
                BtnClickGO = hitGO;
                LBtnClickCount = 1;
            }
            if (LBtnClickCount == 1)
            {
                if (LMouseClickCallback != null)
                    LMouseClickCallback(hitGO, hitPos);
            }
            else if (LBtnClickCount == 2)
            {
                if (LMouseDbClickCallback != null)
                    LMouseDbClickCallback(hitGO, hitPos);
            }
            LClickTime = Time.time;
            LDragTime = LClickTime;
            LStartX = Input.mousePosition.x;
            LStartY = Input.mousePosition.y;
        }
        else if (Input.GetMouseButtonDown(1))//右键点击
        {
            if (Time.time - RClickTime < SpaceTime)
                if (BtnClickGO == hitGO)
                    RBtnClickCount++;
                else
                {
                    BtnClickGO = hitGO;
                    RBtnClickCount = 1;
                }

            else
            {
                BtnClickGO = hitGO;
                RBtnClickCount = 1;
            }
            if (RBtnClickCount == 1)
            {
                if (RMouseClickCallback != null)
                    RMouseClickCallback(hitGO, hitPos);
            }
            else if (RBtnClickCount == 2)
            {
                if (RMouseDbClickCallback != null)
                    RMouseDbClickCallback(hitGO, hitPos);
            }

            RClickTime = Time.time;
            RDragTime = RClickTime;
            RStartX = Input.mousePosition.x;
            RStartY = Input.mousePosition.y;
        }

        //左键拖拽
        if (LDragTime != -1)
        {
            LEndX = Input.mousePosition.x;
            LEndY = Input.mousePosition.y;
            float minusX = LEndX - LStartX;
            float minusY = LEndY - LStartY;

            if (Mathf.Abs(minusX) > 0 || Mathf.Abs(minusY) > 0)
            {
                if (LMouseDragDeltaCallback != null)
                    LMouseDragDeltaCallback(new Vector2(minusX, minusY));
                if (LMouseDragScreenPosCallback != null)
                    LMouseDragScreenPosCallback(Input.mousePosition);
            }
            LStartX = LEndX;
            LStartY = LEndY;
        }

        //右键拖拽
        if (RDragTime != -1)
        {
            REndX = Input.mousePosition.x;
            REndY = Input.mousePosition.y;
            float minusX = REndX - RStartX;
            float minusY = REndY - RStartY;

            if (Mathf.Abs(minusX) > 0 || Mathf.Abs(minusY) > 0)
            {
                if (RMouseDragDeltaCallback != null)
                    RMouseDragDeltaCallback(new Vector2(minusX, minusY));
                if (RMouseDragScreenPosCallback != null)
                    RMouseDragScreenPosCallback(Input.mousePosition);
            }
            RStartX = REndX;
            RStartY = REndY;
        }

        if (Input.GetMouseButtonUp(0))
        {
            LDragTime = -1;
            if (LMouseUpCallback != null)
                LMouseUpCallback();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            RDragTime = -1;
            if (RMouseUpCallback != null)
                RMouseUpCallback();
        }

        //中间滚轮
        float roll = Input.GetAxis("Mouse ScrollWheel");
        if (roll != 0)
        {
            if (MidMouseScrollCallback != null)
                MidMouseScrollCallback(roll);
        }
        else
        {
            if (LastRoll != 0)
            {
                if (MidMouseScrollEndCallback != null)
                    MidMouseScrollEndCallback();
            }
        }
        LastRoll = roll;

        if (CurGO != hitGO)
        {
            if (hitGO != null)
            {
                if (MouseEnterCallback != null)
                    MouseEnterCallback(hitGO);
                //Debug.Log("enter");
            }

            if (CurGO != null)
            {
                if (MouseExitCallback != null)
                    MouseExitCallback(CurGO);
                //Debug.Log("exit");
            }

            CurGO = hitGO;
        }
    }

    void InputMessageByTouch()
    {
        if (Input.touchCount != 0)
        {
            RaycastHit hit;
            GameObject hitGO = null;
            Vector3 hitPos = Vector3.zero;
            Ray ray = RayCamera.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CheckLayer.value))
            {
                hitGO = hit.collider.gameObject;
                hitPos = hit.point;
            }

            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)//获取触发时间
                    TouchTime = Time.time;

                if (CurFingerMode == 0)//进入1手指模式
                {
                    CurFingerMode = 1;
                    if (SingleFingerTouchCallback != null)
                        SingleFingerTouchCallback(hitGO, hitPos);
                }
                else if (CurFingerMode == 1)//单手指拖拽
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                        if (SingleFingerDragDeltaCallback != null)
                            SingleFingerDragDeltaCallback(touchDeltaPosition);
                        if (SingleFingerDragScreenPosCallback != null)
                            SingleFingerDragScreenPosCallback(Input.GetTouch(0).position);
                    }
                }
            }
            else if (Input.touchCount == 2)
            {
                Vector3 firstTouchPoint = Input.GetTouch(0).position;
                Vector3 secondTouchPoint = Input.GetTouch(1).position;
                float distance = Vector2.Distance(firstTouchPoint, secondTouchPoint);//两指间距离

                if (Input.GetTouch(1).phase == TouchPhase.Began)
                {
                    if (CurFingerMode == 0)//进入2手指模式
                    {
                        DoubleFingerDis = distance;
                        CurFingerMode = 2;
                        if (DoubleFingerTouchCallback != null)
                            DoubleFingerTouchCallback(hitGO, hitPos);
                    }
                    else if (CurFingerMode == 1)//如果和1手指模式的时间间隔很短则退出1手指模式进入2手指模式
                    {
                        if (Time.time - TouchTime <= SpaceTime)
                        {
                            //不需要发出单指离开消息
                            //if (SingleFingerLiftCallback != null)
                            //    SingleFingerLiftCallback();

                            DoubleFingerDis = distance;
                            CurFingerMode = 2;
                            if (DoubleFingerTouchCallback != null)
                                DoubleFingerTouchCallback(hitGO, hitPos);
                        }
                    }
                }

                //if (CurFingerMode == 0)
                //{
                //    DoubleFingerDis = distance;
                //    CurFingerMode = 2;
                //    if (DoubleFingerTouchCallback != null)
                //        DoubleFingerTouchCallback(hitGO, hitPos);
                //}
                if (CurFingerMode == 2)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
                    {
                        float ratio = (distance - DoubleFingerDis) / MinScreenSide;//拖拽占屏幕最小边百分比
                        DoubleFingerDis = distance;

                        if (CurInputMode == InputMode.Default)
                        {
                            Vector2 v0 = Input.GetTouch(0).deltaPosition;
                            Vector2 v1 = Input.GetTouch(1).deltaPosition;

                            float angle = Vector2.Angle(v0, v1);
                            if (angle < 45)//双指拖拽
                                CurInputMode = InputMode.Drag;
                            else//双指缩放
                                CurInputMode = InputMode.Zoom;
                        }
                        else if (CurInputMode == InputMode.Drag)//拖拽
                        {
                            Vector2 touchDeltaPosition = (Input.GetTouch(0).deltaPosition + Input.GetTouch(1).deltaPosition) / 2;
                            if (DoubleFingerDragDeltaCallback != null)
                                DoubleFingerDragDeltaCallback(touchDeltaPosition);
                            if (DoubleFingerDragScreenPosCallback != null)
                                DoubleFingerDragScreenPosCallback(firstTouchPoint);
                        }
                        else if (CurInputMode == InputMode.Zoom)//缩放
                        {
                            if (Mathf.Abs(ratio) > 0)
                            {
                                if (DoubleFingerZoomDeltaCallback != null)
                                    DoubleFingerZoomDeltaCallback(ratio);
                            }
                        }
                    }
                }
            }
        }
        else if (Input.touchCount == 0)
        {
            if (CurFingerMode != 0)//模式置位
            {
                if (CurFingerMode == 1)
                {
                    if (SingleFingerLiftCallback != null)//单手指离开
                        SingleFingerLiftCallback();
                }
                else if (CurFingerMode == 2)
                {
                    DoubleFingerDis = 0;
                    InputMode inputMode = CurInputMode;
                    CurInputMode = InputMode.Default;
                    if (DoubleFingerLiftCallback != null)//双手指离开
                        DoubleFingerLiftCallback(inputMode);
                }
                CurFingerMode = 0;
            }
        }
    }
}

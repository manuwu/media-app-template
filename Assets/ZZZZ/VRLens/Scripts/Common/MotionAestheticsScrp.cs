/*
 *Author:StevenLi
 *Email:lclwork@163.com
 *Create Data:2014.11.27
 *Description:These functions of this class could control object moving smoothly.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

//运动美学分支
public enum MABRANCH { MOVE, ROTATE, ROTATEQT, ROTATEBYAXIS, SCALE, TRANSPARENT, COLOR }
public enum MotionStatusPositon { Start, End }

public class MotionAestheticsScrp : MonoBehaviour
{
    float m_accelerated;//加速度
    float m_startV;//起始速度
    float m_endV;//末尾速度
    float m_dis;//运动距离

    Vector3 m_fromPos;//起点
    Vector3 m_toPos;//终点
    bool m_isLocalPos;//是否修改本地位置

    Vector3 m_fromEuler;//起始欧拉角
    Vector3 m_toEuler;//末尾欧拉角
    bool m_isLocalEuler;//是修改本地欧拉角

    Vector3 m_fromScale;//起始缩放
    Vector3 m_toScale;//末尾缩放
    //public Vector3 m_scaleRatio;//本地缩放比全局缩放

    //针对颜色
    Material m_material;
    Graphic m_widget;
    Color m_fromColor;
    Color m_toColor;

    //连续目标
    List<Vector3> m_vector3List;
    int m_goNext;

    GameObject m_moveGo;//运动物体
    float m_startTime;//开始运动时间
    public float m_totalTime;//总共时间
    float m_t;//因子 

    bool m_isTransparent;//是否开启透明
    int m_objTransParentType;//物体透明属性 0:脚本所绑物体不具有透明属性，1:三维透明，2:二维透明
    //针对三位物体
    Material[] m_nowMaterialArray, m_originalMaterialArray;//现在的材质，原始材质
    Color[] m_colorNoAlphaArray;//没有Alpha的颜色组
    //针对二维图片
    Graphic[] m_widgetArray;//存储二维Widget
    Color[] m_originalColor2D;//原始颜色
    float m_fromA;//初始Alpha
    float m_toA;//末尾Alpha

    bool m_isRecoverMaterial;//是否恢复原材质
    public MABRANCH m_maBranch;//哪种类型

    AnimationCurve m_animCurve;//动画曲线
    int m_totalLoop;//总共次数 -1标识不摧毁脚本 -2表示一直循环
    int m_loop;//还剩循环次数 
    string m_sign;//标记

	public Action PlayEndCallback;
    public Action<GameObject, string> PlayEndGOSignCallback;

    Quaternion m_fromQt;//起始四元素
    Quaternion m_toQt;//起始四元素
    bool m_isLocalQt;

    Vector3 m_axis;//旋转轴
    float m_aroundAngle;//已经旋转的角度
    float m_totalAngle;//总共要旋转角度
    bool m_isLocalAround;

    public bool m_isRollback;//当前是否回滚中
    public bool IsKeepLive;//是否保持长存

    MotionAestheticsScrp()
    {
        m_accelerated = 0;
        m_startV = 0;
        m_endV = 0;
        m_dis = 0;
        m_fromPos = Vector3.zero;
        m_toPos = Vector3.zero;
        m_isLocalPos = false;
        m_fromEuler = Vector3.zero;
        m_toEuler = Vector3.zero;
        m_isLocalEuler = false;
        m_fromScale = Vector3.one;
        m_toScale = Vector3.one;
        //m_scaleRatio = Vector3.one;
        m_material = null;
        m_widget = null;
        m_fromColor = Color.black;
        m_toColor = Color.black;

        m_moveGo = null;
        m_startTime = -1;
        m_totalTime = 0;
        m_t = 0;

        m_isTransparent = false;
        m_objTransParentType = 0;
        m_nowMaterialArray = null;
        m_originalMaterialArray = null;
        m_colorNoAlphaArray = null;
        m_fromA = 1;
        m_toA = 1;
        m_isRecoverMaterial = false;
        m_animCurve = null;
        m_totalLoop = 0;
        m_loop = 0;
        m_sign = "";

        m_fromQt = Quaternion.identity;
        m_toQt = Quaternion.identity;
        m_isLocalQt = false;
        m_aroundAngle = 0;
        m_totalAngle = 0;
        m_axis = Vector3.zero;
        m_isLocalAround = false;

        m_isRollback = false;
        IsKeepLive = false;
    }

    void Start()
    {

    }

    void Update()
    {
        AccMoveLoop();
    }

    //开启反向
    public void StartRollback(bool isRollback)
    {
        if(m_isRollback != isRollback)
        {
            PreDefScrp.ValueSwap(ref m_fromPos, ref m_toPos);
            PreDefScrp.ValueSwap(ref m_fromEuler, ref m_toEuler);
            PreDefScrp.ValueSwap(ref m_fromScale, ref m_toScale);
            PreDefScrp.ValueSwap(ref m_fromColor, ref m_toColor);
            PreDefScrp.ValueSwap(ref m_fromA, ref m_toA);
            PreDefScrp.ValueSwap(ref m_fromQt, ref m_toQt);
            PreDefScrp.ValueSwap(ref m_fromPos, ref m_toPos);
            m_axis = -m_axis;

            m_startTime = Time.time + m_totalTime * m_t;//Time.time + 已经播放时间
            m_t = 1 - m_t;//当前分子
            m_aroundAngle = m_totalAngle * m_t;//对于反向后，已经旋转的角度

            if (m_totalLoop > 0)
                m_loop = m_totalLoop - (m_loop - 1);//减一是因为正在旋转，该过程还未结束

            m_isRollback = isRollback;
        }
    }

    //加速模式变化连续目标
    public static void AccTranslatingList(GameObject go, Transform from, List<Transform> toList, float time, float speedV, bool isStartV, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0)
            return;
        List<Vector3>[] branchArray = new List<Vector3>[3];//new的数组其实要删除
        for (int i = 0; i < toList.Count; i++)
        {
            branchArray[0].Add(toList[i].position);
            branchArray[1].Add(toList[i].eulerAngles);
            branchArray[2].Add(toList[i].localScale);
        }

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        MotionAestheticsScrp motionAth1 = go.AddComponent<MotionAestheticsScrp>();
        MotionAestheticsScrp motionAth2 = go.AddComponent<MotionAestheticsScrp>();

        motionAth.BeginAccList(MABRANCH.MOVE, go, from.position, branchArray[0], time, speedV, isStartV, delayTime, sign, isKeepLive);
        motionAth1.BeginAccList(MABRANCH.ROTATE, go, from.eulerAngles, branchArray[1], time, speedV, isStartV, delayTime, sign, isKeepLive);
        motionAth2.BeginAccList(MABRANCH.SCALE, go, from.localScale, branchArray[2], time, speedV, isStartV, delayTime, sign, isKeepLive);
    }

    //加速模式连续透明变化
    public static MotionAestheticsScrp AccTransparentingList(GameObject go, float from, List<float> toList, float time, float speedV, bool isStartV, float delayTime = 0, string sign = "", bool isRecoverMaterial = false)
    {
        if (go == null || time < 0 || from < 0 || from > 1)
            return null;

        List<Vector3> transparentList = new List<Vector3>();
        for (int i = 0; i < toList.Count; i++)
        {
            float a = toList[i];
            if (a < 0 || a > 1)
                return null;

            transparentList.Add(new Vector3(a, 0, 0));
        }

        MotionAestheticsScrp motionAth = GetTransparentScrp(go);
        motionAth.BeginAccList(MABRANCH.TRANSPARENT, go, new Vector3(from, 0, 0), transparentList, time, speedV, isStartV, delayTime, sign, true, isRecoverMaterial);
        return motionAth;
    }

    public static void AccTranslating(GameObject go, Transform from, Transform to, float time, AnimationCurve animCurve, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        MotionAestheticsScrp motionAth1 = go.AddComponent<MotionAestheticsScrp>();
        MotionAestheticsScrp motionAth2 = go.AddComponent<MotionAestheticsScrp>();

        motionAth.BeginAcc(MABRANCH.MOVE, go, from.position, to.position, time, animCurve, loop, delayTime, sign, isKeepLive);

        Vector3 eulerFrom = from.eulerAngles;
        Vector3 eulerTo = to.eulerAngles;
        EulerAnglesOptimize(ref eulerFrom, ref eulerTo);
        motionAth1.BeginAcc(MABRANCH.ROTATE, go, eulerFrom, eulerTo, time, animCurve, loop, delayTime, sign, isKeepLive);

        motionAth2.BeginAcc(MABRANCH.SCALE, go, from.localScale, to.localScale, time, animCurve, loop, delayTime, sign, isKeepLive);
    }

    public static void AccTranslating(GameObject go, Transform from, Transform to, float time, float speedV, bool isStartV, int loop, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return;
        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        MotionAestheticsScrp motionAth1 = go.AddComponent<MotionAestheticsScrp>();
        MotionAestheticsScrp motionAth2 = go.AddComponent<MotionAestheticsScrp>();

        motionAth.BeginAcc(MABRANCH.MOVE, go, from.position, to.position, time, speedV, isStartV, loop, delayTime, sign, isKeepLive);

        Vector3 eulerFrom = from.eulerAngles;
        Vector3 eulerTo = to.eulerAngles;
        EulerAnglesOptimize(ref eulerFrom, ref eulerTo);
        motionAth1.BeginAcc(MABRANCH.ROTATE, go, eulerFrom, eulerTo, time, speedV, isStartV, loop, delayTime, sign, isKeepLive);

        motionAth2.BeginAcc(MABRANCH.SCALE, go, from.localScale, to.localScale, time, speedV, isStartV, loop, delayTime, sign, isKeepLive);
    }

    public static MotionAestheticsScrp AccMoving(GameObject go, Vector3 from, Vector3 to, float time, AnimationCurve animCurve, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.BeginAcc(MABRANCH.MOVE, go, from, to, time, animCurve, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    public static MotionAestheticsScrp AccMoving(GameObject go, Vector3 from, Vector3 to, float time, float speedV, bool isStartV, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.BeginAcc(MABRANCH.MOVE, go, from, to, time, speedV, isStartV, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    public static MotionAestheticsScrp AccLocalMoving(GameObject go, Vector3 from, Vector3 to, float time, AnimationCurve animCurve, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.m_isLocalPos = true;
        motionAth.BeginAcc(MABRANCH.MOVE, go, from, to, time, animCurve, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    public static MotionAestheticsScrp AccLocalMoving(GameObject go, Vector3 from, Vector3 to, float time, float speedV, bool isStartV, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.m_isLocalPos = true;
        motionAth.BeginAcc(MABRANCH.MOVE, go, from, to, time, speedV, isStartV, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    public static MotionAestheticsScrp AccRotating(GameObject go, Vector3 from, Vector3 to, float time, AnimationCurve animCurve, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.BeginAcc(MABRANCH.ROTATE, go, from, to, time, animCurve, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    public static MotionAestheticsScrp AccRotating(GameObject go, Vector3 from, Vector3 to, float time, float speedV, bool isStartV, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.BeginAcc(MABRANCH.ROTATE, go, from, to, time, speedV, isStartV, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    //新添加旋转
    public static MotionAestheticsScrp AccRotating(GameObject go, Quaternion from, Quaternion to, float time, AnimationCurve animCurve, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.BeginAcc(go, from, to, time, animCurve, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    public static MotionAestheticsScrp AccRotating(GameObject go, Vector3 Axis, float angle, float time, AnimationCurve animCurve, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.BeginAcc(go, Axis, angle, time, animCurve, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }
    //新添加旋转


    public static MotionAestheticsScrp AccLocalRotating(GameObject go, Vector3 from, Vector3 to, float time, AnimationCurve animCurve, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.m_isLocalEuler = true;
        motionAth.BeginAcc(MABRANCH.ROTATE, go, from, to, time, animCurve, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    public static MotionAestheticsScrp AccLocalRotating(GameObject go, Vector3 from, Vector3 to, float time, float speedV, bool isStartV, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.m_isLocalEuler = true;
        motionAth.BeginAcc(MABRANCH.ROTATE, go, from, to, time, speedV, isStartV, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    //新添加旋转
    public static MotionAestheticsScrp AccLocalRotating(GameObject go, Quaternion from, Quaternion to, float time, AnimationCurve animCurve, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.m_isLocalQt = true;
        motionAth.BeginAcc(go, from, to, time, animCurve, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    public static MotionAestheticsScrp AccLocalRotating(GameObject go, Vector3 Axis, float angle, float time, AnimationCurve animCurve, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.m_isLocalAround = true;
        motionAth.BeginAcc(go, Axis, angle, time, animCurve, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }
    //新添加旋转

    public static MotionAestheticsScrp AccLocalScaling(GameObject go, Vector3 from, Vector3 to, float time, AnimationCurve animCurve, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.BeginAcc(MABRANCH.SCALE, go, from, to, time, animCurve, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    public static MotionAestheticsScrp AccLocalScaling(GameObject go, Vector3 from, Vector3 to, float time, float speedV, bool isStartV, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.BeginAcc(MABRANCH.SCALE, go, from, to, time, speedV, isStartV, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    public static MotionAestheticsScrp AccColorChanging(GameObject go, Color from, Color to, float time, AnimationCurve animCurve, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        if (go == null || time < 0 || loop == 0)
            return null;

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.BeginAccColor(go, from, to, time, animCurve, loop, delayTime, sign, isKeepLive);
        return motionAth;
    }

    public static MotionAestheticsScrp AccColorChanging(GameObject go, Color from, Color to, float time, float speedV, bool isStartV, int loop = 1, float delayTime = 0, string sign = "", bool isKeepLive = false)
    {
        {
            if (go == null || time < 0 || loop == 0)
                return null;

            MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
            motionAth.BeginAccColor(go, from, to, time, speedV, isStartV, loop, delayTime, sign, isKeepLive);
            return motionAth;
        }
    }

    public static MotionAestheticsScrp AccTransparenting(GameObject go, float from, float to, float time, AnimationCurve animCurve, int loop = 1, float delayTime = 0, string sign = "", bool isRecoverMaterial = false)
    {
        if (go == null || time < 0 || loop == 0 || from < 0 || from > 1 || to < 0 || to > 1)
            return null;

        MotionAestheticsScrp motionAth = GetTransparentScrp(go);
        motionAth.BeginAcc(MABRANCH.TRANSPARENT, go, new Vector3(from, 0, 0), new Vector3(to, 0, 0), time, animCurve, loop, delayTime, sign, true, isRecoverMaterial);
        return motionAth;
    }

    public static MotionAestheticsScrp AccTransparenting(GameObject go, float from, float to, float time, float speedV, bool isStartV, int loop = 1, float delayTime = 0, string sign = "", bool isRecoverMaterial = false)
    {
        if (go == null || time < 0 || loop == 0 || from < 0 || from > 1 || to < 0 || to > 1)
            return null;

        MotionAestheticsScrp motionAth = GetTransparentScrp(go);
        motionAth.BeginAcc(MABRANCH.TRANSPARENT, go, new Vector3(from, 0, 0), new Vector3(to, 0, 0), time, speedV, isStartV, loop, delayTime, sign, true, isRecoverMaterial);
        return motionAth;
    }

    public static MotionAestheticsScrp AccTransparenting(GameObject go, float to, float time, float speedV, bool isStartV, int loop = 1, float delayTime = 0, string sign = "", bool isRecoverMaterial = false)
    {
        if (go == null || time < 0 || loop == 0 || to < 0 || to > 1)
            return null;

        MotionAestheticsScrp motionAth = GetTransparentScrp(go);
        motionAth.BeginAcc(MABRANCH.TRANSPARENT, go, new Vector3(-1, 0, 0), new Vector3(to, 0, 0), time, speedV, isStartV, loop, delayTime, sign, true, isRecoverMaterial);
        return motionAth;
    }

    public void BeginAccList(MABRANCH maBranch, GameObject go, Vector3 from, List<Vector3> toList, float time, float speedV, bool isStartV, float delayTime, string sign, bool isKeepLive, bool isRecoverMaterial = false)
    {
        if (toList.Count > 0)
        {
            m_goNext = 0;
            m_vector3List = toList;
            BeginAcc(maBranch, go, from, toList[m_goNext], time, speedV, isStartV, 1, delayTime, sign, isKeepLive, isRecoverMaterial);
            m_goNext++;
        }
    }

    public void BeginAcc(MABRANCH maBranch, GameObject go, Vector3 from, Vector3 to, float time, float speedV, bool isStartV, int loop, float delayTime, string sign, bool isKeepLive, bool isRecoverMaterial = false)
    {
        m_maBranch = maBranch;
        m_totalLoop = loop;
        m_loop = loop;
        m_sign = sign;
        IsKeepLive = isKeepLive;

        switch (m_maBranch)
        {
            case MABRANCH.MOVE:
                m_fromPos = from;
                m_toPos = to;
                break;
            case MABRANCH.ROTATE:
                m_fromEuler = from;
                m_toEuler = to;
                break;
            case MABRANCH.SCALE:
                m_fromScale = from;
                m_toScale = to;
                break;
            case MABRANCH.TRANSPARENT:
                if (m_originalMaterialArray == null && m_originalColor2D == null)
                    m_isTransparent = true;
                else//说明已经储存了材质，这次不用储存，所以m_isTransparent为false
                    m_isTransparent = false;
                m_fromA = from.x;
                m_toA = to.x;
                break;
        }
        SaveAndInitMaterial(go);
        m_isRecoverMaterial = isRecoverMaterial;

        if (m_maBranch == MABRANCH.TRANSPARENT && from.x == -1)
            from = new Vector3(m_fromA, 0, 0);

        m_dis = Vector3.Distance(from, to);
        m_totalTime = time;

        if (isStartV)
        {
            m_startV = speedV;
            m_accelerated = 2 * (m_dis - m_startV * time) / Mathf.Pow(time, 2);
            m_endV = m_startV + m_accelerated * time;
        }
        else
        {
            m_endV = speedV;
            m_accelerated = (2 * m_endV * time - 2 * m_dis) / Mathf.Pow(time, 2);
            m_startV = m_endV - m_accelerated * time;
        }

        if (loop > 0)
            Invoke("DelayBegin", delayTime);
        else if (loop < 0)
            InvokeRepeating("DelayBegin", delayTime, time);
    }

    //曲线版
    public void BeginAcc(MABRANCH maBranch, GameObject go, Vector3 from, Vector3 to, float time, AnimationCurve animCurve, int loop, float delayTime, string sign, bool isKeepLive, bool isRecoverMaterial = false)
    {
        m_maBranch = maBranch;
        m_totalLoop = loop;
        m_loop = loop;
        m_sign = sign;
        IsKeepLive = isKeepLive;

        switch (m_maBranch)
        {
            case MABRANCH.MOVE:
                m_fromPos = from;
                m_toPos = to;
                break;
            case MABRANCH.ROTATE:
                m_fromEuler = from;
                m_toEuler = to;
                break;
            case MABRANCH.SCALE:
                m_fromScale = from;
                m_toScale = to;
                break;
            case MABRANCH.TRANSPARENT:
                if (m_originalMaterialArray == null && m_originalColor2D == null)
                    m_isTransparent = true;
                else//说明已经储存了材质，这次不用储存，所以m_isTransparent为false
                    m_isTransparent = false;
                m_fromA = from.x;
                m_toA = to.x;
                break;
        }
        SaveAndInitMaterial(go);
        m_isRecoverMaterial = isRecoverMaterial;

        if (m_maBranch == MABRANCH.TRANSPARENT && from.x == -1)
            from = new Vector3(m_fromA, 0, 0);
        m_totalTime = time;
        m_animCurve = animCurve;

        if (loop > 0)
            Invoke("DelayBegin", delayTime);
        else if (loop < 0)
            InvokeRepeating("DelayBegin", delayTime, time);
    }

    public void BeginAcc(GameObject go, Quaternion from, Quaternion to, float time, AnimationCurve animCurve, int loop, float delayTime, string sign, bool isKeepLive)
    {
        m_maBranch = MABRANCH.ROTATEQT;
        m_moveGo = go;
        m_fromQt = from;
        m_toQt = to;
        m_totalTime = time;
        m_animCurve = animCurve;
        m_totalLoop = loop;
        m_loop = loop;
        m_sign = sign;
        IsKeepLive = isKeepLive;

        if (loop > 0)
            Invoke("DelayBegin", delayTime);
        else if (loop < 0)
            InvokeRepeating("DelayBegin", delayTime, time);
    }

    public void BeginAcc(GameObject go, Vector3 axis, float angle, float time, AnimationCurve animCurve, int loop, float delayTime, string sign, bool isKeepLive)
    {
        m_maBranch = MABRANCH.ROTATEBYAXIS;
        m_moveGo = go;
        m_axis = axis;
        m_aroundAngle = 0;
        m_totalAngle = angle;
        m_totalTime = time;
        m_animCurve = animCurve;
        m_totalLoop = loop;
        m_loop = loop;
        m_sign = sign;
        IsKeepLive = isKeepLive;

        if (loop > 0)
            Invoke("DelayBegin", delayTime);
        else if (loop < 0)
            InvokeRepeating("DelayBegin", delayTime, time);
    }

    //函数：开始颜色的加速变化
    public void BeginAccColor(GameObject go, Color from, Color to, float time, float speedV, bool isStartV, int loop, float delayTime, string sign, bool isKeepLive)
    {
        m_maBranch = MABRANCH.COLOR;
        m_moveGo = go;
        m_fromColor = from;
        m_toColor = to;
        m_totalLoop = loop;
        m_loop = loop;
        m_sign = sign;
        IsKeepLive = isKeepLive;

        Renderer render = m_moveGo.GetComponent<Renderer>();
        if (render != null)
            m_material = render.material;
        m_widget = m_moveGo.GetComponent<Graphic>();

        float fromA = m_fromColor.a;
        float toA = m_toColor.a;
        m_dis = Vector3.Distance(new Vector3(m_fromColor.r + fromA, m_fromColor.g, m_fromColor.b), new Vector3(m_toColor.r + toA, m_toColor.g, m_toColor.b));
        m_totalTime = time;

        if (isStartV)
        {
            m_startV = speedV;
            m_accelerated = 2 * (m_dis - m_startV * time) / Mathf.Pow(time, 2);
            m_endV = m_startV + m_accelerated * time;
        }
        else
        {
            m_endV = speedV;
            m_accelerated = (2 * m_endV * time - 2 * m_dis) / Mathf.Pow(time, 2);
            m_startV = m_endV - m_accelerated * time;
        }

        if (loop > 0)
            Invoke("DelayBegin", delayTime);
        else if (loop < 0)
            InvokeRepeating("DelayBegin", delayTime, time);
    }

    //曲线版颜色
    public void BeginAccColor(GameObject go, Color from, Color to, float time, AnimationCurve animCurve, int loop, float delayTime, string sign, bool isKeepLive)
    {
        m_maBranch = MABRANCH.COLOR;
        m_moveGo = go;
        m_fromColor = from;
        m_toColor = to;
        m_totalTime = time;
        m_animCurve = animCurve;
        m_totalLoop = loop;
        m_loop = loop;
        m_sign = sign;
        IsKeepLive = isKeepLive;

        Renderer render = m_moveGo.GetComponent<Renderer>();
        if (render != null)
            m_material = render.material;
        m_widget = m_moveGo.GetComponent<Graphic>();

        if (loop > 0)
            Invoke("DelayBegin", delayTime);
        else if (loop < 0)
            InvokeRepeating("DelayBegin", delayTime, time);
    }

    public static void FixedDecMoving(GameObject go, Vector3 dir, float units, float delayTime = 0, Space relativeTo = Space.Self, string sign = "", bool isKeepLive = false)
    {
        if (go == null)
            return;

        Transform toTrans = (Instantiate(go) as GameObject).transform;
        toTrans.position = go.transform.position;
        toTrans.eulerAngles = go.transform.eulerAngles;
        toTrans.localScale = go.transform.localScale;
        toTrans.Translate(dir.normalized * units, relativeTo);
        Vector3 to = toTrans.position;
        Destroy(toTrans.gameObject);

        MotionAestheticsScrp motionAth = go.AddComponent<MotionAestheticsScrp>();
        motionAth.BeginFixedDecMove(MABRANCH.MOVE, go, go.transform.position, to, delayTime, sign, isKeepLive);
    }

    public void BeginFixedDecMove(MABRANCH maBranch, GameObject go, Vector3 from, Vector3 to, float delayTime, string sign, bool isKeepLive, bool isRecoverMaterial = false)
    {
        m_maBranch = maBranch;
        m_sign = sign;
        IsKeepLive = isKeepLive;

        switch (m_maBranch)
        {
            case MABRANCH.MOVE:
                m_fromPos = from;
                m_toPos = to;
                break;
            case MABRANCH.ROTATE:
                m_fromEuler = from;
                m_toEuler = to;
                break;
            case MABRANCH.SCALE:
                m_fromScale = from;
                m_toScale = to;
                break;
            case MABRANCH.TRANSPARENT:
                if (m_originalMaterialArray == null && m_originalColor2D == null)
                    m_isTransparent = true;
                else//说明已经储存了材质，这次不用储存，所以m_isTransparent为false
                    m_isTransparent = false;
                m_fromA = from.x;
                m_toA = to.x;
                break;
        }

        SaveAndInitMaterial(go);

        m_isRecoverMaterial = isRecoverMaterial;

        if (m_maBranch == MABRANCH.TRANSPARENT && from.x == -1)
            from = new Vector3(m_fromA, 0, 0);

        m_accelerated = -2;
        m_endV = 0;

        m_dis = Vector3.Distance(from, to);
        m_startV = Mathf.Sqrt(0 - 2 * m_accelerated * m_dis);
        m_totalTime = (0 - m_startV) / m_accelerated;

        Invoke("DelayBegin", delayTime);
    }

    //延迟开始
    void DelayBegin()
    {
        m_startTime = Time.time + m_totalTime;
    }

    public void DisableUpdateMotion()
    {
        if (IsInvoking("DelayBegin"))
            CancelInvoke("DelayBegin");
        m_startTime = -1;
    }

    public void MotionByFactor(float t)
    {
        m_t = t;
        NewValue();
    }

    //保存和初始化新材质
    void SaveAndInitMaterial(GameObject go)
    {
        m_moveGo = go;
        if (m_isTransparent)
        {
            //三维
            Renderer[] renderArray = m_moveGo.GetComponentsInChildren<Renderer>();
            m_nowMaterialArray = new Material[renderArray.Length];
            m_originalMaterialArray = new Material[renderArray.Length];
            m_colorNoAlphaArray = new Color[renderArray.Length];

            if (renderArray.Length > 0)
                if (renderArray[0].gameObject == this.gameObject)
                    m_objTransParentType = 1;

            float totalA = 0;
            for (int i = 0; i < renderArray.Length; i++)
            {
                m_nowMaterialArray[i] = renderArray[i].material;
                m_originalMaterialArray[i] = Instantiate(renderArray[i].material) as Material;
                Color oriColor = m_originalMaterialArray[i].color;
                m_colorNoAlphaArray[i] = oriColor -
                    new Color(0, 0, 0, oriColor.a);
                renderArray[i].material.shader = Shader.Find("Transparent/Bumped Diffuse");

                if (m_fromA == -1)
                    totalA += oriColor.a;
                else
                    renderArray[i].material.color = m_colorNoAlphaArray[i] + new Color(0, 0, 0, m_fromA);
            }

            //二维
            m_widgetArray = m_moveGo.GetComponentsInChildren<Graphic>();
            m_originalColor2D = new Color[m_widgetArray.Length];

            if (m_widgetArray.Length > 0)
                if (m_widgetArray[0].gameObject == this.gameObject)
                    m_objTransParentType = 2;

            float totalA2D = 0;
            for (int i = 0; i < m_widgetArray.Length; i++)
            {
                m_originalColor2D[i] = m_widgetArray[i].color;
                Color c = m_widgetArray[i].color;
                c -= new Color(0, 0, 0, c.a);

                if (m_fromA == -1)
                    totalA2D += m_widgetArray[i].color.a;
                else
                    m_widgetArray[i].color = c + new Color(0, 0, 0, m_fromA);
            }

            if (m_fromA == -1)
            {
                float a1, a2;
                if (renderArray.Length != 0)
                    a1 = totalA / renderArray.Length;
                else
                    a1 = 0;

                if (m_widgetArray.Length != 0)
                    a2 = totalA2D / m_widgetArray.Length;
                else
                    a2 = 0;

                m_fromA = (a1 + a2) / 2;
            }
        }
    }

    //计算加速运动的比例
    void ComputeAccRatio()
    {
        float time = m_totalTime - (m_startTime - Time.time);

        if (m_animCurve == null)
        {
            float goDis = m_startV * time + m_accelerated * (Mathf.Pow(time, 2)) / 2;
            if (m_dis != 0)
                m_t = goDis / m_dis;
            else
                m_t = 0;
        }
        else
        {
            if (m_totalTime != 0)
                m_t = m_animCurve.Evaluate(time / m_totalTime);
            else
                m_t = m_animCurve.Evaluate(1);
        }
    }

    //加速运动循环
    void AccMoveLoop()
    {
        if (m_loop == 0)
        {
            if (!IsKeepLive)
            {
                if (PlayEndCallback != null)
                    PlayEndCallback();
                if (PlayEndGOSignCallback != null)
                    PlayEndGOSignCallback(this.gameObject, m_sign);

                Reset();
                Destroy(this);
            }
        }
            
        if (m_startTime != -1)
        {
            if (m_startTime - Time.time >= 0)
            {
                ComputeAccRatio();
                NewValue();
            }
            else
            {
                //精确计算到末尾点
                m_startTime = Time.time;
                ComputeAccRatio();
                NewValue();
                switch (m_maBranch)
                {
                    case MABRANCH.TRANSPARENT:
                        {
                            //还原
                            if (m_isRecoverMaterial)
                            {
                                if (m_nowMaterialArray != null)
                                {
                                    for (int i = 0; i < m_nowMaterialArray.Length; i++)
                                    {
                                        if (m_nowMaterialArray[i] != null)
                                        {
                                            m_nowMaterialArray[i].shader = m_originalMaterialArray[i].shader;
                                            m_nowMaterialArray[i].color = m_originalMaterialArray[i].color;
                                        }
                                    }
                                }

                                if (m_widgetArray != null)
                                {
                                    for (int i = 0; i < m_widgetArray.Length; i++)
                                    {
                                        if(m_widgetArray[i] != null)
                                            m_widgetArray[i].color = m_originalColor2D[i];
                                    }
                                }
                            }
                        }
                        break;
                }

                m_loop--;
                if (m_loop > 0)
                    m_startTime = Time.time + m_totalTime;
                else
                    m_startTime = -1;

                //连续变化点
                if (m_vector3List != null)
                {
                    if (m_goNext < m_vector3List.Count)
                    {
                        switch (m_maBranch)
                        {
                            case MABRANCH.MOVE:
                                m_fromPos = m_toPos;
                                m_toPos = m_vector3List[m_goNext];
                                break;
                            case MABRANCH.ROTATE:
                                m_fromEuler = m_toEuler;
                                m_toEuler = m_vector3List[m_goNext];
                                break;
                            case MABRANCH.SCALE:
                                m_fromScale = m_toScale;
                                m_toScale = m_vector3List[m_goNext];
                                break;
                            case MABRANCH.TRANSPARENT:
                                m_fromA = m_toA;
                                m_toA = m_vector3List[m_goNext].x;
                                break;
                        }

                        m_startTime = Time.time + m_totalTime;
                        m_dis = Vector3.Distance(m_vector3List[m_goNext - 1], m_vector3List[m_goNext]);
                        m_goNext++;
                    }
                    else
                    {
                        m_vector3List.Clear();
                        if (PlayEndCallback != null)
                            PlayEndCallback();
                        if (PlayEndGOSignCallback != null)
                            PlayEndGOSignCallback(this.gameObject, m_sign);

                        if (!IsKeepLive)
                        {
                            Reset();
                            Destroy(this);
                        }
                    }
                }
                else
                {
                    if (m_loop == 0)
                    {
                        if (PlayEndCallback != null)
                            PlayEndCallback();
                        if (PlayEndGOSignCallback != null)
                            PlayEndGOSignCallback(this.gameObject, m_sign);

                        if (!IsKeepLive)
                        {
                            Reset();
                            Destroy(this);
                        }
                    }
                }
            }
        }
    }

    Vector3 MoveToNew()//位置
    {
        //return Vector3.Lerp(m_fromPos, m_toPos, m_t);
        Vector3 value = m_fromPos * (1 - m_t) + m_toPos * m_t;
        return value;
    }

    Vector3 RotateToNew()//旋转
    {
        //return Vector3.Lerp(m_fromEuler, m_toEuler, m_t);
        Vector3 value = m_fromEuler * (1 - m_t) + m_toEuler * m_t;
        return value;
    }

    Quaternion RotateQtToNew()
    {
        Quaternion value = Quaternion.LerpUnclamped(m_fromQt, m_toQt, m_t);
        return value;
    }

    float RotateAngle()
    {
        float value = m_totalAngle * m_t - m_aroundAngle;
        m_aroundAngle = m_totalAngle * m_t;
        return value;
    }

    Vector3 ScaleToNew()//缩放
    {
        //return Vector3.Lerp(m_fromScale, m_toScale, m_t);
        Vector3 value = m_fromScale * (1 - m_t) + m_toScale * m_t;
        return value;
    }

    Color Transparent(int index)//透明
    {
        float a = Mathf.Lerp(m_fromA, m_toA, m_t);
        if (index == 0)
        {
            //查看第一个本身物体是不是具备透明属性
            if (m_objTransParentType != 1)//不等于1说明第一位不是母体（母体不按照现有a的相对a变化，而是直接由a决定）
                a = m_originalMaterialArray[index].color.a * a;
        }
        else
            a = m_originalMaterialArray[index].color.a * a;

        return m_colorNoAlphaArray[index] + new Color(0, 0, 0, a);
    }

    Color Transparent2D(int index)//透明
    {
        float a = Mathf.Lerp(m_fromA, m_toA, m_t);

        if (index == 0)
        {
            //查看第一个本身物体是不是具备透明属性
            if (m_objTransParentType != 2)
                a = m_originalColor2D[index].a * a;
        }
        else
            a = m_originalColor2D[index].a * a;

        Color c = m_originalColor2D[index];
        c -= new Color(0, 0, 0, c.a);
        return c + new Color(0, 0, 0, a);
    }

    Color ColorToNew()
    {
        //return Color.Lerp(m_fromColor, m_toColor, m_t);
        Color value = m_fromColor * (1 - m_t) + m_toColor * m_t;
        return value;
    }

    void NewValue()
    {
        switch (m_maBranch)
        {
            case MABRANCH.MOVE:
                if (!m_isLocalPos)
                    m_moveGo.transform.position = MoveToNew();
                else
                    m_moveGo.transform.localPosition = MoveToNew();
                break;
            case MABRANCH.ROTATE:
                if (!m_isLocalEuler)
                    m_moveGo.transform.eulerAngles = RotateToNew();
                else
                    m_moveGo.transform.localEulerAngles = RotateToNew();
                break;
            case MABRANCH.ROTATEQT:
                if (!m_isLocalQt)
                    m_moveGo.transform.rotation = RotateQtToNew();
                else
                    m_moveGo.transform.localRotation = RotateQtToNew();
                break;
            case MABRANCH.ROTATEBYAXIS:
                if (!m_isLocalAround)
                    m_moveGo.transform.Rotate(m_axis, RotateAngle(), Space.World);
                else
                    m_moveGo.transform.Rotate(m_axis, RotateAngle(), Space.Self);
                break;
            case MABRANCH.SCALE:
                m_moveGo.transform.localScale = ScaleToNew();
                break;
            case MABRANCH.TRANSPARENT:
                if (m_nowMaterialArray != null)
                {
                    for (int i = 0; i < m_nowMaterialArray.Length; i++)
                    {
                        if(m_nowMaterialArray[i] != null)
                            m_nowMaterialArray[i].color = Transparent(i);
                    }
                }
                if (m_widgetArray != null)
                {
                    for (int i = 0; i < m_widgetArray.Length; i++)
                    {
                        if (m_widgetArray[i] != null)
                            m_widgetArray[i].color = Transparent2D(i);
                    }
                }
                break;
            case MABRANCH.COLOR:
                if (m_material != null)
                    m_material.color = ColorToNew();
                if (m_widget != null)
                    m_widget.color = ColorToNew();
                break;
        }
    }

    public void StopMotion(MotionStatusPositon motionStatusPositon)
    {
        //通过改变时间来用于计算
        if (motionStatusPositon == MotionStatusPositon.Start)
            m_startTime = Time.time + m_totalTime;
        else
            m_startTime = Time.time;

        ComputeAccRatio();
        NewValue();

        //时间置位
        m_startTime = -1;

        if (PlayEndCallback != null)
            PlayEndCallback();
        if (PlayEndGOSignCallback != null)
            PlayEndGOSignCallback(this.gameObject, m_sign);

        if (m_maBranch != MABRANCH.TRANSPARENT)
        {
            Reset();
            Destroy(this);
        }
    }

    public void RecoverMaterial()
    {
        switch (m_maBranch)
        {
            case MABRANCH.TRANSPARENT:
                {
                    //还原
                    if (m_isRecoverMaterial)
                    {
                        if (m_nowMaterialArray != null)
                        {
                            for (int i = 0; i < m_nowMaterialArray.Length; i++)
                            {
                                if (m_nowMaterialArray[i] != null)
                                {
                                    m_nowMaterialArray[i].shader = m_originalMaterialArray[i].shader;
                                    m_nowMaterialArray[i].color = m_originalMaterialArray[i].color;
                                }
                            }
                        }

                        if (m_widgetArray != null)
                        {
                            for (int i = 0; i < m_widgetArray.Length; i++)
                            {
                                if (m_widgetArray[i] != null)
                                    m_widgetArray[i].color = m_originalColor2D[i];
                            }
                        }
                    }
                }
                break;
        }
    }

    public void DestroyMA()
    {
        switch (m_maBranch)
        {
            case MABRANCH.TRANSPARENT:
                {
                    //还原
                    if (m_isRecoverMaterial)
                    {
                        if (m_nowMaterialArray != null)
                        {
                            for (int i = 0; i < m_nowMaterialArray.Length; i++)
                            {
                                if (m_nowMaterialArray[i] != null)
                                {
                                    m_nowMaterialArray[i].shader = m_originalMaterialArray[i].shader;
                                    m_nowMaterialArray[i].color = m_originalMaterialArray[i].color;
                                }
                            }
                        }

                        if (m_widgetArray != null)
                        {
                            for (int i = 0; i < m_widgetArray.Length; i++)
                            {
                                if(m_widgetArray[i] != null)
                                    m_widgetArray[i].color = m_originalColor2D[i];
                            }
                        }
                    }
                }

                DestroyImmediate(this);
                break;
            default:
                Destroy(this);
                break;
        }
    }

    void Reset()
    {
        m_isLocalPos = false;
        m_isLocalEuler = false;
        m_material = null;
        m_widget = null;

        m_nowMaterialArray = null;
        m_originalMaterialArray = null;
        m_colorNoAlphaArray = null;

        m_widgetArray = null;
        m_originalColor2D = null;
        m_fromA = 1;
        m_toA = 1;
        m_isRecoverMaterial = false;
        m_animCurve = null;
        m_totalLoop = 0; 
        m_loop = 0;
        m_sign = "";
    }

    //获取透明脚本或者添加脚本
    static MotionAestheticsScrp GetTransparentScrp(GameObject go)
    {
        MotionAestheticsScrp ma = GetBranchScrp(go, MABRANCH.TRANSPARENT);
        if (ma != null)
            return ma;
        else
            return go.AddComponent<MotionAestheticsScrp>();
    }

    //获取第一个相应分支的脚本
    public static MotionAestheticsScrp GetBranchScrp(GameObject go, MABRANCH maBranch)
    {
        MotionAestheticsScrp[] mas = go.GetComponents<MotionAestheticsScrp>();
        foreach (MotionAestheticsScrp ma in mas)
        {
            if (ma.m_maBranch == maBranch)
                return ma;
        }

        return null;
    }

    public static MotionAestheticsScrp[] GetBranchArrayScrp(GameObject go, MABRANCH maBranch)
    {
        List<MotionAestheticsScrp> maList = new List<MotionAestheticsScrp>();
        MotionAestheticsScrp[] mas = go.GetComponents<MotionAestheticsScrp>();
        foreach (MotionAestheticsScrp ma in mas)
        {
            if (ma.m_maBranch == maBranch)
                maList.Add(ma);
        }

        return maList.ToArray();
    }

    //欧拉角优化 避免旋转圈数过多 起始角度 末尾角度
    public static void EulerAnglesOptimize(ref Vector3 from, ref Vector3 to)
    {
        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(from[i] - to[i]) > 180)//如果差值大于180则转换角度
            {
                if (from[i] > 180)//如果大于180度则转换为负数
                    from[i] = from[i] - 360;
                if (to[i] > 180)//如果大于180度则转换为负数
                    to[i] = to[i] - 360;
            }
        }
    }
}

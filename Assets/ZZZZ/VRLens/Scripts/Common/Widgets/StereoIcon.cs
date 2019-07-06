/*
 * Author:李传礼
 * DateTime:2017.11.16
 * Description:多层次立体图标
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class StereoIcon : MonoBehaviour
{
    //图标基本信息
    public Text IconNameText;//图标名称
    public bool IsLookAtCamera = false;

    [HideInInspector]
    public bool IsInit = false;

    RectTransform RectTrans;
    List<RawImage> OriginalImageList;//插入图片前原有自身图片
    RawImage[] Images;//图片组
    List<Vector3> ImageLocalPosList;

    Quaternion LookRotateFrom;
    Quaternion LookRotateTo;
    Vector2 IconSize;

    float Space;//dmm
    float StereoIconSpendTime;
    bool IsOpenEffect;//是否开启动效
    bool IsIconRock;//是否晃动
    Quaternion OriginalLocalRotate;
    float IconRockfrequencies;//晃动频率
    float IconRockLimitAngle;//限制角度
    float CenterToUpAngle;//中间到上边角度

    public Action SetIconCallback;

    void Start ()
    {
        if (!IsInit)
            InitWidget();
    }
    
    void InitWidget()
    {
        IsInit = true;

        RectTrans = GetComponent<RectTransform>();

        OriginalImageList = new List<RawImage>();
        for(int i = 0; i < this.transform.childCount; i++)
        {
            RawImage image = this.transform.GetChild(i).GetComponent<RawImage>();
            if (image != null)
                OriginalImageList.Add(image);
        }

        Images = OriginalImageList.ToArray();

        ImageLocalPosList = new List<Vector3>();
        if (Images != null)
        {
            for (int i = 0; i < Images.Length; i++)
            {
                ImageLocalPosList.Add(Images[i].transform.localPosition);
            }
        }

        LookRotateFrom = Quaternion.identity;
        LookRotateTo = Quaternion.identity;
        IconSize = new Vector2(RectTrans.rect.width, RectTrans.rect.height);

        Space = 20;
        StereoIconSpendTime = 0.3f;
        IsOpenEffect = false;
        IsIconRock = true;
        OriginalLocalRotate = Quaternion.identity;
        IconRockfrequencies = 1.0f / 30;
        IconRockLimitAngle = 3;
        Vector3 zeroPoint = new Vector3(this.transform.position.x, this.transform.position.y, Camera.main.transform.position.z);//面板正后方摄像机位置
        CenterToUpAngle = Vector3.Angle(this.transform.position - zeroPoint, this.transform.TransformPoint(new Vector3(0, RectTrans.rect.height / 2, 0)) - zeroPoint);

        EventTriggerListener.Get(this.gameObject).OnPtEnter += Hover;
        EventTriggerListener.Get(this.gameObject).OnPtExit += Exit;
    }

    protected void SetIconName(string iconName)
    {
        if (iconName != "")
        {
            if (IconNameText != null)
                IconNameText.text = iconName;
        }
    }

    public void Init(string iconName, Texture[] textures)
    {
        if (!IsInit)
            InitWidget();

        if (IconNameText != null)
            IconNameText.text = iconName;

        SetIcon(textures);
    }

    public void Init(string iconName, Texture[] textures, float width, float height)
    {
        if (!IsInit)
            InitWidget();

        if (IconNameText != null)
            IconNameText.text = iconName;

        IconSize = new Vector2(width, height);
        PreDefScrp.SetWidgetSize(RectTrans, IconSize);
        SetIcon(textures);
    }

    public void IconRockSwitch(bool isOn)
    {
        IsIconRock = isOn;
    }

    void SetIcon(Texture[] textures)
    {
        if (textures == null)//使用默认图片
            return;

        int count = textures.Length;
        RawImage[] tempImages = PreDefScrp.NewComponents<RawImage>(Images.Length + count);
        Images.CopyTo(tempImages, 0);

        for (int i = 0; i < count; i++)
        {
            int index = i + Images.Length;

            tempImages[index].texture = textures[i];
            tempImages[index].name = "Pic" + i;
            tempImages[index].transform.SetParent(transform);
            tempImages[index].transform.localPosition = Vector3.zero;
            tempImages[index].transform.localEulerAngles = Vector3.zero;
            tempImages[index].transform.localScale = Vector3.one;
            PreDefScrp.AdaptWidgetSize(tempImages[index].rectTransform, IconSize);

            tempImages[index].raycastTarget = false;
        }

        Images = tempImages;
        if (Images != null)
        {
            ImageLocalPosList.Clear();
            for (int i = 0; i < Images.Length; i++)
            {
                ImageLocalPosList.Add(Images[i].transform.localPosition);
            }
        }

        if (IconNameText != null)
            IconNameText.transform.SetAsLastSibling();

        if (SetIconCallback != null)
            SetIconCallback();
    }

    void Hover(GameObject go)
    {
        EffectSwitch(true);
    }

    void Exit(GameObject go)
    {
        EffectSwitch(false);
    }

    void EffectSwitch(bool isOn)
    {
        if (isOn == IsOpenEffect)
            return;
        IsOpenEffect = isOn;

        if (isOn)
        {
            if (IsIconRock && !IsInvoking("IconRock"))
            {
                OriginalLocalRotate = this.transform.localRotation;
                InvokeRepeating("IconRock", 0, IconRockfrequencies);
            }
            PicsSeparate();

            if (IsLookAtCamera)
                TowardsCamera();
        }
        else
        {
            if (IsInvoking("IconRock"))
                CancelInvoke("IconRock");
            PicsTogether();

            if (IsLookAtCamera)
                AwayCamera();
        }
    }

    //环抱
    public void Encircle(Vector3 pos, Vector3 forward)
    {
        this.transform.position = pos;
        this.transform.forward = forward;
    }

    void ResetMotionStatus()
    {
        Vector3 v = (this.transform.position - Camera.main.transform.position).normalized;
        LookRotateFrom = this.transform.localRotation;
        LookRotateTo = Quaternion.Inverse(this.transform.rotation) * Quaternion.LookRotation(v);
    }

    void TowardsCamera()
    {
        ResetMotionStatus();
        this.gameObject.transform.DOLocalRotateQuaternion(LookRotateTo, 0.1f);
    }

    void AwayCamera()
    {
        this.gameObject.transform.DOLocalRotateQuaternion(LookRotateFrom, 0.1f);
    }

    void PicsSeparate()
    {
        if (Images == null)
            return;

        for (int i = 0; i < Images.Length; i++)
        {
            Vector3 startPos = ImageLocalPosList[i];
            Images[i].gameObject.transform.DOLocalMove(startPos + Vector3.back * Space * i, StereoIconSpendTime);

            float scale = 1 - 2 * Space * i * Mathf.Tan(Mathf.Deg2Rad * (CenterToUpAngle)) / RectTrans.rect.height;
            Images[i].gameObject.transform.DOScale(Vector3.one * scale, StereoIconSpendTime);
        }
    }

    void PicsTogether()
    {
        if (Images == null)
            return;

        for (int i = 0; i < Images.Length; i++)
        {
            Vector3 startPos = ImageLocalPosList[i];
            Images[i].gameObject.transform.DOLocalMove(startPos, StereoIconSpendTime);

            Images[i].gameObject.transform.DOScale(Vector3.one, StereoIconSpendTime);
        }

        if (IsIconRock)
            this.gameObject.transform.DOLocalRotateQuaternion(OriginalLocalRotate, StereoIconSpendTime);
    }

    //图标晃动
    void IconRock()
    {
        Vector3 pos = Vector3.zero;
#if UNITY_STANDALONE || UNITY_EDITOR
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z);
        pos = Camera.main.ScreenToWorldPoint(screenPos);
#elif UNITY_ANDROID
#if UD_I3VR
        pos = I3vrPointerManager.Pointer.CurrentRaycastResult.worldPosition;
#elif UD_GVR
        //pos = VRInputMessageEngine.GetInstance().GvrPtInputModule.pointerData.pointerCurrentRaycast.worldPosition;
        pos = GvrPointerInputModule.CurrentRaycastResult.worldPosition;//by kevin.xie
#elif UD_XIMMERSE
        pos = VRInputMessageEngine.GetInstance().XimVRInputModule.subInputModules[0].pointerData.pointerCurrentRaycast.worldPosition;
#endif
#endif
        Vector3 localPos = this.transform.InverseTransformPoint(pos);
        Vector3 v = localPos.normalized;

        float x = v.y * -IconRockLimitAngle;
        float y = v.x * IconRockLimitAngle;
        x = Mathf.Clamp(x, -IconRockLimitAngle, IconRockLimitAngle);
        y = Mathf.Clamp(y, -IconRockLimitAngle, IconRockLimitAngle);
        this.transform.localEulerAngles = new Vector3(x, y, 0);//目前采用鼠标实现
    }

    //插入新图片后，恢复原始图片
    public void RecoverOriginalImages()
    {
        List<RawImage> allImageList = new List<RawImage>();
        allImageList.AddRange(Images);

        List<RawImage> deleteList, addList;
        PreDefScrp.ComputeDeleteAddList(OriginalImageList, allImageList, out deleteList, out addList);

        foreach(RawImage image in addList)
        {
            DestroyObject(image.gameObject);
        }

        Images = OriginalImageList.ToArray();

        ImageLocalPosList.Clear();
        if (Images != null)
        {
            for (int i = 0; i < Images.Length; i++)
            {
                ImageLocalPosList.Add(Images[i].transform.localPosition);
            }
        }

        if (IconNameText != null)
            IconNameText.transform.SetAsLastSibling();

        if (SetIconCallback != null)
            SetIconCallback();
    }
}

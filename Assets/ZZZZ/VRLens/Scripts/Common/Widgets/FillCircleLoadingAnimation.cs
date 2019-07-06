/*
 * Author:李传礼
 * DateTime:2018.02.12
 * Description:填充圆圈式加载动画
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FillCircleLoadingAnimation : MonoBehaviour
{
    void Start ()
    {
        Half();
    }

    void Go()
    {
        this.gameObject.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.01f).OnComplete(() => Half());
    }

    void Half()
    {
        this.gameObject.transform.DOLocalRotate(new Vector3(0, 0, 180), 1).OnComplete(() => End());
    }

    void End()
    {
        this.gameObject.transform.DOLocalRotate(new Vector3(0, 0, 360), 1).OnComplete(() => Go());
    }
}

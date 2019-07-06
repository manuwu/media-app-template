/*
 * Author:李传礼
 * DateTime:2018.03.09
 * Description:全局弹框遮罩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopDialogMask : SingletonMB<PopDialogMask>
{
    public float CubeSideLength;
    bool IsShow;

	void Start ()
    {
        IsShow = false;

        RawImage[] planes = this.GetComponentsInChildren<RawImage>();
        foreach(RawImage plane in planes)
        {
            plane.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CubeSideLength);
            plane.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CubeSideLength);

            switch (plane.name)
            {
                case "Forward":
                    plane.rectTransform.localPosition = Vector3.forward * CubeSideLength / 2;
                    break;
                case "Back":
                    plane.rectTransform.localPosition = Vector3.back * CubeSideLength / 2;
                    break;
                case "Left":
                    plane.rectTransform.localPosition = Vector3.left * CubeSideLength / 2;
                    break;
                case "Right":
                    plane.rectTransform.localPosition = Vector3.right * CubeSideLength / 2;
                    break;
                case "Up":
                    plane.rectTransform.localPosition = Vector3.up * CubeSideLength / 2;
                    break;
                case "Down":
                    plane.rectTransform.localPosition = Vector3.down * CubeSideLength / 2;
                    break;
            }
        }
    }

    public void Show()
    {
        if (IsShow)
            return;

        IsShow = true;
        this.transform.localScale = Vector3.one;
    }

    public void Hide()
    {
        if (!IsShow)
            return;

        IsShow = false;
        this.transform.localScale = Vector3.zero;
    }
}

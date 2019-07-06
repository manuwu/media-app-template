using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 切换分辨率提示，播放错误相关。包括反馈操作。(事件消失)
/// 位置处于播放器之前，UI面板之后
/// 区分全景/非全景
/// 非全景：固定到播放器窗口前
/// 全景：固定在出现提示的瞬间。
/// </summary>
public class CinemaTipsCanvasControl : SingletonMB<CinemaTipsCanvasControl>
{
    public GlobalToast GlobalToast;

    public void CinemaCanvasNormalTrans()
    {
        transform.parent = Camera.main.transform;
        transform.localPosition = new Vector3(0, 0, 8);
        transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);
        transform.localRotation = Quaternion.identity;
    }
}

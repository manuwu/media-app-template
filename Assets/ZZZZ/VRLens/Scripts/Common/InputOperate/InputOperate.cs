/*
 * Author:李传礼
 * DateTime:2017.12.30
 * Description:输入控制
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputOperate : VRInputMessageBase
{
    public Action BackHomeCallback;
    public Action<Vector3> ResetUIDirCallback;
    public Action<Vector3> RecenteredCallback;

	void Start ()
    {

    }
    
    public override void AppButtonPress(bool isDown)
    {

    }

    public override void HomeButtonUp()
    {
        if (BackHomeCallback != null)
            BackHomeCallback();
    }

    public override void Recentered(Vector3 dir)
    {
        if (IsInvoking("RecenterCallback"))
            CancelInvoke("RecenterCallback");
        Invoke("RecenterCallback", 0.05f);
    }

    private void RecenterCallback()
    {
        if (RecenteredCallback != null)
            RecenteredCallback(Camera.main.transform.forward);
    }

    public override void TouchPadPressDir(Vector3 dir, bool isDown)
    {
        if (!isDown)
        {
            if (ResetUIDirCallback != null)
                ResetUIDirCallback(dir);
        }
    }

    public override void TriggerPressDir(Vector3 dir, bool isDown)
    {
        if (!isDown)
        {
            if (ResetUIDirCallback != null)
                ResetUIDirCallback(dir);
        }
    }
}

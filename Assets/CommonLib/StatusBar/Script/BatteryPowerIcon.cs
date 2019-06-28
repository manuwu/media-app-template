using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryPowerIcon : MonoBehaviour
{
    //public Sprite[] NoChargingBPs;
    //public Sprite[] ChargingBPs;
    //Sprite[] CurUsingBPs;
    private BatteryStatus CurBatteryStatus;
    //BatteryPower CurBatteryPower;
    //int ImageIndex;//动画使用
    [SerializeField]
    private Sprite m_BatteryCharge;
    [SerializeField]
    private Sprite m_BatteryNormal;
    [SerializeField]
    private Sprite m_BatteryLow;
    [SerializeField]
    private Sprite m_BatteryVeryLow;

    [SerializeField]
    private Image m_BaterryTarget;
    [SerializeField]
    private GameObject BatteryChargeLightning;

    void Start ()
    {
        //CurUsingBPs = NoChargingBPs;
        //CurBatteryPower = BatteryPower.One;
        //ImageIndex = (int)CurBatteryPower - 1;

    }
	
	void Update ()
    {

    }

    public void SetBatteryStatus(BatteryStatus batteryStatus)
    {

        Debug.Log("SetBatteryStatus:"+ batteryStatus);
        CurBatteryStatus = batteryStatus;
        if (batteryStatus == BatteryStatus.NoCharging)
        {
            //m_BaterryTarget.sprite = m_BatteryNormal;
            SetBatteryPower(SvrNativeInterface.GetInstance().BatteryUtils.GetCurrentBattery());
            BatteryChargeLightning.SetActive(false);
        }
        else
        {
            m_BaterryTarget.sprite = m_BatteryCharge;
            BatteryChargeLightning.SetActive(true);
        }
        //if (batteryStatus == BatteryStatus.NoCharging)
        //{
        //    ChargingAnimationSwitch(false);//关闭动画

        //    CurUsingBPs = NoChargingBPs;
        //    SetBatteryPower(CurBatteryPower);
        //}
        //else
        //{
        //    CurUsingBPs = ChargingBPs;
        //    ChargingAnimationSwitch(true);
        //}

    }

    public void SetBatteryPower(int percentNum)
    {
        //CurBatteryPower ;
        m_BaterryTarget.fillAmount = percentNum * 0.01f;
        if (CurBatteryStatus == BatteryStatus.NoCharging)
        {
            //if (Image == null || CurUsingBPs == null)
            //    return;

            //int index = (int)batteryPower - 1;
            //if (index <= CurUsingBPs.Length - 1)
            //    Image.sprite = CurUsingBPs[index];

            if (percentNum < 10)
            {
                m_BaterryTarget.sprite = m_BatteryVeryLow;
            }
            else if (percentNum < 20)
            {
                m_BaterryTarget.sprite = m_BatteryLow;
            }
            else
            {
                m_BaterryTarget.sprite = m_BatteryNormal;
            }
        }

    }

    //void ChargingAnimationSwitch(bool isOpen)
    //{
    //    if (isOpen)
    //    {
    //        ImageIndex = (int)CurBatteryPower - 1;
    //        if (!IsInvoking("ChargingAnimation"))
    //            InvokeRepeating("ChargingAnimation", 0, 1);
    //    }
    //    else
    //    {
    //        if (IsInvoking("ChargingAnimation"))
    //            CancelInvoke("ChargingAnimation");
    //    }
    //}
    
    //void ChargingAnimation()
    //{
    //    if (CurBatteryPower != BatteryPower.Five)//未满格
    //    {
    //        if (ImageIndex == CurUsingBPs.Length)
    //            ImageIndex = (int)CurBatteryPower - 1;

    //        Image.sprite = CurUsingBPs[ImageIndex];

    //        ImageIndex++;
    //    }
    //}
}

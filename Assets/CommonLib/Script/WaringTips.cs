using IVR.Language;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaringTips : MonoBehaviour {

    private Text mText;
    private readonly string TIPS_CHINESE = "你已经出了安全区域，请返回到安全区域内";
    private readonly string TIPS_EGNISH = "You are out of the safe zone.For your safty, please keep staying in it";
    private bool ISstarted = false;
    // Use this for initialization
    void Start () {
        mText = GetComponent<Text>();
        SvrBoundaryRegionManager.OnTrackingRangeEvent += SvrBoundaryRegionManager_OnTrackingRangeEvent;
        SvrBoundaryRegionManager_OnTrackingRangeEvent(SvrBoundaryRegionManager.RangeState);

        SetTips();
        ISstarted = true;
    }

    private void SetTips()
    {
        switch (SystemCurrentLanguage.CurrentLanguage)
        {
            case SystemLanguage.Chinese:
                mText.text = TIPS_CHINESE;
                break;
            default:
                mText.text = TIPS_EGNISH;
                break;
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (!pause && ISstarted)
        {
            SetTips();
        }
    }

    private void SvrBoundaryRegionManager_OnTrackingRangeEvent(SvrBoundaryRegionManager.EventsType obj)
    {
        switch (obj)
        {
            case SvrBoundaryRegionManager.EventsType.TrackingOutofRange:
                gameObject.SetActive(true);
                break;
            case SvrBoundaryRegionManager.EventsType.TrackingInRange:
                gameObject.SetActive(false);
                break;

            default:
                break;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}

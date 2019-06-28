using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class SvrBoundaryRegionManager : MonoBehaviour {
    public enum EventsType
    {
        TrackingOutofRange,
        TrackingInRange,
        TurnAround
    }
   
    public float m_safety_radius = 1.0f;
    public GameObject m_Target;
    public GameObject m_Region;
    public static event Action<EventsType> OnTrackingRangeEvent;
    public static EventsType RangeState { get; private set; }
    private bool IsOutOfRange;
    private bool PreOutofRange;
    private void Awake()
    {
        RangeState = EventsType.TrackingInRange;
    }
    // Use this for initialization
    void Start () {
        if (m_Target == null) m_Target = Camera.main.gameObject;

    }
	
	// Update is called once per frame
	void Update () {
        if (CheckHorizontal(m_Target))
        {
            IsOutOfRange = true;
            if (PreOutofRange != IsOutOfRange)
            {
                if (OnTrackingRangeEvent != null) OnTrackingRangeEvent.Invoke(EventsType.TrackingOutofRange);
                m_Region.SetActive(true);
                PreOutofRange = IsOutOfRange;
                RangeState = EventsType.TrackingOutofRange;
            }
        }
        else
        {
            IsOutOfRange = false;
            if (PreOutofRange != IsOutOfRange)
            {
                if (OnTrackingRangeEvent != null) OnTrackingRangeEvent.Invoke(EventsType.TrackingInRange);
                m_Region.SetActive(false);
                PreOutofRange = IsOutOfRange;
                RangeState = EventsType.TrackingInRange;
            }


        }
        

    }

    private bool CheckHorizontal(GameObject target)
    {

        Vector3 positiveCenterPos = Vector3.zero;
        positiveCenterPos.y = target.transform.localPosition.y;

        float distance = Vector3.Distance(target.transform.localPosition, positiveCenterPos);

        return distance > (m_safety_radius - 0.2f);
    }
}

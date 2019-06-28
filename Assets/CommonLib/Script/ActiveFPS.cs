using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveFPS : MonoBehaviour {

    private readonly string DEBUG_KEY = "debug.unity.fps";
    [SerializeField]
    private GameObject m_FPS;
	// Use this for initialization
	void Start () {
#if UNITY_ANDROID && !UNITY_EDITOR
        string debug_enable =  SystemProperties.get(DEBUG_KEY, "0");
        m_FPS.SetActive(debug_enable == "1");
#endif
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnApplicationPause(bool pause)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!pause)
        {
            string debug_enable =  SystemProperties.get(DEBUG_KEY, "0");
            m_FPS.SetActive(debug_enable == "1");
        }
#endif
    }
}

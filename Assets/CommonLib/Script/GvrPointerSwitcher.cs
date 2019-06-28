using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GvrPointerSwitcher : MonoBehaviour {
    [SerializeField]
    private GameObject reticlePointer;
    [SerializeField]
    private GameObject controllerMain;
    [SerializeField]
    private GameObject controllerPointer;
    void Awake()
    {
        GvrControllerInput.OnStateChanged += OnControllerStateChanged;
#if SVR_USE_GAZE
        reticlePointer.SetActive(true);
#else
        reticlePointer.SetActive(false);
#endif
    }

    private void OnControllerStateChanged(GvrConnectionState state, GvrConnectionState oldState)
    {
        bool controllerconnect = state == GvrConnectionState.Connected;
#if SVR_USE_GAZE
        SetGazeInputActive(!controllerconnect);
        SetControllerInputActive(controllerconnect);
#endif
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnDestroy()
    {
        GvrControllerInput.OnStateChanged -= OnControllerStateChanged;
    }
    private void SetGazeInputActive(bool active)
    {
        if (reticlePointer == null)
        {
            return;
        }
        reticlePointer.SetActive(active);

        // Update the pointer type only if this is currently activated.
        if (!active)
        {
            return;
        }

        GvrReticlePointer pointer =
            reticlePointer.GetComponent<GvrReticlePointer>();
        if (pointer != null)
        {
            GvrPointerInputModule.Pointer = pointer;
        }
    }

    private void SetControllerInputActive(bool active)
    {
        if (controllerMain != null)
        {
            controllerMain.SetActive(active);
        }
        if (controllerPointer == null)
        {
            return;
        }
        controllerPointer.SetActive(active);

        // Update the pointer type only if this is currently activated.
        if (!active)
        {
            return;
        }
        GvrLaserPointer pointer =
            controllerPointer.GetComponentInChildren<GvrLaserPointer>(true);
        if (pointer != null)
        {
            GvrPointerInputModule.Pointer = pointer;
        }
    }
}

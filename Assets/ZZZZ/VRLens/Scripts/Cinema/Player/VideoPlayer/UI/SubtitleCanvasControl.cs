using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleCanvasControl : MonoBehaviour
{
    public Text SubtitleFirstText;
    public Text SubtitleSecondText;

    Transform CameraTrans;
    Vector3 OldUIDir;
    Vector3 CameraToUIVector;

    void Start ()
    {
        CameraTrans = Camera.main.transform.parent;
        OldUIDir = Vector3.forward;
        CameraToUIVector = this.transform.position - Camera.main.transform.position;
    }

    void FixedUpdate()
    {
		if (this.transform.parent == CameraTrans)
        {
            ResetUIDir();
        }
	}

    void ResetUIDir()
    {
        Vector3 dir = Camera.main.transform.forward;
        Vector3 yDir = PreDefScrp.ComputeProjection(Vector3.up, dir);
        float angle = Vector3.Angle(OldUIDir, yDir);
        if (angle > 2)
        {
            Vector3 valueDir = Vector3.Cross(Vector3.forward, yDir);
            float valueAngle = Vector3.Angle(Vector3.forward, yDir);

            Vector3 cToUVector = PreDefScrp.RotateAroundAxis(CameraToUIVector, valueDir, valueAngle);
            this.transform.forward = yDir;
            this.transform.position = Camera.main.transform.position + cToUVector;
            OldUIDir = yDir;
        }
    }
}

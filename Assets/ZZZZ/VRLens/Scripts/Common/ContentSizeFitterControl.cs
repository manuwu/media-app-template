using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentSizeFitterControl : MonoBehaviour {
    Text text;
    ContentSizeFitter[] ContentSizeFitters;
    string textString;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        ContentSizeFitters = GetComponentsInParent<ContentSizeFitter>();
        if(text != null)
            textString = text.text;
    }
	
	// Update is called once per frame
	void Update () {
		if (text != null && ContentSizeFitters != null && textString != text.text)
        {
            textString = text.text;
            foreach (ContentSizeFitter con in ContentSizeFitters)
            {
                con.SetLayoutHorizontal();
            }
        }
    }
}

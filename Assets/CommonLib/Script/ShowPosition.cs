using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPosition : MonoBehaviour {


    private Text mText;
    private void Awake()
    {
        mText = GetComponent<Text>();
    }
    // Use this for initialization
    void Start () {
       

    }
	
	// Update is called once per frame
	void Update () {
        mText.text = Camera.main.transform.localPosition.ToString("F4");

    }
}

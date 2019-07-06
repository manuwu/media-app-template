using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MixedButtonHaveLine : MixedButton
{
    [SerializeField]
    private Image Line;

    private float TextLenth;
    private RectTransform LineTrans;
    float LineHeight = 4.0f;
    float AnimationSpendTime = 0.3f;
    int AnimationFrames = 30;//多少帧数
    float Step = 0;//步长
    float UpdateRate;//刷新频率

    private void Start()
    {
        Text text = this.gameObject.GetComponentInChildren<Text>();
        TextLenth = GlobalVariable.CalculateLengthOfText(text.text, text);
        LineTrans = Line.gameObject.GetComponent<RectTransform>();
        UpdateRate = 1.0f / AnimationFrames;

        EventTriggerListener.Get(this.gameObject).OnPtEnter += Hover;
        EventTriggerListener.Get(this.gameObject).OnPtExit += Exit;

        Line.gameObject.SetActive(false);
    }


    


    protected new void Hover(GameObject go)
    {
        base.Hover(go);

        LineTrans.sizeDelta = new Vector2(0, LineHeight);
        Line.gameObject.SetActive(true);

        if (IsInvoking("LoadingMotion"))
            CancelInvoke("LoadingMotion");

        InvokeRepeating("LoadingMotion", 0, UpdateRate);
    }

    protected new void Exit(GameObject go)
    {
        base.Exit(go);

        if (IsInvoking("LoadingMotion"))
            CancelInvoke("LoadingMotion");
        LineTrans.sizeDelta = new Vector2(0, LineHeight);
        Line.gameObject.SetActive(false);
    }

    void LoadingMotion()
    {
        if (Step < TextLenth)
        {
            Step += (TextLenth / AnimationSpendTime) * UpdateRate;
            LineTrans.sizeDelta = new Vector2(Step, LineHeight);
        }
        else
        {
            Step = 0;
            LineTrans.sizeDelta = new Vector2(TextLenth, LineHeight);
            CancelInvoke("LoadingMotion");
        }
    }
}

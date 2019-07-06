using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ScoreType
{
    PercentileSystem, //百分制
    FiveGrade, //五分制
    Percentage //百分比
}

public class ScoreObjCotrol : SingletonMB<ScoreObjCotrol>
{
    private ImageColoringManager[] ScoreImage;
    private int ScoreNum; //评分分数
    private int ImageCount; //分数图片个数

    bool IsInit = false;

    private void Start()
    {
        if (!IsInit)
            InitWidget();
    }

    public void InitWidget()
    {
        IsInit = true;

        ScoreImage = GetComponentsInChildren<ImageColoringManager>();
        ScoreNum = 0;
        if(ScoreImage != null)
            ImageCount = ScoreImage.Length;
    }

    public void SetScore(float score, ScoreType type = ScoreType.FiveGrade)
    {
        if (!IsInit)
            InitWidget();
        if (ScoreImage == null) return;

        float x = 0;
        switch(type)
        {
            case ScoreType.PercentileSystem:
                x = 20.0f;
                break;
            case ScoreType.Percentage:
                x = 0.2f;
                break;
            case ScoreType.FiveGrade:
                x = 1;
                break;
            default:
                x = 20.0f;
                break;
        }
        float lerp = 0;
        if (x <= 0)
            lerp = 0;
        else
            lerp = score / x;
        ScoreNum = Mathf.RoundToInt(lerp);
        for (int i = 0; i < ScoreNum; i++)
            ScoreImage[i].OnSelectedStatus();

        for (int i = ScoreNum; i < ImageCount; i ++)
            ScoreImage[i].OnNormalStatus();
    }
}

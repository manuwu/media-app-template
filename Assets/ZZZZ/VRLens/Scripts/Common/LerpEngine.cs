///*
// * Author:李传礼
// * DateTime:2018.04.18
// * Description:LerpEngine
// */
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;

//public class LerpParam
//{
//    public AnimationCurve AnimCurve;
//    public T From;
//    public T To;
//    public T Curent;
//    public float SpendTime;
//    float StartTime;
//}


//public class LerpEngine<T> : MonoBehaviour  
//{


//    void Update()
//    {
//        if (StartTime != -1)
//        {
//            if (StartTime - Time.time >= 0)
//            {
//                float t = GetFactor();

//                if (typeof(T) == typeof(float))
//                {
//                    float result = Mathf.Lerp(Convert.ToSingle(From), Convert.ToSingle(To), t);
//                    Curent = (T)Convert.ChangeType(result, typeof(T));
//                }
//                else if (typeof(T) == typeof(Vector3))
//                {
//                    Vector3 result = Vector3.Lerp((Vector3)Convert.ChangeType(From, typeof(Vector3)), (Vector3)Convert.ChangeType(To, typeof(Vector3)), t);
//                    Curent = (T)Convert.ChangeType(result, typeof(T));
//                }
//            }
//            else
//                StartTime = -1;
//        }
//    }

//    public void Begin()
//    {
//        StartTime = Time.time + SpendTime;
//    }

//    float GetFactor()
//    {
//        float t = (SpendTime - (StartTime - Time.time)) / SpendTime;//线型t
//        t = PreDefScrp.GetCurveT(AnimCurve, t);//曲线t
//        return t;
//    }

//    IEnumerator Loop()
//    {

//    }
//}
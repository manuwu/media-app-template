/*
 * Author:李传礼
 * DateTime:2017.12.26
 * Description:主线程队列（主要用于解决Native回调结果并非在主线程执行会引起的问题）
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainThreadQueue : MonoBehaviour
{
    public static Queue<Action> ExecuteQueue = new Queue<Action>();
    
	void Start ()
    {

    }

	void Update ()
    {
		while(ExecuteQueue.Count > 0)
        {
            Debug.Log("主线程队列执行");
            ExecuteQueue.Dequeue().Invoke();
        }
	}
}

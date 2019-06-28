/*
 * Author:李传礼
 * DateTime:2017.5.4
 * Description:单例类
 */
using UnityEngine;

public class SingletonPure<T> where T : new()
{
    private static T Instance;

    public static T GetInstance()
    {
        if (Instance == null)
            Instance = new T();
        return Instance;
    }
}
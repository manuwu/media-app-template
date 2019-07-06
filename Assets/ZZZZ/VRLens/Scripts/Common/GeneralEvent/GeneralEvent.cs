/*
 * Author:李传礼
 * DateTime:2017.08.28
 * Description:通用消息 封装EventTrigger使用
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class GeneralEvent : EventTrigger
{

    public void AddListener(EventTriggerType eventID, UnityAction<BaseEventData> callback)
    {
        Entry entry = new Entry();
        entry.eventID = eventID;
        entry.callback.AddListener(callback);
        triggers.Add(entry);
    }

    public void RemoveListener(EventTriggerType eventID, UnityAction<BaseEventData> callback)
    {
        foreach(Entry entry in triggers)
        {
            if (entry.eventID == eventID && entry.callback.Equals(callback))
            {
                triggers.Remove(entry);
                break;
            }
        }
    }
}

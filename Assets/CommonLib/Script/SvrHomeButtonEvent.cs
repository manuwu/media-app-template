using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SvrHomeButtonEvent  : MonoBehaviour{

    private static SvrHomeButtonEvent mInstance;
    public static SvrHomeButtonEvent Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new GameObject("[SvrHomeButtonEvent]").AddComponent<SvrHomeButtonEvent>();
            }
            return mInstance;
        }
    }

    public delegate bool BoolDelegate();

    private List<BoolDelegate> mOnBackButtonClick = new List<BoolDelegate>();
    //public int mCurrentTime;

    public event BoolDelegate OnBackButtonClick
    {
        add
        {
            lock (mOnBackButtonClick)
            {
                mOnBackButtonClick.Insert(0,value);
            }
            
        }
        remove
        {
            lock (mOnBackButtonClick)
            {
                mOnBackButtonClick.Remove(value);
            }
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        //mCurrentTime = mOnBackButtonClick.Count;
        bool State = Input.GetKeyUp(KeyCode.Escape);
        if ((GvrControllerInput.SvrState & (SvrControllerState.NoloLeftContoller | SvrControllerState.NoloRightContoller)) != 0)
        {
            State = State || GvrControllerInput.AppButton;
        }
        else
        {
            State = State || GvrControllerInput.HomeButtonDown;
        }
        if (State)
        {
            foreach (var item in mOnBackButtonClick)
            {
                //Debug.Log(item.Target+","+ item.Method);
                if (item()) break;
            }
        }
    }



}

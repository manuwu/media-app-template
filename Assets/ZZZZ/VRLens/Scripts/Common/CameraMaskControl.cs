using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMaskControl : SingletonMB<CameraMaskControl>
{
    public void ShowMask()
    {
        this.gameObject.SetActive(true);
    }

    public void HideMask()
    {
        this.gameObject.SetActive(false);
    }
}

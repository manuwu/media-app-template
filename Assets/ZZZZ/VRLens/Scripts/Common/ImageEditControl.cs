using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageEditControl : MonoBehaviour
{
    public Sprite NorSprite;
    public Sprite HoverSprite;
    public Sprite SelectSprite;

    public void HoverState()
    {
        if(this.gameObject.GetComponent<Image>() != null)
            this.gameObject.GetComponent<Image>().sprite = HoverSprite;
    }

    public void SelectState()
    {
        if(this.gameObject.GetComponent<Image>() != null)
            this.gameObject.GetComponent<Image>().sprite = SelectSprite;
    }

    public void NormalState()
    {
        if(this.gameObject.GetComponent<Image>() != null)
            this.gameObject.GetComponent<Image>().sprite = NorSprite;
    }
}

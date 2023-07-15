using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Background_Controller : MonoBehaviour
{
    public virtual void Update_Background(Sprite Background)
    {
        GetComponent<Image>().sprite = Background;
    }
    private void OnEnable()
    {
        if(GameInfo.Instance!=null)
            GetComponent<Image>().sprite = GameInfo.Instance.Get_Background();
    }
}

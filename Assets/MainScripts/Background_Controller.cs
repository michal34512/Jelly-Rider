using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Background_Controller : MonoBehaviour
{
    public void Update_Background(Sprite Background)
    {
        GetComponent<Image>().sprite = Background;
    }
    private void OnEnable()
    {
        if(GameInfo.Instance!=null)
            GameInfo.Instance.Assign_Background(GetComponent<Image>());
    }
}

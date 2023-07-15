using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIScene
{
    public class Wheel_Get_Item_Controller : MonoBehaviour
    {
        public GameObject Glow;
        public TextMeshProUGUI RarityText;
        public Image JellyImage;
        private readonly float JellyImageOffset = 30f;
        public void ShowJelly(Jelly_Scriptable_Object _Jelly)
        {
            //Setting rarity text
            if(_Jelly.RarityClass == Jelly_Scriptable_Object._RarityClass.Common)
            {
                RarityText.text = "Common";
                RarityText.color = new Color(0, 0.87f, 1, 1);
            }
            else if (_Jelly.RarityClass == Jelly_Scriptable_Object._RarityClass.Rare)
            {
                RarityText.text = "Rare";
                RarityText.color = new Color(1, 0.3f, 0.31f, 1);
            }
            else if(_Jelly.RarityClass == Jelly_Scriptable_Object._RarityClass.Epic)
            {
                RarityText.text = "Epic";
                RarityText.color = new Color(0.51f, 0, 1, 1);
            }
            else if (_Jelly.RarityClass == Jelly_Scriptable_Object._RarityClass.Legendary)
            {
                RarityText.text = "Legendary";
                RarityText.color = new Color(0.97f, 1, 0.647f, 1);
            }
            //Glow animation
                LeanTween.value(gameObject, 0f, 360f, 2f).setOnUpdate((float val) =>
            {
                Glow.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, val);
            }).setLoopClamp();
            //Set Jelly image
            JellyImage.sprite = _Jelly.MainSprite;
            float PosY = JellyImage.GetComponent<RectTransform>().anchoredPosition.y;
            LeanTween.value(gameObject, PosY - JellyImageOffset, PosY + JellyImageOffset, 1f).setOnUpdate((float val) =>
            {
                JellyImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(JellyImage.GetComponent<RectTransform>().anchoredPosition.x, val);
            }).setLoopPingPong();
        }
    }
}

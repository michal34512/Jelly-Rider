using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace GameScene
{
    public class PointsCounter : MonoBehaviour
    {
        public static PointsCounter Instance;

        public GameObject _Background;
        private void BackgroundEffect()
        {
            _Background.GetComponent<GameBackground_Controller>().Update_Background_Effect();
            if (GameInfo.Instance != null) GameInfo.Instance.Next_Background();
        }
        public void AddPoint()
        {
            if(GameInfo.Instance!=null)
            {
                GameInfo.Instance.ConstantModePoints++;
                GetComponent<TextMeshProUGUI>().text = GameInfo.Instance.ConstantModePoints.ToString();
            }
            //Background Changing
            BackgroundEffect();
        }
        public void SetToZero()
        {
            if (GameInfo.Instance != null)
            {
                GameInfo.Instance.ConstantModePoints=0;
                GetComponent<TextMeshProUGUI>().text = GameInfo.Instance.ConstantModePoints.ToString();
                GameInfo.Instance.Next_Background();
            }
        }
        private void Awake()
        {
            if (Instance != null || (GameInfo.Instance != null && GameInfo.Instance.gamemode != GameInfo._GameMode.Constant))
                Destroy(gameObject);
            else 
            { 
                Instance = this;
                float ScreenRatio = Screen.height / 1080f;
                GetComponent<RectTransform>().localScale = new Vector3(ScreenRatio,ScreenRatio, GetComponent<RectTransform>().localScale.z);
            }
        }
    }
}


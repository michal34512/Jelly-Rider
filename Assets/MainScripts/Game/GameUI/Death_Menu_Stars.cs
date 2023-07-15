using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameScene
{
    public class Death_Menu_Stars : MonoBehaviour
    {
        public Death_Menu_SingleStar[] Stars = new Death_Menu_SingleStar[3];
        private void SetStars()
        {
            if(WinArea.Instance!=null)
                for(int i=0;i < Stars.Length;i++)
                    Stars[i].GetComponentInChildren<TextMeshProUGUI>().text = WinArea.Instance.TimeBorders[i] + " sec";
            if (GameInfo.Instance != null)
            {
                int starsCount = PlayerPrefs.GetInt("LevelStars" + GameInfo.Instance.LoadedLevel.ToString());
                for (int i = 0; i < starsCount; i++)
                {
                    Stars[i].InstantTurnOn();
                }
            }
        }
        private void Awake()
        {
            if (GameInfo.Instance != null && GameInfo.Instance.gamemode != GameInfo._GameMode.Level)
                Destroy(gameObject);
        }
        private void OnEnable()
        {
            SetStars();
        }
    }
}


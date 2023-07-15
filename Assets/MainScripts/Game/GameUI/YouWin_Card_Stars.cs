using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameScene
{
    public class YouWin_Card_Stars : MonoBehaviour
    {
        public YouWin_Card_SingleStar[] Stars = new YouWin_Card_SingleStar[3];

        public void UpdateStars()
        {
            if (WinArea.Instance != null)
            {
                float GameTime = WinArea.Instance.GameTime;
                int StarsColletted = 0;
                for (int i = 0; i < Stars.Length; i++)
                {
                    Stars[i].GetComponentInChildren<TextMeshProUGUI>().text = WinArea.Instance.TimeBorders[i] + " sec";
                    if ((int)GameTime <= (int)WinArea.Instance.TimeBorders[i])
                    {
                        Stars[i].TurnOn(i);
                        StarsColletted++;
                    }
                }
                if(StarsColletted > PlayerPrefs.GetInt("LevelStars" + GameInfo.Instance.LoadedLevel.ToString()))
                    PlayerPrefs.SetInt("LevelStars" + GameInfo.Instance.LoadedLevel.ToString(),StarsColletted);
            }
        }
        private void SetStars()
        {
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
            SetStars();
            //Zerowanie
            //PlayerPrefs.SetInt("LevelStars" + GameInfo.Instance.LoadedLevel.ToString(), 0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIScene
{
    public class Level_List_Stars_Controller : MonoBehaviour
    {
        public Image[] StarImages = new Image[3];
        private readonly Color[] StarColors = { new Color(0.2f, 0.2f, 0.2f, 1),new Color(1, 1, 0.43f, 1)}; // off, on
        public void UpdateStars(int LevelIndex)
        {
            int StarCount = PlayerPrefs.GetInt("LevelStars" + LevelIndex.ToString());
            //fix
            if(StarCount<0) PlayerPrefs.SetInt("LevelStars" + LevelIndex.ToString(), 0);
            else if(StarCount > 3) PlayerPrefs.SetInt("LevelStars" + LevelIndex.ToString(), 3);
            //
            //zerowanie
            for (int i = 0; i < 3; i++)
            {
                StarImages[i].color = StarColors[0];
            }
            for (int i = 0;i < StarCount; i++)
            {
                StarImages[i].color = StarColors[1];
            }
        }
    }
}

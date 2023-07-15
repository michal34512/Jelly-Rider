using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameScene
{
    public class Death_Menu_SingleStar : MonoBehaviour
    {
        private static Color StarColor = new Color(1, 1, 0.43f, 0.9f);
        public void InstantTurnOn()
        {
            GetComponent<Image>().color = StarColor;
            GetComponentInChildren<TextMeshProUGUI>().color = StarColor;
        }
    }
}
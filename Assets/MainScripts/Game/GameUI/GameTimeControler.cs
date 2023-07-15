using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameScene
{
    public class GameTimeControler : MonoBehaviour
    {
        private void FixedUpdate()
        {
            if (WinArea.Instance != null)
            {
                GetComponent<TextMeshProUGUI>().text = "TIME: " + (int)WinArea.Instance.GameTime;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace GameScene
{
    public class ProcessBar : MonoBehaviour
    {
        public TextMeshProUGUI ThisLevel;
        public TextMeshProUGUI NextLevel;
        private float MaxSize;
        void setProcessBarPercent(float value) // value from 0 to 1
        {
            if (value < 0f) value = 0f;
            else if (value > 1f) value = 1f;
            GetComponent<RectTransform>().sizeDelta = new Vector2(MaxSize * value,GetComponent<RectTransform>().sizeDelta.y);
            GetComponent<RectTransform>().anchoredPosition = new Vector2((-MaxSize * 0.5f) + GetComponent<RectTransform>().sizeDelta.x * 0.5f, GetComponent<RectTransform>().anchoredPosition.y);
        }
        private void Start()
        {
            //Setting up
            if (GameInfo.Instance != null && GameInfo.Instance.gamemode == GameInfo._GameMode.Level)
            {
                MaxSize = GetComponent<RectTransform>().sizeDelta.x;
                setProcessBarPercent(0f);
                if (GameInfo.Instance != null)
                {
                    GetComponent<Image>().sprite = GameInfo.Instance.Jellys[GameInfo.Instance.PickedJelly].MainGradnient;
                    if (GameInfo.Instance.isLevelLoaded)
                    {
                        ThisLevel.text = GameInfo.Instance.LoadedLevel.ToString();
                        NextLevel.text = (GameInfo.Instance.LoadedLevel + 1).ToString();
                    }
                }
            }
            else Destroy(transform.parent.gameObject);
        }
        private void FixedUpdate()
        {
            if(WinArea.Instance!=null && GameJelly.Instance != null)
            {
                float Percent = (GameJelly.Instance.transform.position.x - GameJelly.Instance.FirstPoint.x) / (WinArea.Instance.transform.position.x - GameJelly.Instance.FirstPoint.x);
                setProcessBarPercent(Percent);
            }   
        }
    }
}


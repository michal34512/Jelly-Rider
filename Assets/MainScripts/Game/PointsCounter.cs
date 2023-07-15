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
            Sprite ActualBackground = _Background.GetComponent<Image>().sprite;
            GameObject ga = Instantiate(_Background, _Background.transform.parent);
            Destroy(ga.GetComponent<Background_Controller>());
            Image im = ga.GetComponent<Image>();
            im.type = Image.Type.Filled;
            im.fillMethod = Image.FillMethod.Radial360;
            im.fillClockwise = true;
            im.fillAmount = 1f;
            //im.fillOrigin = 0;
            if (GameInfo.Instance != null) GameInfo.Instance.Next_Background();
            im.sprite = ActualBackground;
            LeanTween.value(ga, 1f, 0f, 1f).setOnUpdate((float val) =>
               {
                   im.fillAmount = val;
               }).setOnComplete(() => { Destroy(ga); });
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
        private void Start()
        {
            if (GameInfo.Instance != null && GameInfo.Instance.gamemode != GameInfo._GameMode.Constant)
                Destroy(gameObject);
        }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameScene
{
    public class YouWin_Card_SingleStar : MonoBehaviour
    {
        public GameObject StarEffectPrefab;
        private GameObject EffectClone;
        private static Color [] StarColors = { new Color(0, 0, 0, 0.43f), new Color(1, 1, 0.43f, 0.9f) };
        private bool isTurned = false;
        public void TurnOn(int i)
        {
            if (!isTurned)
            {
                isTurned = true;
                LeanTween.delayedCall(i * 0.5f, () =>
                {
                    EffectClone = Instantiate(StarEffectPrefab, transform.position, Quaternion.identity, transform);

                    LeanTween.value(gameObject, StarColors[0], StarColors[1], 0.5f).setOnUpdate((Color val) =>
                   {
                       GetComponent<Image>().color = val;
                       GetComponentInChildren<TextMeshProUGUI>().color = val;
                   });
                });
            }
            else
            {
                InstantTurnOn();
            }
        }
        public void InstantTurnOn()
        {
            isTurned = true;
            GetComponent<Image>().color = StarColors[1];
            GetComponentInChildren<TextMeshProUGUI>().color = StarColors[1];
        }
        private void OnDisable()
        {
            if(EffectClone!=null)
                Destroy(EffectClone);
        }
    }
}


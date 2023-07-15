using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene
{
    public class AddPointBarier : MonoBehaviour
    {
        public GameObject BarierParticles;
        bool Triggerd = false;
        private void FixedUpdate()
        {
            if(GameJelly.Instance.transform.position.x > transform.position.x&&!Triggerd)
            {
                Triggerd = true;
                //Add point
                PointsCounter.Instance.AddPoint();
                LeanTween.value(gameObject, GetComponent<SpriteRenderer>().color.a, 0f, 0.5f).setOnUpdate((float val) =>
                {
                    GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, val);
                }).setEaseOutCubic().setOnComplete(()=> { Destroy(gameObject); });
                GameObject ga = Instantiate(BarierParticles);
                ga.transform.position = transform.position - new Vector3(-0.2f,0f,0f);
            }
        }
        private void Start()
        {
            if (GameInfo.Instance != null && GameInfo.Instance.gamemode != GameInfo._GameMode.Constant)
                Destroy(gameObject);
        }
    }
}


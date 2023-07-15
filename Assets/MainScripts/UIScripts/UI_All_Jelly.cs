using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UIScene
{
    public class UI_All_Jelly : MonoBehaviour
    {
        public List<GameObject> UI_Jellys;
        [HideInInspector]
        public bool WasChange = false;
        private readonly float AlphaOffset = 1.4f;
        private float SavedTouchX;
        private float JellysPositionX;
        private float Odstep = 0f;
        public void UpdateJellySprites()
        {
            for(int i=0;i<7;i++)
            {
                if (GameInfo.Instance.PickedJelly - 3 + i >= 0 && GameInfo.Instance.PickedJelly - 3 + i < GameInfo.Instance.Jellys.Count)
                {
                    UI_Jellys[i].SetActive(true);
                    UI_Jellys[i].GetComponent<Image>().sprite = GameInfo.Instance.Jellys[GameInfo.Instance.PickedJelly - 3 + i].MainSprite;
                }  
                else UI_Jellys[i].SetActive(false);
            }
        }
        private void UpdateOdstep()
        {
            Odstep = UI_Jellys[1].transform.position.x - UI_Jellys[0].transform.position.x;
        }
        private void UpdateTouch()
        {
            float Difference = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x - SavedTouchX;
            //Checking
            if (Difference > Odstep * 0.5f && GameInfo.Instance.PickedJelly > 0)
            {
                SavedTouchX += Odstep;
                Difference = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x - SavedTouchX;
                Arrow_Left();
            }
            else if (Difference < -Odstep * 0.5f && GameInfo.Instance.PickedJelly < GameInfo.Instance.Jellys.Count - 1)
            {
                SavedTouchX -= Odstep;
                Difference = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x - SavedTouchX;
                Arrow_Right();
            }
            transform.position = new Vector3(JellysPositionX + Difference, transform.position.y, transform.position.z);
            //Updating Alpha
            for(int i=0;i<7;i++)
            {
                float Position = UI_Jellys[i].transform.position.x;// + transform.position.x;
                float Percent = (3f*Odstep - Mathf.Abs(Position))/(3f*Odstep);
                UI_Jellys[i].GetComponent<Image>().color = new Color(1, 1, 1, Percent * AlphaOffset);
            }
        }

        #region Buttons
        public void Arrow_Left()
        {
            GameInfo.Instance.PickedJelly--;
            UpdateJellySprites();
            WasChange = true;
        }
        public void Arrow_Right()
        {
            GameInfo.Instance.PickedJelly++;
            UpdateJellySprites();
            WasChange = true;
        }
        #endregion Buttons
        private void Update()
        {
            if(Input.touchCount==1)
            {
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    SavedTouchX = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x;
                    JellysPositionX = transform.position.x;
                    LeanTween.cancel(gameObject);
                }
                else if(Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    LeanTween.value(gameObject, transform.position.x, 0f, 1f).setOnUpdate((float val) =>
                       {
                           transform.position = new Vector3(val, transform.position.y, transform.position.z);
                           for (int i = 0; i < 7; i++)
                           {
                               float Position = UI_Jellys[i].transform.position.x + transform.position.x;
                               float Percent = (3f * Odstep - Mathf.Abs(Position)) / (3f * Odstep);
                               UI_Jellys[i].GetComponent<Image>().color = new Color(1, 1, 1, Percent * AlphaOffset);
                           }
                       }).setEaseOutCubic();
                }
                else
                {
                    UpdateTouch();
                }
            }
        }
        private void Start()
        {
            UpdateOdstep();
            UpdateJellySprites();
        }
    }
}


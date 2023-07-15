using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIScene
{
    public class UI_All_Jelly : MonoBehaviour
    {
        public Sprite LockedJellySprite;
        public List<GameObject> UI_Jellys;
        [HideInInspector]
        public bool WasChange = false;
        private readonly float AlphaOffset = 1.4f;
        private float SavedTouchX;
        private float JellysPositionX;
        private float Odstep = 0f;

        public TextMeshProUGUI JellyClassText;
        private readonly Color[] JellyClassColor = { new Color(0.78f, 1, 0.8f), new Color(0, 0.87f, 1, 1), new Color(0.51f, 0, 1, 1), new Color(1, 0.3f, 0.31f, 1), new Color(0.97f, 1, 0.647f, 1) };//, new Color(1, 0.8f, 0.39f) };
        private int _JellyClassIndex = 0; //Default
        private int JellyClassIndex
        {
            get
            {
                return _JellyClassIndex;
            }
            set
            {
                if (value < JellyClassColor.Length && value >= 0 && _JellyClassIndex != value)
                {
                    //Update
                    JellyClassText.text = ((Jelly_Scriptable_Object._RarityClass)value).ToString();
                    JellyClassText.color = JellyClassColor[value];
                    _JellyClassIndex = value;
                }
            }
        }

        #region Buttons
        public void Button_Class_Arrow_Right()
        {
            if(JellyClassIndex + 1 < JellyClassColor.Length)
            {
                MenuCardsController.Instance.Sound_Play_LightClick();
                JellyClassIndex++;
                UpdateJellyClass();
                WasChange = true;
            }
        }
        public void Button_Class_Arrow_Left()
        {
            if (JellyClassIndex - 1 >= 0 )
            {
                MenuCardsController.Instance.Sound_Play_LightClick();
                JellyClassIndex--;
                UpdateJellyClass();
                WasChange = true;
            }
        }
        public void Arrow_Left()
        {
            MenuCardsController.Instance.Sound_Play_LightClick();
            GameInfo.Instance.PickedJelly = GameInfo.Instance.GetJellyIndexByClassPos(new Vector2Int(JellyClassIndex, GameInfo.Instance.ClassesPickedJelly.y - 1));
            UpdateJellySprites();
            WasChange = true;
        }
        public void Arrow_Right()
        {
            MenuCardsController.Instance.Sound_Play_LightClick();
            GameInfo.Instance.PickedJelly = GameInfo.Instance.GetJellyIndexByClassPos(new Vector2Int(JellyClassIndex, GameInfo.Instance.ClassesPickedJelly.y + 1));
            UpdateJellySprites();
            WasChange = true;
        }
        #endregion Buttons
        private void UpdateJellyClass() //with Animation
        {
            LeanTween.value(transform.parent.gameObject, 0, 90, 0.1f).setOnUpdate((float val) =>
            {
                for (int i = 0; i < UI_Jellys.Count; i++)
                    UI_Jellys[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(val, 0, 0);
            }).setOnComplete(()=> 
            {
                //Change
                GameInfo.Instance.PickedJelly = GameInfo.Instance.GetJellyIndexByClassPos(new Vector2Int(JellyClassIndex, 0));
                UpdateJellySprites();
                LeanTween.value(transform.parent.gameObject, 90, 0, 0.1f).setOnUpdate((float val) =>
                {
                    for (int i = 0; i < UI_Jellys.Count; i++)
                        UI_Jellys[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(val, 0, 0);
                });
            });
        }
        public void UpdateJellySprites()
        {
            Vector2Int IndexPos = GameInfo.Instance.ClassesPickedJelly;
            for (int i = 0; i < 7; i++)
            {
                if (IndexPos.y - 3 + i >= 0 && IndexPos.y - 3 + i < GameInfo.Instance.Classes[IndexPos.x].Count)
                {
                    UI_Jellys[i].SetActive(true);
                    if (GameInfo.Instance.Classes[IndexPos.x][IndexPos.y - 3 + i].Unlocked)
                    {
                        UI_Jellys[i].GetComponent<Image>().sprite = GameInfo.Instance.Classes[IndexPos.x][IndexPos.y - 3 + i].MainSprite;
                    }
                    else UI_Jellys[i].GetComponent<Image>().sprite = LockedJellySprite;
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
            if (Difference > Odstep * 0.5f && GameInfo.Instance.ClassesPickedJelly.y > 0)
            {
                SavedTouchX += Odstep;
                Difference = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x - SavedTouchX;
                Arrow_Left();
            }
            else if (Difference < -Odstep * 0.5f && GameInfo.Instance.ClassesPickedJelly.y < GameInfo.Instance.Classes[GameInfo.Instance.ClassesPickedJelly.x].Count - 1)
            {
                SavedTouchX -= Odstep;
                Difference = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x - SavedTouchX;
                Arrow_Right();
            }
            transform.position = new Vector3(JellysPositionX + Difference, transform.position.y, transform.position.z);
            //Updating Alpha
            for (int i = 0; i < 7; i++)
            {
                float Position = UI_Jellys[i].transform.position.x;// + transform.position.x;
                float Percent = (3f * Odstep - Mathf.Abs(Position)) / (3f * Odstep);
                UI_Jellys[i].GetComponent<Image>().color = new Color(1, 1, 1, Percent * AlphaOffset);
            }
        }
        private void UpdateJellyClassText()
        {
            if (GameInfo.Instance != null)
                JellyClassIndex = (int)GameInfo.Instance.Jellys[GameInfo.Instance.PickedJelly].RarityClass;
        }
        private void Update()
        {
            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    SavedTouchX = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x;
                    JellysPositionX = transform.position.x;
                    LeanTween.cancel(gameObject);
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
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
        }
        private void OnEnable()
        {
            UpdateJellyClassText();
            UpdateJellySprites();
            for(int i=0;i < UI_Jellys.Count; i++)
                UI_Jellys[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}


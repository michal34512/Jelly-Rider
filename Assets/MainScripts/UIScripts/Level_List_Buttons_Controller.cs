using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace UIScene
{
    public class Level_List_Buttons_Controller : MonoBehaviour
    {
        public GameObject Dots;
        public Sprite DotSprite;
        public GameObject Perfab_Dimmer;
        public Sprite LevelCompletedSprite;

        private List<Image> DotsImages;
        private readonly float DotsSpace = 100;
        private readonly int MaxButtonCount = 18;
        private readonly int MaxCard = 3;
        private int _List_Page = 0;
        public int List_Page 
        {
            get { return _List_Page; }
            set
            {
                if(value >= 0&&value <= MaxCard-1)
                {
                    _List_Page = value;
                    Assign_Buttons();
                    Update_Dots_State();
                }
            }
        }
        [RequireComponent(typeof(Button))]
        public class Single_Level_List_Button : MonoBehaviour
        {
            public int Index = 0;
            public System.Action<int> Action;

            private Image LevelCompletedImage;
            public void Set_Up(Sprite LevelCompletedSprite)
            {
                gameObject.GetComponent<Button>().onClick.AddListener(()=> { Action?.Invoke(Index); });
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = Index.ToString();
                if(PlayerPrefs.GetInt("LevelCompleted"+Index.ToString()) == 1)
                {
                    GameObject ga = new GameObject("LevelCompleted");
                    ga.transform.parent = gameObject.transform;
                    RectTransform rt = ga.AddComponent<RectTransform>();
                    rt.localScale = new Vector3(1, 1, 1);
                    rt.anchoredPosition = new Vector2(0, 0);
                    LevelCompletedImage = ga.AddComponent<Image>();
                    LevelCompletedImage.sprite = LevelCompletedSprite;
                    LevelCompletedImage.SetNativeSize();
                    UpdateColor();
                }
            }
            public void UpdateColor()
            {
                if (LevelCompletedImage != null)
                    LevelCompletedImage.color = GameInfo.Instance.Game_Color;
            }
        }

        #region ButtonsScripts
        public void Get_Button_Action(int _index)
        {
            // Loading Scene
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                GameInfo.Instance.Next_Background();
                GameInfo.Instance.gamemode = GameInfo._GameMode.Level;
                SceneManager.LoadScene(1);
                GameInfo.Instance.LoadedLevel = _index;
                SceneManager.LoadSceneAsync(2 + _index, LoadSceneMode.Additive);
                //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            });
        }
        public void Button_Arrow_Left()
        {
            List_Page--;
        }
        public void Button_Arrow_Right()
        {
            List_Page++;
        }
        #endregion ButtonsScripts

        private void Assign_Buttons()
        {
            int Counter = 0;
            foreach(Transform t in transform)
            {
                Counter++;
                var com = t.gameObject.GetComponent<Single_Level_List_Button>() == null ? t.gameObject.AddComponent<Single_Level_List_Button>() : t.gameObject.GetComponent<Single_Level_List_Button>();
                com.Index = Counter + List_Page * MaxButtonCount;
                com.Action = Get_Button_Action;
                com.Set_Up(LevelCompletedSprite);
            }
        }
        private void Set_Up_Dots()
        {
            DotsImages = new List<Image>();
            for(int i=0;i<MaxCard;i++)
            {
                GameObject NewDot = new GameObject("Dot");
                NewDot.transform.parent = Dots.transform;
                NewDot.AddComponent<RectTransform>().localScale = new Vector3(1,1,1);
                NewDot.GetComponent<RectTransform>().anchoredPosition = new Vector2((-((MaxCard-1)*DotsSpace)*0.5f) + i * DotsSpace,0);
                NewDot.AddComponent<Image>().sprite = DotSprite;
                DotsImages.Add(NewDot.GetComponent<Image>());
                NewDot.GetComponent<Image>().SetNativeSize();
                if (i == List_Page) // Full Dot
                {
                    NewDot.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
                }
                else // Not Full Dot
                {
                    NewDot.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
                }
            }
        }
        private void Update_Dots_State()
        {
            for(int i = 0; i < MaxCard; i++)
            {
                if (i == List_Page) // Full Dot
                {
                    DotsImages[i].GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
                }
                else // Not Full Dot
                {
                    DotsImages[i].GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
                }
            }
        }
        private void Start()
        {
            Assign_Buttons();
            Set_Up_Dots();
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UIScene
{
    public class MenuCardsController : MonoBehaviour
    {
        public static MenuCardsController Instance;
        //PUBLIC
        public GameObject Perfab_Dimmer; 
        public UI_Main_Jelly Main_Jelly;
        public UI_All_Jelly All_Jelly;
        public List<GameObject> Cards;

        public List<Image> Game_Color_Images;
        public List<TextMeshProUGUI> Game_Color_Text;
        public GameObject Game_Color_Level_List;

        #region ButtonsScripts
        public void Button_Play()
        {
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(()=>
            {
                GameInfo.Instance.Next_Background();
                GameInfo.Instance.gamemode = GameInfo._GameMode.Constant;
                SceneManager.LoadScene(1);
                SceneManager.LoadScene(2,LoadSceneMode.Additive);
            });
        }
        public void Button_Choose_Jelly()
        {
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() => 
            {
                Cards[0].SetActive(false);
                Cards[1].SetActive(true);
                Main_Jelly.StopAnimation();
            });
        }
        public void Button_Choose_Jelly_Back_To_First_Card()
        {
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() => 
            {
                if(All_Jelly.WasChange)
                {
                    All_Jelly.WasChange = false;
                    GameInfo.Instance.Next_Background();
                }
                Cards[0].SetActive(true);
                Cards[1].SetActive(false);
                Main_Jelly.StartAnimation();
                Main_Jelly.UpdateJellySprite();
                Update_Game_Color();
            });
        }
        public void Button_Level_List()
        {
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                Cards[0].SetActive(false);
                Cards[2].SetActive(true);
            });
        }
        public void Button_Level_List_Back_To_First_Card()
        {
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                Cards[0].SetActive(true);
                Cards[2].SetActive(false);
                Main_Jelly.StartAnimation();
            });
        }
        
        #endregion ButtonsScripts
        //PRIVATE
        private void Update_Game_Color()
        {
            GameInfo.Instance.Game_Color = GameInfo.Instance.Jellys[GameInfo.Instance.PickedJelly].MainColor;
            foreach (Image i in Game_Color_Images)
                i.color = GameInfo.Instance.Game_Color;
            foreach (TextMeshProUGUI t in Game_Color_Text)
                t.color = GameInfo.Instance.Game_Color;
            foreach(Level_List_Buttons_Controller.Single_Level_List_Button SingleButton in Game_Color_Level_List.GetComponentsInChildren<Level_List_Buttons_Controller.Single_Level_List_Button>())
                SingleButton.UpdateColor();
        }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }
        private void Start()
        {
            Update_Game_Color();
        }
    }
}


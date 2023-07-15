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
        public Wheel_Of_Fortune_Controller Fortune_Wheel;
        public Material ParticleMaterial;
        public List<GameObject> Cards;

        public List<Image> Game_Color_Images;
        public List<TextMeshProUGUI> Game_Color_Text;
        public GameObject Game_Color_Level_List;

        public GameObject MessageBoxPrefab;

        public AudioClip[] Sounds = new AudioClip[1];

        public Sprite[] SpeakerIcons = new Sprite[2]; // 0 - unmuted   1 - muted
        public Image Speaker;

        #region ButtonsScripts
        public void Button_Play()
        {
            Sound_Click();
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
            Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() => 
            {
                Cards[0].SetActive(false);
                Cards[1].SetActive(true);
                Main_Jelly.StopAnimation();
                MoneyCounterActivation();
            });
        }
        public void Button_Choose_Jelly_Back_To_First_Card()
        {
            Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() => 
            {
                if (All_Jelly.WasChange)
                {
                    if (!GameInfo.Instance.Jellys[GameInfo.Instance.PickedJelly].Unlocked)
                    {
                        GameInfo.Instance.PickedJelly = GameInfo.Instance.GetJellyIndexByClassPos(new Vector2Int(0, 0)); //Default
                        
                    }
                        
                    All_Jelly.WasChange = false;
                    GameInfo.Instance.Next_Background();
                }
                Cards[0].SetActive(true);
                Cards[1].SetActive(false);
                Main_Jelly.StartAnimation();
                Main_Jelly.UpdateJellySprite();
                Update_Game_Color();
                MoneyCounterActivation();
            });
        }
        public void Button_Level_List()
        {
            Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                Cards[0].SetActive(false);
                Cards[2].SetActive(true);
                MoneyCounterActivation();
            });
        }
        public void Button_Level_List_Back_To_First_Card()
        {
            Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                Cards[0].SetActive(true);
                Cards[2].SetActive(false);
                Main_Jelly.StartAnimation();
                MoneyCounterActivation();
            });
        }
        public void Button_Wheel_Of_Fortune_Back_To_First_Card()
        {
            Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                Cards[0].SetActive(true);
                Cards[3].SetActive(false);
                Main_Jelly.StartAnimation();
                if(Fortune_Wheel.isUseButtonDimmed)
                    GameInfo.Instance.Next_Background();
                Main_Jelly.UpdateJellySprite();
                Update_Game_Color();
                Fortune_Wheel.Reset();
                MoneyCounterActivation();
            });
        }
        public void Button_Wheel_Of_Fortune()
        {
            Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                Cards[0].SetActive(false);
                Cards[3].SetActive(true);
                Main_Jelly.StopAnimation();
                MoneyCounterActivation();
            });
        }
        public void Button_Play_Totorial()
        {
            Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                GameInfo.Instance.Next_Background();
                GameInfo.Instance.gamemode = GameInfo._GameMode.Tutorial;
                SceneManager.LoadScene(1);
                SceneManager.LoadScene(3, LoadSceneMode.Additive);
            });
        }
        public void Button_Settings()
        {
            Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                Cards[0].SetActive(false);
                Cards[4].SetActive(true);
                Main_Jelly.StopAnimation();
                MoneyCounterActivation();
            });
        }
        public void Button_Settings_Back_To_First_Card()
        {
            Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                Cards[0].SetActive(true);
                Cards[4].SetActive(false);
                Main_Jelly.StartAnimation();
                MoneyCounterActivation();
            });
        }
        public void Button_Music_Switch()
        {
            if (GameInfo.Instance != null)
            {
                GameInfo.Instance.Chance_Mute_State();
                //Change Icon
                if(GameInfo.Instance.isMuted)
                {
                    //Crossed Speaker
                    Speaker.sprite = SpeakerIcons[1];
                }
                else
                {
                    //Speaker
                    Speaker.sprite = SpeakerIcons[0];
                }
            }
        }
        #endregion ButtonsScripts
        #region Sounds
        public void Sound_Click()
        {
            GetComponent<AudioSource>().PlayOneShot(Sounds[0]);
        }
        public void Sound_Failed_Click()
        {
            GetComponent<AudioSource>().PlayOneShot(Sounds[1],0.2f);
        }    
        public void Sound_Get_Item()
        {
            GetComponent<AudioSource>().PlayOneShot(Sounds[2],0.2f);
        }
        public void Sound_Play_LightClick()
        {
            GetComponent<AudioSource>().PlayOneShot(Sounds[3],0.4f);
        }
        public void Sound_Wheel_Of_Fortune_Click()
        {
            GetComponent<AudioSource>().PlayOneShot(Sounds[4],0.2f);
        }
        #endregion Sounds
        //PRIVATE
        private void MoneyCounterActivation()
        {
            if (GameScene.MoneyCounterController.Instance != null)
            {
                if (Cards[0].activeSelf || Cards[4].activeSelf)
                    GameScene.MoneyCounterController.Instance.gameObject.SetActive(false);
                else GameScene.MoneyCounterController.Instance.gameObject.SetActive(true);
            }
        }
        private void Update_Game_Color()
        {
            GameInfo.Instance.Game_Color = GameInfo.Instance.Jellys[GameInfo.Instance.PickedJelly].MainColor;
            foreach (Image i in Game_Color_Images)
                i.color = GameInfo.Instance.Game_Color;
            foreach (TextMeshProUGUI t in Game_Color_Text)
                t.color = GameInfo.Instance.Game_Color;
            ParticleMaterial.SetTexture("_BaseMap", GameInfo.Instance.Jellys[GameInfo.Instance.PickedJelly].MainGradnient.texture);
            //foreach(Level_List_Buttons_Controller.Single_Level_List_Button SingleButton in Game_Color_Level_List.GetComponentsInChildren<Level_List_Buttons_Controller.Single_Level_List_Button>())
            //    SingleButton.UpdateColor();
        }
        private void Check_For_Tutorial()
        {
            if(PlayerPrefs.GetInt("PlayTutorial") != 1&&false)
            {
                LeanTween.delayedCall(1f,() => 
                {
                    //Creating Box
                    if(this!=null)
                    {
                        var mesbx = Instantiate(MessageBoxPrefab, transform).GetComponent<MessageBoxController>();
                        mesbx.ShowBox("Do you want to play tutorial?", "It seems like you are new in the game. Do you want to play Tutorial?", (bool val) =>
                        {
                            if (val)
                            {
                            //YES
                            //PlayerPrefs.SetInt("PlayTutorial", 1);
                            mesbx.HideBox();
                                Button_Play_Totorial();
                            }
                            else
                            {
                            //NO
                            //PlayerPrefs.SetInt("PlayTutorial", 1);
                            mesbx.HideBox();
                            }
                        }, false);
                        LeanTween.delayedCall(15f, () => { if (mesbx != null) mesbx.HideBox(); });
                    }
                    
                });
            }
        }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);

            //Setting
            //Hide money counter
            MoneyCounterActivation();
        }
        private void Start()
        {
            Update_Game_Color();
            Check_For_Tutorial();
            //Setting
            if (GameInfo.Instance != null)
            {
                //SpeakerIcon
                if (GameInfo.Instance.isMuted)
                {
                    //Crossed Speaker
                    Speaker.sprite = SpeakerIcons[1];
                }
                else
                {
                    //Speaker
                    Speaker.sprite = SpeakerIcons[0];
                }
            }
        }
    }
}


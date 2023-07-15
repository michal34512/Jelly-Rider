using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TutorialLevel;

namespace GameScene
{
    public class InGameUIController : MonoBehaviour
    {
        public static InGameUIController Instance;

        public GameObject InGameMenu;
        public GameObject Perfab_Dimmer;

        public AudioClip[] Sounds = new AudioClip[1];

        public Sprite[] SpeakerIcons = new Sprite[2]; // 0 - unmuted   1 - muted
        public Image Speaker;

        #region ButtonsScripts
        public void Button_Open_In_Game_Menu()
        {
            if (!GameOver_Card.Instance.Ejected&& (YouWin_Card.Instance==null||!YouWin_Card.Instance.Ejected))
            {
                Sound_Click();
                InGameMenu.SetActive(true);
                //Reseting Color
                LeanTween.cancel(InGameMenu);
                foreach (Image im in InGameMenu.GetComponentsInChildren<Image>())
                    im.color = new Color(im.color.r, im.color.g, im.color.b, 0.392f);
                foreach (TextMeshProUGUI tx in InGameMenu.GetComponentsInChildren<TextMeshProUGUI>())
                    tx.color = new Color(tx.color.r, tx.color.g, tx.color.b, 0.392f);
                Time.timeScale = 0f;
            }
        }
        public void Button_Continue()
        {
            Time.timeScale = 1f;
            Sound_Click();
            LeanTween.value(InGameMenu, 0.392f, 0f, 0.3f).setOnUpdate((float val) =>
            {
                foreach (Image im in InGameMenu.GetComponentsInChildren<Image>())
                    im.color = new Color(im.color.r, im.color.g, im.color.b, val);
                foreach (TextMeshProUGUI tx in InGameMenu.GetComponentsInChildren<TextMeshProUGUI>())
                    tx.color = new Color(tx.color.r, tx.color.g, tx.color.b, val);
            }).setEaseOutCubic().setOnComplete(()=>{ InGameMenu.SetActive(false); });
        }
        public void Button_Reset()
        {
            Sound_Click();
            GameOver_Card.Instance.Hide();
            if (YouWin_Card.Instance != null) YouWin_Card.Instance.Hide();
            GameJelly.Instance.BackToFirstPoint();
            Form_Controller.Instance.FastOpenForm();

            CameraController.Instance.position = new Vector3(0,0,Camera.main.transform.parent.localPosition.z);

            foreach(Effects.Dimmer d in FindObjectsOfType<Effects.Dimmer>())
            {
                d.gameObject.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 0.5f);
            }
                

            Button_Continue();
            foreach (var bp in FindObjectsOfType<BezierPath>())
            {
                bp.Restart();
            }
            if (GameInfo.Instance != null && GameInfo.Instance.gamemode == GameInfo._GameMode.Constant)
            {
                PointsCounter.Instance.SetToZero();
            } else if (GameInfo.Instance != null && GameInfo.Instance.gamemode == GameInfo._GameMode.Level)
            {
                foreach (MoneyController _m in Resources.FindObjectsOfTypeAll(typeof(MoneyController)))
                {
                    _m.Restart();
                }
            } else if (GameInfo.Instance != null && GameInfo.Instance.gamemode == GameInfo._GameMode.Tutorial)
                TutorialLevelManager.Instance.Restart();
                if (ConstantLevelSpawner.Instance != null)
                    ConstantLevelSpawner.Instance.Restart();
            if(WinArea.Instance!=null)
            {
                WinArea.Instance.Reset_Game_Time();
            }
        }
        public void Button_Total_Reset()
        {
            Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                GameInfo.Instance.Next_Background();
                if(GameInfo.Instance!=null)
                {
                    if(GameInfo.Instance.gamemode == GameInfo._GameMode.Constant)
                    {
                        SceneManager.LoadScene(2);
                    }else if(GameInfo.Instance.gamemode == GameInfo._GameMode.Level)
                    {
                        SceneManager.LoadScene(3 + GameInfo.Instance.LoadedLevel);
                    }
                }
            });
        }
        public void Button_Menu()
        {
            Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                if (GameInfo.Instance != null)
                {
                    if(GameInfo.Instance.gamemode == GameInfo._GameMode.Constant)
                            PointsCounter.Instance.SetToZero();
                    GameInfo.Instance.gamemode = GameInfo._GameMode.UI;
                    GameInfo.Instance.Next_Background();
                }
                SceneManager.LoadScene(0);
            });
            Time.timeScale = 1f;
        }
        public void Button_Next_Level()
        {
            Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                if(GameInfo.Instance != null)
                {
                    if (GameInfo.Instance.gamemode == GameInfo._GameMode.Constant)
                        PointsCounter.Instance.SetToZero();
                    GameInfo.Instance.Next_Background();
                    SceneManager.LoadSceneAsync(1);
                    //SceneManager.UnloadSceneAsync(2 + GameInfo.Instance.LoadedLevel);
                    GameInfo.Instance.LoadedLevel++;
                    SceneManager.LoadSceneAsync(3 + GameInfo.Instance.LoadedLevel,LoadSceneMode.Additive);
                }
                    
            });
            Time.timeScale = 1f;
        }
        public void Button_Music_Switch()
        {
            if (GameInfo.Instance != null)
            {
                GameInfo.Instance.Chance_Mute_State();
                //Change Icon
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
        #endregion ButtonsScripts
        #region Sounds
        public void Sound_Click()
        {
            GetComponent<AudioSource>().PlayOneShot(Sounds[0]);
        }
        public void Sound_Jumping_Platform()
        {
            GetComponent<AudioSource>().PlayOneShot(Sounds[1],.2f);
        }
        #endregion Sounds
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
            //Setting
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
        private void OnApplicationPause(bool pause)
        {
            if(pause)
            {
                Button_Open_In_Game_Menu();
            }
        }
    }
}


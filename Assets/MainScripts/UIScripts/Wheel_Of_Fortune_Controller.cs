using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UIScene
{
    public class Wheel_Of_Fortune_Controller : MonoBehaviour
    {
        public GameObject Perfab_Dimmer;
        public GameObject Prefab_MessageBox;
        public GameObject WheelOfFortune;
        public Wheel_Get_Item_Controller GetItemController;
        public TextMeshProUGUI UseButtonText;

        private bool isSpining = false;
        private bool SpinPerimision = true;

        private float AngularSpeed = 0; // speed in deg / sec
        private readonly float[] AngularSpeedBorders = { 1000f, 1600f };
        private readonly float AngularAcceleration = 300; // deg / sec
        private float ActualRotation; //deg

        private readonly byte WheelStates = 12;
        private readonly Jelly_Scriptable_Object._RarityClass[] WheelClases =
        {
            Jelly_Scriptable_Object._RarityClass.Rare,
            Jelly_Scriptable_Object._RarityClass.Common,
            Jelly_Scriptable_Object._RarityClass.Epic,
            Jelly_Scriptable_Object._RarityClass.Legendary,
            Jelly_Scriptable_Object._RarityClass.Epic,
            Jelly_Scriptable_Object._RarityClass.Common,
            Jelly_Scriptable_Object._RarityClass.Rare,
            Jelly_Scriptable_Object._RarityClass.Common,
            Jelly_Scriptable_Object._RarityClass.Epic,
            Jelly_Scriptable_Object._RarityClass.Common,
            Jelly_Scriptable_Object._RarityClass.Epic,
            Jelly_Scriptable_Object._RarityClass.Common,
        };

        //Sound
        private float WheelPeriod = 0;
        private readonly float PeriodScaler = 3f;
        private float WheelTimer = 0;

        private Vector2Int RandomizedJellyIndexPos = new Vector2Int(0,0);
        public void Reset()
        {
            gameObject.SetActive(true);
            GetItemController.gameObject.SetActive(false);
        }
        #region ButtonsScripts
        int ClickAttempts = 0;
        public void Button_Spin_Wheel_Of_Fortune()
        {
            if (!isSpining && SpinPerimision)
            {
                if (GameScene.MoneyCounterController.Instance != null)
                    if (GameScene.MoneyCounterController.Instance.SubMoney(100))
                    {
                        MenuCardsController.Instance.Sound_Click();
                        isSpining = true;
                        AngularSpeed = Random.Range(AngularSpeedBorders[0], AngularSpeedBorders[1]);
                        ActualRotation = WheelOfFortune.GetComponent<RectTransform>().rotation.eulerAngles.z;
                    }
                    else
                    {
                        ClickAttempts++;
                        if(ClickAttempts>=5&&GetComponentInParent<Canvas>()!=null)
                        {
                            ClickAttempts = 0;
                            var com = Instantiate(Prefab_MessageBox, GetComponentInParent<Canvas>().transform).GetComponent<MessageBoxController>();
                                com.ShowBox("Lack of money", "You need at least 100$ to spin the wheel");
                            LeanTween.delayedCall(5f, () =>
                             {
                                 if (com != null)
                                     com.HideBox();
                             });
                        }
                        else
                        {
                            MenuCardsController.Instance.Sound_Failed_Click();
                            //Shaking
                            LeanTween.rotateLocal(gameObject, new Vector3(0, 0, -2), 0.04f).setEaseInOutCubic().setOnComplete(() =>
                            {
                                LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 2), 0.08f).setEaseInOutCubic().setOnComplete(() =>
                                {
                                    LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 0), 0.04f).setEaseInOutCubic();
                                });
                            });
                        }
                    }
            }
        }
        public void Button_Open_Get_Item_Card()
        {
            UndimUsebutton();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                MenuCardsController.Instance.Sound_Get_Item();
                gameObject.SetActive(false);
                GetItemController.gameObject.SetActive(true);
            });
        }
        public void Button_Spin_Again()
        {
            MenuCardsController.Instance.Sound_Click();
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                gameObject.SetActive(true);
                GetItemController.gameObject.SetActive(false);
            });
        }
        public void Button_Use_Now()
        {
            if (!isUseButtonDimmed)
            {
                MenuCardsController.Instance.Sound_Click();
                DimUseButton();
                GameInfo.Instance.PickedJelly = GameInfo.Instance.GetJellyIndexByClassPos(RandomizedJellyIndexPos);
            }
        }

        #endregion ButtonsScripts
        public bool isUseButtonDimmed = false;
        private void DimUseButton()
        {
            isUseButtonDimmed = true;
            UseButtonText.color = new Color(UseButtonText.color.r, UseButtonText.color.g, UseButtonText.color.b, 0.5f);
        }
        private void UndimUsebutton()
        {
            isUseButtonDimmed = false;
            UseButtonText.color = new Color(UseButtonText.color.r, UseButtonText.color.g, UseButtonText.color.b, 1f);
        }
        private void RoundResult()
        {
            float OneStateDeg = 360f / (float)WheelStates;
            int StateIndex = (int)Mathf.Round(ActualRotation / OneStateDeg);
            int NormalizedStateIndex = StateIndex % WheelStates;
            float DestinationDeg = StateIndex * OneStateDeg;
            SpinPerimision = false;
            float SavedRotation = ActualRotation;
            LeanTween.delayedCall(0.3f, () =>
            {
                LeanTween.value(gameObject, 0f, 1f, 0.5f).setOnUpdate((float val) =>
                        {
                            WheelOfFortune.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, SavedRotation + val * (DestinationDeg - SavedRotation));
                        }).setOnComplete(() =>
                        {
                            ActualRotation = WheelOfFortune.GetComponent<RectTransform>().rotation.eulerAngles.z;
                            SpinPerimision = true;
                            DrawNewJelly(WheelClases[NormalizedStateIndex]);
                        });
            });
        }
        private void DrawNewJelly(Jelly_Scriptable_Object._RarityClass RarityClass)
        {
            // Randomization Jelly by class
            // Sorting Jellys to class
            List<Jelly_Scriptable_Object> RClass = new List<Jelly_Scriptable_Object>();
            if (GameInfo.Instance != null)
            {
                for (int i = 0; i < GameInfo.Instance.Jellys.Count; i++)
                {
                    if (GameInfo.Instance.Jellys[i].RarityClass == RarityClass)
                        RClass.Add(GameInfo.Instance.Jellys[i]);
                }
            }
            //Drawing from this class
            Jelly_Scriptable_Object Result;
            RandomizedJellyIndexPos.x = (int)RarityClass;
            if (RClass.Count > 0)
            {
                RandomizedJellyIndexPos.y = Random.Range(0, RClass.Count);
                Result = RClass[RandomizedJellyIndexPos.y];
                GetItemController.ShowJelly(Result);
                PlayerPrefs.SetInt("JellyUnlocked" + GameInfo.Instance.GetJellyIndexByClassPos(RandomizedJellyIndexPos), 1);
                GameInfo.Instance.UpdateLockedJellys();
            }
            else Debug.LogWarning("There is no object of rarity class: " + RarityClass);
            
            Button_Open_Get_Item_Card();
        }
        private void CheckForUpdateSpinState()
        {
            if (isSpining)
            {
                AngularSpeed -= AngularAcceleration * Time.deltaTime;
                if (AngularSpeed < 0)
                {
                    AngularSpeed = 0f;
                    isSpining = false;
                    RoundResult();
                }
                else
                {
                    //Update Period
                    WheelPeriod = (360f /  (float)WheelStates) / AngularSpeed ;

                    ActualRotation += AngularSpeed * Time.deltaTime;
                    WheelOfFortune.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, ActualRotation);
                }
            }
        }
        private void SetNewWheelRotation()
        {
            float OneStateDeg = 360f / (float)WheelStates;
            float NewDeg = Random.Range(0, (int)WheelStates) * OneStateDeg;
            WheelOfFortune.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, NewDeg);
        }
        private void UpdateSound()
        {
            if(isSpining)
            {
                WheelTimer += Time.deltaTime;
                if(WheelTimer >  WheelPeriod*PeriodScaler)
                {
                    WheelTimer = 0;
                    MenuCardsController.Instance.Sound_Wheel_Of_Fortune_Click();
                }
            }
        }
        void Update()
        {
            CheckForUpdateSpinState();
            UpdateSound();
        }
        private void OnEnable()
        {
            SetNewWheelRotation();
        }
    }
}
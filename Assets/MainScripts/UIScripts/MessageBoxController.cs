using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UIScene
{
    public class MessageBoxController : MonoBehaviour
    {
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Description;
        public GameObject Background;
        public GameObject Line;
        public GameObject[] Buttons = new GameObject[2];
        public GameObject CancelButton;

        public AudioClip[] Sounds = new AudioClip[2];

        private System.Action<bool> ChoiceAction;
        /// <summary>
        /// Showing messagebox without yes, no option
        /// </summary>
        public void ShowBox(string _Title, string _Description, bool _WithCancel = true)
        {
            //Setting labels
            Title.text = _Title;
            Description.text = _Description;
            if (GameInfo.Instance != null)
            {
                Title.color = GameInfo.Instance.Game_Color;
                Description.color = GameInfo.Instance.Game_Color;
            }
            //Setting Cancel
            CancelButton.SetActive(_WithCancel);
            //Hiding Buttons
            for (int i = 0; i < Buttons.Length; i++)
                Buttons[i].SetActive(false);
            //Resizing text
            Description.GetComponent<RectTransform>().offsetMin = new Vector2(Description.GetComponent<RectTransform>().offsetMin.x, 200f);
            //Animation
            EjectAnimation();
        }
        /// <summary>
        /// Showing messagebox with yes, no option
        /// </summary>
        public void ShowBox(string _Title, string _Description, System.Action<bool> _ChoiceAction, bool _WithCancel = true)
        {
            //Setting labels
            Title.text = _Title;
            Description.text = _Description;
            if (GameInfo.Instance != null)
            {
                Title.color = GameInfo.Instance.Game_Color;
                Description.color = GameInfo.Instance.Game_Color;
            }
            ChoiceAction = _ChoiceAction;
            //Setting Cancel
            CancelButton.SetActive(_WithCancel);
            //Animation
            EjectAnimation();
        }
        public void HideBox(bool DontDestroy = false)
        {
            InsertAnimation(!DontDestroy);
        }

        #region Buttons
        public void Button_Yes()
        {

            Sound_Click();
            ChoiceAction?.Invoke(true);
        }
        public void Button_No()
        {
            Sound_Click();
            ChoiceAction?.Invoke(false);
        }
        public void Button_AntiClick()
        {
            Sound_Failed_Click();
            //Shaking
            LeanTween.rotateLocal(gameObject, new Vector3(0, 0, -2), 0.04f).setEaseInOutCubic().setOnComplete(() =>
               {
                   LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 2), 0.08f).setEaseInOutCubic().setOnComplete(() =>
                   {
                       LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 0), 0.04f).setEaseInOutCubic();
                   });
               });
        }
        public void Button_Cancel()
        {
            Sound_Click();
            //Cancel
            HideBox();
        }
        #endregion Buttons
        #region Sounds
        private void Sound_Click()
        {
            GetComponent<AudioSource>().PlayOneShot(Sounds[0]);
        }
        private void Sound_Failed_Click()
        {
            GetComponent<AudioSource>().PlayOneShot(Sounds[1],0.2f);
        }
        private void Sound_Ejecting()
        {
            GetComponent<AudioSource>().PlayOneShot(Sounds[2]);
        }
        #endregion Sounds
        //PRIVATE
        private void EjectAnimation()
        {
            //-----Setting
            float SavedBackgroundSize = Background.GetComponent<RectTransform>().sizeDelta.x;
            float SavedLineAlpha = Line.GetComponent<Image>().color.a;
            float[] SavedTextImageAlpha = new float[Buttons.Length];
            for (int i = 0; i < Buttons.Length; i++)
                SavedTextImageAlpha[i] = Buttons[i].GetComponent<Image>().color.a;
            float CancelSavedTextImageAlpha = CancelButton.GetComponent<Image>().color.a;
            Background.GetComponent<RectTransform>().sizeDelta = new Vector2(0, Background.GetComponent<RectTransform>().sizeDelta.y);
            Line.GetComponent<Image>().color = new Color(Line.GetComponent<Image>().color.r, Line.GetComponent<Image>().color.g, Line.GetComponent<Image>().color.b, 0);
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].GetComponent<Image>().color = new Color(Buttons[i].GetComponent<Image>().color.r, Buttons[i].GetComponent<Image>().color.g, Buttons[i].GetComponent<Image>().color.b, 0);
                Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color = new Color(Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color.r, Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color.g, Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color.b, 0);
            }
            CancelButton.GetComponent<Image>().color = new Color(CancelButton.GetComponent<Image>().color.r, CancelButton.GetComponent<Image>().color.g, CancelButton.GetComponent<Image>().color.b, 0);
            CancelButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(CancelButton.GetComponentInChildren<TextMeshProUGUI>().color.r, CancelButton.GetComponentInChildren<TextMeshProUGUI>().color.g, CancelButton.GetComponentInChildren<TextMeshProUGUI>().color.b, 0);
            Title.color = new Color(Title.color.r, Title.color.g, Title.color.b, 0);
            Description.color = new Color(Description.color.r, Description.color.g, Description.color.b, 0);
            gameObject.SetActive(true);
            Sound_Ejecting();
            //-----Run
            LeanTween.value(gameObject, 0f, 1f, 0.4f).setOnUpdate((float val) =>
            {
                Background.GetComponent<RectTransform>().sizeDelta = new Vector2(val * SavedBackgroundSize, Background.GetComponent<RectTransform>().sizeDelta.y);
                Line.GetComponent<Image>().color = new Color(Line.GetComponent<Image>().color.r, Line.GetComponent<Image>().color.g, Line.GetComponent<Image>().color.b, val * SavedLineAlpha);
                for (int i = 0; i < Buttons.Length; i++)
                {
                    Buttons[i].GetComponent<Image>().color = new Color(Buttons[i].GetComponent<Image>().color.r, Buttons[i].GetComponent<Image>().color.g, Buttons[i].GetComponent<Image>().color.b, val * SavedTextImageAlpha[i]);
                    Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color = new Color(Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color.r, Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color.g, Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color.b, val);
                }
                CancelButton.GetComponent<Image>().color = new Color(CancelButton.GetComponent<Image>().color.r, CancelButton.GetComponent<Image>().color.g, CancelButton.GetComponent<Image>().color.b, val * CancelSavedTextImageAlpha);
                CancelButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(CancelButton.GetComponentInChildren<TextMeshProUGUI>().color.r, CancelButton.GetComponentInChildren<TextMeshProUGUI>().color.g, CancelButton.GetComponentInChildren<TextMeshProUGUI>().color.b, val);
                Title.color = new Color(Title.color.r, Title.color.g, Title.color.b, val);
                Description.color = new Color(Description.color.r, Description.color.g, Description.color.b, val);
            });
        }
        private void InsertAnimation(bool DestroyAfter)
        {
            //-----Setting
            float SavedBackgroundSize = Background.GetComponent<RectTransform>().sizeDelta.x;
            float SavedLineAlpha = Line.GetComponent<Image>().color.a;
            float[] SavedTextImageAlpha = new float[Buttons.Length];
            for (int i = 0; i < Buttons.Length; i++)
                SavedTextImageAlpha[i] = Buttons[i].GetComponent<Image>().color.a;
            float CancelSavedTextImageAlpha = CancelButton.GetComponent<Image>().color.a;
            Background.GetComponent<RectTransform>().sizeDelta = new Vector2(0, Background.GetComponent<RectTransform>().sizeDelta.y);
            Line.GetComponent<Image>().color = new Color(Line.GetComponent<Image>().color.r, Line.GetComponent<Image>().color.g, Line.GetComponent<Image>().color.b, 0);
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].GetComponent<Image>().color = new Color(Buttons[i].GetComponent<Image>().color.r, Buttons[i].GetComponent<Image>().color.g, Buttons[i].GetComponent<Image>().color.b, 0);
                Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color = new Color(Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color.r, Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color.g, Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color.b, 0);
            }
            CancelButton.GetComponent<Image>().color = new Color(CancelButton.GetComponent<Image>().color.r, CancelButton.GetComponent<Image>().color.g, CancelButton.GetComponent<Image>().color.b, 0);
            CancelButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(CancelButton.GetComponentInChildren<TextMeshProUGUI>().color.r, CancelButton.GetComponentInChildren<TextMeshProUGUI>().color.g, CancelButton.GetComponentInChildren<TextMeshProUGUI>().color.b, 0);
            Title.color = new Color(Title.color.r, Title.color.g, Title.color.b, 0);
            Description.color = new Color(Description.color.r, Description.color.g, Description.color.b, 0);
            //-----Run
            LeanTween.value(gameObject, 1f, 0f, 0.4f).setOnUpdate((float val) =>
            {
                Background.GetComponent<RectTransform>().sizeDelta = new Vector2(val * SavedBackgroundSize, Background.GetComponent<RectTransform>().sizeDelta.y);
                Line.GetComponent<Image>().color = new Color(Line.GetComponent<Image>().color.r, Line.GetComponent<Image>().color.g, Line.GetComponent<Image>().color.b, val * SavedLineAlpha);
                for (int i = 0; i < Buttons.Length; i++)
                {
                    Buttons[i].GetComponent<Image>().color = new Color(Buttons[i].GetComponent<Image>().color.r, Buttons[i].GetComponent<Image>().color.g, Buttons[i].GetComponent<Image>().color.b, val * SavedTextImageAlpha[i]);
                    Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color = new Color(Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color.r, Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color.g, Buttons[i].GetComponentInChildren<TextMeshProUGUI>().color.b, val);
                }
                CancelButton.GetComponent<Image>().color = new Color(CancelButton.GetComponent<Image>().color.r, CancelButton.GetComponent<Image>().color.g, CancelButton.GetComponent<Image>().color.b, val * CancelSavedTextImageAlpha);
                CancelButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(CancelButton.GetComponentInChildren<TextMeshProUGUI>().color.r, CancelButton.GetComponentInChildren<TextMeshProUGUI>().color.g, CancelButton.GetComponentInChildren<TextMeshProUGUI>().color.b, val);
                Title.color = new Color(Title.color.r, Title.color.g, Title.color.b, val);
                Description.color = new Color(Description.color.r, Description.color.g, Description.color.b, val);
            }).setOnComplete(()=> { gameObject.SetActive(false); if (DestroyAfter) Destroy(gameObject); });
        }
    }
}

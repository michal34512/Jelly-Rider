using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace GameScene
{
    public class YouWin_Card : MonoBehaviour
    {
        static public YouWin_Card Instance;
        public List<Image> ColoredImages;
        [HideInInspector] public bool Ejected = false;
        private float SavedHeight = 0;
        class SavedImage
        {
            public Image _im;
            public float _alfa;
            public SavedImage(Image im, float alfa)
            {
                _im = im;
                _alfa = alfa;
            }
        }
        class SavedText
        {
            public TextMeshProUGUI _im;
            public float _alfa;
            public SavedText(TextMeshProUGUI im, float alfa)
            {
                _im = im;
                _alfa = alfa;
            }
        }
        private List<SavedImage> _SavedImage = new List<SavedImage>();
        private List<SavedText> _SavedText = new List<SavedText>();

        #region Interaction
        public void Eject()
        {
            if (!Ejected)
            {
                gameObject.SetActive(true);
                EjectAnimation();
                Ejected = true;
            }
        }
        public void Hide()
        {
            if (Ejected)
            {
                UpdateLists();
                HideAnimation();
                Ejected = false;
            }
        }
        public void HideImmidiately()
        {
            if (Ejected)
            {
                UpdateLists();
                InstantHide();
                Ejected = false;
            }
        }
        #endregion Interaction
        private void EjectAnimation()
        {
            LeanTween.value(gameObject, 0f, 1f, 0.2f).setOnUpdate((float val) =>
            {
                GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, val * SavedHeight);
                foreach (SavedImage si in _SavedImage)
                {
                    si._im.color = new Color(si._im.color.r, si._im.color.g, si._im.color.b, val * si._alfa);
                }
                foreach(SavedText st in _SavedText)
                {
                    st._im.color = new Color(st._im.color.r, st._im.color.g, st._im.color.b, val * st._alfa);
                }
            }).setEaseOutCubic().setOnComplete(()=> 
            {
                GetComponentInChildren<YouWin_Card_Stars>().UpdateStars();
            });
        }
        private void HideAnimation()
        {
            LeanTween.value(gameObject, 1f, 0f, 0.2f).setOnUpdate((float val) =>
            {
                GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, val * SavedHeight);
                foreach (SavedImage si in _SavedImage)
                {
                    si._im.color = new Color(si._im.color.r, si._im.color.g, si._im.color.b, val * si._alfa);
                }
                foreach (SavedText st in _SavedText)
                {
                    st._im.color = new Color(st._im.color.r, st._im.color.g, st._im.color.b, val * st._alfa);
                }
            }).setEaseOutCubic().setOnComplete(()=> { gameObject.SetActive(false); });
        }
        private void InstantHide()
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 0);
            foreach (SavedImage si in _SavedImage)
            {
                si._im.color = new Color(si._im.color.r, si._im.color.g, si._im.color.b, 0f);
            }
            foreach (SavedText st in _SavedText)
            {
                st._im.color = new Color(st._im.color.r, st._im.color.g, st._im.color.b, 0f);
            }
            gameObject.SetActive(false);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, 0f);
        }
        private void UpdateLists()
        {
            foreach (SavedImage im in _SavedImage)
                im._alfa = im._im.color.a;
            foreach (SavedText tx in _SavedText)
                tx._alfa = tx._im.color.a;
        }
        private void Start()
        {
            //setting up
            foreach (Image im in ColoredImages)
                im.color = new Color(GameInfo.Instance.Game_Color.r, GameInfo.Instance.Game_Color.g, GameInfo.Instance.Game_Color.b, 0.7f);
            SavedHeight = GetComponent<RectTransform>().sizeDelta.y;
            foreach (Image im in gameObject.GetComponentsInChildren<Image>())
                _SavedImage.Add(new SavedImage(im, im.color.a));
            foreach (TextMeshProUGUI tx in gameObject.GetComponentsInChildren<TextMeshProUGUI>())
                _SavedText.Add(new SavedText(tx, tx.color.a));
            InstantHide();
        }
        private void Awake()
        {
            if (Instance == null&&(GameInfo.Instance!=null&&(GameInfo.Instance.gamemode == GameInfo._GameMode.Level|| GameInfo.Instance.gamemode == GameInfo._GameMode.Tutorial)))
                Instance = this;
            else Destroy(gameObject);
        }
    }
}


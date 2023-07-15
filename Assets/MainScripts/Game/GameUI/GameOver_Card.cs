using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace GameScene
{
    public class GameOver_Card : MonoBehaviour
    {
        static public GameOver_Card Instance;
        public List<Image> ColoredImages;
        public TextMeshProUGUI ConstantModePointsCount;
        public GameObject HighScorePrefab;
        private TextMeshProUGUI HighScoreText;
        [HideInInspector] public bool Ejected = false;
        private float SavedHeight = 0;

        //PointCounter
        private float SavedPointCounterAlpha = 0;
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
                if (GameInfo.Instance != null && GameInfo.Instance.gamemode == GameInfo._GameMode.Constant)
                {
                    ConstantModePointsCount.text = "Your score: " + GameInfo.Instance.ConstantModePoints.ToString();
                }
                EjectAnimation();
                Ejected = true;
            }
        }
        public void Hide()
        {
            if (Ejected)
            {
                HideAnimation();
                Ejected = false;
            }
        }
        public void HideImmidiately()
        {
            if (Ejected)
            {
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
                    if(si._im!=null)
                        si._im.color = new Color(si._im.color.r, si._im.color.g, si._im.color.b, val * si._alfa);
                }
                foreach(SavedText st in _SavedText)
                {
                    if (st._im != null)
                        st._im.color = new Color(st._im.color.r, st._im.color.g, st._im.color.b, val * st._alfa);
                }
                //HIDING POINTCOUNT
                if (PointsCounter.Instance!=null)
                    PointsCounter.Instance.GetComponent<TextMeshProUGUI>().color = new Color(PointsCounter.Instance.GetComponent<TextMeshProUGUI>().color.r, PointsCounter.Instance.GetComponent<TextMeshProUGUI>().color.g, PointsCounter.Instance.GetComponent<TextMeshProUGUI>().color.b, (1-val) * SavedPointCounterAlpha); 
            }).setEaseOutCubic().setOnComplete(()=> 
            {
                if (GameInfo.Instance != null && GameInfo.Instance.gamemode == GameInfo._GameMode.Constant)
                {
                    if (GameInfo.Instance.ConstantModePoints > PlayerPrefs.GetInt("BestScore"))
                    {
                        PlayerPrefs.SetInt("BestScore", GameInfo.Instance.ConstantModePoints);
                        HighScoreText = Instantiate(HighScorePrefab, transform).GetComponent<TextMeshProUGUI>();
                        LeanTween.value(gameObject, 0f, 1f, 0.2f).setOnUpdate((float val) =>
                        {
                            HighScoreText.color = new Color(HighScoreText.color.r, HighScoreText.color.g, HighScoreText.color.b, val);
                        });
                    }
                }
            });
        }
        private void HideAnimation()
        {
            LeanTween.value(gameObject, 1f, 0f, 0.2f).setOnUpdate((float val) =>
            {
                if (HighScoreText != null)
                    HighScoreText.color = new Color(HighScoreText.color.r, HighScoreText.color.g, HighScoreText.color.b, val);
                GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, val * SavedHeight);
                foreach (SavedImage si in _SavedImage)
                {
                    if(si._im != null)
                        si._im.color = new Color(si._im.color.r, si._im.color.g, si._im.color.b, val * si._alfa);
                }
                foreach (SavedText st in _SavedText)
                {
                    if (st._im != null)
                        st._im.color = new Color(st._im.color.r, st._im.color.g, st._im.color.b, val * st._alfa);
                }
                if (PointsCounter.Instance != null)
                    PointsCounter.Instance.GetComponent<TextMeshProUGUI>().color = new Color(PointsCounter.Instance.GetComponent<TextMeshProUGUI>().color.r, PointsCounter.Instance.GetComponent<TextMeshProUGUI>().color.g, PointsCounter.Instance.GetComponent<TextMeshProUGUI>().color.b, (1 - val) * SavedPointCounterAlpha);
            }).setEaseOutCubic().setOnComplete(()=> 
            {
                if (HighScoreText != null)
                    Destroy(HighScoreText.gameObject);
                gameObject.SetActive(false); 
            });
        }
        private void InstantHide()
        {
            if(HighScoreText!=null)
                Destroy(HighScoreText.gameObject);
            GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 0);
            foreach (SavedImage si in _SavedImage)
            {
                if (si._im != null)
                    si._im.color = new Color(si._im.color.r, si._im.color.g, si._im.color.b, 0f);
            }
            foreach (SavedText st in _SavedText)
            {
                if (st._im != null)
                    st._im.color = new Color(st._im.color.r, st._im.color.g, st._im.color.b, 0f);
            }
            gameObject.SetActive(false);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, 0f);
        }
        private void Start()
        {
            //setting up
            if(GameInfo.Instance!=null)
                foreach (Image im in ColoredImages)
                    im.color = new Color(GameInfo.Instance.Game_Color.r, GameInfo.Instance.Game_Color.g, GameInfo.Instance.Game_Color.b, 0.7f);
            SavedHeight = GetComponent<RectTransform>().sizeDelta.y;
            foreach (Image im in gameObject.GetComponentsInChildren<Image>())
                _SavedImage.Add(new SavedImage(im, im.color.a));
            foreach (TextMeshProUGUI tx in gameObject.GetComponentsInChildren<TextMeshProUGUI>())
                _SavedText.Add(new SavedText(tx, tx.color.a));
            InstantHide();
            if (GameInfo.Instance != null && GameInfo.Instance.gamemode != GameInfo._GameMode.Constant)
                    ConstantModePointsCount.gameObject.SetActive(false);
            if (PointsCounter.Instance != null)
                SavedPointCounterAlpha = PointsCounter.Instance.GetComponent<TextMeshProUGUI>().color.a;
        }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }
    }
}


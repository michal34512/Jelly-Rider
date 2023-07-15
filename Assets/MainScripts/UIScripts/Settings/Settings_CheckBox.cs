using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIScene
{
    public class Settings_CheckBox : MonoBehaviour
    {
        public TextMeshProUGUI Text;
        public GameObject ClickArea;
        public Image CheckMark;
        
        private bool isChecked = false;
        private System.Action<bool> ClickedAction;
        public void Show(float YOffset, string _text, System.Action<bool> _ClickedAction, bool _isChecked = false)
        {
            Text.text = _text;
            if (GameInfo.Instance != null)
                Text.color = GameInfo.Instance.Game_Color;
            isChecked = _isChecked;
            ClickedAction = _ClickedAction;
            UpdateMark();
            //Centre text
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(gameObject.GetComponent<RectTransform>().anchoredPosition.x, YOffset);
            Text.ForceMeshUpdate();
            float offset = Text.textBounds.size.x / 2f - CheckMark.GetComponentInParent<RectTransform>().sizeDelta.x / 2f;
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(offset, gameObject.GetComponent<RectTransform>().anchoredPosition.y);
            //SetClickArea
            ClickArea.GetComponent<RectTransform>().anchoredPosition = new Vector2(-offset, ClickArea.GetComponent<RectTransform>().anchoredPosition.y);
            ClickArea.GetComponent<RectTransform>().sizeDelta = new Vector2(Text.textBounds.size.x + CheckMark.GetComponentInParent<RectTransform>().sizeDelta.x + 60f, ClickArea.GetComponent<RectTransform>().sizeDelta.y);
        }
        private void UpdateMark()
        {
            CheckMark.gameObject.SetActive(isChecked);
        }
        private void OnEnable()
        {
            if (GameInfo.Instance != null)
                Text.color = GameInfo.Instance.Game_Color;
        }
        #region Buttons
        public void Button_CheckBox()
        {
            GetComponent<AudioSource>().Play();
            isChecked = !isChecked;
            UpdateMark();
            ClickedAction?.Invoke(isChecked);
        }
        #endregion Buttons

    }
}


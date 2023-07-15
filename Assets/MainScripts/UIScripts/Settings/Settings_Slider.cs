using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIScene
{
    public class Settings_Slider : MonoBehaviour
    {
        public Slider UnitySlider;
        public TextMeshProUGUI AboveText;
        public TextMeshProUGUI SideText;
        private string SideTextString;
        public Image Handle;
        public Image FilledArea;

        private System.Action<float> ChangedValue;
        
        public void Show(float YOffset, string _AboveText, string _SideText, System.Action<float> _OnChangedValue, float _DefaultValue)
        {
            //Action
            UnitySlider.onValueChanged.AddListener(ValueUpdate);
            ChangedValue = _OnChangedValue;
            //Text
            if (_AboveText != "")
            {
                AboveText.text = _AboveText;
            }
            if (_SideText != "")
            {
                SideTextString = _SideText;
                SideText.text = string.Format(SideTextString, _DefaultValue);
            }
            //Color
            if (GameInfo.Instance!=null)
            {
                FilledArea.color = GameInfo.Instance.Game_Color;
                //dim color for handle
                Handle.color = HandleColor(0.1f);
            }
            //Value
            UnitySlider.value = _DefaultValue;
            //Centre
            GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, YOffset);
            AboveText.ForceMeshUpdate();
            SideText.ForceMeshUpdate();
            Vector2 offset = Vector2.zero;
            if (_AboveText != "")//                                 half of slide size     gap
                offset.y = -0.5f*(0.5f*AboveText.textBounds.size.y +       25      +        40 );
            offset.x = SideText.textBounds.size.x / 2f;// - UnitySlider.GetComponentInParent<RectTransform>().sizeDelta.x / 2f;
            UnitySlider.GetComponent<RectTransform>().anchoredPosition = offset;
            AboveText.GetComponent<RectTransform>().anchoredPosition = new Vector2(AboveText.GetComponent<RectTransform>().anchoredPosition.x, -offset.y);
        }
        public void ValueUpdate(float val)
        {
            //Text Change
            SideText.text = string.Format(SideTextString, (int)(val*100));
            ChangedValue?.Invoke(val);
        }
        private Color HandleColor(float _DimPercent)
        {
            Color result = new Color
                (Mathf.Max(GameInfo.Instance.Game_Color.r - _DimPercent, 0f),
                 Mathf.Max(GameInfo.Instance.Game_Color.g - _DimPercent, 0f),
                 Mathf.Max(GameInfo.Instance.Game_Color.b - _DimPercent, 0f),
                 1f
                );
            return result;
        }
        private void OnEnable()
        {
            //Color
            if (GameInfo.Instance != null)
            {
                FilledArea.color = GameInfo.Instance.Game_Color;
                //dim color for handle
                Handle.color = HandleColor(0.1f);
            }
        }
    }
}

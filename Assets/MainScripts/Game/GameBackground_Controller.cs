using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene
{
    public class GameBackground_Controller : Background_Controller
    {
        public GameBackground_Effect BackgroundEffect;
        private Texture2D BackgroundTexture;
        private Sprite BackgroundSprite;
        private void Upscale_Texture(Texture2D tex,int SizeY)
        {
            if(BackgroundTexture.width != 1 || BackgroundTexture.height != SizeY || BackgroundTexture == null)
                BackgroundTexture = new Texture2D(1, SizeY, TextureFormat.ARGB32, false);
            float offset = Random.Range(0.6f, 0.8f);
            for(int j = 0; j < SizeY; j++) // y
            {
                Vector2 InputFloatPosition = new Vector2(0f, (float) j / (float)SizeY * offset);
                Color res = tex.GetPixelBilinear(InputFloatPosition.x, InputFloatPosition.y);
                BackgroundTexture.SetPixel(0,j, res);
            }
            BackgroundTexture.Apply();
        }
        public override void Update_Background(Sprite Background)
        {
            BackgroundTexture = Background.texture;
            Upscale_Texture(BackgroundTexture, (int)(Screen.height * 0.5f));
            //gameObject.GetComponent<RectTransform>().localScale = new Vector2(Screen.width, 20f);
            BackgroundTexture.filterMode = FilterMode.Point;
            BackgroundSprite = Sprite.Create(BackgroundTexture, new Rect(0.0f, 0.0f, BackgroundTexture.width, BackgroundTexture.height), new Vector2(0.5f, 0.5f), 100f); // maybe optional
            GetComponent<Image>().sprite = BackgroundSprite;
        }
        public void Update_Background_Effect()
        {
            BackgroundEffect.Update_Background(BackgroundTexture);
        }
        private void OnEnable()
        {
            if (GameInfo.Instance != null)
            {
                BackgroundTexture = GameInfo.Instance.Get_Background().texture;
                Upscale_Texture(BackgroundTexture, (int)(Screen.height * 0.5f));
                //gameObject.GetComponent<RectTransform>().localScale = new Vector2(Screen.width,20f);
                BackgroundTexture.filterMode = FilterMode.Point;
                BackgroundSprite = Sprite.Create(BackgroundTexture, new Rect(0.0f, 0.0f, BackgroundTexture.width, BackgroundTexture.height), new Vector2(0.5f, 0.5f), 100f);
                GetComponent<Image>().sprite = BackgroundSprite;
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene
{
    public class GameBackground_Effect : MonoBehaviour
    {
        Texture2D BackgroundTexture;
        public void Update_Background(Texture2D Background) //with effect
        {
            BackgroundTexture = Background;
            var im = GetComponent<Image>();
            im.sprite = Sprite.Create(BackgroundTexture, new Rect(0.0f, 0.0f, BackgroundTexture.width, BackgroundTexture.height), new Vector2(0.5f, 0.5f), 100f);
            im.type = Image.Type.Filled;
            im.fillMethod = Image.FillMethod.Radial360;
            im.fillClockwise = true;
            im.fillAmount = 1f;
            gameObject.SetActive(true);
            //im.fillOrigin = 0;
            LeanTween.value(gameObject, 1f, 0f, 1f).setOnUpdate((float val) =>
            {
                im.fillAmount = val;
            }).setOnComplete(() => { gameObject.SetActive(false); });
        }
    }
}


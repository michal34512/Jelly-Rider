using GameScene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameScene
{
    public class Jumping_Platform_Controller : MonoBehaviour
    {
        private readonly float ToSize = 3f;
        private float SavedYPos = 0;
        public float PlatformForce = 1;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == 8 && !LeanTween.isTweening(gameObject)) //Jelly&Wheels
            {
                SavedYPos = transform.localPosition.y;
                GameJelly.Instance.HitJumpPlatform(transform.up,PlatformForce);
                InGameUIController.Instance.Sound_Jumping_Platform();
                LeanTween.value(gameObject, 1f, ToSize, 0.05f).setOnUpdate((float val) =>
                   {
                       transform.localScale = new Vector3(transform.localScale.x, val, transform.localScale.z);
                       transform.localPosition = new Vector3(transform.localPosition.x, SavedYPos + (val - 1) * 0.5f * GetComponent<SpriteRenderer>().size.y, transform.localPosition.z);
                   }).setOnComplete(() =>
                   {
                       LeanTween.value(gameObject, ToSize, 1f, 0.05f).setOnUpdate((float val) =>
                       {
                           transform.localScale = new Vector3(transform.localScale.x, val, transform.localScale.z);
                           transform.localPosition = new Vector3(transform.localPosition.x, SavedYPos + (val - 1) * 0.5f * GetComponent<SpriteRenderer>().size.y, transform.localPosition.z);
                       });
                   });
            }
        }
    }
}


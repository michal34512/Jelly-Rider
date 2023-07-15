using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UIScene
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class UI_Main_Jelly : MonoBehaviour
    {
        public static UI_Main_Jelly Instance;
        private readonly float DelayTime = 6f;
        private bool isAnimating = false;
        private float Zegar = 0;
        private void AnimationUpdate()
        {
            if(isAnimating)
            {
                Zegar += Time.deltaTime;
                if(Zegar > DelayTime)
                {
                    Zegar = 0f;
                    //JUMP
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0,400));
                    LeanTween.delayedCall(gameObject, 0.1f, () => { GetComponent<Rigidbody2D>().AddTorque(Random.value > 0.5f ? 63.5f : -63.5f); });
                }
            }
        }

        public void UpdateJellySprite()
        {
            GetComponent<Image>().sprite = GameInfo.Instance.Jellys[GameInfo.Instance.PickedJelly].MainSprite;
        }
        public void StartAnimation()
        {
            //velocity
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().angularVelocity = 0f;

            LeanTween.cancel(gameObject);
            transform.position = new Vector3(0, 0, transform.position.z);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            isAnimating = true;
            Zegar = 0f;
        }
        public void StopAnimation()
        {
            isAnimating = false;
            LeanTween.cancel(gameObject);
        }
        private void Update()
        {
            AnimationUpdate();
        }
        private void Start()
        {
            StartAnimation();
            UpdateJellySprite();
        }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }
    }
}


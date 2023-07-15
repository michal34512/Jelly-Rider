using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameScene
{
    public class WinArea : MonoBehaviour
    {
        public static WinArea Instance;
        public float[] TimeBorders = { 0,0,0}; 

        private float _GameTime;
        public float GameTime
        {
            get
            {
                return _GameTime;
            }
        }
        private bool isCounting = false;
        public void Reset_Game_Time()
        {
            _GameTime = 0f;
            isCounting = true;
        }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }
        private void Update()
        {
            if(isCounting)
                _GameTime += Time.deltaTime;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.layer == 8) // Jelly & weels
            {
                isCounting = false;
            }
        }

    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameScene
{
    public class WinArea : MonoBehaviour
    {
        public static WinArea Instance;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class SuggestionExclamationMark : MonoBehaviour
    {
        void Start()
        {
            //Destroy(gameObject);
            LeanTween.scale(gameObject, new Vector3(0.7f,0.7f,1f), 1f).setEaseOutQuad().setLoopPingPong();
        }
    }
}

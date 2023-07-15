using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugestionArrow : MonoBehaviour
{
    void Start()
    {
        //Destroy(gameObject);
        LeanTween.moveLocal(gameObject, transform.localPosition+transform.right, 0.5f).setEaseOutQuad().setLoopPingPong();
    }
}

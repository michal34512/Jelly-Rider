using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirJumpAnimationStarter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ParticleSystem>().Simulate(10);
        GetComponent<ParticleSystem>().Play();
    }
}

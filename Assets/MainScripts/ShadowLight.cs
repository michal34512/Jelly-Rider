using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowLight : MonoBehaviour
{
    public static ShadowLight Instance;
    [HideInInspector]
    public bool isCameraMoved = true;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Update()
    {
        transform.position = new Vector3(Camera.main.transform.position.x,transform.position.y, 0);
    }
}

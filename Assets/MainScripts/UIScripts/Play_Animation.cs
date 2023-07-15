using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UIScene
{
public class Play_Animation : MonoBehaviour
{
    public static Play_Animation Instance;
    private readonly float range = 5;
    public void StartAnimation()
    {
        LeanTween.value(gameObject, range, -range, 4f).setOnUpdate((float val) =>
               {
                   gameObject.transform.rotation = Quaternion.Euler(0, 0, val);
               }).setLoopPingPong();
    }
    public void StopAnimation()
    {
        LeanTween.cancel(gameObject);
    }
    private void Start()
    {
        StartAnimation();
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(gameObject);
    }
}
}


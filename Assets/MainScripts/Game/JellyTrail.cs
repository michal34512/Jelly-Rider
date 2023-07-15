using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyTrail : MonoBehaviour
{
    public static JellyTrail Instance;
    float DefaultTrailTime;
    public void StartTrailing()
    {
        gameObject.SetActive(true);
        GetComponent<TrailRenderer>().emitting = true;
        GetComponent<TrailRenderer>().time = DefaultTrailTime;
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, DefaultTrailTime, 0f, 1f).setOnUpdate((float val) => { GetComponent<TrailRenderer>().time = val; }).setOnComplete(() => { GetComponent<TrailRenderer>().emitting = false; });
    }
    public void StopTrailing()
    {
        gameObject.SetActive(false);
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //Setting up
            DefaultTrailTime =  GetComponent<TrailRenderer>().time;
        }
        else Destroy(gameObject);
    }
}

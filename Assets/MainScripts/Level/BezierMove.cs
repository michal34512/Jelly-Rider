using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMove : MonoBehaviour
{
    public bool _isLooping = true;
    public bool _reverseAfterDelay = false;
    public bool _DontDestroyTrigger = false;
    public bool isLooping 
    {
        get
        {
            return _isLooping;
        }
        set
        {
            _isLooping = value;
            StopAnimation();
            StartAnimating();
        }
    }

    public Collider2D Trigger;
    public float TriggerDelay;

    public bool isMoving = false;
    public Vector2[] MoveSteps = new Vector2[2];
    public bool isRotating = false;
    public float[] RotationSteps = new float[2];
    public float TimeBetween;
    public float TimeToReturn = 1f;
    public LeanTweenType TypeA;
    public LeanTweenType TypeB;
    public LeanTweenType LoopType;
    void UpdateStep(float val)
    {
        if (isMoving)
        {
            Vector2 MoveDifference = MoveSteps[1] - MoveSteps[0];
            gameObject.transform.localPosition = new Vector3(MoveDifference.x * val + MoveSteps[0].x, MoveDifference.y * val + MoveSteps[0].y, gameObject.transform.localPosition.z);
        }
        if (isRotating)
        {
            float RotateDifference = RotationSteps[1] - RotationSteps[0];
            gameObject.transform.rotation = Quaternion.Euler(0, 0, RotateDifference * val + RotationSteps[0]);
        }
    }
    public void StartAnimating()
    {
        if(!LeanTween.isTweening(gameObject))
        {

            if (_isLooping)
                LeanTween.value(gameObject, 0, 1, TimeBetween).setOnUpdate(UpdateStep).setEase(TypeA).setLoopType(LoopType);//.setLoopPingPong();
            else
                LeanTween.value(gameObject, 0, 1, TimeBetween).setOnUpdate(UpdateStep).setOnComplete(() => { LeanTween.delayedCall(TimeToReturn, StartReverseAnimation); }).setEase(TypeA);
        }
    }
    public void StartReverseAnimation()
    {
        if(this!=null)
        if (!LeanTween.isTweening(gameObject))
        {
            if (_isLooping)
                LeanTween.value(gameObject, 1, 0, TimeBetween).setOnUpdate(UpdateStep).setEase(TypeB).setLoopType(LoopType);
            else
                LeanTween.value(gameObject, 1, 0, TimeBetween).setOnUpdate(UpdateStep).setEase(TypeB);
        }
    }
    public void StopAnimation()
    {
        if (LeanTween.isTweening(gameObject))
            LeanTween.cancel(gameObject);
    }
    class BezierMoveTrigger : MonoBehaviour
    {
        System.Action CallBack;
        bool DontDestroy = false;
        public void SetAction(System.Action _CallBack, bool _DontDestroy)
        {
            CallBack = _CallBack;
            DontDestroy = _DontDestroy;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            CallBack.Invoke();
            if(!DontDestroy)
                Destroy(gameObject);
        }
    }
    private void Awake()
    {
        //setting trigger
        if (Trigger != null)
        {
            Trigger.gameObject.AddComponent<BezierMoveTrigger>().SetAction(()=> { LeanTween.delayedCall(TriggerDelay, StartAnimating); }, _DontDestroyTrigger);
        }
        else
        {
            StartAnimating();
        }
    }
}

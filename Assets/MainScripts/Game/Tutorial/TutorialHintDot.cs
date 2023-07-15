using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameScene;

namespace TutorialLevel
{
    public class TutorialHintDot : MonoBehaviour
    {
        //STATIC
        static List<TutorialHintDot> Instance = new List<TutorialHintDot>();
        public static void GenerateSlidingDot(int id, Vector2 From, Vector2 To, float AnimationTime, GameObject DotPrefab, float Scale = 1, bool DestroyAfterOnce = false)
        {
            //Generating object
            //GameObject ga = Instantiate(DotPrefab, Camera.main.transform);
            GameObject ga = Instantiate(DotPrefab, InGameUIController.Instance.transform);
            //ga.GetComponent<LineRenderer>().widthMultiplier = ga.GetComponent<LineRenderer>().widthMultiplier * Scale;
            var thd = ga.AddComponent<TutorialHintDot>();
            Instance.Add(thd);
            thd._id = id;
            thd._From = From;
            thd._To = To;
            thd._AnimationTime = AnimationTime;
            thd._Scale = Scale;
            thd._DestroyAfterOnce = DestroyAfterOnce;
            thd._LineMultiplier = ga.GetComponent<LineRenderer>().widthMultiplier;
            thd.StartSlidingDot();
        }
        public static void GenerateBlinkingDot(int id, Vector2 Position, float AnimationTime, GameObject DotPrefab, float Scale = 1, bool DestroyAfterOnce = false)
        {
            //Generating object
            GameObject ga = Instantiate(DotPrefab, InGameUIController.Instance.transform);
            ga.transform.localPosition = new Vector3(Position.x, Position.y, ga.transform.localPosition.z);
            ga.GetComponent<LineRenderer>().enabled = false;
            //ga.GetComponent<LineRenderer>().widthMultiplier = ga.GetComponent<LineRenderer>().widthMultiplier * Scale;
            var thd = ga.AddComponent<TutorialHintDot>();
            Instance.Add(thd);
            thd._id = id;
            thd._AnimationTime = AnimationTime;
            thd._Scale = Scale;
            thd._DestroyAfterOnce = DestroyAfterOnce;
            ga.transform.localScale = new Vector3(1.5f * Scale, 1.5f * Scale, 1);
            ga.GetComponent<Image>().color = new Color(ga.GetComponent<Image>().color.r, ga.GetComponent<Image>().color.g, ga.GetComponent<Image>().color.b, 0f);
            thd.StartBlinkingDot();
        }
        public static void Cancel(int id)
        {
            for (int i = 0; i < Instance.Count; i++)
            {
                if (Instance[i]._id == id)
                {
                    LeanTween.cancel(Instance[i].gameObject);
                    Destroy(Instance[i].gameObject);
                    Instance.RemoveAt(i);
                }
            }
        }
        public static void CancelAll()
        {
            for (int i = 0; i < Instance.Count; i++)
            {
                LeanTween.cancel(Instance[i].gameObject);
                Destroy(Instance[i].gameObject);
                Instance.RemoveAt(i);
            }
        }
        //Public
        [HideInInspector] public int _id;
        [HideInInspector] public Vector2 _From, _To;
        [HideInInspector] public float _AnimationTime, _Scale, _LineMultiplier;
        [HideInInspector] public bool _DestroyAfterOnce;
        public void StartSlidingDot()
        {
            //Setting recuretion
            if (this != null)
            {
                gameObject.transform.localPosition = new Vector3(_From.x, _From.y, gameObject.transform.localPosition.z);
                gameObject.GetComponent<Image>().color = new Color(gameObject.GetComponent<Image>().color.r, gameObject.GetComponent<Image>().color.g, gameObject.GetComponent<Image>().color.b, 0);
                gameObject.transform.localScale = new Vector3(1.5f * _Scale, 1.5f * _Scale, 1);
                if (gameObject.GetComponent<TutorialTrailRenderer>() != null)
                {
                    gameObject.GetComponent<TutorialTrailRenderer>().Clear();
                }
                LeanTween.value(gameObject, 0f, 1f, 0.2f * _AnimationTime).setOnUpdate((float val) =>
                {
                    gameObject.GetComponent<Image>().color = new Color(gameObject.GetComponent<Image>().color.r, gameObject.GetComponent<Image>().color.g, gameObject.GetComponent<Image>().color.b, val);
                    gameObject.transform.localScale = new Vector3((1.5f - 0.5f*val) * _Scale, (1.5f - 0.5f * val) * _Scale, 1);
                }).setOnComplete(() =>
                {
                    LeanTween.moveLocal(gameObject, new Vector3(_To.x, _To.y, gameObject.transform.localPosition.z), _AnimationTime).setOnUpdate((float val)=> 
                    {
                        float Scale = Camera.main.orthographicSize / 5f;
                        GetComponent<LineRenderer>().widthMultiplier = _LineMultiplier * Scale; 
                    }).setOnComplete(() =>
                    {
                        if (_DestroyAfterOnce)
                            LeanTween.delayedCall(0.5f * _AnimationTime,()=> { Destroy(gameObject); });
                        else
                            LeanTween.delayedCall(0.5f * _AnimationTime, StartSlidingDot);
                    });
                });
            }
        }
        public void StartBlinkingDot()
        {
            //Setting recuretion
            if (this != null)
            {
                LeanTween.value(gameObject, 0f, 1f, 1f).setOnUpdate((float val) =>
                {
                    gameObject.GetComponent<Image>().color = new Color(gameObject.GetComponent<Image>().color.r, gameObject.GetComponent<Image>().color.g, gameObject.GetComponent<Image>().color.b, val);
                    gameObject.transform.localScale = new Vector3((1.5f - 0.5f * val) * _Scale, (1.5f - 0.5f * val) * _Scale, 1);
                }).setOnComplete(() =>
                {
                    if (_DestroyAfterOnce)
                        Destroy(gameObject);
                    else
                        LeanTween.delayedCall(_AnimationTime, StartBlinkingDot);
                });
            }
        }
    }
}

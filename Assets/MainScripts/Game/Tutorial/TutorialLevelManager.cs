using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialLevel
{
    public class TutorialLevelManager : MonoBehaviour
    {
        public static TutorialLevelManager Instance;
        public GameObject MainCanvas;
        float XAxis;
        //Jump
        private readonly float MinimumToJump = 0.6f;
        private Vector2 SavedTouch;
        
        public class Trigger : MonoBehaviour
        {
            public System.Action callbackEnter;
            public System.Action callbackExit;
            private void OnTriggerEnter2D(Collider2D collision)
            {
                callbackEnter?.Invoke();
            }
            private void OnTriggerExit2D(Collider2D collision)
            {
                callbackExit?.Invoke();
            }
        }

        public List<GameObject> TriggerGameobjects = new List<GameObject>();
        public GameObject HintDotPrefab;
        public void Restart()
        {
            TutorialHintDot.CancelAll();
            NormalTime();
            Step = 0;
            Start();
        }
        public void NormalTime()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
        private void SlowTime()
        {
            Time.timeScale = 0.2f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
        void Start()
        {
            //TutorialHintDot.GenerateBlinkingDot(0, new Vector2(0, 0), 3f, HintDotPrefab);
            //TutorialHintDot.GenerateSlidingDot(0, new Vector2(-5, 0), new Vector2(5, 0), 2f, HintDotPrefab);
            //LeanTween.delayedCall(10f, () => { TutorialHintDot.Cancel(0); });
            XAxis = 0.5f*((float)Screen.width / (float)Screen.height * 1080f) - 250f;
            for (int i = 0; i < TriggerGameobjects.Count; i++)
            {
                TriggerGameobjects[i].AddComponent<Trigger>();
            }
            LeanTween.delayedCall(2f, () =>
             {
                TutorialHintDot.GenerateBlinkingDot(0, new Vector2(XAxis, -350), 3f, HintDotPrefab, 1f);
             });
            bool[] Nxt = {false, false, false, false };
            TriggerGameobjects[0].GetComponent<Trigger>().callbackEnter = () =>
            {
                if (!Nxt[0])
                {
                    Nxt[0] = true;
                    TutorialHintDot.GenerateBlinkingDot(1, new Vector2(-XAxis, -350), 3f, HintDotPrefab, 1f);
                    Check_For_Next_Step(1);
                }
            };
            TriggerGameobjects[1].GetComponent<Trigger>().callbackEnter = () =>
            {
                if (!Nxt[1])
                {
                    Nxt[1] = true;
                    SlowTime();
                    TutorialHintDot.CancelAll();
                    TutorialHintDot.GenerateSlidingDot(2, new Vector2(XAxis, -350), new Vector2(XAxis, 350), 0.3f, HintDotPrefab, 1f);
                    Step = 3;
                    Check_For_Next_Step(3);
                }
            };
            TriggerGameobjects[2].GetComponent<Trigger>().callbackEnter = () =>
            {
                if (!Nxt[2])
                {
                    Nxt[2] = true;
                    SlowTime();
                    TutorialHintDot.GenerateSlidingDot(3, new Vector2(XAxis, -350), new Vector2(XAxis, 350), 0.3f, HintDotPrefab, 1f);
                    Check_For_Next_Step(5);
                }
            };
            TriggerGameobjects[3].GetComponent<Trigger>().callbackEnter = () =>
            {
                if (!Nxt[3])
                {
                    Nxt[3] = true;
                    SlowTime();
                    Step = 8;
                    TutorialHintDot.CancelAll();
                    TutorialHintDot.GenerateSlidingDot(5, new Vector2(XAxis, -350), new Vector2(XAxis, 350), 0.3f, HintDotPrefab, 1f);
                    Check_For_Next_Step(8);
                }
            };
            /*bool SlowedA = false;
            TriggerGameobjects[0].GetComponent<Trigger>().callbackEnter = () => 
            { 
                if(!SlowedA)
                {
                    SlowedA = true;
                    SlowTime();
                    TutorialHintDot.GenerateSlidingDot(0, new Vector2(5, -3), new Vector2(5, 3), 2f, HintDotPrefab);
                }
            };
            TriggerGameobjects[0].GetComponent<Trigger>().callbackExit = () =>
            {
                NormalTime();
            };
            bool SlowedB = false;
            TriggerGameobjects[1].GetComponent<Trigger>().callbackEnter = () =>
            {
                if (!SlowedB)
                {
                    SlowedB = true;
                    SlowTime();
                    TutorialHintDot.GenerateSlidingDot(0, new Vector2(5, -3), new Vector2(5, 3), 2f, HintDotPrefab);
                }
            };
            TriggerGameobjects[1].GetComponent<Trigger>().callbackExit = () =>
            {
                NormalTime();
            };*/
        }
        #region Controls
        private void Control_Right()
        {
            if(Check_For_Next_Step(0))
            {
                TutorialHintDot.Cancel(0);
            }
        }
        private void Control_Left()
        {
            if (Check_For_Next_Step(2))
            {
                TutorialHintDot.CancelAll();
            }
        }
        private void Control_Up()
        {
            if (Check_For_Next_Step(4))
            {
                TutorialHintDot.CancelAll();
                NormalTime();
            }
            else if (Check_For_Next_Step(6))
            {
                TutorialHintDot.CancelAll();
                LeanTween.delayedCall(0.1f, () =>
                 {
                     TutorialHintDot.GenerateSlidingDot(4, new Vector2(XAxis, 350), new Vector2(XAxis, -350), 0.3f, HintDotPrefab, 1f);
                 });
            }
            else if(Check_For_Next_Step(9))
            {
                TutorialHintDot.CancelAll();
                LeanTween.delayedCall(0.7f, () =>
                {
                    TutorialHintDot.GenerateSlidingDot(6, new Vector2(XAxis, -350), new Vector2(XAxis, 350), 0.3f, HintDotPrefab, 1f);
                });
            }
            else if (Check_For_Next_Step(10))
            {
                TutorialHintDot.CancelAll();
                NormalTime();
            }
        }
        private void Control_Down()
        {
            if (Check_For_Next_Step(7))
            {
                TutorialHintDot.Cancel(4);
                NormalTime();
            }
        }
        #endregion Controls
        int Step = 0;
        private bool Check_For_Next_Step(int _step)
        {
            if (Step == _step)
            {
                Step++;
                return true;
            }
            return false;
        }
        private void CheckControls()
        {
            if (Input.GetKey(KeyCode.RightArrow) || (Input.touchCount == 1 && Input.GetTouch(0).position.x >= Screen.width * 0.5f))
            {
                Control_Right();
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || (Input.touchCount == 1 && Input.GetTouch(0).position.x < Screen.width * 0.5f))
            {
                Control_Left();
            }
            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    SavedTouch = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    Vector2 Difference = (Vector2)Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) - SavedTouch;
                    if (Mathf.Abs(Difference.y) > Mathf.Abs(Difference.x))
                    {
                        if (Difference.y > MinimumToJump)
                        {
                            //Make a jump
                            Control_Up();
                        }
                        else if (Difference.y < -MinimumToJump)
                        {
                            //Make a touch down
                            Control_Down();
                        }
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) Control_Up();
                else if (Input.GetKeyDown(KeyCode.DownArrow)) Control_Down();
            }
        }
        private void Update()
        {
            CheckControls();
        }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }
    }
}


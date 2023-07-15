using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEngine.UI;

namespace GameScene
{
    public class GameJelly : MonoBehaviour
    {
        public class WheelController : MonoBehaviour
        {
            public Vector2 Nrml;
            private void OnCollisionEnter2D(Collision2D collision)
            {
                if (collision.gameObject.layer == 9) // obstacles
                {
                    if (collision.contacts.Length > 0)
                        Nrml = collision.contacts[0].normal;
                    GameJelly.Instance.HittedObstacle();
                }
            }
            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (collision.gameObject.layer == 9) // obstacles
                {
                    GameJelly.Instance.HittedObstacle();
                }
                else if (collision.gameObject.layer == 10) //win
                {
                    GameJelly.Instance.HitWinArea();
                }
            }
            private void OnCollisionStay2D(Collision2D collision)
            {
                if(collision.contacts.Length>0)
                    Nrml = collision.contacts[0].normal;
            }
        }
        public static GameJelly Instance;
        public GameObject DeathParticles;
        public GameObject Confetti;
        public GameObject Perfab_Dimmer;
        
        [HideInInspector]
        public bool isDead=false;

        JointMotor2D motor = new JointMotor2D();
        public float MaxSpeed= 1000f;
        public float Acceleration = 1000f;
        public float JumpRotationAcceleration = 500f;
        public float JumpForce = 8000f;
        private bool First=false;

        [HideInInspector]
        public Vector3 FirstPoint;

        //Jump
        private readonly float MinimumToJump = 1f;
        private Vector2 SavedTouch;
        private int ColliderCounter = 0;

        #region Death
        public void HittedObstacle()
        {
            if(!isDead)
            {
                isDead = true;
                Instantiate(DeathParticles, transform.position + new Vector3(0,0,0.5f), Quaternion.identity);
                JellyTrail.Instance.StopTrailing();
                LeanTween.scale(gameObject, new Vector3(0f, 0f, 1f), 0.5f).setEaseOutCubic().setOnComplete(()=> {
                    foreach (Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>())
                        rb.bodyType = RigidbodyType2D.Static;
                });
                LeanTween.delayedCall(1.3f,() => { Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() => { InGameUIController.Instance.Button_Reset();});  });
                CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
            }
        }
        #endregion Death

        public void HitWinArea()
        {
            if (!isDead)
            {
                isDead = true;
                

                Instantiate(DeathParticles, transform.position, Quaternion.identity);
                LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEaseOutCubic().setOnComplete(() =>
                {
                    foreach (Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>())
                        rb.bodyType = RigidbodyType2D.Static;
                    Instantiate(Confetti, (Vector2)Camera.main.transform.position + new Vector2(11, -7), Quaternion.Euler(0, 0, 45f), Camera.main.transform);
                    Instantiate(Confetti, (Vector2)Camera.main.transform.position + new Vector2(-11, -7), Quaternion.Euler(0, 0, -45f), Camera.main.transform);
                });
                LeanTween.delayedCall(3f, () => { InGameUIController.Instance.Button_Menu(); });
            }
        }
        public void HitJumpPlatform(Vector2 Direction)
        {
            //High Jump
            GetComponent<Rigidbody2D>().AddForce(Direction*JumpForce*1.5f);
            JellyTrail.Instance.StartTrailing();
            //GetComponent<Rigidbody2D>().angularVelocity += GetComponent<Rigidbody2D>().velocity.x * -100;
        }

        public void BackToFirstPoint()
        {
            gameObject.transform.position = FirstPoint;
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            isDead = false;
            foreach(Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>())
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            gameObject.SetActive(false);
            gameObject.GetComponentInChildren<JellyPhysic>().ResetJelly();
        }
        private void CheckControls()
        {
            void SetUseMotor(bool value)
            {
                foreach (WheelJoint2D wj in gameObject.GetComponents<WheelJoint2D>())
                {
                    wj.useMotor = value;
                    if (value)
                    {
                        wj.motor = motor;
                    }
                }
            }
            if (Input.GetKey(KeyCode.RightArrow) || (Input.touchCount==1&&Input.GetTouch(0).position.x >= Screen.width*0.5f))
            {
                //Forward
                if(!First)
                {
                    First = true;
                    motor.motorSpeed = GetComponent<WheelJoint2D>().connectedBody.angularVelocity;
                }
                if(GetComponent<Rigidbody2D>().bodyType!=RigidbodyType2D.Static)
                    GetComponent<Rigidbody2D>().angularVelocity -=JumpRotationAcceleration * Time.deltaTime;
                motor.motorSpeed -= Acceleration * Time.deltaTime;
                if (motor.motorSpeed < -MaxSpeed) motor.motorSpeed = -MaxSpeed;
                motor.maxMotorTorque = 1000;
                SetUseMotor(true);
            }
            else if(Input.GetKey(KeyCode.LeftArrow) || (Input.touchCount == 1 && Input.GetTouch(0).position.x < Screen.width * 0.5f))
            {
                if (!First)
                {
                    First = true;
                    motor.motorSpeed = GetComponent<WheelJoint2D>().connectedBody.angularVelocity;
                }
                if (GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static)
                    GetComponent<Rigidbody2D>().angularVelocity += JumpRotationAcceleration * Time.deltaTime;
                motor.motorSpeed += Acceleration * Time.deltaTime;
                if (motor.motorSpeed > MaxSpeed) motor.motorSpeed = MaxSpeed;
                motor.maxMotorTorque = 1000;
                SetUseMotor(true);
            }
            else
            {
                SetUseMotor(false);
                First = false;
            }

            void MakeAJump()
            {
                Collider2D [] col = Physics2D.OverlapCircleAll(transform.position, 0.6f);
                foreach (Collider2D _col in col)
                {
                    if (_col.gameObject.layer != 8 && _col.gameObject.layer != 11 && _col.gameObject.layer != 9 && (!_col.isTrigger || _col.gameObject.layer == 12)) // wheel
                    {
                        //JumpPossible
                        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0f);
                        if (_col.gameObject.layer == 12)
                            GetComponent<Rigidbody2D>().AddForce(_col.transform.up * JumpForce);
                        else
                            GetComponent<Rigidbody2D>().AddForce(GetComponentInChildren<WheelController>().Nrml * JumpForce);
                        GetComponent<Rigidbody2D>().angularVelocity += GetComponent<Rigidbody2D>().velocity.x * -100;
                        //Start trailing
                        JellyTrail.Instance.StartTrailing();
                        break;
                    }
                }
            }
            //Jump
            if (Input.touchCount == 1)
            {
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    SavedTouch = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                }else if(Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    Vector2 Difference = (Vector2)Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) - SavedTouch;
                    if(Mathf.Abs(Difference.y) > Mathf.Abs(Difference.x) && Difference.y > MinimumToJump)
                    {
                        //Make a jump
                        MakeAJump();
                    }
                }
            }else if(Input.GetKeyDown(KeyCode.UpArrow))
                MakeAJump();
        }

        /*#region ControllsButtons
        public void ButtonLeft()
        {
            Button b;
            
        }
        public void ButtonsRight()
        {

        }
        public void ButtonJump()
        {

        }
        #endregion ControllsButtons*/

        private void Update()
        {
            if(!isDead)
                CheckControls();
        }
        private void Start()
        {
            gameObject.SetActive(false);
        }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                FirstPoint = gameObject.transform.position;
                foreach(WheelJoint2D wj in GetComponents<WheelJoint2D>())
                {
                    wj.connectedBody.gameObject.AddComponent<WheelController>();
                }
            }
            else Destroy(gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            ColliderCounter++;
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            ColliderCounter--;
        }
    }
}


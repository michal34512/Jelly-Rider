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
            public bool AgreementForJump = false;
            private void OnCollisionEnter2D(Collision2D collision)
            {
                AgreementForJump = true;
                if (collision.gameObject.layer == 9) // obstacles
                {
                    if (collision.contacts.Length > 0)
                    {
                        Nrml = collision.contacts[0].normal;
                    }
                    GameJelly.Instance.HittedObstacle();
                }//else { Debug.Log("Hitted: "+ collision.gameObject.name); }
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
            private void OnCollisionExit2D(Collision2D collision)
            {
                AgreementForJump = false;
            }
        }
        public static GameJelly Instance;
        public GameObject DeathParticles;
        public GameObject Confetti;
        public GameObject Perfab_Dimmer;
        public GameObject JellyMaterialGameObject;

        [HideInInspector]
        public bool isDead=false;

        JointMotor2D motor = new JointMotor2D();
        public float MaxSpeed= 1000f;
        public float Acceleration = 1000f;
        public float JumpRotationAcceleration = 500f;
        public float JumpForce = 8000f;
        public float WaitingJumpTime = 0.5f;
        private float WaitingJumpClock = 0;
        private bool First=false;

        [HideInInspector]
        public Vector3 FirstPoint;

        //Jump
        private readonly float MinimumToJump = 0.6f;
        private Vector2 SavedTouch;
        //private int ColliderCounter = 0;

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
                //LeanTween.delayedCall(1.3f,() => { Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() => { InGameUIController.Instance.Button_Reset();});  });
                if (GameOver_Card.Instance != null) LeanTween.delayedCall(1.3f, () => { GameOver_Card.Instance.Eject(); });
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
                    Instantiate(Confetti, (Vector2)Camera.main.transform.position + new Vector2(10, -7), Quaternion.Euler(0, 0, 0f), Camera.main.transform); //45
                    Instantiate(Confetti, (Vector2)Camera.main.transform.position + new Vector2(-10, -7), Quaternion.Euler(0, 0, 0f), Camera.main.transform); // -45
                });
                //LeanTween.delayedCall(3f, () => { InGameUIController.Instance.Button_Menu(); });
                YouWin_Card.Instance.Eject();
            }
        }
        public void HitJumpPlatform(Vector2 Direction,float Force)
        {
            //High Jump
            Vector2 NewVelocity = Skracanie_Vectora(Direction, GetComponent<Rigidbody2D>().velocity, 0.5f) +   Direction * JumpForce * 1.5f * Force;
            GetComponent<Rigidbody2D>().velocity = NewVelocity;
            //GetComponent<Rigidbody2D>().AddForce(Direction*JumpForce*1.5f);
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
                {
                    GetComponent<Rigidbody2D>().angularVelocity -=JumpRotationAcceleration * Time.deltaTime;
                    if(!GetComponentInChildren<WheelController>().AgreementForJump && GetComponent<Rigidbody2D>().velocity.x<10)
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(0.2f, 0f),ForceMode2D.Impulse);
                }
                    
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
                {
                    GetComponent<Rigidbody2D>().angularVelocity += JumpRotationAcceleration * Time.deltaTime;
                    if (!GetComponentInChildren<WheelController>().AgreementForJump && GetComponent<Rigidbody2D>().velocity.x > -10)
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(-0.2f, 0f),ForceMode2D.Impulse);
                }
                    
                motor.motorSpeed += Acceleration * Time.deltaTime;
                if (motor.motorSpeed > MaxSpeed) motor.motorSpeed = MaxSpeed;
                motor.maxMotorTorque = 1000;
                SetUseMotor(true);
            }
            else
            {
                SetUseMotor(false);
                if (First)
                {
                    First = false;

                    JellyMaterialGameObject.GetComponent<Rigidbody2D>().sharedMaterial.friction = 0.5f;
                    JellyMaterialGameObject.GetComponent<Collider2D>().sharedMaterial.friction = 0.5f;
                    JellyMaterialGameObject.GetComponent<Rigidbody2D>().sharedMaterial = JellyMaterialGameObject.GetComponent<Rigidbody2D>().sharedMaterial;
                    JellyMaterialGameObject.GetComponent<Collider2D>().sharedMaterial = JellyMaterialGameObject.GetComponent<Collider2D>().sharedMaterial;
                }
            }
            if (First)
            {
                JellyMaterialGameObject.GetComponent<Rigidbody2D>().sharedMaterial.friction = 1f;
                JellyMaterialGameObject.GetComponent<Collider2D>().sharedMaterial.friction = 1f;
                JellyMaterialGameObject.GetComponent<Rigidbody2D>().sharedMaterial = JellyMaterialGameObject.GetComponent<Rigidbody2D>().sharedMaterial;
                JellyMaterialGameObject.GetComponent<Collider2D>().sharedMaterial = JellyMaterialGameObject.GetComponent<Collider2D>().sharedMaterial;
            }
                
            //Jump
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
                            if (!MakeAJump()) { WaitingJumpClock = 0;  }
                        }
                        else if (Difference.y < -MinimumToJump)
                        {
                            MakeATouchDown();
                        }
                    }
                }
            }
            else
            {
                if ((Input.GetKeyDown(KeyCode.UpArrow)) && !MakeAJump()) { WaitingJumpClock = 0;  }
                else if (Input.GetKeyDown(KeyCode.DownArrow)) MakeATouchDown();
            }
        }
        Vector2 Skracanie_Vectora(Vector2 Kierunkek, Vector2 Skracany, float Procent)
        {
            float Kat_Miedzy(Vector2 vecA, Vector2 vecB) // W radianach przeciwnie do wskazówek zegara
            {
                float Kat_Up(Vector2 vec)
                {
                    if (vec.x > 0)
                    {
                        return Vector2.Angle(vec, Vector2.up);
                    }
                    else
                    {
                        return 360f - Vector2.Angle(vec, Vector2.up);
                    }
                }
                float Odejmij_kat(float a, float b)
                {
                    float c = a - b;
                    if (c < 0)
                        return 360 + c;
                    else return c;
                }
                return Mathf.Deg2Rad * Odejmij_kat(Kat_Up(vecA), Kat_Up(vecB));
            }
            return (Mathf.Sin(Kat_Miedzy(Kierunkek, Skracany)) * Skracany.magnitude) * Procent * new Vector2(-Kierunkek.y, Kierunkek.x);
        }
        bool MakeAJump()
        {
            
            Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 0.5f); // wheel radius = 0.4
            foreach (Collider2D _col in col)
            {
                if (_col.gameObject.layer != 8 && _col.gameObject.layer != 11 && _col.gameObject.layer != 9 && (!_col.isTrigger || _col.gameObject.layer == 12)) // wheel
                {
                    //JumpPossible
                    GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0f);// <- resetownie y

                    //GetComponent<Rigidbody2D>().velocity = Skracanie_Vectora(GetComponentInChildren<WheelController>().Nrml, GetComponent<Rigidbody2D>().velocity);
                    GetComponent<Rigidbody2D>().angularVelocity += GetComponent<Rigidbody2D>().velocity.x * -100;
                    if (_col.gameObject.layer == 12) // Air jump
                        GetComponent<Rigidbody2D>().AddForce(_col.transform.up * JumpForce, ForceMode2D.Impulse);
                    else
                    {
                        Vector2 fixedNormal = ((Vector2)transform.position - Physics2D.ClosestPoint(transform.position, _col)).normalized;
                        Vector2 fixedVel = fixedNormal * JumpForce;// *0.513f;
                        foreach (Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>())//GetComponentInChildren<WheelController>().
                        {
                            //rb.velocity = fixedVel;
                            //rb.velocity = Vector2.zero;
                            rb.velocity = Skracanie_Vectora(fixedNormal, rb.velocity, 1f);
                        }
                        //GetComponent<Rigidbody2D>().AddForce(fixedNormal * JumpForce,ForceMode2D.Impulse);
                        GetComponent<Rigidbody2D>().velocity += fixedVel;


                        //Debug.Log("Vel: " + GetComponent<Rigidbody2D>().velocity.y);
                        //LeanTween.delayedCall(0.01f, () => { Debug.Log("Vel2: " + GetComponent<Rigidbody2D>().velocity.y); });
                    }
                    //Start trailing
                    JellyTrail.Instance.StartTrailing();
                    return true;
                }
            }
            return false;
        }
        bool MakeATouchDown()
        {
            Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 0.5f); // wheel radius = 0.4
            bool isPossible = true;
            foreach (Collider2D _col in col)
            {
                if (_col.gameObject.layer != 8 && _col.gameObject.layer != 11 && _col.gameObject.layer != 9 && (!_col.isTrigger || _col.gameObject.layer == 12))
                    isPossible = false;
            }
            if(isPossible)
            {
                //GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -JumpForce));
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x*0.75f, -30);
                JellyTrail.Instance.StartTrailing();
                return true;
            }
            return false;
        }
        void Check_For_Jump()
        {
            if (WaitingJumpClock <= WaitingJumpTime)
            {
                WaitingJumpClock += Time.deltaTime;
                if (MakeAJump()) WaitingJumpClock = WaitingJumpTime + 1f;
            }
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
        private void CheckDeath()
        {
            if(transform.position.y < -50)
                HittedObstacle();
        }

        private void Update()
        {
            if(!isDead)
            {
                CheckControls();
                CheckDeath();
            }
        }
        private void LateUpdate()
        {
            Check_For_Jump();
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
        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    ColliderCounter++;
        //}
        //private void OnTriggerExit2D(Collider2D collision)
        //{
        //    ColliderCounter--;
        //}
    }
}


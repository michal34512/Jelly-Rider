using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class Form_Controller : MonoBehaviour
    {
        public static Form_Controller Instance;

        public GameObject[] ArmsJoint = new GameObject[2]; // 0 - left    1 - right
        public GameObject[] CupsJoint = new GameObject[2]; // 0 - left    1 - right
        public GameObject MeltedGameObject;
        public GameObject FormParticlesPerfab;
        public Material EffectMaterial;

        private readonly float MaxAngle = 35f;
        private readonly Vector2[] Destinations = { new Vector2(-4f, 5f), new Vector2(-1f, 7f) }; // Opened, Closed
        private readonly float[] MeltedDestinations = { -2.19f, -0.67f }; // Opened, Closed
        public void OpenForm()
        {
            //Setting up
            gameObject.SetActive(true);
            LeanTween.cancel(gameObject);
            gameObject.transform.localPosition = new Vector3(Destinations[1].x, Destinations[1].y, gameObject.transform.localPosition.z);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 90f);
            for (int i = 0; i < 2; i++)
            {
                ArmsJoint[i].transform.localRotation = Quaternion.Euler(0, 0, 0f);
                CupsJoint[i].transform.localRotation = Quaternion.Euler(0, 0, 0f);
            }
            MeltedGameObject.SetActive(true);
            MeltedGameObject.transform.localPosition = new Vector3(MeltedGameObject.transform.localPosition.x, MeltedDestinations[1], MeltedGameObject.transform.localPosition.z);
            //Animation
            LeanTween.moveLocal(gameObject, new Vector3(Destinations[0].x, Destinations[0].y, gameObject.transform.position.z), 1f).setEaseOutCubic();
            LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 0f), 1.5f).setEaseOutElastic().setOnComplete(() =>
            {
                LeanTween.moveLocalY(MeltedGameObject, MeltedDestinations[0], 0.5f).setEaseOutCubic();
                LeanTween.scaleY(MeltedGameObject, 70f, 0.5f).setEaseShake().setOnComplete(() =>
                {
                    Instantiate(FormParticlesPerfab, (Vector3)Destinations[0] + new Vector3(0, -2.2f, gameObject.transform.position.z), Quaternion.identity);
                    LeanTween.delayedCall(1.2f, () =>
                    {
                        MeltedGameObject.SetActive(false);
                        GameJelly.Instance.gameObject.SetActive(true);
                        LeanTween.value(gameObject, 0f, MaxAngle, 0.4f).setOnUpdate((float val) =>
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                ArmsJoint[i].transform.localRotation = i != 0 ? Quaternion.Euler(0, 0, val) : Quaternion.Euler(0, 0, -val);
                                CupsJoint[i].transform.localRotation = i == 0 ? Quaternion.Euler(0, 0, val) : Quaternion.Euler(0, 0, -val);
                            }
                        }).setEaseOutCubic().setOnComplete(() =>
                        {
                            LeanTween.delayedCall(0.5f, () =>
                            {
                                LeanTween.value(gameObject, MaxAngle, 0f, 0.4f).setOnUpdate((float val) =>
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        ArmsJoint[i].transform.localRotation = i != 0 ? Quaternion.Euler(0, 0, val) : Quaternion.Euler(0, 0, -val);
                                        CupsJoint[i].transform.localRotation = i == 0 ? Quaternion.Euler(0, 0, val) : Quaternion.Euler(0, 0, -val);
                                    }
                                }).setEaseOutCubic().setOnComplete(() =>
                                {
                                    LeanTween.moveLocalY(gameObject, gameObject.transform.localPosition.y + 5f, 1f).setEaseOutCubic().setOnComplete(() => { gameObject.SetActive(false); });
                                });
                            });
                        });
                    });
                });
            });
        }
        public void FastOpenForm()
        {
            //Setting up
            gameObject.SetActive(true);
            LeanTween.cancel(gameObject);
            gameObject.transform.localPosition = new Vector3(Destinations[0].x, Destinations[0].y, gameObject.transform.localPosition.z);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0f);
            if (MeltedGameObject != null)
                MeltedGameObject.SetActive(false);
            for (int i = 0; i < 2; i++)
            {
                ArmsJoint[i].transform.localRotation = Quaternion.Euler(0, 0, 0f);
                CupsJoint[i].transform.localRotation = Quaternion.Euler(0, 0, 0f);
            }
            //Animation
            GameJelly.Instance.gameObject.SetActive(true);
            LeanTween.value(gameObject, 0f, MaxAngle, 0.4f).setOnUpdate((float val) =>
            {
                for (int i = 0; i < 2; i++)
                {
                    ArmsJoint[i].transform.localRotation = i != 0 ? Quaternion.Euler(0, 0, val) : Quaternion.Euler(0, 0, -val);
                    CupsJoint[i].transform.localRotation = i == 0 ? Quaternion.Euler(0, 0, val) : Quaternion.Euler(0, 0, -val);
                }
            }).setEaseOutCubic().setOnComplete(() =>
            {
                LeanTween.delayedCall(0.5f, () =>
                {
                    LeanTween.value(gameObject, MaxAngle, 0f, 0.4f).setOnUpdate((float val) =>
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            ArmsJoint[i].transform.localRotation = i != 0 ? Quaternion.Euler(0, 0, val) : Quaternion.Euler(0, 0, -val);
                            CupsJoint[i].transform.localRotation = i == 0 ? Quaternion.Euler(0, 0, val) : Quaternion.Euler(0, 0, -val);
                        }
                    }).setEaseOutCubic().setOnComplete(() => 
                    { 
                        LeanTween.moveLocalY(gameObject, gameObject.transform.localPosition.y + 5f, 1f).setEaseOutCubic().setOnComplete(() => { gameObject.SetActive(false); }); 
                    });
                });
            });
        }
        private void Start()
        {
            //Setting Up
            if (GameInfo.Instance != null)
            {
                MeltedGameObject.GetComponent<SpriteRenderer>().sprite = GameInfo.Instance.Jellys[GameInfo.Instance.PickedJelly].MainGradnient;
                if(GameInfo.Instance.Jellys[GameInfo.Instance.PickedJelly].MainGradnient.texture!=null)
                    EffectMaterial.SetTexture("_BaseMap", GameInfo.Instance.Jellys[GameInfo.Instance.PickedJelly].MainGradnient.texture);
            }
            //Start Animation
            LeanTween.delayedCall(0.5f, OpenForm);
        }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else Destroy(gameObject);
        }
    }
}


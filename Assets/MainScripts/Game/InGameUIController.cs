using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameScene
{
    public class InGameUIController : MonoBehaviour
    {
        public static InGameUIController Instance;

        public GameObject InGameMenu;
        public GameObject Perfab_Dimmer;

        #region ButtonsScripts
        public void Button_Open_In_Game_Menu()
        {
            InGameMenu.SetActive(true);
            //Reseting Color
            LeanTween.cancel(InGameMenu);
            foreach (Image im in InGameMenu.GetComponentsInChildren<Image>())
                im.color = new Color(im.color.r, im.color.g, im.color.b, 0.392f);
            foreach (TextMeshProUGUI tx in InGameMenu.GetComponentsInChildren<TextMeshProUGUI>())
                tx.color = new Color(tx.color.r, tx.color.g, tx.color.b, 0.392f);

            Time.timeScale = 0f;
        }
        public void Button_Continue()
        {
            Time.timeScale = 1f;
            LeanTween.value(InGameMenu, 0.392f, 0f, 0.3f).setOnUpdate((float val) =>
            {
                foreach (Image im in InGameMenu.GetComponentsInChildren<Image>())
                    im.color = new Color(im.color.r, im.color.g, im.color.b, val);
                foreach (TextMeshProUGUI tx in InGameMenu.GetComponentsInChildren<TextMeshProUGUI>())
                    tx.color = new Color(tx.color.r, tx.color.g, tx.color.b, val);
            }).setEaseOutCubic().setOnComplete(()=>{ InGameMenu.SetActive(false); });
        }
        public void Button_Reset()
        {
            GameJelly.Instance.BackToFirstPoint();
            Form_Controller.Instance.FastOpenForm();

            CameraController.Instance.position = new Vector3(0,0,Camera.main.transform.parent.localPosition.z);

            foreach(Effects.Dimmer d in FindObjectsOfType<Effects.Dimmer>())
            {
                d.gameObject.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 0.5f);
            }
                

            Button_Continue();
            foreach (var bp in FindObjectsOfType<BezierPath>())
            {
                bp.Restart();
            }
            if (GameInfo.Instance != null && GameInfo.Instance.gamemode == GameInfo._GameMode.Constant)
            {
                PointsCounter.Instance.SetToZero();
            }
            if (ConstantLevelSpawner.Instance != null)
                    ConstantLevelSpawner.Instance.Restart();
        }
        public void Button_Menu()
        {
            Instantiate(Perfab_Dimmer).GetComponent<Effects.Dimmer>().DimmerStart(() =>
            {
                GameInfo.Instance.gamemode = GameInfo._GameMode.UI;
                GameInfo.Instance.Next_Background();
                SceneManager.LoadScene(0);
            });
            Time.timeScale = 1f;
        }
        #endregion ButtonsScripts

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }
        private void OnApplicationPause(bool pause)
        {
            if(pause)
            {
                Button_Open_In_Game_Menu();
            }
        }
    }
}


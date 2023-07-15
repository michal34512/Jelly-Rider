using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIScene
{
    public class Settings_Card_Controller : MonoBehaviour
    {
        public GameObject[] Prefab;
        // Start is called before the first frame update
        void Start()
        {
            //MusicVolume
            Instantiate(Prefab[1], transform).GetComponent<Settings_Slider>().Show(200, "Music Volume", "{0}%", (float val) =>
            {
                if (GameInfo.Instance != null)
                    GameInfo.Instance.GetComponent<AudioSource>().volume = val;
                //Save
                PlayerPrefs.SetFloat("MusicVolume", val);
            }, PlayerPrefs.GetFloat("MusicVolume"));
            //ShowFps
            Instantiate(Prefab[0], transform).GetComponent<Settings_CheckBox>().Show(-200, "Show FPS",(bool val)=> 
            {
                if (GameInfo.Instance != null)
                    GameInfo.Instance.GetComponent<fpsCheck>().isShowing = val;
                //Save
                if (val)
                    PlayerPrefs.SetInt("ShowFps", 1);
                else PlayerPrefs.SetInt("ShowFps", 0);
            }, PlayerPrefs.GetInt("ShowFps") == 1);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

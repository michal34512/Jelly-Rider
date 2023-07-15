using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace Effects
{
    public class Dimmer : MonoBehaviour
    {
        public void DimmerStart(System.Action Action)
        {
            DontDestroyOnLoad(gameObject);
            
            gameObject.transform.localScale = new Vector3(Screen.width, Screen.width, 1f);
            LeanTween.value(gameObject, 0f, 1f, 0.2f).setOnUpdate((float val) =>
            {
                GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b,val);
            }).setOnComplete(()=> 
            {
                Action?.Invoke();
                LeanTween.value(gameObject, 1f, 0f, 0.2f).setOnUpdate((float val) =>
                {
                    GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, val);
                }).setOnComplete(() => { Destroy(gameObject); });
            });
        }
        private void LateUpdate()
        {
            gameObject.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 0.5f);
        }
    }
}


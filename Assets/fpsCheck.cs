using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class fpsCheck : MonoBehaviour
{
	public bool isShowing = false;
	float deltaTime = 0.0f;

	Rect rect;
	string text;
	GUIStyle style;
	void Update()
	{
		if(isShowing)
			deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}
    private void Awake()
    {
        int w = Screen.width, h = Screen.height;

		style = new GUIStyle();

		rect = new Rect(10, 10, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

		isShowing = (PlayerPrefs.GetInt("ShowFps") == 1);
    }

    void OnGUI()
	{
        if (isShowing)
        {
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
}

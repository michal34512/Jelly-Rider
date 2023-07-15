using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialLevel;

public class InGameTutorial : MonoBehaviour
{
    public static InGameTutorial Instance;

    public GameObject HintDotPrefab;
    private float XAxis;
    private bool isPlaying = false;
    private float ActivityTimer = 0;
    private readonly float MinimumTimeToTuriotal = 30; // in sec
    public void PlayTutorial()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            XAxis = 0.5f * ((float)Screen.width / (float)Screen.height * 1080f) - 250f;
            TutorialHintDot.GenerateBlinkingDot(0, new Vector2(XAxis, -350), 0.1f, HintDotPrefab, 1f, true);
            LeanTween.delayedCall(0.6f, () => { TutorialHintDot.GenerateBlinkingDot(1, new Vector2(-XAxis, -350), 0.1f, HintDotPrefab, 1f, true); });
            LeanTween.delayedCall(1.2f, () => { TutorialHintDot.GenerateSlidingDot(2, new Vector2(XAxis, -350), new Vector2(XAxis, 350), 0.3f, HintDotPrefab, 1f, true); });
            LeanTween.delayedCall(1.8f, () => { TutorialHintDot.GenerateSlidingDot(2, new Vector2(XAxis, 350), new Vector2(XAxis, -350), 0.3f, HintDotPrefab, 1f, true); });
            LeanTween.delayedCall(2.2f, () => { isPlaying = false; });
        }
    }

    // Update is called once per frame
    private void Start()
    {
        LeanTween.delayedCall(0.5f, () =>
                {
                    PlayTutorial();
                });
    }
    private void FixedUpdate()
    {
        if (ShadowLight.Instance != null && ShadowLight.Instance.isCameraMoved)
            ActivityTimer = 0;
        else
        {
            ActivityTimer += Time.fixedDeltaTime;
            if(ActivityTimer>MinimumTimeToTuriotal)
            {
                ActivityTimer = 0;
                PlayTutorial();
            }
        }
        
    }
    private void Awake()
    {
        if (Instance != null||( GameInfo.Instance != null && GameInfo.Instance.gamemode == GameInfo._GameMode.Tutorial))
            Destroy(this);
        else Instance = this;
    }
}

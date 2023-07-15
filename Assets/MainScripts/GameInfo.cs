using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour
{
    public static GameInfo Instance;

    public Color Game_Color;
    public enum _GameMode
    {
        UI,
        Level,
        Constant
    }
    public _GameMode gamemode = _GameMode.UI;

    public float ConstantModePoints = 0f;

    public List<Sprite> Backgrounds_Sprites;
    private Sprite Choosed_Background;
    private Color Average_Color(Sprite A)
    {
        Color Rezult = new Color(0, 0, 0);
        if (A != null && A.texture.isReadable)
        {
            int counter = 0;
            for (int y = 0; y < A.texture.height; y++)
                for (int x = 0; x < A.texture.width; x++)
                {
                    counter++;
                    Rezult += A.texture.GetPixel(x, y);
                }
            Rezult /= counter;
        }
        return Rezult;
    }
    private readonly float MinColorDifferenceFactor = 1.2f;
    public void Next_Background()
    {
        Color JellyColor = Jellys[PickedJelly].MainColor;
        Sprite BackgroundSprite = null;
        for (int i = 0; i < Backgrounds_Sprites.Count*10; i++) // zabezpiecznie
        {
            BackgroundSprite = Backgrounds_Sprites[Random.Range(0, Backgrounds_Sprites.Count)];
            Color BackgroundColor = Average_Color(BackgroundSprite);
            if (Mathf.Abs(JellyColor.r - BackgroundColor.r) + Mathf.Abs(JellyColor.g - BackgroundColor.g) + Mathf.Abs(JellyColor.b - BackgroundColor.b) > MinColorDifferenceFactor)
            {
                if(BackgroundSprite != Choosed_Background)
                {
                    break;
                }
                    
            }
        }
        
        foreach(Background_Controller bc in FindObjectsOfType<Background_Controller>())
        {
            bc.Update_Background(BackgroundSprite);
        }
        Choosed_Background = BackgroundSprite;
    }
    public void Assign_Background(Image A)
    {
        A.sprite = Choosed_Background;
    }

    public List<Jelly_Scriptable_Object> Jellys;
    private int _PickedJelly;
    public int PickedJelly 
    {
        get
        {
            return _PickedJelly;
        }
        set
        {
            PlayerPrefs.SetInt("PickedJelly", value);
            _PickedJelly = value;
        }
    }
    public Jelly_Scriptable_Object PickedJellyObject
    {
        get
        {
            return Jellys[PickedJelly];
        }
    }

    /*public List<Jelly_Eye_Scriptable_Object> Eyes;
    private int _PickedEyes;
    public int PickedEyes
    {
        get
        {
            return _PickedEyes;
        }
        set
        {
            if(value>=0&&value<Eyes.Count)
            {
                PlayerPrefs.SetInt("PickedEyes", value);
                _PickedEyes = value;
            }
        }
    }
    public Jelly_Eye_Scriptable_Object PickedEyesObject
    {
        get
        {
            return Eyes[PickedEyes];
        }
    }*/

    private int _LoadedLevel;
    public int LoadedLevel 
    {
        get
        {
            return _LoadedLevel;
        }
        set
        {
            isLevelLoaded = true;
            _LoadedLevel = value;
        }
    }
    public bool isLevelLoaded = false;

    private long _PlayerMoney;
    public long PlayerMoney
    {
        get
        {
            return _PlayerMoney;
        }
        set
        {
            PlayerPrefs.SetString("PlayerMoney", value.ToString());
            _PlayerMoney = value;
        }
    }

    private void Start()
    {
        Next_Background();
        /*Color JellyColor = Jellys[PickedJelly].MainColor;
        Sprite BackgroundSprite = Backgrounds_Sprites[12];
        Color BackgroundColor = Average_Color(BackgroundSprite);
        Debug.Log(BackgroundSprite.name+" _ "+(Mathf.Abs(JellyColor.r - BackgroundColor.r) + Mathf.Abs(JellyColor.g - BackgroundColor.g) + Mathf.Abs(JellyColor.b - BackgroundColor.b)));
        */
    }
    private void Awake()
    {
        if (Instance == null)
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;

            Instance = this;
            DontDestroyOnLoad(gameObject);
            _PickedJelly = PlayerPrefs.GetInt("PickedJelly");
            
            if(!long.TryParse(PlayerPrefs.GetString("PlayerMoney"), out _PlayerMoney))
                Debug.LogWarning("Player money reading error");

            //PickedEyes = PlayerPrefs.GetInt("PickedEyes");
            //First Run
            if (PlayerPrefs.GetInt("FirstRun") != 1)
            {
                PlayerPrefs.SetInt("FirstRun", 1);
                PickedJelly = 0;
                PlayerMoney = 0;
                //PickedEyes = 0;
            }
        }
        else Destroy(gameObject);
    }
}

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
        Constant,
        Tutorial
    }
    public _GameMode gamemode = _GameMode.UI;

    public int ConstantModePoints = 0;

    #region Backgrounds


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
        for (int i = 0; i < Backgrounds_Sprites.Count * 10; i++) // zabezpiecznie
        {
            BackgroundSprite = Backgrounds_Sprites[Random.Range(0, Backgrounds_Sprites.Count)];
            Color BackgroundColor = Average_Color(BackgroundSprite);
            if (Mathf.Abs(JellyColor.r - BackgroundColor.r) + Mathf.Abs(JellyColor.g - BackgroundColor.g) + Mathf.Abs(JellyColor.b - BackgroundColor.b) > MinColorDifferenceFactor)
            {
                if (BackgroundSprite != Choosed_Background)
                {
                    Change_Spikes_Color(BackgroundColor);
                    break;
                }
            }
            if(i == (Backgrounds_Sprites.Count * 10)-1) Change_Spikes_Color(BackgroundColor);
        }
        foreach (Background_Controller bc in FindObjectsOfType<Background_Controller>())
        {
            bc.Update_Background(BackgroundSprite);
        }
        Choosed_Background = BackgroundSprite;
    }
    public Sprite Get_Background()
    {
        return Choosed_Background;
    }

    public Material SpikeMaterial;
    [HideInInspector] public bool SpikesRedYellow = true;
    //255 49 49
    //255 186 95

    //247 255 143
    //255 180 0
    private readonly Color[,] SpikesColors = { { new Color(1f, 0.1922f, 0.1922f), new Color(1f, 0.73f, 0.373f) }, { new Color(0.97f, 1f, 0.56f), new Color(1f, 0.706f, 0f) } };
    private void Change_Spikes_Color(Color BackgroundColor)
    {
        //Debug.Log("SETTING");
        Color SpikeColorA = SpikeMaterial.GetColor("ColorA");
        Color SpikeColorB = SpikeMaterial.GetColor("ColorB");
        float Factor = Mathf.Max((Mathf.Abs(SpikeColorA.r - BackgroundColor.r) + Mathf.Abs(SpikeColorA.g - BackgroundColor.g) + Mathf.Abs(SpikeColorA.b - BackgroundColor.b)), (Mathf.Abs(SpikeColorB.r - BackgroundColor.r) + Mathf.Abs(SpikeColorB.g - BackgroundColor.g) + Mathf.Abs(SpikeColorB.b - BackgroundColor.b)));
        if (Factor > 0.65)
        {
            //RED
            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject, 0f, 1f, 0.4f).setOnUpdate((float val) =>
               {
                   SpikeMaterial.SetColor("ColorA", (SpikesColors[0, 0] - SpikeColorA)*val + SpikeColorA);
                   SpikeMaterial.SetColor("ColorB", (SpikesColors[0, 1] - SpikeColorB) * val + SpikeColorB);
               });
        }
        else
        {
            //Yellow
            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject, 0f, 1f, 0.4f).setOnUpdate((float val) =>
            {
                SpikeMaterial.SetColor("ColorA", (SpikesColors[1, 0] - SpikeColorA) * val + SpikeColorA);
                SpikeMaterial.SetColor("ColorB", (SpikesColors[1, 1] - SpikeColorB) * val + SpikeColorB);
            });
        }
    }
    #endregion Backgrounds

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
            if(value < 0)
            {
                PlayerPrefs.SetInt("PickedJelly", 0);
                _PickedJelly = 0;
            }
            else if(value >= Jellys.Count)
            {
                PlayerPrefs.SetInt("PickedJelly", Jellys.Count - 1);
                _PickedJelly = Jellys.Count - 1;
            }
            else
            {
                PlayerPrefs.SetInt("PickedJelly", value);
                _PickedJelly = value;
            }

            int x = (int)Jellys[value].RarityClass;
            for (int i = 0; i < Classes[x].Count; i++)
            {
                if (Classes[x][i] == Jellys[value])
                {
                    ClassesPickedJelly = new Vector2Int(x, i);
                    break;
                }
            }
        }
    }
    public Jelly_Scriptable_Object PickedJellyObject
    {
        get
        {
            return Jellys[PickedJelly];
        }
    }



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
    #region JellyClasses
    public List<Jelly_Scriptable_Object> [] Classes = new List<Jelly_Scriptable_Object>[5]; // Default, Common, Rare, Epic, Legendary
    private void SetJellyClasses()
    {
        for (int i = 0; i < Classes.Length; i++)
            Classes[i] = new List<Jelly_Scriptable_Object>();
        foreach(Jelly_Scriptable_Object jso in Jellys)
        {
            Classes[(int)jso.RarityClass].Add(jso);
        }
    }
    public Vector2Int ClassesPickedJelly;
    public int GetJellyIndexByClassPos(Vector2Int pos)
    {
        if (pos.y < 0)
        {
            pos.y = 0;
        }
        else if (pos.y >= Classes[pos.x].Count)
        {
            pos.y = Classes[pos.x].Count - 1;
        }
        for (int i = 0; i < Jellys.Count; i++)
        {
            if (Jellys[i] == Classes[pos.x][pos.y])
                return i;
        }
        return 0;
    }
    #endregion JellyClasses
    public void UpdateLockedJellys()
    {
        for (int i = 0; i < Jellys.Count; i++)
        {
            if (PlayerPrefs.GetInt("JellyUnlocked" + i.ToString()) == 1 || Jellys[i].RarityClass == Jelly_Scriptable_Object._RarityClass.Default)
                Jellys[i].Unlocked = true;
            else Jellys[i].Unlocked = false;
        }
    }

    #region GameMusic
    public bool isMuted
    {
        get
        {
            return _isMuted;
        }
        private set
        {
            _isMuted = value;
            GetComponent<AudioSource>().enabled = !_isMuted;
            PlayerPrefs.SetInt("MusicMuted",value ? 1 : 0);
        }
    }
    private bool _isMuted = false;
    public void Chance_Mute_State()
    {
        isMuted = !isMuted;
    }
    #endregion GameMusic
    private void Start()
    {
        Next_Background();
    }
    private void Awake()
    {
        if (Instance == null)
        {
            //QualitySettings.vSyncCount = 2;
            Application.targetFrameRate = 60;
            //Time.captureFramerate = 30;
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _PickedJelly = PlayerPrefs.GetInt("PickedJelly");
            if (!Jellys[PickedJelly].Unlocked)
                _PickedJelly = 0; // Default
            if (!long.TryParse(PlayerPrefs.GetString("PlayerMoney"), out _PlayerMoney))
                Debug.LogWarning("Player money reading error");
            //PlayerMoney += 100;
            //Locking Jelly
            UpdateLockedJellys();
            //Unlock All Jellys
                for (int i = 0; i < Jellys.Count; i++)
                {
                    PlayerPrefs.SetInt("JellyUnlocked" + i.ToString(), 1);
                }
            //First Run
            if (PlayerPrefs.GetInt("FirstRun") != 1)
            {
                PlayerPrefs.SetInt("FirstRun", 1);
                PlayerPrefs.SetFloat("MusicVolume", GetComponent<AudioSource>().volume);
                PickedJelly = 0;
                PlayerMoney = 0;
            }
            SetJellyClasses();
            int x = (int)Jellys[PickedJelly].RarityClass;
            for (int i = 0; i < Classes[x].Count; i++)
            {
                if (Classes[x][i] == Jellys[PickedJelly])
                {
                    ClassesPickedJelly = new Vector2Int(x, i);
                    break;
                }
            }
            //Music
            if(PlayerPrefs.GetInt("MusicMuted") == 1)
            {
                isMuted = true;
            }
            GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        else Destroy(gameObject);
    }
}

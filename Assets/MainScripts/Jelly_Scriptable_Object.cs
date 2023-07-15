using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Jelly", menuName = "Jelly/Jelly")]
public class Jelly_Scriptable_Object : ScriptableObject
{
    public enum _RarityClass
    {
        Default,
        Common,
        Epic,
        Rare,
        Legendary
    }
    public _RarityClass RarityClass;
    public Sprite MainSprite;
    public Sprite MainGradnient;
    public Color MainColor;
    public bool Unlocked = false;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Jelly Eye", menuName = "Jelly/Eye")]
public class Jelly_Eye_Scriptable_Object : ScriptableObject
{
    public Sprite Eye_Sprite;
    public List<Vector2> Offset;
    public List<float> Sizes;
}

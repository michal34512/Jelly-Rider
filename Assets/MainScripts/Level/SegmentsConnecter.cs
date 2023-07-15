using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameScene
{
    public class SegmentsConnecter : MonoBehaviour
    {
        public Vector2 Start;
        public Vector2 Stop;
        [Range(1, 5)]
        public int Difficulty = 1;
    }
}


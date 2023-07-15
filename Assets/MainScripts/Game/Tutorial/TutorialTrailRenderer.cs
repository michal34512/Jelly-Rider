using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class TutorialTrailRenderer : MonoBehaviour
{
    public int PointCount;
    public float Lenght;
    public float Multi = 1;

    private float Distance;
    private Vector2[] SavedPositions;

    public void Clear()
    {
        for (int i = 0; i < GetComponent<LineRenderer>().positionCount; i++)
        {
            GetComponent<LineRenderer>().SetPosition(i, Vector2.zero);
            SavedPositions[i] = gameObject.transform.localPosition * Multi;
        }
    }

    private void FixPoints()
    {
        SavedPositions[0] = gameObject.transform.localPosition * Multi;
        for (int i = 1;i < PointCount; i++)
        {
            Vector2 Difference;
            Difference = SavedPositions[i] - SavedPositions[i - 1];
            if (Difference.x*Difference.x+Difference.y*Difference.y > Distance*Distance)
            {
                //Correction
                SavedPositions[i] = SavedPositions[i - 1] + Difference.normalized * Distance;
                
            }GetComponent<LineRenderer>().SetPosition(i, SavedPositions[i] - (Vector2)transform.localPosition * Multi);
        }
    }
    private void Update()
    {
        FixPoints();
    }
    private void Awake()
    {
        Distance = Lenght / PointCount;
        SavedPositions = new Vector2[PointCount];
        GetComponent<LineRenderer>().positionCount = PointCount;
        Clear();
    }
}

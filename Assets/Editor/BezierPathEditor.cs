using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System;

[CustomEditor(typeof(BezierPath))]
public class BezierPathEditor : Editor
{
    BezierPath bezierCollider;
    SerializedObject _SObezierCollider;
    PolygonCollider2D polygonCollider;
    LineRenderer lineRenderer;

    int lastPointsQuantity = 0;
    Vector2 lastFirstPoint = Vector2.zero;
    Vector2 lastHandlerFirstPoint = Vector2.zero;
    Vector2 lastSecondPoint = Vector2.zero;
    Vector2 lastHandlerSecondPoint = Vector2.zero;
    Vector2 EditorLight = new Vector2(0, 20);
    private Vector2[] CalculatePolygonColliderPoints(Vector2[] baseVerticles)
    {
        Vector2 ObrocVector(Vector2 vec, float kat)
        {
            return new Vector2(Mathf.Cos(kat) * vec.x - Mathf.Sin(kat) * vec.y, Mathf.Sin(kat) * vec.x + Mathf.Cos(kat) * vec.y);
        }
        Vector2 Angle90(Vector2 A)
        {
            return new Vector2(-A.y, A.x);
        }

        List<Vector2> Points = new List<Vector2>();
        //Points
        for (int i = 0; i < baseVerticles.Length; i++)
        {
            if (i != baseVerticles.Length - 1)
            {
                Points.Add(bezierCollider.ColliderOffset * Angle90((baseVerticles[i + 1] - baseVerticles[i]).normalized));
            }
            else
            {
                Points.Add(bezierCollider.ColliderOffset * Angle90((baseVerticles[i] - baseVerticles[i - 1]).normalized));
            }
        }
        //Caps
        float SingleAngle = 180f / (lineRenderer.numCapVertices + 1);
        Vector2 FirstPoint = baseVerticles[0];
        Vector2 LastPoint = baseVerticles[baseVerticles.Length - 1];

        List<Vector2> Output = new List<Vector2>();
        //Top
        for (int i = 0; i < baseVerticles.Length; i++)
            Output.Add(Points[i] + baseVerticles[i]);
        //First Cap
        for (int i = 0; i < lineRenderer.numCapVertices; i++)
            Output.Add(ObrocVector(Points[Points.Count - 1], -SingleAngle * Mathf.Deg2Rad * (i + 1)) + LastPoint);
        //Bottom
        for (int i = baseVerticles.Length - 1; i >= 0; i--)
            Output.Add(-Points[i] + baseVerticles[i]);
        //Last Cap
        for (int i = lineRenderer.numCapVertices - 1; i >= 0; i--)
            Output.Add(ObrocVector(Points[0], SingleAngle * Mathf.Deg2Rad * (i + 1)) + FirstPoint);
        return Output.ToArray();
    }
    private void CalculateEndingPoints()
    {
        //LEFT
        int index = 0;
        for (int i = 1; i < polygonCollider.points.Length; i++)
        {
            if (polygonCollider.points[i].x < polygonCollider.points[index].x)
                index = i;
        }
        bezierCollider.EndingPoints[0] = index;
        //RIGHT
        for (int i = 1; i < polygonCollider.points.Length; i++)
        {
            if (polygonCollider.points[i].x > polygonCollider.points[index].x)
                index = i;
        }
        bezierCollider.EndingPoints[1] = index;
    }
    private void CalculateShadowMesh()
    {
        //bezierCollider.ShadowMesh = new Mesh();
        /*Vector2 ToBottomScreen(Vector2 PointPosition)
        {
            Vector2 FixedLight = EditorLight - (Vector2)bezierCollider.transform.position;
            float ScreenBottom = Camera.main.transform.position.y - Camera.main.orthographicSize - 1f;
            Vector2 Direction = (PointPosition  - FixedLight).normalized;
            return FixedLight + Direction * (ScreenBottom - FixedLight.y) / Direction.y;
        }*/

        List<Vector2> Vertexes = new List<Vector2>();
        Vertexes.AddRange(polygonCollider.points);
        //Points to bottom
        /*for(int i = 0;i < polygonCollider.points.Length; i++)
        {
            Vertexes.Add(ToBottomScreen(polygonCollider.points[i]));
        }*/
        //Points At Zero
        Vertexes.AddRange(polygonCollider.points);
        //bezierCollider.ShadowMesh.vertices = System.Array.ConvertAll(Vertexes.ToArray(), new System.Converter<Vector2, Vector3>((Vector2 A) => { return new Vector3(A.x, A.y, 0.1f); }));
        //Triangulate
        List<int> Triangles = new List<int>();
        for (int i = 0; i < polygonCollider.points.Length - 1; i++)
        {
            //T1
            Triangles.Add(i);
            Triangles.Add(i + 1 + polygonCollider.points.Length);
            Triangles.Add(i + polygonCollider.points.Length);
            //T2
            Triangles.Add(i);
            Triangles.Add(i + 1);
            Triangles.Add(i + 1 + polygonCollider.points.Length);
        }
        //T1
        Triangles.Add(polygonCollider.points.Length - 1);
        Triangles.Add(0);
        Triangles.Add(polygonCollider.points.Length);
        //T2
        Triangles.Add(polygonCollider.points.Length - 1);
        Triangles.Add(polygonCollider.points.Length);
        Triangles.Add(polygonCollider.points.Length * 2 - 1);

        _SObezierCollider.Update();
        SerializedProperty PVerticles = _SObezierCollider.FindProperty("verticles");
        PVerticles.arraySize = Vertexes.Count;
        for (int i = 0; i < PVerticles.arraySize; i++)
            PVerticles.GetArrayElementAtIndex(i).vector3Value = new Vector3(Vertexes[i].x, Vertexes[i].y, 0.1f);

        SerializedProperty PTriangles = _SObezierCollider.FindProperty("triangles");
        PTriangles.arraySize = Triangles.Count;
        for (int i = 0; i < PTriangles.arraySize; i++)
            PTriangles.GetArrayElementAtIndex(i).intValue = Triangles[i];
        _SObezierCollider.ApplyModifiedProperties();


        Vector3 Conventer(Vector2 A) { return new Vector3(A.x, A.y, 0.1f); }

        Mesh _mesh = new Mesh();
        _mesh.vertices = Array.ConvertAll(Vertexes.ToArray(), Conventer);
        _mesh.triangles = Triangles.ToArray();
        _mesh.uv = Vertexes.ToArray();
        bezierCollider.GetComponent<MeshFilter>().sharedMesh = _mesh;
    }
    public override void OnInspectorGUI()
    {
        if (_SObezierCollider == null || bezierCollider == null)
        {
            bezierCollider = (BezierPath)target;
            _SObezierCollider = new SerializedObject(target);
        }


        polygonCollider = bezierCollider.GetComponent<PolygonCollider2D>();
        lineRenderer = bezierCollider.GetComponent<LineRenderer>();

        if (polygonCollider != null)
        {
            bezierCollider.pointsQuantity = EditorGUILayout.IntField("curve points", bezierCollider.pointsQuantity, GUILayout.MinWidth(100));
            bezierCollider.ColliderOffset = EditorGUILayout.FloatField("collider offset", bezierCollider.ColliderOffset, GUILayout.MinWidth(100));
            bezierCollider.firstPoint = EditorGUILayout.Vector2Field("first point", bezierCollider.firstPoint, GUILayout.MinWidth(100));
            bezierCollider.handlerFirstPoint = EditorGUILayout.Vector2Field("handler first Point", bezierCollider.handlerFirstPoint, GUILayout.MinWidth(100));
            bezierCollider.secondPoint = EditorGUILayout.Vector2Field("second point", bezierCollider.secondPoint, GUILayout.MinWidth(100));
            bezierCollider.handlerSecondPoint = EditorGUILayout.Vector2Field("handler secondPoint", bezierCollider.handlerSecondPoint, GUILayout.MinWidth(100));
            if (GUILayout.Button("Update Perfab"))
            {
                if (PrefabUtility.IsPartOfPrefabInstance(bezierCollider.gameObject))
                {
                    PrefabUtility.ApplyPrefabInstance(bezierCollider.gameObject, InteractionMode.AutomatedAction);
                    Debug.Log("Updated");
                }
            }
            if (GUILayout.Button("Round"))
            {
                bezierCollider.firstPoint.x = Mathf.RoundToInt(bezierCollider.firstPoint.x);
                bezierCollider.firstPoint.y = Mathf.RoundToInt(bezierCollider.firstPoint.y);

                bezierCollider.handlerFirstPoint.x = Mathf.RoundToInt(bezierCollider.handlerFirstPoint.x);
                bezierCollider.handlerFirstPoint.y = Mathf.RoundToInt(bezierCollider.handlerFirstPoint.y);

                bezierCollider.secondPoint.x = Mathf.RoundToInt(bezierCollider.secondPoint.x);
                bezierCollider.secondPoint.y = Mathf.RoundToInt(bezierCollider.secondPoint.y);

                bezierCollider.handlerSecondPoint.x = Mathf.RoundToInt(bezierCollider.handlerSecondPoint.x);
                bezierCollider.handlerSecondPoint.y = Mathf.RoundToInt(bezierCollider.handlerSecondPoint.y);
            }
            if (bezierCollider.pointsQuantity > 0 && !bezierCollider.firstPoint.Equals(bezierCollider.secondPoint) &&
                (
                    lastPointsQuantity != bezierCollider.pointsQuantity ||
                    lastFirstPoint != bezierCollider.firstPoint ||
                    lastHandlerFirstPoint != bezierCollider.handlerFirstPoint ||
                    lastSecondPoint != bezierCollider.secondPoint ||
                    lastHandlerSecondPoint != bezierCollider.handlerSecondPoint
                ))
            {
                EditorUtility.SetDirty(bezierCollider);
                lastPointsQuantity = bezierCollider.pointsQuantity;
                lastFirstPoint = bezierCollider.firstPoint;
                lastHandlerFirstPoint = bezierCollider.handlerFirstPoint;
                lastSecondPoint = bezierCollider.secondPoint;
                lastHandlerSecondPoint = bezierCollider.handlerSecondPoint;

                //Line Renderer
                lineRenderer.positionCount = bezierCollider.pointsQuantity + 1;
                Vector3[] LPolisions = bezierCollider.calculate3DPoints();
                /*for (int i = 0; i < LPolisions.Length; i++)
                    LPolisions[i] += new Vector3(bezierCollider.transform.position.x,bezierCollider.transform.position.y,0);*/
                lineRenderer.SetPositions(LPolisions);
                //Polygeon Collider
                polygonCollider.points = CalculatePolygonColliderPoints(bezierCollider.calculate2DPoints());
                //Shadows
                CalculateEndingPoints();
                CalculateShadowMesh();
            }

        }
    }

    void OnSceneGUI()
    {
        if (_SObezierCollider == null || bezierCollider == null)
        {
            bezierCollider = (BezierPath)target;
            _SObezierCollider = new SerializedObject(target);
        }
        if (bezierCollider != null)
        {

            Handles.color = Color.grey;

            Handles.DrawLine(bezierCollider.transform.position + (Vector3)bezierCollider.handlerFirstPoint, bezierCollider.transform.position + (Vector3)bezierCollider.firstPoint);
            Handles.DrawLine(bezierCollider.transform.position + (Vector3)bezierCollider.handlerSecondPoint, bezierCollider.transform.position + (Vector3)bezierCollider.secondPoint);

            bezierCollider.firstPoint = Handles.FreeMoveHandle(bezierCollider.transform.position + ((Vector3)bezierCollider.firstPoint), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(bezierCollider.transform.position + ((Vector3)bezierCollider.firstPoint)), Vector3.zero, Handles.DotHandleCap) - bezierCollider.transform.position;
            bezierCollider.secondPoint = Handles.FreeMoveHandle(bezierCollider.transform.position + ((Vector3)bezierCollider.secondPoint), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(bezierCollider.transform.position + ((Vector3)bezierCollider.secondPoint)), Vector3.zero, Handles.DotHandleCap) - bezierCollider.transform.position;
            bezierCollider.handlerFirstPoint = Handles.FreeMoveHandle(bezierCollider.transform.position + ((Vector3)bezierCollider.handlerFirstPoint), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(bezierCollider.transform.position + ((Vector3)bezierCollider.handlerFirstPoint)), Vector3.zero, Handles.DotHandleCap) - bezierCollider.transform.position;
            bezierCollider.handlerSecondPoint = Handles.FreeMoveHandle(bezierCollider.transform.position + ((Vector3)bezierCollider.handlerSecondPoint), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(bezierCollider.transform.position + ((Vector3)bezierCollider.handlerSecondPoint)), Vector3.zero, Handles.DotHandleCap) - bezierCollider.transform.position;

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}

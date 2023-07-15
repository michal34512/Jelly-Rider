using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameScene;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System;
using JetBrains.Annotations;

[CustomEditor(typeof(JellyPhysic))]
public class JellyPhysicEditor : Editor
{
    SerializedObject _SOJellyPhysic;
    JellyPhysic _JellyPhysic;

    private readonly Vector2Int MeshQuality = new Vector2Int(8, 8);
    private readonly Vector2 MeshSize = new Vector2(0.8f, 0.8f);

    public override void OnInspectorGUI()
    {
        if (_SOJellyPhysic == null|| _JellyPhysic==null)
        {
            _SOJellyPhysic = new SerializedObject(target);
            _JellyPhysic = (JellyPhysic)target;
        }
        
        if(GUILayout.Button("Update Mesh"))
        {
            Debug.Log("Updated");
            GenerateMesh();
        }
        base.OnInspectorGUI();
    }

    #region Update
    private void GenerateMesh()
    {
        Vector3 Conventer(Vector2 A)
        {
            return new Vector3(A.x, A.y, _JellyPhysic.transform.position.z);
        }

        _SOJellyPhysic.Update();
        List<Vector2> Verticles = new List<Vector2>();
        List<int> Triangles = new List<int>();
        Vector2 SingleSize = MeshSize / MeshQuality;
        for (int i = 0; i < MeshQuality.y + 1; i++)
            for (int j = 0; j < MeshQuality.x + 1; j++)
            {
                Verticles.Add(new Vector2((j - MeshQuality.x * 0.5f) * SingleSize.x, (i - MeshQuality.y * 0.5f) * SingleSize.y));

                if (j < MeshQuality.x && i < MeshQuality.y)
                {
                    //T1
                    Triangles.Add(Verticles.Count - 1);
                    Triangles.Add(Verticles.Count + MeshQuality.x + 1);
                    Triangles.Add(Verticles.Count);
                    //T2
                    Triangles.Add(Verticles.Count - 1);
                    Triangles.Add(Verticles.Count + MeshQuality.x);
                    Triangles.Add(Verticles.Count + MeshQuality.x + 1);
                }
            }
        SerializedProperty PVerticles = _SOJellyPhysic.FindProperty("verticles");
        PVerticles.arraySize = Verticles.Count;
        for (int i=0;i< Verticles.Count; i++)
            PVerticles.GetArrayElementAtIndex(i).vector3Value = Conventer(Verticles[i]);

        SerializedProperty PTriangles = _SOJellyPhysic.FindProperty("triangles");
        PTriangles.arraySize = Triangles.Count;
        for (int i = 0; i < Triangles.Count; i++)
            PTriangles.GetArrayElementAtIndex(i).intValue = Triangles[i];
        _SOJellyPhysic.ApplyModifiedProperties();
        //Editor prewiew mesh
        Mesh _mesh = _JellyPhysic.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        _mesh.vertices = _JellyPhysic.verticles;
        _mesh.triangles = _JellyPhysic.triangles;
        _mesh.uv = Verticles.ToArray();
        _mesh.RecalculateNormals();

        PrefabUtility.ApplyPrefabInstance(_JellyPhysic.gameObject,InteractionMode.AutomatedAction);
    }
    #endregion Update
}

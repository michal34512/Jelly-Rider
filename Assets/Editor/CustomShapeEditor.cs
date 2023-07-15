using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using System;
using TMPro;

public class Triangulator
{
    private List<Vector2> m_points = new List<Vector2>();

    public Triangulator(Vector2[] points)
    {
        m_points = new List<Vector2>(points);
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();

        int n = m_points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float Area()
    {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector2 A = m_points[V[u]];
        Vector2 B = m_points[V[v]];
        Vector2 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}

[CustomEditor(typeof(CustomShape))]
public class CustomShapeEditor : Editor
{
    CustomShape customshape;
    SerializedObject _SOcustomshape;
    SerializedProperty PVerticles;
    SerializedProperty PTriangles;
    SerializedProperty PFixedUv;

    private void UpdateMeshInformation()
    {
        Triangulator t = new Triangulator(customshape.verticles.ToArray());
        int[] triangles = t.Triangulate();
        PTriangles.arraySize = triangles.Length;
        for (int i = 0; i < PTriangles.arraySize; i++)
            PTriangles.GetArrayElementAtIndex(i).intValue = triangles[i];
    }
    private void GenerateCollider()
    {
        if (customshape.GetComponent<PolygonCollider2D>() != null)
            customshape.GetComponent<PolygonCollider2D>().points = customshape.verticles.ToArray();
    }
    private void UpdateEditorMesh()
    {
        Vector3 Conventer(Vector2 A)
        {
            return new Vector3(A.x, A.y, 0f);
        }
        Mesh _mesh = new Mesh();
        _mesh.vertices = Array.ConvertAll(customshape.verticles.ToArray(), Conventer);
        _mesh.triangles = customshape.triangles;
        if (customshape.fixeduv)
        {
            Vector2 MinVertice = Vector2.positiveInfinity;
            Vector2 MaxVertice = Vector2.negativeInfinity;
            foreach(Vector2 vc in _mesh.vertices)
            {
                if (vc.x < MinVertice.x) MinVertice.x = vc.x;
                if (vc.y < MinVertice.y) MinVertice.y = vc.y;

                if (vc.x > MaxVertice.x) MaxVertice.x = vc.x;
                if (vc.y > MaxVertice.y) MaxVertice.y = vc.y;
            }

            Vector2 Remap(Vector2 Pos)
            {
                return  (Pos - MinVertice) / (MaxVertice - MinVertice);
            }
            _mesh.uv = new Vector2[_mesh.vertices.Length];
            for (int i = 0; i < _mesh.vertices.Length; i++)
            {
                 _mesh.uv[i] = Remap(_mesh.vertices[i]);
            }
        }else
            _mesh.uv = customshape.verticles.ToArray();
        customshape.GetComponent<MeshFilter>().sharedMesh = _mesh;
    }
    public override void OnInspectorGUI()
    {
        if (_SOcustomshape==null)
        {
            customshape = (CustomShape)target;
            _SOcustomshape = new SerializedObject(target);
            PVerticles = _SOcustomshape.FindProperty("verticles");
            PTriangles = _SOcustomshape.FindProperty("triangles");
            PFixedUv = _SOcustomshape.FindProperty("fixeduv");
        }
        _SOcustomshape.Update();
        if (GUILayout.Button("Update"))
        {
            Debug.Log("Updated");
            UpdateMeshInformation();
            UpdateEditorMesh();
            GenerateCollider();
            if (PrefabUtility.IsPartOfPrefabInstance(customshape.gameObject))
                PrefabUtility.ApplyPrefabInstance(customshape.gameObject, InteractionMode.AutomatedAction);
        }
        EditorGUILayout.PropertyField(PVerticles, true);
        EditorGUILayout.PropertyField(PFixedUv,true);
        if (GUI.changed)
        {
            //EditorUtility.SetDirty(target);
            UpdateMeshInformation();
            UpdateEditorMesh();
            GenerateCollider();
            if (PrefabUtility.IsPartOfPrefabInstance(customshape.gameObject))
                PrefabUtility.ApplyPrefabInstance(customshape.gameObject, InteractionMode.AutomatedAction);
        }
        _SOcustomshape.ApplyModifiedProperties();
        
    }
    void OnSceneGUI()
    {
        if (_SOcustomshape == null)
        {
            customshape = (CustomShape)target;
            _SOcustomshape = new SerializedObject(target);
            PVerticles = _SOcustomshape.FindProperty("verticles");
            PTriangles = _SOcustomshape.FindProperty("triangles");
            PFixedUv = _SOcustomshape.FindProperty("fixeduv");
        }
        _SOcustomshape.Update();
        Handles.color = Color.grey;
        for(int i = 0;i < customshape.verticles.Count;i++)
        {
            PVerticles.GetArrayElementAtIndex(i).vector2Value = Handles.FreeMoveHandle(customshape.transform.position + ((Vector3)customshape.verticles[i]), Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(customshape.transform.position + ((Vector3)customshape.verticles[i])), Vector3.zero, Handles.DotHandleCap) - customshape.transform.position;
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            UpdateMeshInformation();
            UpdateEditorMesh();
            GenerateCollider();
            if (PrefabUtility.IsPartOfPrefabInstance(customshape.gameObject))
                PrefabUtility.ApplyPrefabInstance(customshape.gameObject, InteractionMode.AutomatedAction);
        }
        _SOcustomshape.ApplyModifiedProperties();
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CustomShape : MonoBehaviour
{
    //mesh information
    [SerializeField]
    public List<Vector2> verticles = new List<Vector2>();// = new Vector2[3];
    [SerializeField, HideInInspector]
    public int[] triangles;
    public bool fixeduv = false;

    private void GenerateMesh()
    {
        Vector3 Conventer(Vector2 A)
        {
            return new Vector3(A.x, A.y, 0f);
        }
        Mesh _mesh = new Mesh();
        _mesh.vertices = Array.ConvertAll(verticles.ToArray(), Conventer);
        _mesh.triangles = triangles;
        if (fixeduv)
        {
            Vector2 MinVertice = Vector2.positiveInfinity;
            Vector2 MaxVertice = Vector2.negativeInfinity;
            foreach (Vector2 vc in _mesh.vertices)
            {
                if (vc.x < MinVertice.x) MinVertice.x = vc.x;
                if (vc.y < MinVertice.y) MinVertice.y = vc.y;

                if (vc.x > MaxVertice.x) MaxVertice.x = vc.x;
                if (vc.y > MaxVertice.y) MaxVertice.y = vc.y;
            }

            Vector2 Remap(Vector2 Pos)
            {
                return (Pos - MinVertice) / (MaxVertice - MinVertice);
            }
            Vector2[] uvs = new Vector2[_mesh.vertices.Length] ;
            for (int i = 0; i < _mesh.vertices.Length; i++)
            {
                uvs[i] = Remap(_mesh.vertices[i]);
            }
            _mesh.uv = uvs;
        }
        else
            _mesh.uv = verticles.ToArray();
        GetComponent<MeshFilter>().mesh = _mesh;
    }
    private void GenerateCollider()
    {
        if(GetComponent<PolygonCollider2D>()!=null)
            GetComponent<PolygonCollider2D>().points = verticles.ToArray();
    }
    void Start()
    {
        GenerateMesh();
        GenerateCollider();
    }
}

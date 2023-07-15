using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(LineRenderer))]
//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
public class BezierPath : MonoBehaviour
{
    public bool DynamicShadow = false; 
    public Vector2 firstPoint;
    public Vector2 secondPoint;

    public Vector2 handlerFirstPoint;
    public Vector2 handlerSecondPoint;

    public int pointsQuantity;
    public float ColliderOffset = 0;

    public Material ShadowMaterial;
    //Mesh Information
    [SerializeField, HideInInspector]
    public Vector3[] verticles;
    [SerializeField, HideInInspector]
    public int[] triangles;
    GameObject ShadowGameObject;
    Mesh ShadowMesh;
    //private 

    [HideInInspector]
    public int[] EndingPoints = new int[2];// 0 = lewy, 1 = prawy

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 handlerP0, Vector3 handlerP1, Vector3 p1)
    {
        float u = 1.0f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; //first term
        p += 3f * uu * t * handlerP0; //second term
        p += 3f * u * tt * handlerP1; //third term
        p += ttt * p1; //fourth term

        return p;
    }

    public Vector2[] calculate2DPoints()
    {
        List<Vector2> points = new List<Vector2>();

        points.Add(firstPoint);
        for (int i = 1; i < pointsQuantity; i++)
        {
            points.Add(CalculateBezierPoint((1f / pointsQuantity) * i, firstPoint, handlerFirstPoint, handlerSecondPoint, secondPoint));
        }
        points.Add(secondPoint);

        return points.ToArray();
    }

    public Vector3[] calculate3DPoints()
    {
        List<Vector3> points = new List<Vector3>();

        points.Add(new Vector3(firstPoint.x, firstPoint.y, transform.position.z));
        for (int i = 1; i < pointsQuantity; i++)
        {
            Vector2 Calculated = CalculateBezierPoint((1f / pointsQuantity) * i, firstPoint, handlerFirstPoint, handlerSecondPoint, secondPoint);
            points.Add(new Vector3(Calculated.x,Calculated.y,transform.position.z));
        }
        points.Add(new Vector3(secondPoint.x, secondPoint.y, transform.position.z));

        return points.ToArray();
    }

    private void GenerateMesh()
    {

        ShadowMesh = new Mesh();
        ShadowMesh.vertices = verticles;
        ShadowMesh.triangles = triangles;
        //ShadowMesh.uv = System.Array.ConvertAll(verticles, Conventer);
        ShadowMesh.RecalculateNormals();
        if (DynamicShadow)
        {
            ShadowGameObject = new GameObject();
            //ShadowGameObject.transform.parent = transform.parent;
            ShadowGameObject.AddComponent<MeshFilter>().mesh = ShadowMesh;
            ShadowGameObject.AddComponent<MeshRenderer>().material = ShadowMaterial;
        }
        else
        {
            if(GetComponent<MeshFilter>()==null)
                gameObject.AddComponent<MeshFilter>().mesh = ShadowMesh;
            else GetComponent<MeshFilter>().mesh = ShadowMesh;
            if (GetComponent<MeshRenderer>() == null)
                gameObject.AddComponent<MeshRenderer>().material = ShadowMaterial;
        }
            
    }
    Vector3 ToBottomScreen(Vector3 PointPosition)
    {
        Vector2 FixedLight = (Vector2)ShadowLight.Instance.transform.position;
        float ScreenBottom = Camera.main.transform.position.y - Camera.main.orthographicSize - 1f;
        if (!DynamicShadow) { FixedLight -= (Vector2)transform.position; ScreenBottom -= transform.position.y; } 
        Vector2 Direction = ((Vector2)PointPosition - FixedLight).normalized;
        Vector2 Rezult = FixedLight + Direction * (ScreenBottom - FixedLight.y) / Direction.y;
        return new Vector3(Rezult.x, Rezult.y, PointPosition.z);
    }
    Vector3 GetVerticlePosition(int index, float ZPos)
    {
        Vector2 rezult = (transform.rotation * GetComponent<PolygonCollider2D>().points[index]) + transform.position;
        return new Vector3(rezult.x, rezult.y, ZPos);
    }
    private void UpdateShadows()
    {
        
        int HalfShadowVerticles = (int)(verticles.Length * 0.5f);
        if (DynamicShadow)
            for (int i = 0; i < HalfShadowVerticles; i++)
            {
                verticles[i] = GetVerticlePosition(i, verticles[i].z);
            }
        for (int i = 0; i < HalfShadowVerticles; i++)
        {
            verticles[i + HalfShadowVerticles] = ToBottomScreen(verticles[i]);
        }
        if (ShadowMesh==null)
            ShadowMesh = new Mesh();
        ShadowMesh.vertices = verticles;
        ShadowMesh.triangles = triangles;
        ShadowMesh.RecalculateBounds();
        if (ShadowGameObject != null)
        {
            ShadowGameObject.GetComponent<MeshFilter>().mesh = ShadowMesh;
            ShadowGameObject.GetComponent<MeshRenderer>().material = ShadowMaterial;
        }
        else
            GetComponent<MeshFilter>().mesh = ShadowMesh;
    }
    private void LookForUdpateShadows()
    {
        if(DynamicShadow)
        {
            UpdateShadows();
            return;
        }
        else if (ShadowLight.Instance.isCameraMoved)
        {
            float LewyKraniec()
            {
                return Camera.main.transform.position.x - (float)Screen.width / Screen.height * Camera.main.orthographicSize - 1f;
            }
            float PrawyKraniec()
            {
                return Camera.main.transform.position.x + (float)Screen.width / Screen.height * Camera.main.orthographicSize + 1f;
            }
            if (verticles[EndingPoints[1]].x + transform.position.x < LewyKraniec() || verticles[EndingPoints[0]].x + transform.position.x > PrawyKraniec())
                return;
            else
            {
                UpdateShadows();
                return;
            }
        }
    }
    private void Update()
    {
        LookForUdpateShadows();
    }
    public void Restart()
    {
        int HalfShadowVerticles = (int)(verticles.Length * 0.5f);
        for (int i = 0; i < HalfShadowVerticles; i++)
        {
            verticles[HalfShadowVerticles + i] = verticles[i];
        }
        ShadowMesh = new Mesh();
        ShadowMesh.vertices = verticles;
        ShadowMesh.triangles = triangles;
        if (ShadowGameObject != null)
        {
            ShadowGameObject.GetComponent<MeshFilter>().mesh = ShadowMesh;
            ShadowGameObject.GetComponent<MeshRenderer>().material = ShadowMaterial;
        }
        else
            GetComponent<MeshFilter>().mesh = ShadowMesh;
    }
    private void Start()
    {
        GenerateMesh();
    }
    private void OnDestroy()
    {
        if (ShadowGameObject != null)
            Destroy(ShadowGameObject);
    }
}

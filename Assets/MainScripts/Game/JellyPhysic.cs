using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class JellyPhysic : MonoBehaviour
    {
        private readonly Vector2[] ReferencePointsPositions = { new Vector2(-0.2f, 0.2f), new Vector2(0.2f, 0.2f), new Vector2(-0.2f, -0.2f), new Vector2(0.2f, -0.2f) };

        public float mappingDetail = 5;
        public float springFrequency = 2;
        public float referencePointsMass = 1f;
        public float dampingRatio = 0f;
        [Range(0f, 1f)]
        public float FixingFactor = 1f;
        public float PointsLinearDrag = 0f;

        [SerializeField, HideInInspector]
        public int[] triangles = new int[1];
        [SerializeField, HideInInspector]
        public Vector3[] verticles = new Vector3[1];
        public GameObject[] wheels;

        GameObject[] referencePoints;
        private Vector3[,] offsets;
        private float[,] weights;


        public void ResetJelly()
        {
            for(int i=0;i< ReferencePointsPositions.Length;i++)
            {
                referencePoints[i].transform.transform.localPosition = new Vector3(ReferencePointsPositions[i].x, ReferencePointsPositions[i].y, referencePoints[i].transform.transform.localPosition.z);
            }
            Mesh MNet = GetComponent<MeshFilter>().mesh;
            MNet.vertices = verticles;
        }

        private void GenerateMesh()
        {
            Vector2 Conventer(Vector3 A)
            {
                return new Vector2(A.x, A.y);
            }
            Mesh MNet = GetComponent<MeshFilter>().mesh;
            MNet.vertices = verticles;
            MNet.uv = Array.ConvertAll(verticles,Conventer);
            MNet.triangles = triangles;

            MapVerticesToReferencePoints();
        }
        void MapVerticesToReferencePoints()
        {
            offsets = new Vector3[verticles.Length, referencePoints.Length];
            weights = new float[verticles.Length, referencePoints.Length];

            for (int i = 0; i < verticles.Length; i++)
            {
                float totalWeight = 0;

                for (int j = 0; j < referencePoints.Length; j++)
                {
                    offsets[i, j] = verticles[i] - LocalPosition(referencePoints[j]);
                    weights[i, j] =
                        1 / Mathf.Pow(offsets[i, j].magnitude, mappingDetail);
                    totalWeight += weights[i, j];
                }

                for (int j = 0; j < referencePoints.Length; j++)
                {
                    weights[i, j] /= totalWeight;
                }
            }
        }
        void AssignReferencePoints()
        {
            Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
            referencePoints = new GameObject[ReferencePointsPositions.Length]; //+ wheels.Length];
            //ReferencePoints
            for (int i = 0; i < ReferencePointsPositions.Length; i++)
            {
                referencePoints[i] = new GameObject("ReferencePoint");
                referencePoints[i].layer = 11; //ReferencePoint
                referencePoints[i].tag = gameObject.tag;
                referencePoints[i].AddComponent<CircleCollider2D>().radius = 0.18f;
                referencePoints[i].transform.parent = transform;
                referencePoints[i].transform.localPosition = new Vector3(ReferencePointsPositions[i].x, ReferencePointsPositions[i].y,gameObject.transform.position.z);

                Rigidbody2D body = referencePoints[i].AddComponent<Rigidbody2D>();
                body.interpolation = rigidbody.interpolation;
                body.collisionDetectionMode = rigidbody.collisionDetectionMode;
                body.mass = referencePointsMass;
                body.drag = PointsLinearDrag;
                AttachWithSpringJoint(referencePoints[i], gameObject, springFrequency);
            }
            //Wheels
            //for (int i = 0; i < wheels.Length; i++)
            //{
            //    referencePoints[ReferencePointsPositions.Length + i] = wheels[i];
            //}
        }
        void AttachWithSpringJoint(GameObject referencePoint, GameObject connected, float frequency)
        {
            SpringJoint2D springJoint =referencePoint.AddComponent<SpringJoint2D>();

            springJoint.connectedBody = connected.GetComponent<Rigidbody2D>();
            springJoint.connectedAnchor = LocalPosition(referencePoint) -LocalPosition(connected);

            springJoint.distance = 0;
            springJoint.autoConfigureDistance = false;
            springJoint.dampingRatio = dampingRatio;
            springJoint.frequency = frequency;
        }
        
        void UpdateVertexPositions()
        {
            Vector3[] _vertices = new Vector3[GetComponent<MeshFilter>().mesh.vertices.Length];
            Vector2 Average = (Vector2)LocalPosition(referencePoints[0]) - referencePoints[0].GetComponent<SpringJoint2D>().connectedAnchor;
            for (int i = 1; i < referencePoints.Length; i++)
                Average += (Vector2)LocalPosition(referencePoints[i]) - referencePoints[i].GetComponent<SpringJoint2D>().connectedAnchor;
            Average /= referencePoints.Length;

            for (int i = 0; i < _vertices.Length; i++)
            {
                _vertices[i] = Vector3.zero;

                for (int j = 0; j < referencePoints.Length; j++)
                {
                    _vertices[i] += weights[i, j] *
                        (LocalPosition(referencePoints[j]) - new Vector3(Average.x,Average.y,0f) * FixingFactor + offsets[i, j]);
                }
            }

            Mesh mesh = GetComponent<MeshFilter>().mesh;
            mesh.vertices = _vertices;
            mesh.RecalculateBounds();
        }
        Vector3 LocalPosition(GameObject obj)
        {
            return transform.InverseTransformPoint(obj.transform.position);
        }
        private void Start()
        {
            //Setting Up
            if(GameInfo.Instance!=null)
                GetComponent<MeshRenderer>().material.SetTexture("_MainTex", GameInfo.Instance.Jellys[GameInfo.Instance.PickedJelly].MainSprite.texture);
        }
        private void Awake()
        {
            AssignReferencePoints();
            GenerateMesh();
        }
        void Update()
        {
            UpdateVertexPositions();
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScene;

public class MoneyController : MonoBehaviour
{
    readonly float HalfXsize = 0.3f;
    Transform SavedParent;
    Vector2 SavedPos;
    //private readonly float AnimationVelocity = 5f; // Units per sec
    //private bool isAnimating = false;
    public void Restart()
    {
        gameObject.SetActive(true);
        if (GetComponentInParent<ParticleSystem>() != null)
            GetComponentInParent<ParticleSystem>().Play();
    }
    private void GenerateMesh()
    {
        Vector3 []verticles = {new Vector3(0f, 2*HalfXsize, 0f), new Vector3(HalfXsize, 0f, 0f), new Vector3(0f, -2*HalfXsize, 0f), new Vector3(-HalfXsize, 0, 0f)};
        GetComponent<MeshFilter>().mesh.vertices = verticles;
        Vector2[] uvs = { new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)};
        GetComponent<MeshFilter>().mesh.uv = uvs;
        int[] Triangles = { 0, 1, 2, 2, 3, 0 };
        GetComponent<MeshFilter>().mesh.triangles = Triangles;
    }

    void Start()
    {
        //Generating Mesh
        GenerateMesh();
        //Animation
        LeanTween.moveLocalY(gameObject, transform.localPosition.y + 0.6f, 1f).setLoopPingPong();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            LeanTween.cancel(gameObject);
            if(GetComponentInParent<ParticleSystem>()!=null)
                GetComponentInParent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
            SavedParent = transform.parent;
            SavedPos = transform.position;
            transform.parent = MoneyCounterController.Instance.transform;
            LeanTween.scale(gameObject, new Vector3(0, 0, 1), 0.8f);
            LeanTween.moveLocal(gameObject, transform.InverseTransformPoint(MoneyCounterController.Instance.GetPosition()), 0.5f).setOnComplete(()=> 
            {
                if(MoneyCounterController.Instance!=null)
                    MoneyCounterController.Instance.AddMoney();
            });
            LeanTween.delayedCall(1f,() => 
            {
                if(this!=null&& GameInfo.Instance!=null&& GameInfo.Instance.gamemode == GameInfo._GameMode.Constant)
                {
                    Destroy(gameObject); 
                }else
                {
                    gameObject.SetActive(false);
                    transform.parent = SavedParent;
                    transform.localScale = new Vector3(1, 1, 1);
                    transform.position = new Vector3(SavedPos.x, SavedPos.y,transform.position.z);
                }
                    
            } );
        }
    }
}

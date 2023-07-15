using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameScene
{
    public class ConstantLevelSpawner : MonoBehaviour
    {
        public static ConstantLevelSpawner Instance;

        public GameObject AntiBack;

        public List<GameObject> StartSegments = new List<GameObject>();
        public List<GameObject> NormalSegments = new List<GameObject>();

        private GameObject PreviousSegment = null;

        private Vector2 EndingPoint;
        private readonly float SpawnRange = 20f;
        private readonly float DestroingRange = 60f;

        public void Restart()
        {
            //Destroying
            foreach (SegmentsConnecter sc in FindObjectsOfType<SegmentsConnecter>())
            {
                    Destroy(sc.gameObject);
            }
            SetStartSegment();
        }
        void SetStartSegment()
        {
            GameObject ga = Instantiate(StartSegments[Random.Range(0, StartSegments.Count)]);
            EndingPoint = ga.GetComponent<SegmentsConnecter>().Stop + (Vector2)ga.transform.position;
            AntiBack.transform.position = new Vector3(-20, 0, 0);
        }
        void CheckForUpdate()
        {
            while (GameJelly.Instance.transform.position.x + SpawnRange > EndingPoint.x)
            {
                //Spawn 
                GameObject segment = NormalSegments[0] ;
                for(int i=0;i<20;i++)
                {
                    segment = NormalSegments[Random.Range(0, NormalSegments.Count)];
                    if (segment.GetComponent<SegmentsConnecter>().Start.y == EndingPoint.y&&segment!=PreviousSegment)
                        break;
                }
                PreviousSegment = segment;
                GameObject ga = Instantiate(segment);
                ga.transform.position = new Vector3(EndingPoint.x - segment.GetComponent<SegmentsConnecter>().Start.x, ga.transform.position.y,ga.transform.position.z);
                EndingPoint = ga.GetComponent<SegmentsConnecter>().Stop + (Vector2)ga.transform.position;
            }
            foreach(SegmentsConnecter sc in FindObjectsOfType<SegmentsConnecter>()) // Destroying
            {
                if (GameJelly.Instance.transform.position.x > sc.Stop.x + sc.gameObject.transform.position.x  + DestroingRange)
                {
                    AntiBack.transform.position = new Vector3(sc.Stop.x+sc.transform.position.x+18f, 0, 0);
                    Destroy(sc.gameObject);
                }
            }
        }

        void Start()
        {
            SetStartSegment();
        }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }
        // Update is called once per frame
        private void FixedUpdate()
        {
            CheckForUpdate();
        }
    }
}

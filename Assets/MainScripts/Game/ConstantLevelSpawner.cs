using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameScene
{
    public class ConstantLevelSpawner : MonoBehaviour
    {
        public static ConstantLevelSpawner Instance;

        public GameObject AntiBack;

        public bool TestSegment = false;
        public int TestSegmentIndex = 0;

        public List<GameObject> StartSegments = new List<GameObject>();
        public float[] test = new float[2];
        public List<GameObject> NormalSegments = new List<GameObject>();
        private List<GameObject>[] ClassifiedNormalSegments = new List<GameObject>[5];

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
        #region Previous
        private GameObject[] PreviousSegment = null;
        public int PreviousSegmentCount = 5;
        bool WasSegmentPrevious(GameObject seg)
        {
            foreach (GameObject ga in PreviousSegment)
                if (ga == seg) return true;
            return false;
        }
        void PushPreviousSegment(GameObject seg)
        {
            //Offset
            for (int i = PreviousSegmentCount - 1; i > 0; i--)
            {
                PreviousSegment[i] = PreviousSegment[i - 1];
            }
                
            PreviousSegment[0] = seg;
        }
        void SetPrevoiusSegmentCount()
        {
            PreviousSegment = new GameObject[PreviousSegmentCount];
        }
        int GenerateDifficulty()
        {
            /*
             *   LEVEL  DIFFICULTY 
             *  ~ 0-5   ==> 1
             *  ~ 6-10  ==> 2
             *  ~ 11-20 ==> 3
             *  ~ 21-30 ==> 4
             *  ~ 31-40 ==> 5
             *  
             *  + 10% Chance to spawn higher difficulty
             */
            int MaxDif = 1;
            int CurrentLevel = GameInfo.Instance.ConstantModePoints;
            if (CurrentLevel >= 6 && CurrentLevel <= 10)
                MaxDif = 2;
            else if (CurrentLevel >= 11 && CurrentLevel <= 20)
                MaxDif = 3;
            else if (CurrentLevel >= 21 && CurrentLevel <= 30)
                MaxDif = 4;
            else if (CurrentLevel >= 31 && CurrentLevel <= 40)
                MaxDif = 5;
            int Result = Random.Range(1,MaxDif);
            if (Result < 5 && Random.Range(1, 100) <= 10)
                Result++;
            return Result-1;
        }
        #endregion Previous
        void CheckForUpdate()
        {
            while (GameJelly.Instance.transform.position.x + SpawnRange > EndingPoint.x)
            {
                //Spawn 
                GameObject segment = NormalSegments[0];
                if (TestSegment)
                {
                    segment = NormalSegments[TestSegmentIndex];
                }
                else for (int i = 0; i < 30; i++)
                    {
                        int Dif;
                        do
                            Dif = GenerateDifficulty();
                        while (ClassifiedNormalSegments[Dif].Count == 0);
                        segment = ClassifiedNormalSegments[Dif][Random.Range(0, ClassifiedNormalSegments[Dif].Count)];
                        if (segment.GetComponent<SegmentsConnecter>().Start.y == EndingPoint.y && !WasSegmentPrevious(segment))
                            break;
                    }
                PushPreviousSegment(segment);
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
            SetPrevoiusSegmentCount();
            SetStartSegment();
        }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //Setup
                for(int i=0;i<5;i++)
                    ClassifiedNormalSegments[i] = new List<GameObject>();
                foreach (GameObject ga in NormalSegments)
                {
                    ClassifiedNormalSegments[ga.GetComponent<SegmentsConnecter>().Difficulty-1].Add(ga);
                }
            }
            else Destroy(gameObject);
        }
        // Update is called once per frame
        private void FixedUpdate()
        {
            CheckForUpdate();
        }
    }
}

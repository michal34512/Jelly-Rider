using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameScene
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;

        public Transform target;
        public Vector2 Range;
        public float Volume;

        public bool ReverseX = false;

        private byte FixFirstShadowFrames = 10;
        private bool PositionChanged = false;

        public Vector3 position
        {
            set
            {
                gameObject.transform.position = value;
                PositionChanged = true;
            }
            get
            {
                return gameObject.transform.position;
            }
        }
        Vector2 GetTargetPosition()
        {
            return transform.InverseTransformPoint(target.position);
        }
        Vector2 GetClosestPoint(Vector2 A)
        {
            Vector2 Point;
            if(ReverseX)
            {
                Point.x = 0;
            }
            else
            {
                if (A.x > Range.x * 0.5f)
                    Point.x = Range.x * 0.5f;
                else if (A.x < Range.x * -0.5f)
                    Point.x = -Range.x * 0.5f;
                else Point.x = A.x;
            }

            if (A.y > Range.y * 0.5f)
                Point.y = Range.y * 0.5f;
            else if (A.y < Range.y * -0.5f)
                Point.y = -Range.y * 0.5f;
            else Point.y = A.y;

            return Point;
        }
        void FollowPoint(Vector2 Difference)
        {
            // transform.localPosition -= new Vector3(Difference.x * Time.deltaTime * 4, Difference.y, 0f);
            if(FixFirstShadowFrames>0||PositionChanged)
            {
                FixFirstShadowFrames--;
                ShadowLight.Instance.isCameraMoved = true;
            }else 
                ShadowLight.Instance.isCameraMoved = Difference != Vector2.zero;
            transform.localPosition -= new Vector3(Difference.x , Difference.y, 0f);
        }
        void UpdateRange()
        {
            Vector2 TPosition = GetTargetPosition();
            Vector2 TargetPoint = GetClosestPoint(TPosition);
            Vector2 Difference = TargetPoint - TPosition;
            FollowPoint(Difference);
        }
        void Update()
        {
            UpdateRange();
        }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }
    }
}


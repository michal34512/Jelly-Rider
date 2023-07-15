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
        public Vector2 RangeOffset = new Vector2(0,0);
        public float Volume;

        public bool ReverseX = false;

        public bool SizeByVelocity = true;
        private readonly float[] CameraSizes = {5, 8};
        private readonly float Velocity =  15;
        private float SizeVelocity = 0f;
        private float SizeAcceleration = 1f;

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
                if (A.x > Range.x * 0.5f + RangeOffset.x)
                    Point.x = Range.x * 0.5f + RangeOffset.x;
                else if (A.x < Range.x * -0.5f + RangeOffset.x)
                    Point.x = -Range.x * 0.5f + RangeOffset.x;
                else Point.x = A.x;
            }

            if (A.y > Range.y * 0.5f + RangeOffset.y)
                Point.y = Range.y * 0.5f + RangeOffset.y;
            else if (A.y < Range.y * -0.5f + RangeOffset.y)
                Point.y = -Range.y * 0.5f + RangeOffset.y;
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
        private void UpdateSizeByVelocity()
        {
            if (SizeByVelocity && target.GetComponentInChildren<Rigidbody2D>() != null)
            {
                float factor = target.GetComponentInChildren<Rigidbody2D>().velocity.x / Velocity;
                float _Size = CameraSizes[0] +  Mathf.Abs(factor *(CameraSizes[1] - CameraSizes[0]));
                SizeVelocity = SizeAcceleration * Time.deltaTime * (_Size - Camera.main.orthographicSize); //velocity per frame
                Camera.main.orthographicSize = Camera.main.orthographicSize + SizeVelocity;
            }
        }
        void UpdateRange()
        {
            Vector2 TPosition = GetTargetPosition();
            Vector2 TargetPoint = GetClosestPoint(TPosition);
            Vector2 Difference = TargetPoint - TPosition;
            FollowPoint(Difference);
            UpdateSizeByVelocity();
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


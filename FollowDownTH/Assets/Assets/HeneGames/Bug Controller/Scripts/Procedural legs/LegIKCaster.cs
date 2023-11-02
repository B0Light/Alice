using UnityEngine;

namespace HeneGames.BugController
{
    public class LegIKCaster : MonoBehaviour
    {
        private LayerMask raycastMask;
        private Transform body;
        private float stepHeight;
        private float distanceToBody;
        private float verticalRaycastLenght;
        private Vector3 correctGroundPos;
        private Transform casterStartPos;
        private bool horizontal;
        private float horizontalPoint;

        [Header("Transform references")]
        public Transform caster;
        public Transform stepHeightPos;
        [SerializeField] private Transform groundPos;

        [Header("Raycast settings")]
        [SerializeField] private bool drawLineInEditor;

        private void Update()
        {
            stepHeightPos.position = new Vector3(groundPos.position.x, groundPos.position.y + stepHeight, groundPos.position.z);

            //Cannot reach raycast
            Ray reachRay = new Ray(casterStartPos.position, casterStartPos.forward);
            RaycastHit reachHitInfo;

            if (Physics.Raycast(reachRay, out reachHitInfo, Mathf.Infinity, raycastMask))
            {
                verticalRaycastLenght = reachHitInfo.distance;

                if (drawLineInEditor)
                {
                    Debug.DrawLine(reachRay.origin, reachHitInfo.point, Color.cyan);
                }
            }

            //Raycast body to ground pos
            Vector3 _directionToGroundpos = groundPos.position - body.position;
            Ray adjustLegCast = new Ray(body.position, _directionToGroundpos);
            RaycastHit adjustLegHitInfo;

            if (Physics.Raycast(adjustLegCast, out adjustLegHitInfo, Mathf.Infinity, raycastMask))
            {
                correctGroundPos = adjustLegHitInfo.point;

                if (drawLineInEditor)
                {
                    Debug.DrawLine(adjustLegCast.origin, adjustLegHitInfo.point, Color.blue);
                }
            }

            //Vertical raycast
            Ray ray = new Ray(caster.transform.position, caster.transform.forward);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, raycastMask))
            {
                groundPos.transform.position = hitInfo.point;

                if (drawLineInEditor)
                {
                    Debug.DrawLine(ray.origin, hitInfo.point, Color.green);
                }
            }
            else
            {
                if (drawLineInEditor)
                {
                    Debug.DrawLine(ray.origin, ray.GetPoint(5f), Color.red);
                }
            }

            //Horizontal raycast
            Vector3 _direction = transform.position - body.position;
            Ray ray2 = new Ray(body.position, _direction);
            RaycastHit hitInfo2;

            if (Physics.Raycast(ray2, out hitInfo2, distanceToBody, raycastMask))
            {
                caster.transform.position = hitInfo2.point;
                Debug.DrawLine(ray2.origin, hitInfo2.point, Color.red);
            }
            else
            {
                if(horizontal)
                {
                    caster.transform.position = ray2.GetPoint(horizontalPoint);
                    Debug.DrawLine(ray2.origin, ray2.GetPoint(horizontalPoint), Color.green);
                }
                else
                {
                    caster.transform.localPosition = Vector3.zero;
                    Debug.DrawLine(ray2.origin, ray2.GetPoint(distanceToBody), Color.green);
                }
            }
        }

        public Transform GetGroundPos()
        {
            return groundPos;
        }

        public Vector3 GroundPos()
        {
            //If blue ray is way too long return groundPos position instead
            float _distanceToCorrectPos = Vector3.Distance(transform.position, correctGroundPos);
            if(_distanceToCorrectPos > 10f)
            {
                return groundPos.position;
            }
            else
            {
                return correctGroundPos;
            }
        }

        public void SetHorizontalPoint(bool _value ,float _point)
        {
            horizontal = _value;
            horizontalPoint = _point;
        }

        public Vector3 GetPointFromGroundCaster(float _point)
        {
            Ray ray = new Ray(caster.transform.position, caster.transform.forward);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, raycastMask))
            {
                return ray.GetPoint(_point);
            }
            else
            {
                return ray.GetPoint(_point);
            }
        }

        public float VerticalRaycastLenght()
        {
            return verticalRaycastLenght;
        }

        public void SetupBodyAndLeg(Transform _body, Transform _leg)
        {
            body = _body;
            distanceToBody = Vector3.Distance(transform.position, body.position);
            stepHeightPos.SetParent(_leg);

            GameObject _cannotReachCasterOB = new GameObject("Cannot reach ob");
            _cannotReachCasterOB.transform.parent = body;
            _cannotReachCasterOB.transform.localPosition = transform.localPosition;
            _cannotReachCasterOB.transform.localRotation = transform.localRotation;
            casterStartPos = _cannotReachCasterOB.transform;
        }

        public void SetupStepHeight(float _height)
        {
            stepHeight = _height;
        }

        public void SetupRaycastMask(LayerMask _mask)
        {
            raycastMask = _mask;
        }
    }
}
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

namespace HeneGames.BugController
{
    public class IKLeg : MonoBehaviour
    {
        //private float maxReachDistance = 2f;

        private bool verticalOverride;
        private float verticalPos;
        private bool horizontalOverride;
        private float horizontalPos;
        private bool footHitGroundBool;
        private bool forceMoveLeg;
        private bool step;
        private bool walkCompleted;
        private Transform groundPos;
        private Transform walkPos;
        private Vector3 nextGroundPos;
        private Vector3 moveDirLastPos;
        private Vector3 stepAnticipatePos;
        private Vector3 stepAnticipatePosSmooth;
        private LegIKCaster iKCaster;
        private BugController bugController;

        [Header("References")]
        [SerializeField] private Transform ikCasterPos;
        [SerializeField] private Transform ikTarget;
        [SerializeField] private RigBuilder rigBuilder;

        [Header("Values")]
        [SerializeField] private float stepSpeed = 20f;
        [SerializeField] private float downRayMaxLength = 3f;
        [SerializeField] private float horizontalPointIfOverMaxLength = 1f;

        [Header("Events")]
        [SerializeField] private UnityEvent footHitGround;

        private void Update()
        {
            if (ikTarget != null && !verticalOverride)
            {
                float _distanceToLastPos = Vector3.Distance(groundPos.position, nextGroundPos);

                if(forceMoveLeg)
                {
                    nextGroundPos = iKCaster.GroundPos();
                    forceMoveLeg = false;
                    walkCompleted = false;
                    footHitGroundBool = false;
                }

                if (step)
                {
                    if (walkCompleted)
                    {
                        footHitGround.Invoke();
                        nextGroundPos = iKCaster.GroundPos();
                        walkCompleted = false;
                        step = false;
                        footHitGroundBool = false;
                    }
                }

                //Step anticipate 
                Vector3 _moveDir = (transform.position - moveDirLastPos) * bugController.StepAnticipateStrenght();
                moveDirLastPos = transform.position;
                stepAnticipatePos = new Vector3(_moveDir.x, 0f, _moveDir.z);
                stepAnticipatePosSmooth = Vector3.Lerp(stepAnticipatePosSmooth, stepAnticipatePos, 10f * Time.deltaTime);

                //Walk
                float _distanceToWalkPos = Vector3.Distance(ikTarget.position, walkPos.position + stepAnticipatePosSmooth);

                if (_distanceToWalkPos > 0.4f && !walkCompleted)
                {
                    ikTarget.position = Vector3.Lerp(ikTarget.position, walkPos.position + stepAnticipatePosSmooth, Time.deltaTime * stepSpeed);
                }
                else
                {
                    walkCompleted = true;

                    float _distanceToNextGroundPos = Vector3.Distance(ikTarget.position, nextGroundPos + stepAnticipatePosSmooth);

                    if (_distanceToNextGroundPos > 0.1f)
                    {
                        ikTarget.position = Vector3.Lerp(ikTarget.position, nextGroundPos + stepAnticipatePosSmooth, Time.deltaTime * stepSpeed);
                    }
                    else
                    {
                        ikTarget.position = nextGroundPos + stepAnticipatePosSmooth;
                    }
                }
            }

            if(verticalOverride)
            {
                ikTarget.position = Vector3.Lerp(ikTarget.position, iKCaster.GetPointFromGroundCaster(verticalPos), Time.deltaTime * stepSpeed);
            }

            if (horizontalOverride)
            {
                iKCaster.SetHorizontalPoint(horizontalOverride, horizontalPos);
            }
            else if (CannotReach())
            {
                iKCaster.SetHorizontalPoint(true, horizontalPointIfOverMaxLength);
            }
            else
            {
                iKCaster.SetHorizontalPoint(false, horizontalPos);
            }

            //Foot hit ground event when stopped
            float _distanceToNextGroundPos2 = Vector3.Distance(ikTarget.position, nextGroundPos + stepAnticipatePosSmooth);
            if (_distanceToNextGroundPos2 < 0.01f && !footHitGroundBool)
            {
                footHitGround.Invoke();
                footHitGroundBool = true;
            }
        }

        private void FixedUpdate()
        {
    
        }

        public void SetupIK(Transform _groundPos, Transform _walkPos, LegIKCaster _legIKCaster, BugController _bugController)
        {
            groundPos = _groundPos;
            walkPos = _walkPos;
            nextGroundPos = _groundPos.position;
            rigBuilder.enabled = true;
            iKCaster = _legIKCaster;
            bugController = _bugController;
        }

        public Transform IKCasterPos()
        {
            return ikCasterPos;
        }

        public bool CannotReach()
        {
            if(iKCaster.VerticalRaycastLenght() > downRayMaxLength)
            {
                return true;
            }

            return false;
        }

        public void MoveLeg()
        {
            forceMoveLeg = true;
        }

        public void Step()
        {
            step = true;
        }

        public void SetLegVerticalPosition(bool _value, float _pos)
        {
            verticalOverride = _value;
            verticalPos = _pos;
        }

        public void SetMaxHorizontalPoint(bool _horizontal, float _horizontalPoint)
        {
            horizontalOverride = _horizontal;
            horizontalPos = _horizontalPoint;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames.BugController
{
    public class BugController : MonoBehaviour
    {
        private float stopTimer;
        private float lastYAngle;
        private int currentLegIndex;
        private bool legsCanBeCorrected;
        private Vector3 lastPos;
        private Vector3 pos;
        private Vector3 bodyLocalPos;
        private IKLeg[] iKLegs;
        private List<Transform> legsGroundPositions = new List<Transform>();

        [Header("References")]
        [SerializeField] private LegIKCaster legIKCasterPrefab;
        [SerializeField] private Transform body;
        [SerializeField] private Transform groundCheck;

        [Header("Moving values")]
        [Tooltip("This determines whether the bug's body conforms to the terrain")]
        [SerializeField] private bool groundAlignment = true;

        [Tooltip("See documentation")]
        [SerializeField] private bool moveTwoLegsAtTheSameTime = true;

        [Tooltip("Feet only hit objects of this physics level")]
        [SerializeField] private LayerMask groundMask;

        [Tooltip("This defines how strongly the bug's body adapts to the terrain")]
        [Range(1f, 10f)]
        [SerializeField] private float groundAlignmentStrenght = 3f;

        [Tooltip("This determines how strongly the legs anticipate which direction the body is going")]
        [Range(0f, 15f)]
        [SerializeField] private float anticipateStepStrenght = 5f;

        [Tooltip("This adjustment determines how high the body is off the ground")]
        [Range(0.01f, 2f)]
        [SerializeField] private float bodyHeight = 1.5f;

        [Tooltip("This value defines how strongly the height of the body is fine-tuned when parts of the legs are higher than the body")]
        [Range(0.01f, 4f)]
        [SerializeField] private float adjustBodyHeight = 1.5f;

        [Tooltip("This determines how high the foot rises when taking a step")]
        [Range(0.5f, 4f)]
        [SerializeField] private float stepHeight = 1f;

        [Tooltip("How far the body must move before taking the next step")]
        [Range(0.01f, 4f)]
        [SerializeField] private float moveTreshold = 0.4f;

        [Tooltip("How many degrees does the body have to rotate before the leg correction is done")]
        [Range(5f, 50f)]
        [SerializeField] private float moveAngleTreshold = 20f;

        [Tooltip("How long does the bug have to be in place before the legs are placed again")]
        [Range(0f, 1f)]
        [SerializeField] private float correctLegWhenSoppedTime = 0.3f;

        private void Awake()
        {
            iKLegs = GetComponentsInChildren<IKLeg>();
        }

        private void Start()
        {
            SetupLegs();
        }

        private void Update()
        {
            MoveLegs();

            LegCorrection();

            StopTimer();

            AdjustBody();
        }

        #region Core

        private void SetupLegs()
        {
            for (int i = 0; i < iKLegs.Length; i++)
            {
                LegIKCaster _ikCaster = Instantiate(legIKCasterPrefab, body);
                _ikCaster.transform.position = iKLegs[i].IKCasterPos().position;
                _ikCaster.transform.rotation = iKLegs[i].IKCasterPos().rotation;

                _ikCaster.SetupBodyAndLeg(body, iKLegs[i].transform);
                _ikCaster.SetupStepHeight(stepHeight);
                _ikCaster.SetupRaycastMask(groundMask);

                legsGroundPositions.Add(_ikCaster.GetGroundPos());

                iKLegs[i].SetupIK(_ikCaster.GetGroundPos(), _ikCaster.stepHeightPos, _ikCaster, this);
            }

            lastPos = transform.position;
            lastYAngle = transform.eulerAngles.y;
        }

        private void AdjustBody()
        {
            groundCheck.localPosition = new Vector3(0f, bodyHeight, 0f);

            bodyLocalPos = new Vector3(0f, AvarageHeight() + bodyHeight, 0f);

            body.localPosition = Vector3.Lerp(body.localPosition, bodyLocalPos, 10f * Time.deltaTime);

            if (groundAlignment)
            {
                Quaternion _rotation = Quaternion.FromToRotation(body.transform.up, GroundNormal()) * body.transform.rotation;
                body.transform.rotation = Quaternion.Lerp(body.transform.rotation, _rotation, groundAlignmentStrenght * Time.deltaTime);
            }
        }

        private void StopTimer()
        {
            if (!Moving(1f))
            {
                stopTimer += Time.deltaTime;
            }
            else
            {
                stopTimer = 0f;
            }
        }

        private void MoveLegs()
        {
            float _distanceToLastPos = Vector3.Distance(body.transform.position, lastPos);

            if (_distanceToLastPos > moveTreshold)
            {
                MoveNextLeg();
            }
        }

        private void LegCorrection()
        {
            if (stopTimer > correctLegWhenSoppedTime && legsCanBeCorrected)
            {
                StartCoroutine(CorrectLegs());
                legsCanBeCorrected = false;
            }

            if (body.transform.eulerAngles.y > lastYAngle + moveAngleTreshold || body.transform.eulerAngles.y < lastYAngle - moveAngleTreshold)
            {
                MoveNextLeg();
                lastYAngle = body.transform.eulerAngles.y;
            }
        }

        private void MoveNextLeg()
        {
            legsCanBeCorrected = true;
            lastPos = body.transform.position;

            if(moveTwoLegsAtTheSameTime)
            {
                if (iKLegs.Length == 2)
                {
                    OneLegAtTheSameTime();
                }
                else if (iKLegs.Length >= 4)
                {
                    TwoLegAtTheSameTime();
                }
            }
            else
            {
                OneLegAtTheSameTime();
            }
        }

        private void OneLegAtTheSameTime()
        {
            int _lastIndex = iKLegs.Length - 1;

            if (currentLegIndex < _lastIndex)
            {
                iKLegs[currentLegIndex].Step();

                currentLegIndex++;
            }
            else if (currentLegIndex == _lastIndex) //Last leg index
            {
                iKLegs[_lastIndex].Step();

                currentLegIndex = 0;
            }
        }

        private void TwoLegAtTheSameTime()
        {
            int _lastIndex = iKLegs.Length - 1;

            if (currentLegIndex < _lastIndex - 1)
            {
                iKLegs[currentLegIndex].Step();
                iKLegs[currentLegIndex + 1].Step();

                currentLegIndex += 2;
            }
            else if (currentLegIndex == _lastIndex - 1) //Last leg index
            {
                iKLegs[currentLegIndex].Step();
                iKLegs[currentLegIndex + 1].Step();

                currentLegIndex = 0;
            }
        }

        private void FourLegAtTheSameTime()
        {
            int _lastIndex = iKLegs.Length - 1;

            if (currentLegIndex < _lastIndex - 4)
            {
                iKLegs[currentLegIndex].Step();
                iKLegs[currentLegIndex + 1].Step();
                iKLegs[currentLegIndex + 2].Step();
                iKLegs[currentLegIndex + 3].Step();

                currentLegIndex += 4;
            }
            else if (currentLegIndex == _lastIndex - 3) //Last leg index
            {
                iKLegs[currentLegIndex].Step();
                iKLegs[currentLegIndex + 1].Step();
                iKLegs[currentLegIndex + 2].Step();
                iKLegs[currentLegIndex + 3].Step();

                currentLegIndex = 0;
            }
        }

        private float AvarageHeight()
        {
            float _heights = 0f;
            int _current = 0;

            while(_current < legsGroundPositions.Count)
            {
                float _height = legsGroundPositions[_current].position.y - transform.position.y;
                _heights += _height;

                _current++;
            }

            float _avarageHeight = _heights / legsGroundPositions.Count;

            if(_avarageHeight > 0f && _avarageHeight < bodyHeight)
            {
                return _avarageHeight * adjustBodyHeight;
            }
            else
            {
                return 0f;
            }
        }

        private Vector3 GroundNormal()
        {
            Ray ray = new Ray(groundCheck.position, -transform.up);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, groundMask))
            {
                Debug.DrawLine(ray.origin, hitInfo.point, Color.green);

                return hitInfo.normal;
            }

            return Vector3.zero;
        }

        private float DistanceToGround()
        {
            Ray ray = new Ray(groundCheck.position, -transform.up);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, groundMask))
            {
                float _distance = Vector3.Distance(body.position, hitInfo.point);

                return _distance;
            }

            return 0f;
        }

        private bool OneOfTheLegsDoesNotREach()
        {
            for (int i = 0; i < iKLegs.Length; i++)
            {
                if (iKLegs[i].CannotReach())
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerator CorrectLegs()
        {
            int _currentLeg = 0;

            while (_currentLeg < iKLegs.Length)
            {
                iKLegs[_currentLeg].MoveLeg();
                _currentLeg++;
                yield return new WaitForSeconds(0.05f);
            }
        }

        private bool Moving(float _treshold)
        {
            float _speed = (pos - transform.position).magnitude / Time.deltaTime;

            pos = transform.position;

            if (_speed > _treshold)
            {
                return true;
            }

            return false;
        }

        public float StepAnticipateStrenght()
        {
            return anticipateStepStrenght;
        }

        #endregion

        #region Public functions

        public void AdjustLegs()
        {
            StartCoroutine(CorrectLegs());
            legsCanBeCorrected = false;
        }

        public void SetBodyHeight(float _newHeight)
        {
            bodyHeight = _newHeight;
        }

        public void SetGroundAlignment(bool _value)
        {
            groundAlignment = _value;
        }

        public void SetLegsVerticalPostition(bool _override, float _toPoint)
        {
            foreach(IKLeg _iKLeg in iKLegs)
            {
                _iKLeg.SetLegVerticalPosition(_override, _toPoint);
            }
        }

        public void SetLegsHorizontalPosition(bool _override, float _horizontalPoint)
        {
            foreach (IKLeg _iKLeg in iKLegs)
            {
                _iKLeg.SetMaxHorizontalPoint(_override, _horizontalPoint);
            }
        }

        public float Speed()
        {
            float _speed = (pos - transform.position).magnitude / Time.deltaTime;

            pos = transform.position;

            return _speed;
        }

        public void LookTarget(Transform _target, float _rotateSpeed)
        {
            Vector3 direction = (_target.position - body.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            body.transform.rotation = Quaternion.Slerp(body.transform.rotation, lookRotation, Time.deltaTime * _rotateSpeed);
        }

        public void LookPos(Vector3 _target, float _rotateSpeed)
        {
            Vector3 direction = (_target - body.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            body.transform.rotation = Quaternion.Slerp(body.transform.rotation, lookRotation, Time.deltaTime * _rotateSpeed);
        }

        #endregion
    }
}
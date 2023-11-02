using UnityEngine;
using UnityEngine.AI;

namespace HeneGames.BugController
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshController : MonoBehaviour
    {
        private NavMeshAgent agent;

        public virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        #region Public functions

        /// <summary>
        /// Set nav mesh component speed
        /// </summary>
        /// <param name="_speed"></param>
        public void NavControllerSetSpeed(float _speed)
        {
            agent.speed = _speed;
        }

        /// <summary>
        /// The nav mesh component calculates the route away from the given point
        /// </summary>
        /// <param name="_runAway"></param>
        public void NavControllerRunAwayFromPos(Vector3 _runAway)
        {
            Vector3 _dirToTarget = (transform.position - _runAway);

            Vector3 _newPos = transform.position + _dirToTarget;

            agent.SetDestination(_newPos * 4f);

           //NavControllerMoveToNextPosition(_newPos, 5f, true);
        }

        /// <summary>
        /// Returns nav mesh current destination in Vector3
        /// </summary>
        /// <returns></returns>
        public Vector3 NavControllerCurrentWalkDestination()
        {
            return agent.destination;
        }

        /// <summary>     
        /// Gives an order to the nav mesh component to move to the given point. If the offset area value is above zero, the nav mesh component sets a sphere size of the offset area at the given point,and searches within it for a point to navigate to.
        /// If force move boolean is false, the nav mesh component moves to the given point only when it has reached the previous point. 
        /// If force move boolean is true, the nav mesh component moves immediately to the given point.
        /// </summary>
        /// <param name="_movePosition"></param>
        /// <param name="_offsetArea"></param>
        /// <param name="_forceMove"></param>
        public void NavControllerMoveToNextPosition(Vector3 _movePosition, float _offsetArea, bool _forceMove = false)
        {
            if (agent.enabled)
            {
                if(_offsetArea > 0f)
                {
                    Vector3 randomPos = Random.insideUnitSphere * _offsetArea;
                    NavMeshHit navHit;
                    NavMesh.SamplePosition(_movePosition + randomPos, out navHit, _offsetArea, NavMesh.AllAreas);

                    if (!NavControllerCurrentlyNavigate() && !_forceMove)
                    {
                        agent.SetDestination(navHit.position);
                    }
                    else if (_forceMove)
                    {
                        agent.SetDestination(navHit.position);
                    }
                }
                else
                {
                    if (!NavControllerCurrentlyNavigate() && !_forceMove)
                    {
                        agent.SetDestination(_movePosition);
                    }
                    else if (_forceMove)
                    {
                        agent.SetDestination(_movePosition);
                    }
                }
            }
        }

        /// <summary>
        /// This can be used to stop or release the nav mehs component completely
        /// </summary>
        /// <param name="_value"></param>
        public void NavControllerStopNow(bool _value)
        {
            if (agent.enabled)
            {
                agent.isStopped = _value;
            }
        }

        /// <summary>
        /// If nav mesh component is currently calculating path or moving to destination this return true
        /// </summary>
        /// <returns></returns>
        public bool NavControllerCurrentlyNavigate()
        {
            if (agent.enabled)
            {
                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
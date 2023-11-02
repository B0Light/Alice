using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames.BugController
{
    public class BugAI : NavMeshController
    {
        private float randomLookTimer;
        private bool lookWhereYouGoing;
        private float forceMoveTimer;
        private float stoppedTimer;
        private float foodFindTimer;
        private Food foodInSight;
        private Vector3 pos;

        [Header("State machine")]
        [SerializeField] private StateOfMind stateOfMind;

        [Header("References")]
        [SerializeField] private BugController bugController;
        [SerializeField] private Transform body;
        [SerializeField] private Transform forwardDirection;

        [Header("Settings")]
        [SerializeField] private float findFoodArea = 10f;
        [SerializeField] private float walkSpeed = 2f;
        [SerializeField] private float runSpeed = 4f;
        [SerializeField] private float wanderDistance = 10f;
        [SerializeField] private float rotateSpeed = 3f;

        private void Update()
        {
            Brains();

            //Stop timer
            if (!Moving(1f))
            {
                stoppedTimer += Time.deltaTime;
            }
            else
            {
                stoppedTimer = 0f;
            }
        }

        private void Brains()
        {
            switch (stateOfMind)
            {
                case StateOfMind.Wander:
                    Wandering();
                    break;
                case StateOfMind.ChasingFood:
                    ChasingFoodUpdate();
                    break;
            }
        }

        private void Wandering()
        {
            //Set speed
            NavControllerSetSpeed(walkSpeed);

            //Move random location
            NavControllerMoveToNextPosition(transform.position, wanderDistance);

            //If without moving for too long
            if (stoppedTimer > 1f && forceMoveTimer <= 0f)
            {
                NavControllerMoveToNextPosition(transform.position, wanderDistance, true);
                forceMoveTimer = 0.5f;
            }

            if(forceMoveTimer > 0f)
            {
                forceMoveTimer -= Time.deltaTime;
            }

            //Randomly look where you are going
            if (randomLookTimer <= 0f)
            {
                int _randomInt = Random.Range(0, 2);

                if(_randomInt == 1)
                {
                    lookWhereYouGoing = true;
                }
                else
                {
                    lookWhereYouGoing = false;
                }

                randomLookTimer = Random.Range(1f, 5f);
            }
            else
            {
                randomLookTimer -= Time.deltaTime;
            }

            if(lookWhereYouGoing)
            {
                bugController.LookPos(NavControllerCurrentWalkDestination(), rotateSpeed);
            }
            else
            {
                bugController.LookPos(forwardDirection.position, rotateSpeed);
            }

            //Try find food in area
            if(foodFindTimer <= 0f)
            {
                foodInSight = FoodInArea(findFoodArea);

                //Food was found
                if(foodInSight != null)
                {
                    NavControllerMoveToNextPosition(foodInSight.transform.position, 0f, true);
                    stateOfMind = StateOfMind.ChasingFood;
                }

                foodFindTimer = 1f;
            }
            else
            {
                foodFindTimer -= Time.deltaTime;
            }
        }

        private void ChasingFoodUpdate()
        {
            //Set speed
            NavControllerSetSpeed(runSpeed);

            //Go near food
            if (foodInSight != null)
            {
                //Look food
                bugController.LookTarget(foodInSight.transform, rotateSpeed);

                //Eat food
                float _distanceToFood = Vector3.Distance(transform.position, foodInSight.transform.position);
                if (_distanceToFood < 2f)
                {
                    foodInSight.Eat();
                    stateOfMind = StateOfMind.Wander;
                }

                //Someone else eat food
                if(foodInSight.Eaten())
                {
                    stateOfMind = StateOfMind.Wander;
                }
                
                //Go near food
                NavControllerMoveToNextPosition(foodInSight.transform.position, 2f);

                //If without moving for too long
                if (stoppedTimer > 1f && forceMoveTimer <= 0f)
                {
                    NavControllerMoveToNextPosition(transform.position, wanderDistance, true);
                    forceMoveTimer = 0.5f;
                }
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

        #region Methods

        private Food FoodInArea(float _area)
        {
            Collider[] _colliders = Physics.OverlapSphere(body.position, _area);

            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i].GetComponent<Food>() != null)
                {
                    Food _food = _colliders[i].GetComponent<Food>();

                    if (!_food.Eaten())
                    {
                        return _colliders[i].GetComponent<Food>();
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
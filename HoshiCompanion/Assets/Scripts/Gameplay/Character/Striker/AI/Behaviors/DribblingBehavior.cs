using Gameplay.Environment.Ball;
using Gameplay.Managers;
using Gameplay.Striker;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace Gameplay.AI.Behaviors
{
    public class DribblingBehavior : StrikerBehavior
    {
        private enum DribbleState
        {
            MovingToPoint,
            WaitingForNewPoint
        }

        [Header("Settings")]
        [SerializeField] private Transform strikerTransform;
        [SerializeField] private DribblingBehaviorData dribblingBehaviorData;
        [SerializeField] private StrikerAI striker;

        private BallBehavior currentBall;

        private Vector3 targetPosition;

        private DribbleState currentDribbleState = DribbleState.WaitingForNewPoint;

        private float staminaCost;
        private float timePassed = 0f;

        private bool isDribbling;
        private bool isDribblingThisFrame;
        private bool hasMovedToBall;

        private void Awake()
        {
            SetName("Dribbling");

            //initialize variables
            if (dribblingBehaviorData)
            {
                xpGain = dribblingBehaviorData.xpGain;
                initialTimeToComplete = dribblingBehaviorData.timeToComplete;
                SetTimeToComplete(dribblingBehaviorData.timeToComplete);
                staminaCost = dribblingBehaviorData.staminaCost;
            }
        }

        protected override void OnBehaviorChanged(StrikerBehavior newBehavior)
        {
            base.OnBehaviorChanged(newBehavior);
        }

        #region FIND NEAREST BALL
        private BallBehavior FindNearestAvailableBall()
        {
            //Enable ball socket trigger
            striker.GetBallSocketTrigger().enabled = true;

            BallBehavior nearestBall = null;

            float minDistance = Mathf.Infinity;

            //For each ball in the list
            foreach (BallBehavior ball in BallManager.Instance.GetObjectsInScene())
            {
                if (!ball.IsPossessed() && !ball.IsBeingUsed() /*&& ball.GetPossessor() == striker*/ && !ball.GetMiniGame().enabled)
                {
                    //Check for the nearest ball
                    if (Vector3.Distance(strikerTransform.position, ball.transform.position) < minDistance)
                    {
                        nearestBall = ball;
                        nearestBall.SetIsBeingUsed(true);
                        minDistance = Vector3.Distance(transform.position, ball.transform.position);
                        break;
                    }
                }
            }

            //If the balls list is empty or the nearest ball is null
            if (BallManager.Instance.GetObjectsInScene().Count == 0 || nearestBall == null)
            {
                Debug.LogWarning("No ball is available");
                return null;
            }

            //Check if the nearest ball is possessed by another striker
            if (nearestBall.IsPossessed() && nearestBall.GetPossessor() != striker)
            {
                return null;
            }

            return nearestBall;
        }
        #endregion

        #region DRIBBLING
        private void Dribble()
        {
            if (!currentBall)
            {
               // currentBall = FindNearestAvailableBall();
                return;
            }

            //if (currentBall.GetPossessor() != striker && currentBall.IsPossessed())
            //{
            //    Debug.Log(striker.gameObject.name + " cannot dribble, ball is already possessed by other striker");
            //    isComplete = true;
            //    return;
            //}

            if (!IsComplete() && !isDribblingThisFrame && currentBall.IsPossessed() && currentBall.GetPossessor() == striker)
            {
                SetIsDribbling(true);

                timePassed += Time.deltaTime;

                switch (currentDribbleState)
                {
                    case DribbleState.WaitingForNewPoint:
                        targetPosition = new Vector3(Random.Range(0, 27f), strikerTransform.position.y, Random.Range(-9f, 4f));
                        currentDribbleState = DribbleState.MovingToPoint;
                        break;

                    case DribbleState.MovingToPoint:
                        strikerTransform.position = Vector3.MoveTowards(strikerTransform.position, targetPosition,
                    striker.GetStrikerMovement().GetSpeed() * Time.deltaTime);
                        if(Vector3.Distance(strikerTransform.position, targetPosition) <= 0.1f)
                        {
                            currentDribbleState = DribbleState.WaitingForNewPoint;
                        }
                        break;
                }

                Vector3 targetDirection = targetPosition - strikerTransform.position;

                if (targetDirection != Vector3.zero)
                {
                    strikerTransform.rotation = Quaternion.Slerp(strikerTransform.rotation, Quaternion.LookRotation(targetPosition - strikerTransform.position),
                        12f * Time.deltaTime);
                }

                //Drain stamina
                striker.GetStrikerStamina().DrainStamina(staminaCost);

                //Gain XP
                if (!isXpBeingGained)
                {
                    StartCoroutine(GainXPFromBehavior(striker, xpGain));
                    isXpBeingGained = true;
                }
            }
        }
        #endregion

        #region MOVE TO BALL
        private void MoveToBall()
        {
            if (!currentBall)
            {
                currentBall = FindNearestAvailableBall();
                if (!currentBall)
                {
                    return;
                }
            }

            //Check if the ball is possessed by another striker
            if (currentBall.GetPossessor() != null && currentBall.GetPossessor() != striker)
            {
                currentBall = FindNearestAvailableBall();
                if (!currentBall)
                {
                    return;
                }
            }

            if (currentBall.GetPossessor() == null && !hasMovedToBall)
            {
                Vector3 ballPosition = new Vector3(currentBall.transform.position.x, strikerTransform.position.y, currentBall.transform.position.z);

                strikerTransform.position = Vector3.MoveTowards(strikerTransform.position, ballPosition, striker.GetStrikerMovement().GetSpeed() * Time.deltaTime);

                strikerTransform.rotation = Quaternion.Slerp(strikerTransform.rotation, Quaternion.LookRotation(ballPosition - strikerTransform.position),
                    12f * Time.deltaTime);
            } 
        }
        #endregion

        public override IEnumerator Execute()
        {
            currentBall = FindNearestAvailableBall();
            if (currentBall)
            {
                Debug.Log(striker.gameObject.name + " found " + currentBall.gameObject.name);
            }

            do
            {
                if (striker.GetStrikerMode() != StrikerAI.StrikerMode.Training)
                {
                    SetIsComplete(true);
                    yield break;
                }

                isDribblingThisFrame = false;
                this.isComplete = false;

                MoveToBall();

                if (!isDribblingThisFrame)
                {
                    Dribble();
                    isDribblingThisFrame = true;
                }

                yield return new WaitForEndOfFrame();

            } while (timePassed < GetTimeToComplete());

            if (timePassed >= GetTimeToComplete())
            {
                SetIsComplete(true);
                yield break;
            }
        }

        #region GETTERS & SETTERS
        public override void SetTimeToComplete(float time)
        {
            timeToComplete = time;
        }

        public override void SetIsComplete(bool value)
        {
            timePassed = 0f;
            isXpBeingGained = false;
            SetIsDribbling(false);
            striker.SetHasBallPossession(false);
            striker.SetPossessedBall(null);
            striker.GetBallSocketTrigger().enabled = false;
            currentBall?.SetPossession(null);
            currentBall?.SetIsBeingUsed(false);
            currentBall = null;
            hasMovedToBall = false;
            this.isComplete = value;
            base.SetIsComplete(value);
            ResetTimeToComplete();
        }

        public void SetIsDribbling(bool value)
        {
            isDribbling = value;
        }

        public override float GetSpeed() { return dribblingBehaviorData.movementSpeed; }
        public override float GetStaminaCost() { return staminaCost; }
        public override bool IsComplete() { return isComplete; }
        public bool IsDribbling() { return isDribbling; }
        #endregion
    }
}


using Gameplay.AI.Behaviors;
using Gameplay.Environment.Ball;
using Gameplay.Managers;
using Gameplay.Striker;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gameplay.AI.StrikerAI;

namespace Gameplay.AI.Behaviors
{
    public class CombinedPassingBehavior : CombinedStrikerBehavior
    {
        [Header("Settings")]
        [SerializeField] private Transform strikerTransform;
        [SerializeField] private PassingBehaviorData passingBehaviorData;
        [SerializeField] private StrikerAI striker;
        [SerializeField] private StrikerPass strikerPass;

        private Transform currentWaypoint;

        private BallBehavior currentBall;
        private Vector3 targetPosition;

        private float staminaCost;
        private float timePassed = 0f;

        private bool isPassing;
        private bool isPassingThisFrame;
        private bool isPartnerReady;
        private bool hasMovedToBall;
        private bool hasMovedToWaypoint;
        private bool hasFoundBall;

        private void Awake()
        {
            SetName("Combined Passing");

            //initialize variables
            if (passingBehaviorData)
            {
                xpGain = passingBehaviorData.xpGain;
                initialTimeToComplete = passingBehaviorData.timeToComplete;
                SetTimeToComplete(passingBehaviorData.timeToComplete);
                staminaCost = passingBehaviorData.staminaCost;
            }
        }

        #region FIND NEAREST BALL
        private BallBehavior FindNearestAvailableBall()
        {
            BallBehavior nearestBall = null;

            float minDistance = Mathf.Infinity;

            //For each ball in the list
            foreach (BallBehavior ball in BallManager.Instance.GetObjectsInScene())
            {
                //Check if the ball is not possessed or if the possessor is this current striker
                if (!ball.IsPossessed() && !ball.IsBeingUsed() /*&& ball.GetPossessor() == striker*/ && !ball.GetMiniGame().enabled)
                {
                    //Check for the nearest ball
                    if (Vector3.Distance(strikerTransform.position, ball.transform.position) < minDistance)
                    {
                        nearestBall = ball;
                        minDistance = Vector3.Distance(transform.position, ball.transform.position);
                        break;
                    }
                }
            }

            //If the balls list is empty or the nearest ball is null
            if (BallManager.Instance.GetObjectsInScene().Count == 0 || nearestBall == null)
            {
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

        #region MOVE TO BALL
        private void MoveToBall()
        {
            if (!currentBall && !hasMovedToBall)
            {
                isComplete = true;
                return;
            }

            if (!currentBall.IsPossessed() && !hasMovedToBall)
            {
                //Enable ball socket trigger
                striker.GetBallSocketTrigger().enabled = true;

                Vector3 ballPosition = new Vector3(currentBall.transform.position.x, strikerTransform.position.y, currentBall.transform.position.z);

                strikerTransform.position = Vector3.MoveTowards(strikerTransform.position, ballPosition, striker.GetStrikerMovement().GetSpeed() * Time.deltaTime);

                Vector3 targetDirection = ballPosition - strikerTransform.position;

                if (targetDirection != Vector3.zero)
                {
                    strikerTransform.rotation = Quaternion.Slerp(strikerTransform.rotation, Quaternion.LookRotation(ballPosition - strikerTransform.position),
                   12f * Time.deltaTime);
                }
            }

            if (currentBall.IsPossessed() && currentBall.GetPossessor() == striker && !hasMovedToBall)
            {
                striker.GetBallSocketTrigger().enabled = false;
                hasMovedToBall = true;
            }
        }
        #endregion

        #region MOVE TO WAYPOINT
        private void MoveToDesignatedWaypoint()
        {
            if (hasMovedToBall && !hasMovedToWaypoint)
            {
                if (striker.GetPossessedBall() == null)
                {
                    SetIsComplete(true);
                    return;
                }

                currentWaypoint = WaypointManager.Instance.GetWaypointAtIndex(0, WaypointManager.Instance.GetPassingWaypoints());

                Vector3 targetPos = new Vector3(currentWaypoint.position.x, strikerTransform.position.y, currentWaypoint.position.z);

                strikerTransform.position = Vector3.MoveTowards(strikerTransform.position, targetPos, striker.GetStrikerMovement().GetSpeed() * Time.deltaTime);

                Vector3 targetDirection = targetPos - strikerTransform.position;

                if (targetDirection != Vector3.zero)
                {
                    strikerTransform.rotation = Quaternion.Slerp(strikerTransform.rotation, Quaternion.LookRotation(targetPos - strikerTransform.position),
                        12f * Time.deltaTime);
                }

                //If distance from striker to target position is nearly equal
                if (Vector3.Distance(strikerTransform.position, targetPos) < 0.1f)
                {
                    Vector3 facingPos = new Vector3(WaypointManager.Instance.GetWaypointAtIndex(1, WaypointManager.Instance.GetPassingWaypoints()).position.x,
                        strikerTransform.position.y, WaypointManager.Instance.GetWaypointAtIndex(1, WaypointManager.Instance.GetPassingWaypoints()).position.z);

                    Vector3 targetDir = facingPos - strikerTransform.position;

                    //Rotate to face other waypoint
                    if (targetDir != Vector3.zero)
                    {
                        strikerTransform.rotation = Quaternion.Slerp(strikerTransform.rotation, Quaternion.LookRotation(facingPos - strikerTransform.position),
                            12f * Time.deltaTime);

                        if (Quaternion.Angle(strikerTransform.rotation, Quaternion.LookRotation(facingPos - strikerTransform.position)) < 1f)
                        {
                            striker.GetStrikerMovement().SetSpeed(0f);
                            behaviorSpeed = 0f;

                            hasMovedToWaypoint = true;
                        }
                    }
                }
            }
        }
        #endregion

        #region FIND AVAILABLE PARTNER
        public override StrikerAI FindPartnerToJoin()
        {
            if (hasMovedToWaypoint && !HasPartner() && striker.GetPossessedBall() != null)
            {
                //Loop through all the strikers in the scene
                foreach (StrikerAI availableStriker in StrikerManager.Instance.GetObjectsInScene())
                {
                    //Check if the striker is not the current behavior owner and is not currently possessing the ball
                    if (availableStriker != behaviorOwner && !availableStriker.GetPossessedBall())
                    {
                        //Check if the striker is currently executing the passing behavior
                        CombinedStrikerBehavior strikerBehavior = availableStriker.GetCurrentBehavior() as CombinedStrikerBehavior;

                        if (strikerBehavior != null && strikerBehavior.GetType() == typeof(CombinedPassingBehavior))
                        {
                            //Check if the striker has not already joined this behavior owner as a partner
                            if (strikerBehavior.GetPartner() == null && availableStriker.GetStrikerMode() == StrikerMode.Training)
                            {
                                //Found an available partner
                                Debug.Log(behaviorOwner?.gameObject.name + " found passing partner " + availableStriker?.gameObject.name);
                                //strikerPartner = availableStriker;
                                SetPartner(availableStriker);
                                GetPartner().SetCurrentBehavior(null);
                                return availableStriker;
                            }
                        }

                        if (availableStriker.GetCurrentBehavior() == null || availableStriker.GetCurrentBehavior().IsComplete())
                        {
                            //Found an available partner
                            Debug.Log(behaviorOwner?.gameObject.name + " found passing partner " + availableStriker?.gameObject.name);
                            //strikerPartner = availableStriker;
                            SetPartner(availableStriker);
                            GetPartner().SetCurrentBehavior(null);
                            return availableStriker;
                        }
                    }
                }
            }
            // No available partners found
            return null;
        }
        #endregion

        #region MOVE INFRONT OF STRIKER
        private void MovePartnerToStriker()
        {
            if (!HasPartner()) return;

            if (!isPartnerReady)
            {
                GetPartner().GetStrikerMovement().SetSpeed(10f);
                Transform point = WaypointManager.Instance.GetWaypointAtIndex(1, WaypointManager.Instance.GetPassingWaypoints());
                targetPosition = new Vector3(point.position.x, strikerTransform.position.y, point.position.z);
                targetPosition.y = GetPartner().transform.position.y;

                GetPartner().transform.position = Vector3.MoveTowards(GetPartner().transform.position, targetPosition, GetPartner().GetStrikerMovement().GetSpeed() * Time.deltaTime);

                Vector3 targetDirection = targetPosition - strikerTransform.position;

                if (targetDirection != Vector3.zero)
                {
                    GetPartner().transform.rotation = Quaternion.Slerp(GetPartner().transform.rotation, Quaternion.LookRotation(targetPosition - GetPartner().transform.position),
                        12f * Time.deltaTime);
                }
            }

            if (Vector3.Distance(GetPartner().transform.position, targetPosition) <= 0.1f)
            {
                Vector3 partnerDirection = strikerTransform.position - GetPartner().transform.position;
                partnerDirection.y = 0f;

                Quaternion targetRotation = Quaternion.LookRotation(partnerDirection);
                GetPartner().transform.rotation = Quaternion.Slerp(GetPartner().transform.rotation, targetRotation, 15f * Time.deltaTime);
                GetPartner().GetStrikerMovement().SetSpeed(0f);
                isPartnerReady = true;
            }
        }
        #endregion

        #region PASSING
        private void Pass(StrikerAI sender, StrikerAI receiver)
        {
            if (!IsComplete() && !isPassingThisFrame)
            {
                strikerPass.Pass(sender, receiver, sender.GetPossessedBall());
            }
        }
        #endregion

        public override IEnumerator Execute()
        {
            do
            {
                if (striker.GetStrikerMode() != StrikerAI.StrikerMode.Training)
                {
                    SetIsComplete(true);
                    yield break;
                }

                isPassingThisFrame = false;
                this.isComplete = false;
                this.behaviorOwner = striker;

                // Find an available ball on the field
                if (!hasFoundBall)
                {
                    currentBall = FindNearestAvailableBall();

                    if (currentBall)
                    {
                        currentBall.SetIsBeingUsed(true);
                        hasFoundBall = true;
                    }
                }

                //Move to ball
                MoveToBall();

                //Move to waypoint
                MoveToDesignatedWaypoint();

                //Move partner infront of striker
                MovePartnerToStriker();

                if (hasMovedToWaypoint && isPartnerReady && !isPassingThisFrame)
                {
                    if (striker.GetPossessedBall() != null)
                    {
                        //Wait and pass the ball
                        yield return new WaitForSeconds(passingBehaviorData.passDelay);

                        Pass(striker, GetPartner());
                        isPassingThisFrame = true;
                    }

                    if (GetPartner() && GetPartner().GetPossessedBall() != null)
                    {
                        //Wait and pass the ball
                        yield return new WaitForSeconds(passingBehaviorData.passDelay);

                        Pass(GetPartner(), striker);
                        isPassingThisFrame = true;
                    }

                    striker.GetStrikerStamina().DrainStamina(staminaCost);
                    GetPartner().GetStrikerStamina().DrainStamina(staminaCost);

                    //Gain XP
                    if (!isXpBeingGained)
                    {
                        StartCoroutine(GainXPFromBehavior(striker, xpGain));
                        StartCoroutine(GainXPFromBehavior(GetPartner(), xpGain));
                        isXpBeingGained = true;
                    }

                    timePassed += Time.deltaTime;
                }

                yield return new WaitForEndOfFrame();

            } while (timePassed < GetTimeToComplete());

            if (timePassed >= GetTimeToComplete())
            {
                SetIsComplete(true);
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
            SetIsPassing(false);
            striker.SetHasBallPossession(false);
            striker.GetBallSocketTrigger().enabled = false;
            striker.SetPossessedBall(null);
            if (HasPartner())
            {
                GetPartner()?.SetHasBallPossession(false);
                GetPartner().GetBallSocketTrigger().enabled = false;
                GetPartner()?.SetPossessedBall(null);
                RemovePartner();
            }
            currentBall?.SetIsBeingUsed(false);
            currentBall?.SetPossession(null);
            currentBall = null;
            isPassingThisFrame = false;
            hasFoundBall = false;
            hasMovedToBall = false;
            hasMovedToWaypoint = false;
            isPartnerReady = false;
            this.isComplete = value;
            ResetTimeToComplete();
            ResetBehaviorSpeed();
        }

        public void SetIsPassing(bool value)
        {
            isPassing = value;
        }

        public override float GetSpeed() { return passingBehaviorData.movementSpeed; }
        public override float GetStaminaCost() { return staminaCost; }
        public override bool IsComplete() { return isComplete; }
        public bool IsPassing() { return isPassing; }
        #endregion
    }

}

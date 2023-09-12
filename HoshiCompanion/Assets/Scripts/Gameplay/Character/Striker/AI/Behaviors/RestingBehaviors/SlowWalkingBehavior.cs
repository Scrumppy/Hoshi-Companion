using Gameplay.Managers;
using Gameplay.Striker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Gameplay.AI.Behaviors
{
    public class SlowWalkingBehavior : StrikerBehavior
    {
        [Header("Settings")]
        [SerializeField] private Transform strikerTransform;
        [SerializeField] private SlowWalkingBehaviorData slowWalkingBehaviorData;
        [SerializeField] private StrikerMovement strikerMovement;
        [SerializeField] private StrikerStamina strikerStamina;
        [SerializeField] private StrikerAI striker;

        private Transform currentWaypoint;
        private int currentWaypointIndex = 0;

        private float staminaRestore;
        private float timePassed = 0f;

        private bool isSlowWalking;
        private bool isSlowWalkingThisFrame;

        private void Awake()
        {
            SetName("Slow Walking");

            //Initialize variables
            if (slowWalkingBehaviorData)
            {
                initialTimeToComplete = slowWalkingBehaviorData.timeToComplete;
                SetTimeToComplete(slowWalkingBehaviorData.timeToComplete);
                staminaRestore = slowWalkingBehaviorData.staminaRestoreAmount;
            }
        }

        protected override void OnBehaviorChanged(StrikerBehavior newBehavior)
        {
            base.OnBehaviorChanged(newBehavior);
        }


        #region SLOW WALKING
        private void SlowWalk()
        {
            //If behavior is not yet complete and is not sprinting this frame
            if (!IsComplete() && !isSlowWalkingThisFrame)
            {
                SetIsSlowWalking(true);

                timePassed += Time.deltaTime;

                currentWaypoint = WaypointManager.Instance.GetWaypointAtIndex(currentWaypointIndex, WaypointManager.Instance.GetRestingWaypoints());

                Vector3 targetPosition = new Vector3(currentWaypoint.position.x, strikerTransform.position.y, currentWaypoint.position.z);

                strikerTransform.position = Vector3.MoveTowards(strikerTransform.position, targetPosition, strikerMovement.GetSpeed() * Time.deltaTime);

                Vector3 targetDirection = targetPosition - strikerTransform.position;

                if (targetDirection != Vector3.zero)
                {
                    strikerTransform.rotation = Quaternion.Slerp(strikerTransform.rotation, Quaternion.LookRotation(targetPosition - strikerTransform.position),
                        12f * Time.deltaTime);
                }

                //If distance from striker to target position is nearly equal, move to next waypoint
                if (Vector3.Distance(strikerTransform.position, targetPosition) < 0.1f)
                {
                    currentWaypointIndex++;
                }

                //Restart waypoint index if striker reaches the last waypoint
                if (currentWaypointIndex == WaypointManager.Instance.GetRestingWaypoints().Count)
                {
                    currentWaypointIndex = 0;
                }

                striker.GetStrikerStamina().RestoreStamina(staminaRestore);

                //Gain XP
                if (!isXpBeingGained)
                {
                    StartCoroutine(GainXPFromBehavior(striker, slowWalkingBehaviorData.xpGain));
                    isXpBeingGained = true;
                }
            }
        }
        #endregion

        public override IEnumerator Execute()
        {
            //Break out of the function if the waypoint list is empty
            if (WaypointManager.Instance.GetRestingWaypoints()?.Count == 0)
            {
                Debug.LogWarning("No waypoints found for sprinting behavior to start!");
                SetIsComplete(true);
                yield break;
            }

            do 
            {
                if (striker.GetStrikerMode() != StrikerAI.StrikerMode.Resting)
                {
                    SetIsComplete(true);
                    yield break;
                }

                isSlowWalkingThisFrame = false;
                this.isComplete = false;

                if (!isSlowWalkingThisFrame)
                {
                    SlowWalk();
                    isSlowWalkingThisFrame = true;
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
        private void SetIsSlowWalking(bool value)
        {
            isSlowWalking = value;
        }

        public override void SetTimeToComplete(float time)
        {
            timeToComplete = time;
        }

        public override void SetIsComplete(bool value)
        {
            timePassed = 0f;
            isXpBeingGained = false;
            SetIsSlowWalking(false);
            this.isComplete = value;
            ResetTimeToComplete();
        }

        public override float GetSpeed() { return slowWalkingBehaviorData.movementSpeed; }
        public override float GetStaminaRestore() { return staminaRestore; }
        public override bool IsComplete() { return isComplete; }
        public bool IsSlowWalking() { return isSlowWalking; }
        #endregion
    }
}


using Gameplay.AI;
using Gameplay.Managers;
using Gameplay.Striker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Behaviors
{
    public class SprintingBehavior : StrikerBehavior
    {
        [Header("Settings")]
        [SerializeField] private Transform strikerTransform;
        [SerializeField] private SprintingBehaviorData sprintingBehaviorData;
        [SerializeField] private StrikerAI striker;

        private Transform currentWaypoint;
        private int currentWaypointIndex = 0;

        private float staminaCost;
        private float timePassed = 0f;

        private bool isSprinting;
        private bool isSprintingThisFrame;

        private void Awake()
        {
            SetName("Sprinting");

            //Initialize variables
            if (sprintingBehaviorData)
            {
                xpGain = sprintingBehaviorData.xpGain;
                initialTimeToComplete = sprintingBehaviorData.timeToComplete;
                SetTimeToComplete(sprintingBehaviorData.timeToComplete);
                staminaCost = sprintingBehaviorData.staminaCost;
                initialBehaviorSpeed = sprintingBehaviorData.movementSpeed;
                behaviorSpeed = sprintingBehaviorData.movementSpeed;
            }
        }

        protected override void OnBehaviorChanged(StrikerBehavior newBehavior)
        {
            base.OnBehaviorChanged(newBehavior);
        }

        #region SPRINTING
        private void Sprint()
        {
            //If behavior is not yet complete and is not sprinting this frame
            if (!IsComplete() && !isSprintingThisFrame)
            {
                SetIsSprinting(true);

                timePassed += Time.deltaTime;

                currentWaypoint = WaypointManager.Instance.GetWaypointAtIndex(currentWaypointIndex, WaypointManager.Instance.GetJoggingWaypoints());

                Vector3 targetPosition = new Vector3(currentWaypoint.position.x, strikerTransform.position.y, currentWaypoint.position.z);

                strikerTransform.position = Vector3.MoveTowards(strikerTransform.position, targetPosition, behaviorSpeed * Time.deltaTime);

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
                if (currentWaypointIndex == WaypointManager.Instance.GetJoggingWaypoints().Count)
                {
                    currentWaypointIndex = 0;
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

        public override IEnumerator Execute()
        {
            //Break out of the function if the waypoint list is empty
            if (WaypointManager.Instance.GetJoggingWaypoints()?.Count == 0)
            {
                Debug.LogWarning("No waypoints found for sprinting behavior to start!");
                SetIsComplete(true);
                yield break;
            }

            do 
            {
                if (striker.GetStrikerMode() != StrikerAI.StrikerMode.Training)
                {
                    SetIsComplete(true);
                    yield break;
                }

                isSprintingThisFrame = false;
                this.isComplete = false;

                if (!isSprintingThisFrame)
                {
                    Sprint();
                    isSprintingThisFrame = true;
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
        private void SetIsSprinting(bool value)
        {
            isSprinting = value;
        }

        public override void SetIsComplete(bool value)
        {
            timePassed = 0f;
            isXpBeingGained = false;
            SetIsSprinting(false);
            this.isComplete = value;
            base.SetIsComplete(value);
            ResetTimeToComplete();
            ResetBehaviorSpeed();
        }

        public override void SetTimeToComplete(float time)
        {
            timeToComplete = time;
        }
        public override float GetSpeed() { return sprintingBehaviorData.movementSpeed; }
        public override float GetStaminaCost() { return staminaCost; }
        public override bool IsComplete() { return isComplete; }
        public bool IsSprinting() { return isSprinting; }
        #endregion
    }
}





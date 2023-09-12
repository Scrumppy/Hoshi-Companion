using Gameplay.Managers;
using Gameplay.Striker;
using PlayerData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.InputManagerEntry;

namespace Gameplay.AI.Behaviors
{
    public class JoggingBehavior : StrikerBehavior
    {
        [Header("Settings")]
        [SerializeField] private Transform strikerTransform;
        [SerializeField] private JoggingBehaviorData joggingBehaviorData;
        [SerializeField] private StrikerAI striker;

        private Transform currentWaypoint;
        private int currentWaypointIndex = 0;

        private float staminaCost;
        private float timePassed = 0f;

        private bool isJogging;
        private bool isJoggingThisFrame;

        private void Awake()
        {
            SetName("Jogging");

            //Initialize variables
            if (joggingBehaviorData)
            {
                xpGain = joggingBehaviorData.xpGain;
                initialTimeToComplete = joggingBehaviorData.timeToComplete;
                SetTimeToComplete(joggingBehaviorData.timeToComplete);
                staminaCost = joggingBehaviorData.staminaCost;
                initialBehaviorSpeed = joggingBehaviorData.movementSpeed;
                behaviorSpeed = joggingBehaviorData.movementSpeed;
            }
        }

        protected override void OnBehaviorChanged(StrikerBehavior newBehavior)
        {
            base.OnBehaviorChanged(newBehavior);
        }

        #region JOGGING
        private void Jog()
        {
            //If this behavior is not complete and is not jogging in the current frame
            if (!IsComplete() && !isJoggingThisFrame)
            {
                SetIsJogging(true);

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

                //If striker position is near the target position, move to next waypoint    
                if (Vector3.Distance(strikerTransform.position, targetPosition) < 0.1f)
                {
                    currentWaypointIndex++;
                }

                //If the striker has reached the last waypoint, restart 
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
            //If the waypoint list is empty, break out of the function
            if (WaypointManager.Instance.GetJoggingWaypoints()?.Count == 0)
            {
                Debug.LogWarning("No waypoints found for jogging behavior to start!");
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

                isJoggingThisFrame = false;
                this.isComplete = false;

                if (!isJoggingThisFrame)
                {
                    Jog();
                    isJoggingThisFrame = true;
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
        private void SetIsJogging(bool value)
        {
            isJogging = value;
        }

        public override void SetIsComplete(bool value)
        {
            timePassed = 0f;
            isXpBeingGained = false;
            SetIsJogging(false);
            this.isComplete = value;
            base.SetIsComplete(value);
            behaviorSpeed = initialTimeToComplete;
            ResetTimeToComplete();
            ResetBehaviorSpeed();
        }

        public override void SetTimeToComplete(float time)
        {
            timeToComplete = time;
        }

        //public override float GetSpeed() { return joggingBehaviorData.movementSpeed; }
        public override float GetStaminaCost() { return staminaCost; }
        public override bool IsComplete() { return isComplete; }
        public bool IsJogging() { return isJogging; }
        #endregion
    }
}


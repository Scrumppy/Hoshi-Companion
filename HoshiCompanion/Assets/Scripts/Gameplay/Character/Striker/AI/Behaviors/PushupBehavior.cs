using Gameplay.Managers;
using Gameplay.Striker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Behaviors
{
    public class PushupBehavior : StrikerBehavior
    {
        [Header("Settings")]
        [SerializeField] private PushupBehaviorData pushupBehaviorData;
        [SerializeField] private StrikerAI striker;

        public delegate void PushupBehaviorChanged(bool isDoingPushups);
        public event PushupBehaviorChanged OnPushupBehaviorChanged;

        private float staminaCost;
        private float timePassed = 0f;

        private Transform currentWaypoint;

        private bool isDoingPushups;
        private bool isDoingPushupsThisFrame;
        private bool hasMovedToWaypoint;
        private bool isWaypointOccupied = false;

        private void Awake()
        {
            SetName("Pushups");

            //Initialize variables
            if (pushupBehaviorData)
            {
                xpGain = pushupBehaviorData.xpGain;
                initialTimeToComplete = pushupBehaviorData.timeToComplete;
                SetTimeToComplete(pushupBehaviorData.timeToComplete);
                staminaCost = pushupBehaviorData.staminaCost;
            }

            striker.OnBehaviorChanged += OnBehaviorChanged;
        }

        protected override void OnBehaviorChanged(StrikerBehavior newBehavior)
        {
            base.OnBehaviorChanged(newBehavior);
        }

        #region GET WAYPOINT
        private Transform GetRandomPushupWaypoint()
        {
            //Get list of waypoints
            List<Transform> pushupWaypoints = WaypointManager.Instance.GetPushupWaypoints();
            List<Transform> availableWaypoints = new List<Transform>();

            //For each waypoint
            foreach (Transform waypoint in pushupWaypoints)
            {
                isWaypointOccupied = false;

                //Check each striker in the scene
                foreach (StrikerAI availableStriker in StrikerManager.Instance.GetObjectsInScene())
                {
                    //Check if they are occupying the waypoint
                    if (Vector3.Distance(availableStriker.transform.position, waypoint.position) < 0.1f && availableStriker != striker)
                    {
                        Debug.Log(availableStriker.gameObject.name + " is occupying " + waypoint.gameObject.name);
                        isWaypointOccupied = true;
                        break;
                    }
                }

                //Add the waypoint if it is not occupied
                if (!isWaypointOccupied)
                {
                    availableWaypoints.Add(waypoint);
                }
            }

            if (availableWaypoints.Count > 0)
            {
                int randomIndex = Random.Range(0, availableWaypoints.Count);
                return availableWaypoints[randomIndex];
            }

            return null;
        }
        #endregion

        #region MOVE TO WAYPOINT
        private void MoveToWaypoint()
        {
            if (!hasMovedToWaypoint)
            {
                if (currentWaypoint)
                {
                    striker.GetStrikerMovement().SetSpeed(7.5f);

                    Vector3 targetPos = new Vector3(currentWaypoint.position.x, striker.transform.position.y, currentWaypoint.position.z);

                    striker.transform.position = Vector3.MoveTowards(striker.transform.position, targetPos, striker.GetStrikerMovement().GetSpeed() * Time.deltaTime);

                    Vector3 targetDirection = targetPos - striker.transform.position;

                    if (targetDirection != Vector3.zero)
                    {
                        striker.transform.rotation = Quaternion.Slerp(striker.transform.rotation, Quaternion.LookRotation(targetPos - striker.transform.position),
                            12f * Time.deltaTime);
                    }

                    if (Vector3.Distance(striker.transform.position, targetPos) < 0.1f)
                    {
                        striker.GetStrikerMovement().SetSpeed(0f);
                        hasMovedToWaypoint = true;
                    }
                }
            }
        }
        #endregion

        #region PUSHUPS
        private void Pushups()
        {
            //If behavior is not yet complete and is not doing pushups this frame
            if (!IsComplete() && !isDoingPushupsThisFrame && hasMovedToWaypoint)
            {
                SetIsDoingPushups(true);

                timePassed += Time.deltaTime;

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
            do
            {
                if (striker.GetStrikerMode() != StrikerAI.StrikerMode.Training)
                {
                    SetIsComplete(true);
                    yield break;
                }

                isDoingPushupsThisFrame = false;
                this.isComplete = false;

                if (!currentWaypoint)
                {
                    currentWaypoint = GetRandomPushupWaypoint();
                }

                MoveToWaypoint();

                if (!isDoingPushupsThisFrame)
                {
                    Pushups();
                    isDoingPushupsThisFrame = true;
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
        private void SetIsDoingPushups(bool value)
        {
            isDoingPushups = value;
            OnPushupBehaviorChanged?.Invoke(value);
        }

        public override void SetTimeToComplete(float time)
        {
            timeToComplete = time;
        }

        public override void SetIsComplete(bool value)
        {
            timePassed = 0f;
            isXpBeingGained = false;
            SetIsDoingPushups(false);
            hasMovedToWaypoint = false;
            this.isComplete = value;
            base.SetIsComplete(value);
            ResetTimeToComplete();
        }

        public override float GetStaminaCost() { return staminaCost; }
        public override bool IsComplete() { return isComplete; }
        public bool IsDoingPushups() { return isDoingPushups; }
        #endregion
    }
}

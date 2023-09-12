using Gameplay.AI.Behaviors;
using Gameplay.Environment.Ball;
using Gameplay.Managers;
using Gameplay.Rewards;
using Gameplay.Striker;
using PlayerData.Strikers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Security;
using UnityEngine;
using static Gameplay.AI.StrikerAI;
using static StrikersParser;
using Random = UnityEngine.Random;

namespace Gameplay.AI
{
    /// <summary>
    /// AI controller for the strikers.
    /// </summary>
    public class StrikerAI : MonoBehaviour
    {
        #region STRIKER MODES
        public enum StrikerMode
        {
            Idle,
            Training,
            Resting
        }

        [Header("Striker Mode")]
        [SerializeField] private StrikerMode currentMode;
        #endregion

        [Header("Components")]
        [SerializeField] private Transform ballSocket;
        [SerializeField] private SphereCollider ballSocketTrigger;
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private SphereCollider crossReceiverCollider;

        [Header("Striker Stamina")]
        [SerializeField] private StrikerStamina strikerStamina;

        [Header("Striker Movement")]
        [SerializeField] private StrikerMovement strikerMovement;

        #region BEHAVIOR VARIABLES
        [Header("Training Behaviors")]
        [SerializeField] private List<StrikerBehavior> trainingBehaviors;
        private StrikerBehavior currentBehavior;
        private StrikerBehavior previousBehavior;

        [Header("Resting Behaviors")]
        [SerializeField] private List<StrikerBehavior> restingBehaviors;

        public event Action<StrikerBehavior> OnBehaviorChanged;
        #endregion

        [SerializeField] private CharacterInfo strikerCharacterInfo;

        private bool hasBallPossession = false;
        private bool isPartner = false;
        private bool foundValidBehavior = false;
        private bool hasTrainingBonus = false;

        private BallBehavior ball;

        private void Awake()
        {
            ballSocketTrigger.enabled = false;
            crossReceiverCollider.enabled = false;
        }

        private void OnDisable()
        {
            if (StrikerManager.Instance)
            {
                StrikerManager.Instance.RemoveObject(this);
            }
        }

        private void OnDestroy()
        {
            if (StrikerManager.Instance)
            {
                StrikerManager.Instance.RemoveObject(this);
            }
        }

        private void OnEnable()
        {
            if (StrikerManager.Instance)
            {
                StrikerManager.Instance.AddObject(this);
            }
        }

        private void Start()
        {
            if (StrikerManager.Instance)
            {
                StrikerManager.Instance.AddObject(this);
            }

            SetStrikerMode(StrikerMode.Training);

            //Select a training behavior after 0.5 seconds
            Invoke(nameof(SelectTrainingBehavior), 0.5f);
        }

        private void LateUpdate()
        {
            //Return if the striker is a partner
            if (isPartner) return;

            //Find a partner if this striker is executing a combined behavior
            FindPartnerForCombinedBehavior();

            //Select new behavior if current behavior is null, used when this striker stops being a partner in a combined behavior
            if (!currentBehavior)
            {
                SelectBehavior();
            }

            //Switch to new behavior if the current one is complete
            SwitchToNewBehaviorIfComplete();

            //Switch back to training if striker stamina has recovered fully
            SwitchToTraining();
        }

        /// <summary>
        /// This function finds an available partner if the striker is executing a combined behavior.
        /// </summary>
        private void FindPartnerForCombinedBehavior()
        {
            //If the current behavior of the striker is of the type CombinedStrikerBehavior
            if (currentBehavior is CombinedStrikerBehavior combinedStrikerBehavior)
            {
                //Call the combined behavior's function to find a partner to join the behavior
                combinedStrikerBehavior.FindPartnerToJoin();
            }
        }

        #region BEHAVIOR RELATED FUNCTIONS
        /// <summary>
        /// This function switches to a new behavior once the current one is complete.
        /// </summary>
        private void SwitchToNewBehaviorIfComplete()
        {
            //Return early if the current behavior is null
            if (currentBehavior == null) return;

            //If the current behavior is complete
            if (currentBehavior.IsComplete())
            {
                //If the current striker mode is training, select training behavior
                if (GetStrikerMode() == StrikerMode.Training)
                {
                    previousBehavior?.SetIsComplete(true);
                    previousBehavior?.StopAllCoroutines();
                    currentBehavior = SelectTrainingBehavior();
                }
                //If current striker mode is resting, select resting behavior
                else if (GetStrikerMode() == StrikerMode.Resting)
                {
                    previousBehavior?.SetIsComplete(true);
                    previousBehavior?.StopAllCoroutines();
                    currentBehavior = SelectRestingBehavior();
                }
            }
        }

        /// <summary>
        /// This function switches the striker to training, once his stamina is restored while resting
        /// </summary>
        private void SwitchToTraining()
        {
            //if the current mode is resting and the stamina has fully regenerated, return to training
            if (GetStrikerMode() == StrikerMode.Resting && strikerStamina.GetStamina() >= strikerStamina.GetMaxStamina())
            {
                Debug.Log(gameObject.name + " has recovered stamina and returned to training");
                SetStrikerMode(StrikerMode.Training);
                previousBehavior?.SetIsComplete(true);
                previousBehavior?.StopAllCoroutines();
                currentBehavior = SelectTrainingBehavior();
            }
        }

        /// <summary>
        /// This function checks the availability of a given class type of combined behavior in the list of available behaviors.
        /// </summary>
        /// <param name="T">The class to check, must of a child of CombinedStrikerBehavior.</param>
        /// <param name="availableBehaviors">The reference to the list of available behaviors.</param>
        private void CheckBehaviorAvailability<T>(ref List<StrikerBehavior> availableBehaviors) where T: CombinedStrikerBehavior
        {
            //Check if the given behavior is the same as the current one
            if (currentBehavior is T)
            {
                foundValidBehavior = true;

                //Check each active striker in the scene
                foreach (StrikerAI otherStriker in StrikerManager.Instance.GetObjectsInScene())
                {
                    //If the other striker is not this, and his behavior is the same as the given one
                    if (otherStriker != this && otherStriker.GetCurrentBehavior() is T)
                    {
                        //Remove this behavior from the available behavior list
                        availableBehaviors.Remove(currentBehavior);
                        Debug.LogWarning(typeof(T).Name +  " is already in use! Caller: " + gameObject.name);
                        foundValidBehavior = false;
                        break;
                    }
                }
            }
            else
            {
                foundValidBehavior = true;
            }
        }

        /// <summary>
        /// This function selects a training behavior from the list of available behaviors.
        /// </summary>
        private StrikerBehavior SelectTrainingBehavior()
        {
            //If the current behavior is not null or is incomplete, return early
            if (currentBehavior != null && !currentBehavior.IsComplete())
            {
                return currentBehavior;
            }

            //Get a list of available behaviors
            List<StrikerBehavior> availableBehaviors = new List<StrikerBehavior>(trainingBehaviors);

            //Remove the previous behavior from the available behaviors
            if (previousBehavior != null)
            {
                Debug.Log(gameObject.name + " Previous Behavior: " + previousBehavior.GetName());
                availableBehaviors.Remove(previousBehavior);

                //If previous behavior was passing or dribbling, remove the corresponding behavior
                if (previousBehavior is CombinedPassingBehavior)
                {
                    availableBehaviors.Remove(GetComponent<DribblingBehavior>());
                }
                else if (previousBehavior is DribblingBehavior)
                {
                    availableBehaviors.Remove(GetComponent<CombinedPassingBehavior>());
                }
            }

            //If there are no available behaviors, return null
            if (availableBehaviors.Count == 0)
            {
                Debug.LogWarning("No Available Behaviors!");
                return null;
            }

            //If there is only one available behavior, return it
            if (availableBehaviors.Count == 1)
            {
                currentBehavior = availableBehaviors[0];
                previousBehavior = currentBehavior;

                //Check if the current behavior is a type of Combined behavior, if so, dont execute the behavior and find a new one
                CheckBehaviorAvailability<CombinedStrikerBehavior>(ref availableBehaviors);

                //Found valid behavior
                if (foundValidBehavior)
                {
                    Debug.LogWarning(gameObject.name + " Only Available Behavior: " + currentBehavior.GetName());

                    //Set the striker speed
                    strikerMovement?.SetSpeed(currentBehavior.GetSpeed());

                    //Invoke the BehaviorChanged event to start the behavior execute coroutine
                    OnBehaviorChanged?.Invoke(currentBehavior);

                    //Check striker exhaustion
                    CheckExhaustion(currentBehavior);

                    //If the striker has an active bonus, apply the respective speed and time bonuses to the behavior
                    if (HasTrainingBonus())
                    {
                        currentBehavior.SetSpeed(currentBehavior.GetSpeed() * RewardsManager.Instance.goalRewardsData.trainingSpeedBonus);
                        strikerMovement?.SetSpeed(currentBehavior.GetSpeed());
                        currentBehavior?.SetTimeToComplete(currentBehavior.GetTimeToComplete() * RewardsManager.Instance.goalRewardsData.trainingTimeBonus);
                    }

                    SetStrikerMode(StrikerMode.Training);

                    return currentBehavior;
                }
                else
                {
                    Debug.Log(gameObject.name + " is finding a new behavior");
                    return null;
                }
            }

            //Choose a random behavior from the available behaviors
            int randomIndex = Random.Range(0, availableBehaviors.Count);
            currentBehavior = availableBehaviors[randomIndex];
            previousBehavior = currentBehavior;

            //Check if the current behavior is a type of Combined behavior, if so, dont execute the behavior and find a new one
            CheckBehaviorAvailability<CombinedStrikerBehavior>(ref availableBehaviors);

            if (foundValidBehavior)
            {
                strikerMovement?.SetSpeed(currentBehavior.GetSpeed());

                CheckExhaustion(currentBehavior);

                Debug.Log(gameObject.name + " Selected Behavior: " + currentBehavior.GetName());

                SetStrikerMode(StrikerMode.Training);

                OnBehaviorChanged?.Invoke(currentBehavior);

                if (HasTrainingBonus())
                {
                    currentBehavior.SetSpeed(currentBehavior.GetSpeed() * RewardsManager.Instance.goalRewardsData.trainingSpeedBonus);
                    strikerMovement?.SetSpeed(currentBehavior.GetSpeed());
                    currentBehavior?.SetTimeToComplete(currentBehavior.GetTimeToComplete() * RewardsManager.Instance.goalRewardsData.trainingTimeBonus);
                }

                return currentBehavior;
            }
            else
            {
                Debug.Log(gameObject.name + " is finding a new behavior");
                return null;
            }
        }

        /// <summary>
        /// This function checks if the striker is exhausted.
        /// </summary>
        /// <param name="behavior">The current striker's behavior.</param>
        private void CheckExhaustion(StrikerBehavior behavior)
        {
            //If striker stamina is less or equal to the chosen behavior stamina cost
            if (strikerStamina?.GetStamina() <= behavior.GetStaminaCost())
            {
                Debug.Log(gameObject.name + " Exhausted! Actions will be performed slowly");

                //Set the striker to exhausted and drain the stamina
                strikerStamina.DrainStamina(currentBehavior.GetStaminaCost());

                //Apply exhausted modifiers to time and speed
                behavior.SetTimeToComplete(behavior.GetTimeToComplete() * strikerStamina.GetExhaustedTimeModifier());
                behavior?.SetXPGain(behavior.GetXPGain() * 0.3f);
                //strikerMovement?.SetSpeed(strikerMovement.GetSpeed() * strikerStamina.GetExhaustedSpeedModifier());
                strikerMovement?.SetSpeed(behavior.GetSpeed() * strikerStamina.GetExhaustedSpeedModifier());
            }
        }

        /// <summary>
        /// This function selects a resting behavior from the list of available behaviors.
        /// </summary>
        private StrikerBehavior SelectRestingBehavior()
        {
            //If the current behavior is not null and is incomplete, return early
            if (currentBehavior != null && !currentBehavior.IsComplete())
            {
                return currentBehavior;
            }

            //Get a list of available behaviors
            List<StrikerBehavior> availableBehaviors = new List<StrikerBehavior>(restingBehaviors);

            //Remove the previous behavior from the available behaviors
            if (previousBehavior != null)
            {
                Debug.Log("Previous Behavior: " + previousBehavior.GetName());
                availableBehaviors.Remove(previousBehavior);
            }

            //If there are no available behaviors, return null
            if (availableBehaviors.Count == 0)
            {
                Debug.LogWarning("No Available Behaviors!");
                return null;
            }

            //If there is only one available behavior, return it
            if (availableBehaviors.Count == 1)
            {
                currentBehavior = availableBehaviors[0];
                previousBehavior = currentBehavior;

                Debug.Log("Only Available Behavior: " + currentBehavior.GetName());

                strikerMovement?.SetSpeed(currentBehavior.GetSpeed());

                OnBehaviorChanged?.Invoke(currentBehavior);

                SetStrikerMode(StrikerMode.Resting);

                return currentBehavior;
            }

            //Choose a random behavior from the available behaviors
            int randomIndex = Random.Range(0, availableBehaviors.Count);
            currentBehavior = availableBehaviors[randomIndex];
            previousBehavior = currentBehavior;

            //Set the striker speed
            strikerMovement?.SetSpeed(currentBehavior.GetSpeed());

            Debug.Log("Selected Behavior: " + currentBehavior.GetName());

            //Invoke the behavior change event to start the coroutine
            OnBehaviorChanged?.Invoke(currentBehavior);

            //Set the striker mode to resting
            SetStrikerMode(StrikerMode.Resting);

            return currentBehavior;
        }

        /// <summary>
        /// This function selects a behavior based on the current striker mode.
        /// </summary>
        public void SelectBehavior()
        {
            //if the current mode is training, select a training behavior
            if (GetStrikerMode() == StrikerMode.Training)
            {
                previousBehavior?.SetIsComplete(true);
                previousBehavior?.StopAllCoroutines();
                currentBehavior = SelectTrainingBehavior();
            }
            //If the current mode is resting, select a resting behavior
            else if (GetStrikerMode() == StrikerMode.Resting)
            {
                previousBehavior?.SetIsComplete(true);
                previousBehavior?.StopAllCoroutines();
                currentBehavior = SelectRestingBehavior();
            }
        }
        #endregion

        #region GETTERS & SETTERS
        public void SetHasBallPossession(bool value)
        {
            hasBallPossession = value;
        }

        public void SetStrikerMode(StrikerMode mode)
        {
            currentMode = mode;
        }

        public void SetIsPartner(bool value)
        {
            isPartner = value;
        }

        public void SetCurrentBehavior(StrikerBehavior newBehavior)
        {
            if (newBehavior == null)
            {
                currentBehavior = null;
            }
            else
            {
                currentBehavior = newBehavior;
                strikerMovement.SetSpeed(currentBehavior.GetSpeed());
                Debug.Log(gameObject.name + " Selected Behavior: " + currentBehavior.GetName());
            }
        }

        public void SetPossessedBall(BallBehavior possessedBall)
        {
            ball = possessedBall;
        }

        /// <summary>
        /// This function assigns the striker's info data to the respective given ID.
        /// </summary>
        /// <param name="strikerID">The ID to assign the respective striker info data.</param>
        public void AssignStrikerInfoData(int strikerID)
        {
            StrikerInfoData strikerInfo = StrikerDataManager.Instance.GetDataFromID(strikerID);

            if (GetStrikerCharacterInfo() != null && strikerInfo != null)
            {
                GetStrikerCharacterInfo().SetData(strikerInfo);
            }
        }

       public void SetHasTrainingBonus(bool value)
        {
            hasTrainingBonus = value;
        }

        public StrikerMode GetStrikerMode() { return currentMode; }
        public bool IsCurrentBehaviorComplete() { return currentBehavior.IsComplete(); }
        public StrikerBehavior GetCurrentBehavior() { return currentBehavior; }
        public bool HasBallPossession() { return hasBallPossession; }
        public Rigidbody GetRigidbody() { return rigidBody; }
        public Transform GetBallSocket() { return ballSocket; }
        public Vector3 GetMovementDirection() { return transform.forward; }
        public StrikerMovement GetStrikerMovement() { return strikerMovement; }
        public StrikerStamina GetStrikerStamina() { return strikerStamina; }
        public SphereCollider GetBallSocketTrigger() { return ballSocketTrigger; }
        public BallBehavior GetPossessedBall() { return ball; }
        public bool IsMoving() { return strikerMovement?.GetSpeed() > 0f; }
        public CharacterInfo GetStrikerCharacterInfo() { return strikerCharacterInfo; }
        public SphereCollider GetCrossReceiver() { return crossReceiverCollider; }
        public bool HasTrainingBonus() { return hasTrainingBonus; }
        #endregion
    }
}
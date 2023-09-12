using Gameplay.Managers;
using PlayerData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Behaviors
{
    public abstract class StrikerBehavior : MonoBehaviour
    {
        protected string behaviorName;
        protected bool isComplete;
        protected float timeToComplete;
        protected float xpGain;
        protected float initialTimeToComplete;
        protected bool isXpBeingGained;
        protected float behaviorSpeed;
        protected float initialBehaviorSpeed;

        private bool canGainXp = true;
        private float gainXpInterval = 1.5f;

        /// <summary>
        /// This function is called when the event is invoked to start the execution of the behavior coroutine.
        /// </summary>
        /// <param name="newBehavior">The behavior to start executing.</param>
        protected virtual void OnBehaviorChanged(StrikerBehavior newBehavior)
        {
            StartCoroutine(newBehavior.Execute());
        }

        /// <summary>
        /// This is the core function of the behavior scripts. It is called to perform the behavior.
        /// </summary>
        /// <returns>An enumerator that can be used to pause the execution of the behavior.</returns>
        public virtual IEnumerator Execute()
        {
            yield return null;
        }

        /// <summary>
        /// This function adds XP to a specified striker, by calling the PlayerInfoManager.
        /// </summary>
        /// <param name="striker_">The striker to give xp.</param>
        /// <param name="xpAmount">The amount of xp to give.</param>
        protected IEnumerator GainXPFromBehavior(StrikerAI striker_, float xpAmount)
        {
            while (canGainXp)
            {
                PlayerInfoManager.Instance.GiveXpToSpecificStriker(StrikerDataManager.Instance.GetDataFromID(
                    striker_.GetStrikerCharacterInfo().Data.ID.id), xpAmount);
                //PlayerInfoManager.Instance.GiveXpToSpecificStriker(striker_.GetStrikerCharacterInfo().Data, xpAmount);

                yield return new WaitForSeconds(gainXpInterval);
            }
        }

        public virtual void SetIsComplete(bool value) {}

        public virtual bool IsComplete() { return false; }
        public virtual float GetStaminaCost() { return 0f; }
        public virtual float GetStaminaRestore() { return 0f;  }
        public virtual float GetSpeed() { return behaviorSpeed; }
        public virtual string GetName() { return behaviorName; }

        protected void SetName(string newTag) 
        {
            behaviorName = newTag;
        }

        public virtual void SetSpeed(float speed)
        {
            behaviorSpeed = speed;
        }

        public virtual void SetTimeToComplete(float time)
        {
            timeToComplete = time;
        }

        public void ResetTimeToComplete()
        {
            timeToComplete = initialTimeToComplete;
        }

        public void ResetBehaviorSpeed()
        {
            behaviorSpeed = initialBehaviorSpeed;
        }

        public void SetXPGain(float xpModifier)
        {
            xpGain = xpModifier;
        }

        public float GetTimeToComplete() { return timeToComplete; }
        public float GetInitialTimeToComplete() { return initialTimeToComplete; }
        public float GetXPGain() { return xpGain; }
    }
}

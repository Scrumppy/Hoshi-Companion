using Gameplay.AI;
using Gameplay.Rewards;
using PlayerData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StrikersParser;

namespace Gameplay.Managers
{
    public class RewardsManager : Manager<RewardsManager>
    {
        [Header("Rewards")]
        public GoalRewardsData goalRewardsData;

        private StrikerAI striker;

        private float bonusTimer = 0f;

        private void Update()
        {
            if (!striker) return;

            //If the striker has a bonus, start decrementing the bonus timer
            if (striker.HasTrainingBonus())
            {
                bonusTimer -= Time.deltaTime;

                //Reset the bonuses when the timer reaches 0
                if (bonusTimer <= 0f)
                {
                    Debug.Log("Training Bonus ended!");
                    ResetBonuses();
                }
            }
        }

        private void ApplyTrainingBonus()
        {
            //If the striker is moving, apply the movement speed bonuses
            if (striker.IsMoving())
            {
                striker.GetCurrentBehavior()?.SetSpeed(striker.GetCurrentBehavior().GetSpeed() * goalRewardsData.trainingSpeedBonus);
                striker.GetStrikerMovement().SetSpeed(striker.GetCurrentBehavior().GetSpeed());
            }

            //Apply the time to complete bonus to the current behavior
            striker.GetCurrentBehavior()?.SetTimeToComplete(striker.GetCurrentBehavior().GetTimeToComplete() * goalRewardsData.trainingTimeBonus);

            //Set the timer
            bonusTimer = goalRewardsData.trainingBonusDuration;
            striker.SetHasTrainingBonus(true);
        }

        private void ResetBonuses()
        {
            //Reset the applied bonuses
            striker.GetCurrentBehavior()?.ResetBehaviorSpeed();
            striker.GetStrikerMovement().SetSpeed(striker.GetCurrentBehavior().GetSpeed());
            striker.GetCurrentBehavior()?.ResetTimeToComplete();

            //Reset the timer and variables
            bonusTimer = 0f;
            striker.SetHasTrainingBonus(false);
            striker = null;
        }

        public void GiveRandomRewardToStriker(StrikerAI _striker)
        {
            if (!_striker) return;

            striker = _striker;

            //Choose a random number
            int randomIndex = Random.Range(0, 2);

            if (randomIndex == 0)
            {
                //If the striker does not have a bonus, apply the training bonus
                if (!striker.HasTrainingBonus())
                {
                    ApplyTrainingBonus();
                    Debug.Log("Rewarded training bonus to " + _striker.gameObject.name);
                }
                //Striker already has a bonus
                else
                {
                    Debug.Log(_striker.gameObject.name + " already has a training bonus!");
                }
            }

            //Add BL bonus to the player
            else if (randomIndex == 1)
            {
                PlayerInfoManager.Instance.AddBl(goalRewardsData.bLBonus);
                Debug.Log("Rewarded " +  goalRewardsData.bLBonus + " BL to player");
            }
        }
    }
}

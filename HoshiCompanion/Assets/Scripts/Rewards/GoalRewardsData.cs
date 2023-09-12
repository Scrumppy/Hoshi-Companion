using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gameplay.Rewards
{
    [CreateAssetMenu(menuName = "Gameplay/Rewards")]
    public class GoalRewardsData : ScriptableObject
    {
        public float trainingSpeedBonus;
        public float trainingTimeBonus;
        public float trainingBonusDuration;
        public int bLBonus;
    }
}

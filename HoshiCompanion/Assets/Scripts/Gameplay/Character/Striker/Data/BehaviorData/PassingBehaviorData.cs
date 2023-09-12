using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Behaviors
{
    [CreateAssetMenu(menuName = "Striker/PassingData")]
    public class PassingBehaviorData : ScriptableObject
    {
        [Header("Settings")]
        public float timeToComplete;
        public float movementSpeed;
        public float staminaCost;
        public float xpGain;
        public float passDelay;
    }

}

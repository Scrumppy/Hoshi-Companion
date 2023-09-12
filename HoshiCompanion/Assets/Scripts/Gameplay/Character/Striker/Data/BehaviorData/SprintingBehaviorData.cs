using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Behaviors
{
    [CreateAssetMenu(menuName = "Striker/SprintingData")]
    public class SprintingBehaviorData : ScriptableObject
    {
        [Header("Settings")]
        public float timeToComplete;
        public float staminaCost;
        public float movementSpeed;
        public float xpGain;
    }
}

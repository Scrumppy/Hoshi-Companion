using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Behaviors
{
    [CreateAssetMenu(menuName = "Striker/SlowWalkingData")]
    public class SlowWalkingBehaviorData : ScriptableObject
    {
        [Header("Settings")]
        public float timeToComplete;
        public float movementSpeed;
        public float staminaRestoreAmount;
        public float xpGain;
    }
}
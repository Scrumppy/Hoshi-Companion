using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Behaviors
{
    [CreateAssetMenu(menuName = "Striker/DribblingBehaviorData")]
    public class DribblingBehaviorData : ScriptableObject
    {
        [Header("Settings")]
        public float timeToComplete;
        public float staminaCost;
        public float movementSpeed;
        public float xpGain;
    }

}

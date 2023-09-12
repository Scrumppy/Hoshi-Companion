using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Behaviors
{
    [CreateAssetMenu(menuName = "Striker/PushupData")]
    public class PushupBehaviorData : ScriptableObject
    {
        [Header("Settings")]
        public float timeToComplete;
        public float staminaCost;
        public float xpGain;
    }
}

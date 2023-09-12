using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Striker
{
    [CreateAssetMenu(menuName = "Striker/StrikerStamina")]
    public class StrikerStaminaData : ScriptableObject
    {
        [Header("Settings")]
        public float stamina;
        public float exhaustedTimeModifier;
        public float exhaustedSpeedModifier;
    }
}

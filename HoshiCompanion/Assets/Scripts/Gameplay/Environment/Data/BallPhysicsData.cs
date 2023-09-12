using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Environment.Ball
{
    [CreateAssetMenu(menuName = "Environment/BallPhysicsData")]
    public class BallPhysicsData : ScriptableObject
    {
        [Header("Gravity")]
        public float gravityModifier = 0f;

        [Header("Bounciness")]
        [Range(0, 1)] public float bounceness = 1;
        public LayerMask bouncenessLayer;

        [Header("Friction")]
        public LayerMask floorLayer;
        public float floorToleranceDistance = 1;
        public float frictionForce = 0f;
        public float timeToReduceFrictionAfterShot = 0.7f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Environment.Ball
{
    [CreateAssetMenu(menuName = "Environment/BallMiniGame/BallMiniGameData")]
    public class BallMiniGameData : ScriptableObject
    {
        [Header("Glow")]
        public Color glowColor;

        [Header("Launch")]
        public float curveDuration;
        public float curveHeight;
        public float offsetValue;
    }
}
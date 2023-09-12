using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Environment.Ball
{
    [CreateAssetMenu(menuName = "Environment/BallData")]
    public class BallData : ScriptableObject
    {
        public float untouchDelay = 0.1f;
        public float snapDelay = 0.2f;
        public float magnusAcceleration = 5f;
        public float triggerSize = 2f;
        public float rotationSpeedWhenInPossesion = 50;
        public float distanceToPassAutoSwitch = 20f;
        public float maxVelocityToPassAutoSwitch = 5f;
        public bool kinematicOnGoal = true;
        public bool dissolveOnGoal = true;
        //public ShootMath.BallTrajectoryType ballTrajectory;
        //public ShootMath.ShootCurveType shootCurve;
        //public BallGroundMark groundMarkModel;

        public float dribleWaitTimeToStartMovingBall = 0.35f;
        public float ballRollingBackDribleSpeed = 30f;
        public float ballRollingForwardDribleSpeed = 2f;
        public float ballDribleDistance = 0.1f;

        public float TriggerSize { get => Mathf.Max(Mathf.Abs(triggerSize), 1); }
    }
}

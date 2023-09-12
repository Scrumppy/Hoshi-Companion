using Gameplay.AI;
using Gameplay.Environment.Ball;
using Gameplay.Environment.Goal;
using Gameplay.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gameplay.Striker
{
    public class StrikerCrossReceive : MonoBehaviour
    {
        [Header("Striker")]
        [SerializeField] private StrikerAI striker;

        [Header("Shooting")]
        [SerializeField] private float shootForce = 5f;
        private float tempShootForce;

        private GoalGate goalGate = null;

        private BallBehavior ball = null;

        public delegate void StrikerCrossReceived(bool value);
        public event StrikerCrossReceived OnStrikerCrossReceived;

        private void OnTriggerEnter(Collider other)
        {
            ball = other.gameObject.GetComponent<BallBehavior>();

            if (ball && ball.GetMiniGame().enabled /*&& striker.GetPossessedBall() == null*/)
            {
                OnStrikerCrossReceived?.Invoke(true);
                ball.SetPossessor(striker);
                ShootToNearestGoal();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            OnStrikerCrossReceived?.Invoke(false);
        }

        private void ShootToNearestGoal()
        {
            tempShootForce = shootForce;

            //Find the nearest goal gate
            goalGate = GoalGateManager.Instance.GetClosestObjectInScene(striker.transform.position);

            if (goalGate)
            {
                //Calculate the shooting direction
                Vector3 shootingDir = goalGate.transform.position - striker.transform.position;

                shootingDir.y *= 20f;

                //If the striker is close to the goal, increase the force
                if (Vector3.Distance(striker.transform.position, goalGate.transform.position) <= 9.5f)
                {
                    tempShootForce *= 3.5f;
                }

                ball.GetRigidBody().AddForce(shootingDir * tempShootForce, ForceMode.Force);
            }
        }
    }
}

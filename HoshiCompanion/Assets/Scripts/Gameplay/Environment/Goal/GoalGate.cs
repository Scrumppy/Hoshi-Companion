using Gameplay.Environment.Ball;
using Gameplay.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Environment.Goal
{
    public class GoalGate : MonoBehaviour
    {
        private BallBehavior ball;
        void Start()
        {
            if (GoalGateManager.Instance != null) 
            {
                GoalGateManager.Instance.AddObject(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            ball = other.gameObject.GetComponent<BallBehavior>();

            if (ball && ball.GetMiniGame().enabled)
            {
                if (RewardsManager.Instance != null)
                {
                    RewardsManager.Instance.GiveRandomRewardToStriker(ball.GetPossessor());
                }

                ball.SetPossessor(null);
                Destroy(ball.gameObject);
                GameManager.Instance.RespawnBall();
            }
        }
    }
}

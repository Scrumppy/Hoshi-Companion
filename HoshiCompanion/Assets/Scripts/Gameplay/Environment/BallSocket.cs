using Gameplay.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Environment.Ball
{
    public class BallSocket : MonoBehaviour
    {
        [Header("Striker")]
        [SerializeField] private StrikerAI striker;

        [Header("Collider")]
        [SerializeField] private SphereCollider sphereCollider;

        private void OnTriggerEnter(Collider other)
        {
           BallBehavior ball = other.gameObject.GetComponent<BallBehavior>();

            if (ball && !striker.GetPossessedBall())
            {
                ball.SetPossession(striker);
                sphereCollider.enabled = false;            
            }
        }
    }
}


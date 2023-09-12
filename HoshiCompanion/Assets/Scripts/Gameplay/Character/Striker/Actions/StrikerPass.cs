using Gameplay.AI;
using Gameplay.Environment.Ball;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gameplay.Striker
{
    public class StrikerPass : MonoBehaviour
    {
        private float passForce = 28f;

        private BallBehavior ball;
        private StrikerAI sender;
        private StrikerAI receiver;

        private bool isPassing;

        public delegate void PassChanged(bool passed);
        public event PassChanged OnPassChanged;

        /// <summary>
        /// Pass function used to initiate the CombinedPassingBehavior's passing.
        /// </summary>
        /// <param name="sender">The sender of the ball</param>
        /// <param name="receiver">The receiver of the ball</param>
        /// <param name="ball">The ball to launch</param>
        public void Pass(StrikerAI sender, StrikerAI receiver, BallBehavior ball)
        {
            if (isPassing) return;

            if (!ball) return;

            if (!receiver) return;

            this.ball = ball;
            this.sender = sender;
            this.receiver = receiver;

            //Calculate the pass direction based on the sender's facing direction
            Vector3 passDirection = sender.transform.forward;

            sender.GetBallSocketTrigger().enabled = false;

            receiver.GetBallSocketTrigger().enabled = true;

            sender.GetComponent<StrikerPass>().OnPassChanged?.Invoke(true);

            ball.ReleaseBall();
            //ball.SetIsBeingUsed(true);
            // Reset the velocity of the ball's Rigidbody component
            ball.GetRigidBody().velocity = Vector3.zero;

            // Apply the pass force to the ball
            ball.GetRigidBody().AddForce(passDirection * passForce, ForceMode.Impulse);

            //sender.GetComponent<StrikerPass>().OnPassChanged?.Invoke(false);

            // Wait for the specified time before allowing another pass
            StartCoroutine(WaitToPass(1f));
            //sender.GetBallSocketTrigger().enabled = true;

            //isPassing = false;
        }

        private IEnumerator WaitToPass(float delay)
        {
            isPassing = true;

            yield return new WaitForSeconds(delay);

            //OnPassChanged?.Invoke(false);
            sender.GetComponent<StrikerPass>().OnPassChanged?.Invoke(false);
            //ball.SetIsPassed(false);
            //sender.GetBallSocketTrigger().enabled = true;
            isPassing = false;
        }
    }   
}

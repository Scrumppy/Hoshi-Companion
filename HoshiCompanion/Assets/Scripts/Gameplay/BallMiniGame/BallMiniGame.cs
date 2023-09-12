using Gameplay.AI;
using Gameplay.AI.Behaviors;
using Gameplay.Environment.Ball;
using Gameplay.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using static StrikersParser;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

namespace Gameplay.MiniGame
{
    public class BallMiniGame : MonoBehaviour
    {
        [Header("Ball")]
        [SerializeField] private BallBehavior ball;

        [Header("Data")]
        [SerializeField] private BallMiniGameData ballMiniGameData;

        [Header("Glow")]
        [SerializeField] private Renderer ballRenderer;
        [SerializeField] private Color glowColor;

        private StrikerAI nearestStriker = null;
        private Vector3 endPoint;

        private void Start()
        {
            ballRenderer.material.color = ballMiniGameData.glowColor;
        }

        #region LAUNCH BALL
        public void LaunchBallTowardsStriker()
        {
            //Get the nearest striker
            nearestStriker = StrikerManager.Instance.GetRandomObjectInScene();

            if (nearestStriker.GetStrikerMode() != StrikerAI.StrikerMode.Training)
            {
                Debug.Log("Striker " + nearestStriker.gameObject.name + " is not training");
                return;
            }

            //If the nearest striker is not null
            if (nearestStriker != null)
            {
                nearestStriker.GetCrossReceiver().enabled = true;
                //Calculate the control point for the Bezier curve
                Vector3 controlPoint = (transform.position + nearestStriker.transform.position) * 0.5f;

                controlPoint += Vector3.up * ballMiniGameData.curveHeight;

                //Get the strikers current movement direction
                Vector3 strikerMovementDirection = nearestStriker.GetMovementDirection();

                //Add an offset to the endpoint
                float offset = strikerMovementDirection.magnitude * ballMiniGameData.offsetValue;

                //Launch the ball along the Bezier curve and update the endpoint
                StartCoroutine(LaunchBallAlongCurve(transform.position, controlPoint, ballMiniGameData.curveDuration, strikerMovementDirection, offset));
            }
        }

        private IEnumerator LaunchBallAlongCurve(Vector3 startPoint, Vector3 controlPoint, float duration, Vector3 strikerMovementDirection, float offset)
        {
            float elapsedTime = 0f;

            //Calculate bezier points
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                Vector3 newPosition = CalculateBezierCurvePoint(startPoint, controlPoint, endPoint, t);
                ball.transform.position = newPosition;

                //Update the endpoint to the strikers current position
                endPoint = nearestStriker.transform.position + strikerMovementDirection * Time.deltaTime;

                elapsedTime += Time.deltaTime;
                endPoint += offset * strikerMovementDirection;
                endPoint.y *= -50f;

                yield return null;
            }

            //Ensure the ball reaches the final position 
            ball.transform.position = endPoint;
        }

        private Vector3 CalculateBezierCurvePoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 point = uu * p0 + 2f * u * t * p1 + tt * p2;
            return point;
        }
        #endregion
    }
}

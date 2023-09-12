using Gameplay.AI;
using Gameplay.Striker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
using System.Runtime.InteropServices.WindowsRuntime;
using Gameplay.Managers;
using Gameplay.MiniGame;

namespace Gameplay.Environment.Ball
{
    public class BallBehavior : MonoBehaviour
    {
        public const string ballTag = "Ball";
        public const string floorTag = "Floor";

        public const string wallTag = "WallStadium";
        public const string stadiumLayer = "Stadium";

        [Header("Components")]
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private SphereCollider sphereCollider = null;
        [SerializeField] private SphereCollider colliderTrigger = null;
        [SerializeField] private BallPhysics ballPhysics;
        private BallMiniGame ballMiniGame;
        public MeshRenderer ballRenderer = null;

        [Header("Data")]
        [SerializeField] private BallData ballData;

        [Header("Magnus Effect")]
        public float maxAngularVelocity = 1000f;
        public float rotationMultiplier = 100f;
        public float magnusForceMultiplier = 1f;
        public float magnusSmoothVelocity = 1f;

        [Header("Behavior")]
        private bool isPossessed = false;
        private bool isInUse = false;
        private StrikerAI striker = null;
        private StrikerAI lastPosessionStriker = null;
        public Coroutine snapBehaviour = null;

        [Header("Movement Curves")]
        [SerializeField] private AnimationCurve forwardCurve;
        [SerializeField] private AnimationCurve backwardCurve;

        private float lastPossedCharacterTimeMoving = 0f ;
        private float forwardSpeed = 0f;
        private float backwardSpeed = 0f;
        private float dribbleDistance = 0f;
        private float travelDistance = 0f;
        private bool isMovingForwards = false;


        private void Awake()
        {
            if (ballData)
            {
                colliderTrigger.radius = ballData.triggerSize;
                forwardSpeed = ballData.ballRollingForwardDribleSpeed;
                backwardSpeed = ballData.ballRollingBackDribleSpeed;    
                dribbleDistance = ballData.ballDribleDistance;
            }
        }

        private void Start()
        {
            BallManager.Instance.AddObject(this);
            ballMiniGame = GetComponent<BallMiniGame>();
            ActivateMiniGame(false);
        }

        private void Update()
        {
            BehaviorUpdate();
        }

        #region SNAPPING FUNCTIONS
        private void SnapToHolder(Transform holder)
        {
            if (snapBehaviour != null)
            {
                StopCoroutine(snapBehaviour);
            }

            if (!holder) return;

            snapBehaviour = StartCoroutine(SnapBehavior(holder, ballData.snapDelay));
        }

        IEnumerator SnapBehavior(Transform holder, float snapTime)
        {
            transform.SetParent(holder);
            transform.localPosition = holder.localPosition;
            transform.localRotation = holder.localRotation;
            Vector3 startPosition = transform.localPosition;

            float p = 0;
            while (p < 1f && snapTime > 0f)
            {
                p += Time.deltaTime / snapTime;
                transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, p);

                yield return new WaitForEndOfFrame();

                if (transform.parent != holder)
                {
                    if (snapBehaviour != null)
                    {
                        StopCoroutine(snapBehaviour);
                    }
                       
                    snapBehaviour = null;

                    yield return null;
                }
            }

            transform.localPosition = Vector3.zero;
            snapBehaviour = null;
        }
        #endregion

        private void BehaviorUpdate()
        {
            TimeDurationTheCharacterIsMoving();
            RotateBallAtPosession();
            MoveBallAtPossession();
        }

        private void TimeDurationTheCharacterIsMoving()
        {
            //if the character has one of this parameters wrong, it means we dont want the ball to move while dribling, so we make the lastPossedCharacterTimeMoving 0
            if (!isPossessed || !lastPosessionStriker.IsMoving() || IsBallCollidingWithWall())
            {
                lastPossedCharacterTimeMoving = 0;
                return;
            }

            lastPossedCharacterTimeMoving += Time.deltaTime;
        }

        #region ROTATION AND MOVEMENT FUNCTIONS WHEN POSSESSED
        private void RotateBallAtPosession()
        {
            if (!isPossessed) return;

            if (!striker) return;

            Vector3 movementDirection = striker.GetMovementDirection();

            if (movementDirection != Vector3.zero && striker.IsMoving())
            {
                float rotationAngle = ballData.rotationSpeedWhenInPossesion * Time.deltaTime;
                float dotProduct = Vector3.Dot(movementDirection, striker.transform.forward);
                Quaternion rotation = Quaternion.AngleAxis(rotationAngle, Vector3.right * Mathf.Sign(dotProduct));
                transform.Rotate(rotation.eulerAngles, Space.Self);
            }
        }

        private void MoveBallAtPossession()
        {
            if (!isPossessed) return;

            if (lastPossedCharacterTimeMoving < 0) return;

            if (IsBeingUsed()) return;

            //Get ball socket position
            Vector3 ballSocketPos = lastPosessionStriker.GetBallSocket().transform.position;

            //Get striker movement direction
            Vector3 movementDirection = lastPosessionStriker.GetMovementDirection();

            if (striker.IsMoving())
            {
                if (!isMovingForwards)
                {
                    //Move the ball forward
                    Vector3 targetPos = ballSocketPos + movementDirection * dribbleDistance;

                    //Evaluate the the movement curve, and apply it to the interpolation
                    float t = travelDistance / dribbleDistance;
                    float curveT = forwardCurve.Evaluate(t);

                    transform.position = Vector3.Lerp(ballSocketPos, targetPos, curveT);

                    travelDistance += Time.deltaTime * forwardSpeed;

                    //Check if the ball has reached the maximum forward distance
                    if (travelDistance >= dribbleDistance)
                    {
                        isMovingForwards = true;
                        travelDistance = dribbleDistance;
                    }
                }
                else
                {
                    //Move the ball backwards
                    Vector3 targetPos = ballSocketPos;

                    //Evaluate the the movement curve, and apply it to the interpolation
                    float t = travelDistance / dribbleDistance;
                    float curveT = backwardCurve.Evaluate(t);

                    transform.position = Vector3.Lerp(ballSocketPos + movementDirection * dribbleDistance, targetPos, curveT);
                    travelDistance -= Time.deltaTime * backwardSpeed;

                    //Check if the ball has reached the original position
                    if (travelDistance <= 0f)
                    {
                        isMovingForwards = false;
                        travelDistance = 0f;
                    }
                }
            }
            else
            {
                transform.position = ballSocketPos;
            }
        }
        #endregion

        private bool IsBallCollidingWithWall()
        {
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, transform.localScale.x, LayerMask.GetMask(stadiumLayer));
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].transform.CompareTag(wallTag))
                {
                    return true;
                }
            }

            return false;
        }

        public void SetPossession(StrikerAI possessor)
        {
            isPossessed = possessor != null;
            striker = possessor;

            if (ballPhysics)
            {
                ballPhysics.SetKinematic(isPossessed);
            }

            if (sphereCollider)
            {
                sphereCollider.enabled = !isPossessed;
            }

            if (striker)
            {
                SnapToHolder(striker.GetBallSocket());
                striker.SetHasBallPossession(true);
                striker.SetPossessedBall(this);
               // Debug.Log("Attached " + this.gameObject.name + " to " + striker.gameObject.name);
            }
            else 
            {
               // Debug.Log("Detached ball from striker ");
                transform.SetParent(null);
            }

            if (striker)
            {
                lastPosessionStriker = striker;
            }
        }

        public void SetPossessor(StrikerAI possessor)
        {
            striker = possessor;

            if (striker)
            {
                lastPosessionStriker = striker;
            }
        }

        public void ReleaseBall()
        {
            lastPosessionStriker.SetPossessedBall(null);
            SetPossession(null);
        }

        public void ActivateMiniGame(bool value)
        {
            if (ballMiniGame)
            {
                ballMiniGame.enabled = value;
            }
        }

        #region GETTERS & SETTERS
        public void SetIsBeingUsed(bool value)
        {
            isInUse = value;
        }

        public bool IsBeingUsed() { return isInUse; }
        public Rigidbody GetRigidBody() { return rigidBody; }
        public bool IsPossessed() { return isPossessed; }
        public StrikerAI GetPossessor() { return striker; }
        public BallMiniGame GetMiniGame() { return ballMiniGame; }
        #endregion
    }

}

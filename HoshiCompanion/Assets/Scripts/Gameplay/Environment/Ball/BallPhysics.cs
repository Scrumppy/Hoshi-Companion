using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Environment.Ball
{
    public class BallPhysics : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private SphereCollider sphereCollider;

        [Header("Data")]
        [SerializeField] private BallPhysicsData ballPhysicsData;

        [Header("Behavior")]
        public Vector3 velocity = Vector3.zero;
        public List<Vector3> contacts = new List<Vector3>();
        public bool onFloor;
        public float currentLobMultiplier = 0;
        public bool wasShot = false;
        public Vector3 shotInitialForce;
        private float bouncinessTime;
        public bool movementConstrainted = false;

        [Header("Reduce Friction Data")]
        [Range(0f, 1f)]
        [HideInInspector] public float frictionReducer = 1f; //0 to 1, reduces the friction for x time
        private float timeLeftForReducingFriction;
        private PhysicMaterial initialMaterialStats;

        private void Awake()
        {
            initialMaterialStats = sphereCollider.material;
        }
        private void Update()
        {
            if (timeLeftForReducingFriction > 0)
            {
                ReduceFriction();
            }   
        }

        //this makes the friction reducer go from 0 to 1, in order to reduce the friction over time
        private void ReduceFriction()
        {
            if (timeLeftForReducingFriction > 0)
            {
                timeLeftForReducingFriction -= Time.deltaTime;
                frictionReducer = timeLeftForReducingFriction / ballPhysicsData.timeToReduceFrictionAfterShot;
            }
            //if we stop reducing friction
            if (timeLeftForReducingFriction <= 0)
            {
                timeLeftForReducingFriction = 0;
                frictionReducer = 1;
                //put the friction of the physic material back to its initial stats
                sphereCollider.material.frictionCombine = initialMaterialStats.frictionCombine;
                sphereCollider.material.staticFriction = initialMaterialStats.staticFriction;
                sphereCollider.material.dynamicFriction = initialMaterialStats.dynamicFriction;
            }
        }

        private void FixedUpdate()
        {
            Behaviour(Time.fixedDeltaTime);
        }

        #region PHYSICS BEHAVIORS
        private void Behaviour(float delta)
        {
            if (!movementConstrainted)
            {
                velocity = rigidBody.velocity;
            }
                
            GravityBehaviour(delta);
            FloorCheck();
            FrictionRotation(delta);
            Simulate(delta);
        }

        private void GravityBehaviour(float delta)
        {
            rigidBody.useGravity = true;

            //Gravity
            if (rigidBody != null && rigidBody.useGravity && ballPhysicsData.gravityModifier != 0)
            { 
                rigidBody.AddForce(Vector3.up * ballPhysicsData.gravityModifier, ForceMode.Acceleration);
            }
        }

        private void Simulate(float delta)
        {
            Vector3 resultVelocity = velocity;
            if (contacts != null && contacts.Count > 0)
            {
                contacts.ForEach(v => resultVelocity += v);
                contacts = new List<Vector3>();
            }

            contacts = null;
        }

        private void FloorCheck()
        {
            onFloor = Physics.Raycast(transform.position, Vector3.down * ballPhysicsData.floorToleranceDistance, out RaycastHit hit, ballPhysicsData.floorToleranceDistance, ballPhysicsData.floorLayer);

            if (onFloor)
            {
                wasShot = false;
            }
        }

        private void FrictionRotation(float delta)
        {
            if (ballPhysicsData.frictionForce != 0)
            {
                Vector3 originalForce = (movementConstrainted ? velocity : rigidBody.velocity);
                originalForce.y = rigidBody.velocity.y;

                Vector3 force = originalForce;
                force.y = 0;

                float v = force.magnitude;

                if (v <= 0)
                {
                    return;
                }

                v -= (ballPhysicsData.frictionForce * frictionReducer) * delta;
                if (v < 0)
                {
                    v = 0;
                }
                    
                force *= v / force.magnitude;
                force.y = originalForce.y;

                if (movementConstrainted)
                {
                    velocity = force;
                }
                else
                {
                    rigidBody.velocity = force;
                }
            }
        }
        #endregion

        #region GETTERS & SETTERS
        public void SetKinematic(bool state)
        {
            rigidBody.isKinematic = state;

            if (!state)
            {
               rigidBody.velocity = velocity;
            }  
        }
        #endregion
    }
}

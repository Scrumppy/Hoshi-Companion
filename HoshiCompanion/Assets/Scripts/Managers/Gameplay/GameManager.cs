using Gameplay.AI;
using Gameplay.Environment.Ball;
using PlayerData.Strikers;
using System.Collections;
using System.Collections.Generic;
using UI.Character;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Managers
{
    public class GameManager : Manager<GameManager>
    {
        [Header("Components")]
        [SerializeField] private GameObject strikerPrefab;
        [SerializeField] private Camera cameraTarget;

        [Header("Ball Mini-Game")]
        [SerializeField] private float minCooldownDuration = 5f;
        [SerializeField] private float maxCooldownDuration = 10f;
        [SerializeField] private bool startMiniGame = true;

        [Header("Ball Spawners")]
        [SerializeField] private List<Transform> ballSpawners;

        [Header("Prefabs")]
        [SerializeField] private GameObject ballPrefab;

        private BallBehavior activeBall;

        private void Start()
        {
            //Start the ball mini-game after its cooldown has passed
            if (startMiniGame)
            {
                StartCoroutine(BallMiniGameCooldown());
            }
        }

        private void LateUpdate()
        {
            //Read if the player clicks on a ball
            if (RaycastBall())
            {
                activeBall.GetMiniGame().LaunchBallTowardsStriker();
            }
        }

        #region BALL MINI GAME
        private void ActivateRandomBallMinigame()
        {
            List<BallBehavior> eligibleBalls = new List<BallBehavior>();

            foreach (BallBehavior ball in BallManager.Instance.GetObjectsInScene())
            {
                //If the ball is not being used and has no possessor, add it to the list
                if (!ball.IsBeingUsed() && ball.GetPossessor() == null)
                {
                    eligibleBalls.Add(ball);
                }
            }

            //Select a random ball from the eligible list
            if (eligibleBalls.Count > 0)
            {
                int randomIndex = Random.Range(0, eligibleBalls.Count);
                activeBall = eligibleBalls[randomIndex];
                activeBall.ActivateMiniGame(true);
                eligibleBalls.RemoveAt(randomIndex);
            }
        }

        private IEnumerator BallMiniGameCooldown()
        {
            while (true)
            {
                float cooldownDuration = Random.Range(minCooldownDuration, maxCooldownDuration);

                yield return new WaitForSeconds(cooldownDuration);

                ActivateRandomBallMinigame();
            }
        }

        private bool RaycastBall()
        {
            if (Input.GetMouseButtonDown(0))
            {
#pragma warning disable UNT0028 // Use non-allocating physics APIs
                RaycastHit[] hits = Physics.RaycastAll(cameraTarget.ScreenPointToRay(Input.mousePosition));
#pragma warning restore UNT0028 // Use non-allocating physics APIs
                foreach (RaycastHit hit in hits)
                {
                    //Check if the hit game object has the Ball tag and is the active ball
                    if (hit.collider.gameObject.CompareTag("Ball") && activeBall != null)
                    {
                        if (hit.collider.gameObject == activeBall.gameObject && (hit.collider.isTrigger || !hit.collider.isTrigger))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        public void SpawnStriker(int strikerID)
        {
            GameObject strikerInstance = Instantiate(strikerPrefab, new Vector3(3, 1, 0), Quaternion.identity);
            StrikerAI strikerAI = strikerInstance.GetComponent<StrikerAI>();
            strikerAI.AssignStrikerInfoData(strikerID);
        }

        public void RespawnBall()
        {
            List<Transform> availableSpawners = new List<Transform>();

            foreach (Transform spawner in ballSpawners)
            {
                //Check if the spawner has no ball currently spawned
                if (spawner.childCount == 0)
                {
                    availableSpawners.Add(spawner);
                }
            }

            //Select a random available ball spawner
            if (availableSpawners.Count > 0)
            {
                int randomIndex = Random.Range(0, availableSpawners.Count);
                Transform spawnPosition = availableSpawners[randomIndex];

                //Instantiate a new ball at the selected spawner position
                GameObject newBall = Instantiate(ballPrefab, spawnPosition.position, Quaternion.identity);

                newBall.transform.parent = spawnPosition;
            }
            else
            {
                Debug.LogWarning("No available spawner for the ball to spawn!");
                return;
            }
        }
    }
}


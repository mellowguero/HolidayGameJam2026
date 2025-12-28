using UnityEngine;

namespace HolidayGameJam
{
    public class AIWatcherController : MonoBehaviour
    {
        [Header("Runner Tracking")]
        [SerializeField] private Transform runnerTransform;
        [SerializeField] private Rigidbody runnerRigidbody;

        [Header("Block Spawning")]
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private float spawnHeight = 25f;

        [Header("Prediction Settings")]
        [SerializeField] private float predictionTime = 1.5f;
        [SerializeField] private float inaccuracyRadius = 2.5f;

        [Header("Timing")]
        [SerializeField] private float spawnCooldown = 2.5f;

        [Header("Grid Settings")]
        [SerializeField] private float cellSize = 5f;
        [SerializeField] private float worldFloorMinX = -50f;
        [SerializeField] private float worldFloorMaxX = 50f;
        [SerializeField] private float worldFloorMinZ = -50f;
        [SerializeField] private float worldFloorMaxZ = 50f;

        private float nextSpawnTime;
        private bool isActive = true;

        private void Start()
        {
            if (runnerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    runnerTransform = player.transform;
                    runnerRigidbody = player.GetComponent<Rigidbody>();
                }
                else
                {
                    Debug.LogError("AIWatcherController: No Runner found with Player tag!");
                }
            }

            if (runnerRigidbody == null && runnerTransform != null)
            {
                runnerRigidbody = runnerTransform.GetComponent<Rigidbody>();
            }

            if (blockPrefab == null)
            {
                Debug.LogError("AIWatcherController: Block prefab not assigned!");
            }

            nextSpawnTime = Time.time + spawnCooldown;
            SubscribeToGameEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromGameEvents();
        }

        private void SubscribeToGameEvents()
        {
            GoalTrigger goalTrigger = FindAnyObjectByType<GoalTrigger>();
            if (goalTrigger != null)
            {
                goalTrigger.onGoalReached.AddListener(OnRoundEnd);
            }

            // TODO: Re-enable when using Opsive CharacterHealth
            // RunnerHealth runnerHealth = FindAnyObjectByType<RunnerHealth>();
            // if (runnerHealth != null)
            // {
            //     runnerHealth.onDeath.AddListener(OnRoundEnd);
            // }
        }

        private void UnsubscribeFromGameEvents()
        {
            GoalTrigger goalTrigger = FindAnyObjectByType<GoalTrigger>();
            if (goalTrigger != null)
            {
                goalTrigger.onGoalReached.RemoveListener(OnRoundEnd);
            }

            // TODO: Re-enable when using Opsive CharacterHealth
            // RunnerHealth runnerHealth = FindAnyObjectByType<RunnerHealth>();
            // if (runnerHealth != null)
            // {
            //     runnerHealth.onDeath.RemoveListener(OnRoundEnd);
            // }
        }

        private void OnRoundEnd()
        {
            isActive = false;
            Debug.Log("AIWatcherController: Round ended, stopping block spawns.");
        }

        private void Update()
        {
            if (!isActive || runnerTransform == null || blockPrefab == null)
                return;

            if (Time.time >= nextSpawnTime)
            {
                SpawnPredictedBlock();
                nextSpawnTime = Time.time + spawnCooldown;
            }
        }

        private void SpawnPredictedBlock()
        {
            Vector3 predictedPosition = PredictRunnerPosition();
            Vector3 snappedPosition = SnapToGrid(predictedPosition);
            Vector3 inaccuratePosition = ApplyInaccuracy(snappedPosition);
            Vector3 spawnPosition = new Vector3(inaccuratePosition.x, spawnHeight, inaccuratePosition.z);

            if (IsPositionInBounds(spawnPosition))
            {
                GameObject block = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
                block.tag = "Hazard";
            }
        }

        private Vector3 PredictRunnerPosition()
        {
            Vector3 currentPosition = runnerTransform.position;
            Vector3 velocity = Vector3.zero;

            if (runnerRigidbody != null)
            {
                velocity = runnerRigidbody.linearVelocity;
            }

            Vector3 futurePosition = currentPosition + (velocity * predictionTime);
            return futurePosition;
        }

        private Vector3 SnapToGrid(Vector3 position)
        {
            float snappedX = Mathf.Round(position.x / cellSize) * cellSize;
            float snappedZ = Mathf.Round(position.z / cellSize) * cellSize;

            return new Vector3(snappedX, position.y, snappedZ);
        }

        private Vector3 ApplyInaccuracy(Vector3 position)
        {
            float randomX = Random.Range(-inaccuracyRadius, inaccuracyRadius);
            float randomZ = Random.Range(-inaccuracyRadius, inaccuracyRadius);

            return new Vector3(position.x + randomX, position.y, position.z + randomZ);
        }

        private bool IsPositionInBounds(Vector3 position)
        {
            return position.x >= worldFloorMinX && position.x <= worldFloorMaxX &&
                   position.z >= worldFloorMinZ && position.z <= worldFloorMaxZ;
        }

        private void OnDrawGizmos()
        {
            if (runnerTransform != null)
            {
                Vector3 predictedPos = PredictRunnerPosition();
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(predictedPos, 1f);

                Vector3 snappedPos = SnapToGrid(predictedPos);
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(snappedPos, Vector3.one * cellSize);
            }
        }
    }
}

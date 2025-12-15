using UnityEngine;
using UnityEngine.InputSystem;
using HolidayGameJam;

namespace HolidayGameJam
{
    public class FallingBlockTest : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private GameObject fallingBlockPrefab;
        [SerializeField] private AIGridSetup gridSetup;
        
        [Header("Player Reference")]
        [SerializeField] private Transform playerTransform;
        
        [Header("Test Controls")]
        [Tooltip("Height offset above player for spawning test blocks")]
        [SerializeField] private float testSpawnHeight = 15f;

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                SpawnRandomBlock();
            }
            
            if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
            {
                SpawnBlockAbovePlayer();
            }
        }

        private void SpawnRandomBlock()
        {
            if (fallingBlockPrefab == null)
            {
                Debug.LogWarning("FallingBlockTest: No falling block prefab assigned!");
                return;
            }

            if (gridSetup == null)
            {
                Debug.LogWarning("FallingBlockTest: No AIGridSetup assigned!");
                return;
            }

            Vector2Int randomCell = new Vector2Int(
                Random.Range(0, gridSetup.GridResolution),
                Random.Range(0, gridSetup.GridResolution)
            );

            Vector3 spawnPos = gridSetup.GridCellToWorld(randomCell);
            spawnPos.y = gridSetup.SpawnHeight;

            Instantiate(fallingBlockPrefab, spawnPos, Quaternion.identity);

            Debug.Log($"FallingBlockTest: Spawned block at grid cell {randomCell} (world pos: {spawnPos})");
        }
        
        private void SpawnBlockAbovePlayer()
        {
            if (fallingBlockPrefab == null)
            {
                Debug.LogWarning("FallingBlockTest: No falling block prefab assigned!");
                return;
            }

            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
                else
                {
                    Debug.LogWarning("FallingBlockTest: No player found! Assign playerTransform or tag player as 'Player'");
                    return;
                }
            }

            Vector3 spawnPos = playerTransform.position;
            spawnPos.y += testSpawnHeight;

            Instantiate(fallingBlockPrefab, spawnPos, Quaternion.identity);

            Debug.Log($"FallingBlockTest: Spawned block directly above player at {spawnPos}");
        }
    }
}

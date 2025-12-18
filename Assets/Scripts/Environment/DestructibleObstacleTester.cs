using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace HolidayGameJam
{
    public class DestructibleObstacleTester : MonoBehaviour
    {
        [Header("Test Settings")]
        [Tooltip("Destructible obstacles to test (can add multiple)")]
        [SerializeField] private DestructibleObstacle[] targetObstacles;
        
        [Tooltip("Damage amount per test")]
        [SerializeField] private int damageAmount = 1;
        
        private Keyboard _keyboard;
        
        private void Awake()
        {
            _keyboard = Keyboard.current;
        }
        
        private void Update()
        {
            if (targetObstacles == null || targetObstacles.Length == 0 || _keyboard == null) return;
            
            if (_keyboard.oKey.wasPressedThisFrame)
            {
                Debug.Log($"[ObstacleTester] Dealing {damageAmount} damage to {targetObstacles.Length} obstacle(s)");
                foreach (var obstacle in targetObstacles)
                {
                    if (obstacle != null && !obstacle.IsDestroyed())
                    {
                        Debug.Log($"[ObstacleTester] Damaging '{obstacle.gameObject.name}' - Health before: {obstacle.GetHealthPercent() * 100:F0}%");
                        obstacle.TakeDamage(damageAmount);
                    }
                }
            }
            
            if (_keyboard.pKey.wasPressedThisFrame)
            {
                Debug.Log($"[ObstacleTester] Destroying {targetObstacles.Length} obstacle(s) instantly!");
                foreach (var obstacle in targetObstacles)
                {
                    if (obstacle != null && !obstacle.IsDestroyed())
                    {
                        Debug.Log($"[ObstacleTester] Destroying '{obstacle.gameObject.name}'");
                        obstacle.DestroyObstacle();
                    }
                }
            }
            
            if (_keyboard.rKey.wasPressedThisFrame)
            {
                Debug.Log("[ObstacleTester] Resetting scene...");
                ResetScene();
            }
        }
        
        private void ResetScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
        
        private void OnGUI()
        {
            if (targetObstacles == null || targetObstacles.Length == 0) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 400, 300));
            GUILayout.Box("=== DESTRUCTIBLE OBSTACLE TESTER ===");
            GUILayout.Label($"Testing {targetObstacles.Length} obstacle(s)");
            GUILayout.Space(5);
            
            int activeCount = 0;
            int destroyedCount = 0;
            
            foreach (var obstacle in targetObstacles)
            {
                if (obstacle != null)
                {
                    if (obstacle.IsDestroyed())
                        destroyedCount++;
                    else
                        activeCount++;
                }
            }
            
            GUILayout.Label($"Active: {activeCount} | Destroyed: {destroyedCount}");
            GUILayout.Space(10);
            
            GUILayout.Label("Individual Obstacle Health:");
            foreach (var obstacle in targetObstacles)
            {
                if (obstacle != null && !obstacle.IsDestroyed())
                {
                    GUILayout.Label($"• {obstacle.gameObject.name}: {obstacle.GetHealthPercent() * 100:F0}%");
                }
            }
            
            GUILayout.Space(10);
            GUILayout.Label($"Press [O] to damage ALL ({damageAmount})");
            GUILayout.Label($"Press [P] to destroy ALL instantly");
            GUILayout.Label($"Press [R] to RESET scene");
            GUILayout.Space(10);
            GUILayout.Label("(T key spawns falling blocks)");
            GUILayout.EndArea();
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

namespace HolidayGameJam
{
    public class DestructibleObstacleTester : MonoBehaviour
    {
        [Header("Test Settings")]
        [Tooltip("Destructible obstacle to test")]
        [SerializeField] private DestructibleObstacle targetObstacle;
        
        [Tooltip("Damage amount per test")]
        [SerializeField] private int damageAmount = 1;
        
        private Keyboard _keyboard;
        
        private void Awake()
        {
            _keyboard = Keyboard.current;
        }
        
        private void Update()
        {
            if (targetObstacle == null || _keyboard == null) return;
            
            if (_keyboard.oKey.wasPressedThisFrame)
            {
                Debug.Log($"[ObstacleTester] Dealing {damageAmount} damage to obstacle. Current Health: {targetObstacle.GetHealthPercent() * 100:F0}%");
                targetObstacle.TakeDamage(damageAmount);
                Debug.Log($"[ObstacleTester] After damage, Health: {targetObstacle.GetHealthPercent() * 100:F0}%");
            }
            
            if (_keyboard.pKey.wasPressedThisFrame)
            {
                Debug.Log("[ObstacleTester] Destroying obstacle instantly!");
                targetObstacle.DestroyObstacle();
            }
        }
        
        private void OnGUI()
        {
            if (targetObstacle == null) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 350, 200));
            GUILayout.Box("=== DESTRUCTIBLE OBSTACLE TESTER ===");
            GUILayout.Label($"Obstacle Health: {targetObstacle.GetHealthPercent() * 100:F0}%");
            GUILayout.Label($"Is Destroyed: {targetObstacle.IsDestroyed()}");
            GUILayout.Space(10);
            GUILayout.Label($"Press [O] to damage ({damageAmount})");
            GUILayout.Label($"Press [P] to destroy instantly");
            GUILayout.Space(10);
            GUILayout.Label("(T key spawns falling blocks)");
            GUILayout.EndArea();
        }
    }
}

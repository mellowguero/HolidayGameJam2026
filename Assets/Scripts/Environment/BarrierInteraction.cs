using UnityEngine;

namespace HolidayGameJam
{
    public enum BarrierType
    {
        Glass,
        Wood,
        Stone
    }

    public class BarrierInteraction : MonoBehaviour
    {
        [Header("Barrier Configuration")]
        [SerializeField] private BarrierType barrierType = BarrierType.Glass;

        [Header("Glass Barrier Settings")]
        [Tooltip("Glass barriers instantly break on contact with Runner")]
        [SerializeField] private bool autoDestroyOnContact = true;

        private DestructibleObstacle _obstacle;

        private void Awake()
        {
            _obstacle = GetComponent<DestructibleObstacle>();
            
            if (_obstacle == null)
            {
                Debug.LogError($"[BarrierInteraction] No DestructibleObstacle component found on {gameObject.name}");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[BarrierInteraction] OnTriggerEnter with '{other.gameObject.name}' (Tag: '{other.tag}')");
            HandleCollision(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"[BarrierInteraction] OnCollisionEnter with '{collision.gameObject.name}' (Tag: '{collision.gameObject.tag}')");
            HandleCollision(collision.gameObject);
        }

        private void HandleCollision(GameObject other)
        {
            if (!other.CompareTag("Player"))
            {
                Debug.Log($"[BarrierInteraction] Ignoring collision - not tagged 'Player'");
                return;
            }

            Debug.Log($"[BarrierInteraction] Runner collided with {barrierType} barrier");

            if (barrierType == BarrierType.Glass && autoDestroyOnContact)
            {
                if (_obstacle != null && !_obstacle.IsDestroyed())
                {
                    Debug.Log($"[BarrierInteraction] Auto-destroying glass barrier");
                    _obstacle.TakeDamage(999);
                }
            }
        }
    }
}

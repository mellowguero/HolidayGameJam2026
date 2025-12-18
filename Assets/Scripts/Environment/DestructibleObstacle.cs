using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HolidayGameJam
{
    public class DestructibleObstacle : MonoBehaviour
    {
        [Header("Health Settings")]
        [Tooltip("Starting health of the obstacle")]
        [SerializeField] private int maxHealth = 1;
        
        [Tooltip("Should GameObject be destroyed when health reaches 0?")]
        [SerializeField] private bool destroyOnZeroHealth = true;
        
        [Tooltip("Convert to non-colliding debris instead of destroying")]
        [SerializeField] private bool becomeDebris = false;
        
        [Tooltip("How long debris exists before despawn")]
        [SerializeField] private float debrisLifetime = 3f;
        
        [Header("Visual Feedback")]
        [Tooltip("Duration of damage flash effect")]
        [SerializeField] private float damageFlashDuration = 0.1f;
        
        [Tooltip("How much obstacle shakes when hit")]
        [SerializeField] private float shakeIntensity = 0.2f;
        
        [Tooltip("Flash color on damage")]
        [SerializeField] private Color damageColor = Color.red;
        
        [Header("Effects")]
        [Tooltip("Particle effect spawned on destruction")]
        [SerializeField] private GameObject destructionParticlePrefab;
        
        [Tooltip("Debris chunks spawned on destruction")]
        [SerializeField] private GameObject debrisPrefab;
        
        [Tooltip("How long destruction particle effect lasts")]
        [SerializeField] private float destructionParticleLifetime = 1.5f;
        
        [Tooltip("Number of debris chunks to spawn")]
        [SerializeField] private int debrisCount = 3;
        
        [Header("Drops")]
        [Tooltip("Optional collectible to spawn on destruction")]
        [SerializeField] private GameObject dropOnDestruction;
        
        [Tooltip("Probability of dropping collectible (0-1)")]
        [SerializeField] private float dropChance = 0.1f;
        
        [Header("Events")]
        public UnityEvent<int> onDamaged;
        public UnityEvent onDestroyed;
        
        private int _currentHealth;
        private bool _isDestroyed;
        private Renderer _renderer;
        private Collider _collider;
        private MaterialPropertyBlock _propertyBlock;
        private Vector3 _originalPosition;
        private Coroutine _shakeCoroutine;
        private Coroutine _flashCoroutine;
        
        private const string EMISSION_COLOR_PROPERTY = "_EmissionColor";
        private const string BASE_COLOR_PROPERTY = "_BaseColor";
        
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _collider = GetComponent<Collider>();
            _propertyBlock = new MaterialPropertyBlock();
            _originalPosition = transform.localPosition;
        }
        
        private void Start()
        {
            _currentHealth = maxHealth;
            _isDestroyed = false;
        }
        
        public void TakeDamage(int damageAmount)
        {
            if (_isDestroyed) return;
            
            _currentHealth -= damageAmount;
            _currentHealth = Mathf.Max(_currentHealth, 0);
            
            Debug.Log($"[DestructibleObstacle] Took {damageAmount} damage. Current health: {_currentHealth}/{maxHealth}");
            
            onDamaged?.Invoke(_currentHealth);
            
            TriggerDamageVisuals();
            
            if (_currentHealth <= 0)
            {
                Debug.Log($"[DestructibleObstacle] Health reached 0, calling Destroy()");
                DestroyObstacle();
            }
        }
        
        public void DestroyObstacle()
        {
            if (_isDestroyed)
            {
                Debug.Log($"[DestructibleObstacle] Already destroyed, ignoring");
                return;
            }
            
            Debug.Log($"[DestructibleObstacle] Destroying obstacle. becomeDebris={becomeDebris}, destroyOnZeroHealth={destroyOnZeroHealth}");
            _isDestroyed = true;
            onDestroyed?.Invoke();
            
            SpawnDestructionEffects();
            SpawnDebris();
            HandleDrop();
            
            if (becomeDebris)
            {
                Debug.Log($"[DestructibleObstacle] Converting to debris");
                ConvertToDebris();
            }
            else if (destroyOnZeroHealth)
            {
                Debug.Log($"[DestructibleObstacle] Destroying GameObject '{gameObject.name}'");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"[DestructibleObstacle] Not destroying (destroyOnZeroHealth=false)");
            }
        }
        
        public bool IsDestroyed()
        {
            return _isDestroyed;
        }
        
        public float GetHealthPercent()
        {
            if (maxHealth <= 0) return 0f;
            return (float)_currentHealth / maxHealth;
        }
        
        private void TriggerDamageVisuals()
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }
            _flashCoroutine = StartCoroutine(DamageFlashCoroutine());
            
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }
            _shakeCoroutine = StartCoroutine(ShakeCoroutine());
        }
        
        private IEnumerator DamageFlashCoroutine()
        {
            if (_renderer == null) yield break;
            
            _renderer.GetPropertyBlock(_propertyBlock);
            Color originalColor = _propertyBlock.GetColor(BASE_COLOR_PROPERTY);
            
            _propertyBlock.SetColor(BASE_COLOR_PROPERTY, damageColor);
            _renderer.SetPropertyBlock(_propertyBlock);
            
            yield return new WaitForSeconds(damageFlashDuration);
            
            _propertyBlock.SetColor(BASE_COLOR_PROPERTY, originalColor);
            _renderer.SetPropertyBlock(_propertyBlock);
            
            _flashCoroutine = null;
        }
        
        private IEnumerator ShakeCoroutine()
        {
            float elapsed = 0f;
            
            while (elapsed < damageFlashDuration)
            {
                Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;
                transform.localPosition = _originalPosition + randomOffset;
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.localPosition = _originalPosition;
            _shakeCoroutine = null;
        }
        
        private void SpawnDestructionEffects()
        {
            if (destructionParticlePrefab != null)
            {
                Vector3 spawnPosition = transform.position;
                Quaternion spawnRotation = destructionParticlePrefab.transform.rotation;
                
                GameObject particle = Instantiate(destructionParticlePrefab, spawnPosition, spawnRotation);
                particle.transform.localScale = Vector3.one;
                Destroy(particle, destructionParticleLifetime);
            }
        }
        
        private void SpawnDebris()
        {
            if (debrisPrefab == null) return;
            
            for (int i = 0; i < debrisCount; i++)
            {
                Vector3 randomOffset = Random.insideUnitSphere * 0.5f;
                GameObject debris = Instantiate(debrisPrefab, transform.position + randomOffset, Random.rotation);
                
                Rigidbody rb = debris.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 randomVelocity = Random.insideUnitSphere * 3f;
                    randomVelocity.y = Mathf.Abs(randomVelocity.y);
                    rb.linearVelocity = randomVelocity;
                    rb.angularVelocity = Random.insideUnitSphere * 5f;
                }
                
                Destroy(debris, debrisLifetime);
            }
        }
        
        private void HandleDrop()
        {
            if (dropOnDestruction == null) return;
            
            float randomValue = Random.value;
            if (randomValue <= dropChance)
            {
                Instantiate(dropOnDestruction, transform.position, Quaternion.identity);
            }
        }
        
        private void ConvertToDebris()
        {
            if (_collider != null)
            {
                _collider.isTrigger = true;
            }
            
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            
            Vector3 randomVelocity = Random.insideUnitSphere * 2f;
            randomVelocity.y = Mathf.Abs(randomVelocity.y);
            rb.linearVelocity = randomVelocity;
            rb.angularVelocity = Random.insideUnitSphere * 3f;
            
            Destroy(gameObject, debrisLifetime);
        }
    }
}

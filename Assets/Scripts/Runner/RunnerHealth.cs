using UnityEngine;
using UnityEngine.Events;

namespace HolidayGameJam
{
    public class RunnerHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        [Tooltip("Starting health of the runner")]
        public int maxHealth = 1;
        
        [Header("Events")]
        public UnityEvent onDeath;
        public UnityEvent<int> onHealthChanged;
        
        private int _currentHealth;
        private bool _isDead;
        
        private void Start()
        {
            _currentHealth = maxHealth;
            _isDead = false;
        }
        
        public void TakeDamage(int damage)
        {
            if (_isDead) return;
            
            _currentHealth -= damage;
            _currentHealth = Mathf.Max(_currentHealth, 0);
            
            onHealthChanged?.Invoke(_currentHealth);
            
            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        
        public void Heal(int amount)
        {
            if (_isDead) return;
            
            _currentHealth += amount;
            _currentHealth = Mathf.Min(_currentHealth, maxHealth);
            
            onHealthChanged?.Invoke(_currentHealth);
        }
        
        public void Die()
        {
            if (_isDead) return;
            
            _isDead = true;
            onDeath?.Invoke();
        }
        
        public void Respawn()
        {
            _currentHealth = maxHealth;
            _isDead = false;
            onHealthChanged?.Invoke(_currentHealth);
        }
        
        public int GetCurrentHealth()
        {
            return _currentHealth;
        }
        
        public bool IsDead()
        {
            return _isDead;
        }
    }
}

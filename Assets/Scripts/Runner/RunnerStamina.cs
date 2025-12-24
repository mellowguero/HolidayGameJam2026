using UnityEngine;
using UnityEngine.Events;
using StarterAssets;

namespace HolidayGameJam
{
    public class RunnerStamina : MonoBehaviour
    {
        [Header("Stamina Settings")]
        [Tooltip("Maximum stamina value")]
        public float MaxStamina = 100f;
        
        [Tooltip("Stamina consumed per second while sprinting")]
        public float SprintConsumptionRate = 20f;
        
        [Tooltip("Stamina regenerated per second when not sprinting")]
        public float RegenerationRate = 15f;
        
        [Tooltip("Delay in seconds before regeneration starts after stamina is depleted")]
        public float RegenerationDelay = 0.5f;
        
        [Header("Events")]
        public UnityEvent<float, float> OnStaminaChanged;
        
        private float _currentStamina;
        private StarterAssetsInputs _input;
        private ThirdPersonController _controller;
        private float _regenerationDelayTimer;
        private bool _wasDepletedRecently;
        
        public float CurrentStamina => _currentStamina;
        
        private void Start()
        {
            _currentStamina = MaxStamina;
            _input = GetComponent<StarterAssetsInputs>();
            _controller = GetComponent<ThirdPersonController>();
            _regenerationDelayTimer = 0f;
            _wasDepletedRecently = false;
            
            OnStaminaChanged?.Invoke(_currentStamina, MaxStamina);
        }
        
        private void Update()
        {
            if (_input == null) return;
            
            bool isSprinting = _input.sprint && IsSprintAvailable();
            
            if (isSprinting)
            {
                ConsumeStamina(SprintConsumptionRate * Time.deltaTime);
            }
            else
            {
                RegenerateStamina(RegenerationRate * Time.deltaTime);
            }
            
            if (!IsSprintAvailable())
            {
                _input.sprint = false;
            }
        }
        
        private void ConsumeStamina(float amount)
        {
            _currentStamina -= amount;
            _currentStamina = Mathf.Max(_currentStamina, 0f);
            
            if (_currentStamina <= 0f)
            {
                _wasDepletedRecently = true;
                _regenerationDelayTimer = RegenerationDelay;
            }
            
            OnStaminaChanged?.Invoke(_currentStamina, MaxStamina);
        }
        
        private void RegenerateStamina(float amount)
        {
            if (_wasDepletedRecently)
            {
                _regenerationDelayTimer -= Time.deltaTime;
                if (_regenerationDelayTimer <= 0f)
                {
                    _wasDepletedRecently = false;
                }
                return;
            }
            
            if (_currentStamina < MaxStamina)
            {
                _currentStamina += amount;
                _currentStamina = Mathf.Min(_currentStamina, MaxStamina);
                OnStaminaChanged?.Invoke(_currentStamina, MaxStamina);
            }
        }
        
        public bool IsSprintAvailable()
        {
            return _currentStamina > 0f;
        }

        public void UseStamina(float amount)
        {
            ConsumeStamina(amount);
        }
        
        public void ResetStamina()
        {
            _currentStamina = MaxStamina;
            _wasDepletedRecently = false;
            _regenerationDelayTimer = 0f;
            OnStaminaChanged?.Invoke(_currentStamina, MaxStamina);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace HolidayGameJam
{
    public class StaminaUI : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Reference to the RunnerStamina component")]
        public RunnerStamina runnerStamina;
        
        [Tooltip("The fill image that represents stamina")]
        public Image fillImage;
        
        [Header("Color Settings")]
        [Tooltip("Color when stamina is high (100-60%)")]
        public Color highColor = new Color(0f, 1f, 0f, 1f);
        
        [Tooltip("Color when stamina is medium (60-30%)")]
        public Color mediumColor = new Color(1f, 1f, 0f, 1f);
        
        [Tooltip("Color when stamina is low (30-0%)")]
        public Color lowColor = new Color(1f, 0f, 0f, 1f);
        
        [Header("Optional Settings")]
        [Tooltip("Smooth transition for visual polish")]
        public bool useSmoothTransition = true;
        
        [Tooltip("Speed of smooth transition")]
        public float transitionSpeed = 5f;
        
        private float _targetFillAmount;
        
        private void Start()
        {
            if (runnerStamina == null)
            {
                Debug.LogError("StaminaUI: RunnerStamina reference is not set!");
                return;
            }
            
            if (fillImage == null)
            {
                Debug.LogError("StaminaUI: Fill Image reference is not set!");
                return;
            }
            
            runnerStamina.OnStaminaChanged.AddListener(OnStaminaChanged);
            
            _targetFillAmount = 1f;
            fillImage.fillAmount = 1f;
        }
        
        private void OnDestroy()
        {
            if (runnerStamina != null)
            {
                runnerStamina.OnStaminaChanged.RemoveListener(OnStaminaChanged);
            }
        }
        
        private void Update()
        {
            if (fillImage == null) return;
            
            if (useSmoothTransition)
            {
                fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, _targetFillAmount, Time.deltaTime * transitionSpeed);
            }
            else
            {
                fillImage.fillAmount = _targetFillAmount;
            }
        }
        
        private void OnStaminaChanged(float current, float max)
        {
            float normalizedStamina = current / max;
            _targetFillAmount = normalizedStamina;
            
            UpdateColor(normalizedStamina);
        }
        
        private void UpdateColor(float normalizedStamina)
        {
            if (fillImage == null) return;
            
            if (normalizedStamina > 0.6f)
            {
                fillImage.color = highColor;
            }
            else if (normalizedStamina > 0.3f)
            {
                fillImage.color = mediumColor;
            }
            else
            {
                fillImage.color = lowColor;
            }
        }
    }
}

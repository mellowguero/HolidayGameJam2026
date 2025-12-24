using System.Collections;
using UnityEngine;
using StarterAssets;

namespace HolidayGameJam
{
    [DefaultExecutionOrder(-10)]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ThirdPersonController))]
    [RequireComponent(typeof(StarterAssetsInputs))]
    public class RunnerVault : MonoBehaviour
    {
        [Header("Detection")]
        [Tooltip("Distance to detect vaultable obstacles ahead")]
        public float detectionDistance = 1.8f;

        [Tooltip("Minimum height for a vaultable obstacle")]
        public float minVaultHeight = 0.5f;

        [Tooltip("Maximum height for a vaultable obstacle")]
        public float maxVaultHeight = 1.5f;

        [Tooltip("Radius for spherecast detection")]
        public float detectionRadius = 0.5f;

        [Tooltip("Layer mask for vaultable objects")]
        public LayerMask vaultableLayer;

        [Header("Vault Settings")]
        [Tooltip("Duration of the vault animation")]
        public float vaultDuration = 0.7f;

        [Tooltip("Minimum speed required to vault")]
        public float requiredSpeed = 0.3f;

        [Tooltip("Cooldown duration between vaults")]
        public float cooldownDuration = 0.5f;

        [Tooltip("Forward distance to travel during vault")]
        public float vaultForwardDistance = 2.5f;

        [Tooltip("Arc height during manual vault")]
        public float vaultArcHeight = 1.0f;

        [Tooltip("Use root motion for vault animation")]
        public bool useRootMotion = true;

        [Header("Stamina (Optional)")]
        [Tooltip("Enable stamina cost for vaulting")]
        public bool useStaminaCost = false;

        [Tooltip("Stamina consumed per vault")]
        public float staminaCost = 10f;

        [Header("Animation")]
        [Tooltip("Animator trigger parameter name")]
        public string vaultTriggerParam = "VaultTrigger";

        [Tooltip("Animator bool parameter name")]
        public string isVaultingParam = "IsVaulting";

        [Header("Debug")]
        [Tooltip("Enable debug logging")]
        public bool debugMode = false;

        private CharacterController characterController;
        private Animator animator;
        private ThirdPersonController thirdPersonController;
        private StarterAssetsInputs inputs;
        private RunnerStamina stamina;
        private RunnerSlide slide;

        private bool isVaulting = false;
        private bool onCooldown = false;

        private int animIDVaultTrigger;
        private int animIDIsVaulting;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            thirdPersonController = GetComponent<ThirdPersonController>();
            inputs = GetComponent<StarterAssetsInputs>();
            stamina = GetComponent<RunnerStamina>();
            slide = GetComponent<RunnerSlide>();
        }

        private void Start()
        {
            animIDVaultTrigger = Animator.StringToHash(vaultTriggerParam);
            animIDIsVaulting = Animator.StringToHash(isVaultingParam);
        }

        private void Update()
        {
            if (!isVaulting && inputs.jump && CanVault() && DetectVaultableObstacle(out RaycastHit hit))
            {
                if (debugMode)
                {
                    Debug.Log("Vaulting! Obstacle detected at: " + hit.collider.name);
                }
                TryVault(hit);
                inputs.jump = false;
            }
            else if (debugMode && inputs.jump)
            {
                if (isVaulting)
                    Debug.Log("Cannot vault: Already vaulting");
                else if (!CanVault())
                    Debug.Log("Cannot vault: CanVault() returned false");
                else if (!DetectVaultableObstacle(out _))
                    Debug.Log("Cannot vault: No vaultable obstacle detected");
            }
        }

        public void TryVault(RaycastHit hit)
        {
            if (!CanVault())
            {
                return;
            }

            ConsumeStamina();
            StartCoroutine(ExecuteVault(hit));
        }

        private bool CanVault()
        {
            if (isVaulting || onCooldown)
            {
                return false;
            }

            if (!IsGrounded())
            {
                return false;
            }

            if (!HasSufficientSpeed())
            {
                return false;
            }

            if (slide != null && slide.IsSliding())
            {
                return false;
            }

            if (useStaminaCost && stamina != null && stamina.CurrentStamina < staminaCost)
            {
                return false;
            }

            return true;
        }

        private bool DetectVaultableObstacle(out RaycastHit hit)
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 direction = transform.forward;

            if (Physics.Raycast(origin, direction, out hit, detectionDistance, vaultableLayer))
            {
                float obstacleHeight = hit.collider.bounds.max.y - transform.position.y;
                return IsValidVaultHeight(obstacleHeight);
            }

            return false;
        }

        private bool IsValidVaultHeight(float height)
        {
            return height >= minVaultHeight && height <= maxVaultHeight;
        }

        private bool IsGrounded()
        {
            return thirdPersonController != null && thirdPersonController.Grounded;
        }

        private bool HasSufficientSpeed()
        {
            Vector3 horizontalVelocity = new Vector3(
                characterController.velocity.x,
                0f,
                characterController.velocity.z
            );

            return horizontalVelocity.magnitude >= requiredSpeed;
        }

        private void ConsumeStamina()
        {
            if (useStaminaCost && stamina != null)
            {
                stamina.UseStamina(staminaCost);
            }
        }

        private IEnumerator ExecuteVault(RaycastHit obstacleHit)
        {
            isVaulting = true;

            if (useRootMotion)
            {
                yield return ExecuteRootMotionVault();
            }
            else
            {
                Vector3 vaultEndPosition = CalculateVaultEndPosition(obstacleHit);
                yield return ExecuteManualVault(vaultEndPosition);
            }

            isVaulting = false;
            StartCoroutine(CooldownTimer());
        }

        private IEnumerator ExecuteRootMotionVault()
        {
            characterController.enabled = false;
            animator.applyRootMotion = true;

            TriggerVaultAnimation();

            yield return new WaitForSeconds(vaultDuration);

            EndVaultAnimation();

            animator.applyRootMotion = false;
            characterController.enabled = true;
        }

        private IEnumerator ExecuteManualVault(Vector3 endPosition)
        {
            characterController.enabled = false;

            TriggerVaultAnimation();

            Vector3 startPosition = transform.position;
            float elapsed = 0f;

            while (elapsed < vaultDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / vaultDuration;

                Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, t);
                currentPos.y += Mathf.Sin(t * Mathf.PI) * vaultArcHeight;

                transform.position = currentPos;

                yield return null;
            }

            transform.position = endPosition;

            EndVaultAnimation();

            characterController.enabled = true;
        }

        private Vector3 CalculateVaultEndPosition(RaycastHit obstacleHit)
        {
            Vector3 obstacleCenter = obstacleHit.collider.bounds.center;
            Vector3 forwardDirection = transform.forward;
            float obstacleDepth = obstacleHit.collider.bounds.extents.z * 2f;
            
            Vector3 endPosition = obstacleCenter + forwardDirection * (obstacleDepth * 0.5f + 1f);
            endPosition.y = transform.position.y;

            return endPosition;
        }

        private void TriggerVaultAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger(animIDVaultTrigger);
                animator.SetBool(animIDIsVaulting, true);
            }
        }

        private void EndVaultAnimation()
        {
            if (animator != null)
            {
                animator.SetBool(animIDIsVaulting, false);
            }
        }

        private IEnumerator CooldownTimer()
        {
            onCooldown = true;
            yield return new WaitForSeconds(cooldownDuration);
            onCooldown = false;
        }

        public bool IsVaulting()
        {
            return isVaulting;
        }

        public bool CanVaultNow()
        {
            return CanVault() && DetectVaultableObstacle(out RaycastHit _);
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 direction = transform.forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(origin, direction * detectionDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(origin + direction * detectionDistance, detectionRadius);
        }
    }
}

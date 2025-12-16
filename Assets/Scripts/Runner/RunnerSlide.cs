using UnityEngine;
using StarterAssets;

namespace HolidayGameJam
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ThirdPersonController))]
    [RequireComponent(typeof(StarterAssetsInputs))]
    public class RunnerSlide : MonoBehaviour
    {
        [Header("Slide Settings")]
        [Tooltip("Duration of the slide in seconds")]
        public float slideDuration = 0.8f;

        [Tooltip("Speed multiplier during slide")]
        public float slideSpeedMultiplier = 1.5f;

        [Tooltip("Percentage of normal height during slide (0-1)")]
        [Range(0.1f, 1f)]
        public float slideHeightPercent = 0.5f;

        [Tooltip("Minimum speed required to initiate a slide")]
        public float minimumSpeedToSlide = 1f;

        private CharacterController characterController;
        private Animator animator;
        private ThirdPersonController thirdPersonController;
        private StarterAssetsInputs inputs;

        private bool isSliding = false;
        private float slideTimer = 0f;
        private float originalHeight;
        private Vector3 originalCenter;

        private int animIDIsSliding;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            thirdPersonController = GetComponent<ThirdPersonController>();
            inputs = GetComponent<StarterAssetsInputs>();

            originalHeight = characterController.height;
            originalCenter = characterController.center;
        }

        private void Start()
        {
            animIDIsSliding = Animator.StringToHash("IsSliding");
        }

        private void Update()
        {
            if (isSliding)
            {
                UpdateSlide();
            }
            else
            {
                CheckForSlideInput();
            }
        }

        private void CheckForSlideInput()
        {
            if (inputs.slide)
            {
                TrySlide();
                inputs.slide = false;
            }
        }

        public void TrySlide()
        {
            if (!CanSlide())
            {
                return;
            }

            StartSlide();
        }

        private bool CanSlide()
        {
            if (isSliding)
            {
                return false;
            }

            if (!thirdPersonController.Grounded)
            {
                return false;
            }

            Vector3 horizontalVelocity = new Vector3(
                characterController.velocity.x,
                0f,
                characterController.velocity.z
            );

            if (horizontalVelocity.magnitude < minimumSpeedToSlide)
            {
                return false;
            }

            return true;
        }

        private void StartSlide()
        {
            isSliding = true;
            slideTimer = slideDuration;

            float targetHeight = originalHeight * slideHeightPercent;
            float heightDifference = originalHeight - targetHeight;

            characterController.height = targetHeight;
            characterController.center = new Vector3(
                originalCenter.x,
                originalCenter.y - (heightDifference * 0.5f),
                originalCenter.z
            );

            if (animator != null)
            {
                animator.SetBool(animIDIsSliding, true);
            }
        }

        private void UpdateSlide()
        {
            slideTimer -= Time.deltaTime;

            if (slideTimer <= 0f)
            {
                EndSlide();
            }
        }

        private void EndSlide()
        {
            isSliding = false;
            slideTimer = 0f;

            characterController.height = originalHeight;
            characterController.center = originalCenter;

            if (animator != null)
            {
                animator.SetBool(animIDIsSliding, false);
            }
        }

        public bool IsSliding()
        {
            return isSliding;
        }

        public float GetSlideSpeedMultiplier()
        {
            return isSliding ? slideSpeedMultiplier : 1f;
        }
    }
}

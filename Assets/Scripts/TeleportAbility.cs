using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;

namespace HolidayGJ.Abilities
{
    public class TeleportAbility : Ability
    {
        [Header("Teleport Settings")]
        [SerializeField] private float maxRange = 5f;
        [SerializeField] private float cooldownDuration = 3f;
        
        [Header("Camera Settings")]
        [SerializeField] private bool enableCameraRotation = true;
        [SerializeField] private float cameraRotationSpeed = 180f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private bool isInAimMode = false;
        private bool wasMovementRestricted = false;

        public float MaxRange
        {
            get => maxRange;
            set => maxRange = value;
        }

        public float CooldownDuration
        {
            get => cooldownDuration;
            set => cooldownDuration = value;
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override bool CanStartAbility()
        {
            if (isInAimMode)
            {
                return false;
            }

            return true;
        }

        protected override void AbilityStarted()
        {
            base.AbilityStarted();
            
            EnterAimMode();
        }

        private void EnterAimMode()
        {
            isInAimMode = true;
            
            LockCharacterMovement();
            EnableCameraRotation();
            
            if (showDebugLogs)
            {
                Debug.Log("TeleportAbility: Entered aim mode");
            }
        }

        private void LockCharacterMovement()
        {
            if (m_CharacterLocomotion != null)
            {
                wasMovementRestricted = m_CharacterLocomotion.UseRootMotionPosition;
                m_CharacterLocomotion.UseRootMotionPosition = true;
            }
            
            if (showDebugLogs)
            {
                Debug.Log("TeleportAbility: Character movement locked");
            }
        }

        private void EnableCameraRotation()
        {
            if (enableCameraRotation)
            {
                if (showDebugLogs)
                {
                    Debug.Log("TeleportAbility: Camera rotation enabled");
                }
            }
        }

        public override void Update()
        {
            base.Update();
            
            if (isInAimMode)
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    ExitAimMode(false);
                    StopAbility();
                }
            }
        }

        protected override void AbilityStopped(bool force)
        {
            if (isInAimMode)
            {
                ExitAimMode(false);
            }
            
            base.AbilityStopped(force);
        }

        private void ExitAimMode(bool teleported)
        {
            if (!isInAimMode)
            {
                return;
            }

            isInAimMode = false;
            
            UnlockCharacterMovement();
            DisableCameraRotation();
            
            if (showDebugLogs)
            {
                string reason = teleported ? "teleported" : "cancelled";
                Debug.Log($"TeleportAbility: Exited aim mode ({reason})");
            }
        }

        private void UnlockCharacterMovement()
        {
            if (m_CharacterLocomotion != null)
            {
                m_CharacterLocomotion.UseRootMotionPosition = wasMovementRestricted;
            }
            
            if (showDebugLogs)
            {
                Debug.Log("TeleportAbility: Character movement unlocked");
            }
        }

        private void DisableCameraRotation()
        {
            if (enableCameraRotation)
            {
                if (showDebugLogs)
                {
                    Debug.Log("TeleportAbility: Camera rotation disabled");
                }
            }
        }

        public bool IsInAimMode()
        {
            return isInAimMode;
        }
    }
}

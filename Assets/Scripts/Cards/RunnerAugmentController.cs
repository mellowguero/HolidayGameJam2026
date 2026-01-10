using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.AddOns.Agility;
using Opsive.UltimateCharacterController.AddOns.Climbing;

namespace HolidayGJ.Cards
{
    public class RunnerAugmentController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UltimateCharacterLocomotion locomotion;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private Dictionary<RunnerAugmentType, List<CardInstance>> activePassiveAugments = new Dictionary<RunnerAugmentType, List<CardInstance>>();
        private Dictionary<RunnerAugmentType, int> activeAbilityCharges = new Dictionary<RunnerAugmentType, int>();

        private Dictionary<string, float> baseValues = new Dictionary<string, float>();
        private bool baseValuesStored = false;

        private void Start()
        {
            if (locomotion == null)
            {
                locomotion = GetComponent<UltimateCharacterLocomotion>();
            }

            if (locomotion == null)
            {
                Debug.LogError("RunnerAugmentController: No UltimateCharacterLocomotion component found!");
                return;
            }

            StoreBaseValues();
        }

        private void StoreBaseValues()
        {
            if (baseValuesStored) return;

            var jumpAbility = locomotion.GetAbility<Jump>();
            if (jumpAbility != null)
            {
                baseValues["Jump.Force"] = GetAbilityFieldValue<float>(jumpAbility, "m_Force");
            }
            else if (showDebugLogs)
            {
                Debug.LogWarning("RunnerAugmentController: Jump ability not found on character");
            }

            var speedChangeAbility = locomotion.GetAbility<SpeedChange>();
            if (speedChangeAbility != null)
            {
                baseValues["SpeedChange.Multiplier"] = GetAbilityFieldValue<float>(speedChangeAbility, "m_SpeedChangeMultiplier");
                baseValues["SpeedChange.MaxValue"] = GetAbilityFieldValue<float>(speedChangeAbility, "m_MaxSpeedChangeValue");
            }
            else if (showDebugLogs)
            {
                Debug.LogWarning("RunnerAugmentController: SpeedChange ability not found on character");
            }

            var freeClimbAbility = locomotion.GetAbility<FreeClimb>();
            if (freeClimbAbility != null)
            {
                baseValues["FreeClimb.Speed"] = GetAbilityFieldValue<float>(freeClimbAbility, "m_Speed");
            }
            else if (showDebugLogs)
            {
                Debug.LogWarning("RunnerAugmentController: FreeClimb ability not found on character");
            }

            var hangAbility = locomotion.GetAbility<Hang>();
            if (hangAbility != null)
            {
                baseValues["Hang.MinDistance"] = GetAbilityFieldValue<float>(hangAbility, "m_MinHangDistance");
            }
            else if (showDebugLogs)
            {
                Debug.LogWarning("RunnerAugmentController: Hang ability not found on character");
            }

            var dodgeAbility = locomotion.GetAbility<Dodge>();
            if (dodgeAbility != null)
            {
                baseValues["Dodge.Speed"] = GetAbilityFieldValue<float>(dodgeAbility, "m_Speed");
            }
            else if (showDebugLogs)
            {
                Debug.LogWarning("RunnerAugmentController: Dodge ability not found on character");
            }

            baseValuesStored = true;

            if (showDebugLogs)
            {
                Debug.Log($"RunnerAugmentController: Stored {baseValues.Count} base values");
            }
        }

        public void ApplyCard(CardInstance card)
        {
            if (card == null || card.cardData == null)
            {
                Debug.LogWarning("RunnerAugmentController: Cannot apply null card");
                return;
            }

            EnsureInitialized();

            bool isTacticalCard = IsTacticalAugment(card.cardData.augmentType);

            if (isTacticalCard)
            {
                GrantActiveAbility(card);
            }
            else
            {
                ApplyPassiveAugment(card);
            }

            if (showDebugLogs)
            {
                string type = isTacticalCard ? "Active Ability" : "Passive Augment";
                Debug.Log($"RunnerAugmentController: Applied {type} - {card.cardData.cardName} (Level {card.currentLevel})");
            }
        }

        private void EnsureInitialized()
        {
            if (locomotion == null)
            {
                locomotion = GetComponent<UltimateCharacterLocomotion>();
            }

            if (locomotion == null)
            {
                Debug.LogError("RunnerAugmentController: No UltimateCharacterLocomotion component found!");
                return;
            }

            if (!baseValuesStored)
            {
                StoreBaseValues();
            }
        }

        private void ApplyPassiveAugment(CardInstance card)
        {
            RunnerAugmentType augmentType = card.cardData.augmentType;

            if (!activePassiveAugments.ContainsKey(augmentType))
            {
                activePassiveAugments[augmentType] = new List<CardInstance>();
            }

            activePassiveAugments[augmentType].Add(card);

            switch (augmentType)
            {
                case RunnerAugmentType.JumpHeight:
                    ApplySuperJump();
                    break;
                case RunnerAugmentType.SprintSpeed:
                    ApplySpeedDash();
                    break;
                case RunnerAugmentType.ClimbVaultSpeed:
                    ApplyBuildingClimb();
                    break;
                case RunnerAugmentType.LedgeGrabRange:
                    ApplyLedgeRecovery();
                    break;
                case RunnerAugmentType.DodgeEffectiveness:
                    ApplyDodgeControl();
                    break;
                case RunnerAugmentType.TeleportRange:
                    ApplyTeleportation();
                    break;
                default:
                    if (showDebugLogs)
                    {
                        Debug.LogWarning($"RunnerAugmentController: Unknown passive augment type: {augmentType}");
                    }
                    break;
            }
        }

        private void GrantActiveAbility(CardInstance card)
        {
            RunnerAugmentType augmentType = card.cardData.augmentType;

            if (!activeAbilityCharges.ContainsKey(augmentType))
            {
                activeAbilityCharges[augmentType] = 0;
            }

            int charges = Mathf.RoundToInt(card.GetAugmentValue());
            activeAbilityCharges[augmentType] += charges;

            if (showDebugLogs)
            {
                Debug.Log($"RunnerAugmentController: Granted {charges} charges of {augmentType} (Total: {activeAbilityCharges[augmentType]})");
            }
        }

        private void ApplySuperJump()
        {
            var jumpAbility = locomotion.GetAbility<Jump>();
            if (jumpAbility == null)
            {
                if (showDebugLogs) Debug.LogWarning("RunnerAugmentController: Jump ability not found");
                return;
            }

            if (!baseValues.ContainsKey("Jump.Force"))
            {
                if (showDebugLogs) Debug.LogWarning("RunnerAugmentController: Jump.Force base value not stored");
                return;
            }

            float multiplier = CalculateMultiplicativeBonus(RunnerAugmentType.JumpHeight);
            float baseForce = baseValues["Jump.Force"];
            float newForce = baseForce * multiplier;

            SetAbilityFieldValue(jumpAbility, "m_Force", newForce);

            if (showDebugLogs)
            {
                Debug.Log($"RunnerAugmentController: Jump Force {baseForce:F2} → {newForce:F2} (×{multiplier:F2})");
            }
        }

        private void ApplySpeedDash()
        {
            var speedChangeAbility = locomotion.GetAbility<SpeedChange>();
            if (speedChangeAbility == null)
            {
                if (showDebugLogs) Debug.LogWarning("RunnerAugmentController: SpeedChange ability not found");
                return;
            }

            if (!baseValues.ContainsKey("SpeedChange.Multiplier"))
            {
                if (showDebugLogs) Debug.LogWarning("RunnerAugmentController: SpeedChange.Multiplier base value not stored");
                return;
            }

            float multiplier = CalculateMultiplicativeBonus(RunnerAugmentType.SprintSpeed);
            float baseSpeed = baseValues["SpeedChange.Multiplier"];
            float newSpeed = baseSpeed * multiplier;

            SetAbilityFieldValue(speedChangeAbility, "m_SpeedChangeMultiplier", newSpeed);
            SetAbilityFieldValue(speedChangeAbility, "m_MaxSpeedChangeValue", newSpeed);

            if (showDebugLogs)
            {
                Debug.Log($"RunnerAugmentController: Speed Multiplier & Max {baseSpeed:F2} → {newSpeed:F2} (×{multiplier:F2})");
            }
        }

        private void ApplyBuildingClimb()
        {
            var freeClimbAbility = locomotion.GetAbility<FreeClimb>();
            if (freeClimbAbility == null)
            {
                if (showDebugLogs) Debug.LogWarning("RunnerAugmentController: FreeClimb ability not found");
                return;
            }

            if (!baseValues.ContainsKey("FreeClimb.Speed"))
            {
                if (showDebugLogs) Debug.LogWarning("RunnerAugmentController: FreeClimb.Speed base value not stored");
                return;
            }

            float multiplier = CalculateMultiplicativeBonus(RunnerAugmentType.ClimbVaultSpeed);
            float baseSpeed = baseValues["FreeClimb.Speed"];
            float newSpeed = baseSpeed * multiplier;

            SetAbilityFieldValue(freeClimbAbility, "m_Speed", newSpeed);

            if (showDebugLogs)
            {
                Debug.Log($"RunnerAugmentController: Climb Speed {baseSpeed:F2} → {newSpeed:F2} (×{multiplier:F2})");
            }
        }

        private void ApplyLedgeRecovery()
        {
            var hangAbility = locomotion.GetAbility<Hang>();
            if (hangAbility == null)
            {
                if (showDebugLogs) Debug.LogWarning("RunnerAugmentController: Hang ability not found");
                return;
            }

            if (!baseValues.ContainsKey("Hang.MinDistance"))
            {
                if (showDebugLogs) Debug.LogWarning("RunnerAugmentController: Hang.MinDistance base value not stored");
                return;
            }

            float additiveBonus = CalculateAdditiveBonus(RunnerAugmentType.LedgeGrabRange);
            float baseDistance = baseValues["Hang.MinDistance"];
            float newDistance = baseDistance + additiveBonus;

            SetAbilityFieldValue(hangAbility, "m_MinHangDistance", newDistance);

            if (showDebugLogs)
            {
                Debug.Log($"RunnerAugmentController: Hang Distance {baseDistance:F2} → {newDistance:F2} (+{additiveBonus:F2})");
            }
        }

        private void ApplyDodgeControl()
        {
            var dodgeAbility = locomotion.GetAbility<Dodge>();
            if (dodgeAbility == null)
            {
                if (showDebugLogs) Debug.LogWarning("RunnerAugmentController: Dodge ability not found");
                return;
            }

            if (!baseValues.ContainsKey("Dodge.Speed"))
            {
                if (showDebugLogs) Debug.LogWarning("RunnerAugmentController: Dodge.Speed base value not stored");
                return;
            }

            float multiplier = CalculateMultiplicativeBonus(RunnerAugmentType.DodgeEffectiveness);
            float baseSpeed = baseValues["Dodge.Speed"];
            float newSpeed = baseSpeed * multiplier;

            SetAbilityFieldValue(dodgeAbility, "m_Speed", newSpeed);

            if (showDebugLogs)
            {
                Debug.Log($"RunnerAugmentController: Dodge Speed {baseSpeed:F2} → {newSpeed:F2} (×{multiplier:F2})");
            }
        }

        private void ApplyTeleportation()
        {
            if (showDebugLogs)
            {
                Debug.Log("RunnerAugmentController: Teleportation augment applied (custom implementation required)");
            }
        }

        private float CalculateMultiplicativeBonus(RunnerAugmentType augmentType)
        {
            if (!activePassiveAugments.ContainsKey(augmentType))
            {
                return 1f;
            }

            float totalMultiplier = 1f;

            foreach (CardInstance card in activePassiveAugments[augmentType])
            {
                float cardValue = card.GetAugmentValue();
                totalMultiplier *= cardValue;
            }

            return totalMultiplier;
        }

        private float CalculateAdditiveBonus(RunnerAugmentType augmentType)
        {
            if (!activePassiveAugments.ContainsKey(augmentType))
            {
                return 0f;
            }

            float totalBonus = 0f;

            foreach (CardInstance card in activePassiveAugments[augmentType])
            {
                totalBonus += card.GetAugmentValue();
            }

            return totalBonus;
        }

        public void ClearAllEffects()
        {
            EnsureInitialized();

            activePassiveAugments.Clear();
            activeAbilityCharges.Clear();

            RestoreBaseValues();

            if (showDebugLogs)
            {
                Debug.Log("RunnerAugmentController: All augments cleared and base values restored");
            }
        }

        private void RestoreBaseValues()
        {
            var jumpAbility = locomotion.GetAbility<Jump>();
            if (jumpAbility != null && baseValues.ContainsKey("Jump.Force"))
            {
                SetAbilityFieldValue(jumpAbility, "m_Force", baseValues["Jump.Force"]);
            }

            var speedChangeAbility = locomotion.GetAbility<SpeedChange>();
            if (speedChangeAbility != null && baseValues.ContainsKey("SpeedChange.Multiplier"))
            {
                SetAbilityFieldValue(speedChangeAbility, "m_SpeedChangeMultiplier", baseValues["SpeedChange.Multiplier"]);
                if (baseValues.ContainsKey("SpeedChange.MaxValue"))
                {
                    SetAbilityFieldValue(speedChangeAbility, "m_MaxSpeedChangeValue", baseValues["SpeedChange.MaxValue"]);
                }
            }

            var freeClimbAbility = locomotion.GetAbility<FreeClimb>();
            if (freeClimbAbility != null && baseValues.ContainsKey("FreeClimb.Speed"))
            {
                SetAbilityFieldValue(freeClimbAbility, "m_Speed", baseValues["FreeClimb.Speed"]);
            }

            var hangAbility = locomotion.GetAbility<Hang>();
            if (hangAbility != null && baseValues.ContainsKey("Hang.MinDistance"))
            {
                SetAbilityFieldValue(hangAbility, "m_MinHangDistance", baseValues["Hang.MinDistance"]);
            }

            var dodgeAbility = locomotion.GetAbility<Dodge>();
            if (dodgeAbility != null && baseValues.ContainsKey("Dodge.Speed"))
            {
                SetAbilityFieldValue(dodgeAbility, "m_Speed", baseValues["Dodge.Speed"]);
            }
        }

        private bool IsTacticalAugment(RunnerAugmentType augmentType)
        {
            return augmentType == RunnerAugmentType.BlindSpotDuration ||
                   augmentType == RunnerAugmentType.InvisibilityDuration ||
                   augmentType == RunnerAugmentType.CamouflageLevel;
        }

        private T GetAbilityFieldValue<T>(Ability ability, string fieldName)
        {
            var field = ability.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (field != null)
            {
                return (T)field.GetValue(ability);
            }
            return default(T);
        }

        private void SetAbilityFieldValue<T>(Ability ability, string fieldName, T value)
        {
            var field = ability.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (field != null)
            {
                field.SetValue(ability, value);
            }
        }

        public List<CardInstance> GetActiveAugments()
        {
            List<CardInstance> allAugments = new List<CardInstance>();

            foreach (var kvp in activePassiveAugments)
            {
                allAugments.AddRange(kvp.Value);
            }

            return allAugments;
        }

        public Dictionary<RunnerAugmentType, int> GetActiveAbilities()
        {
            return new Dictionary<RunnerAugmentType, int>(activeAbilityCharges);
        }

        public int GetAbilityCharges(RunnerAugmentType augmentType)
        {
            return activeAbilityCharges.ContainsKey(augmentType) ? activeAbilityCharges[augmentType] : 0;
        }

        public bool ConsumeAbilityCharge(RunnerAugmentType augmentType)
        {
            if (activeAbilityCharges.ContainsKey(augmentType) && activeAbilityCharges[augmentType] > 0)
            {
                activeAbilityCharges[augmentType]--;

                if (showDebugLogs)
                {
                    Debug.Log($"RunnerAugmentController: Consumed 1 charge of {augmentType} (Remaining: {activeAbilityCharges[augmentType]})");
                }

                return true;
            }

            return false;
        }
    }
}

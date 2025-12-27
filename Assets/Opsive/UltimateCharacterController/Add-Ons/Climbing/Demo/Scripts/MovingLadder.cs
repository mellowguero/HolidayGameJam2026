/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.AddOns.Climbing.Demo
{
    using Opsive.Shared.Events;
    using Opsive.Shared.StateSystem;
    using Opsive.UltimateCharacterController.AddOns.Climbing.Objects;
    using Opsive.UltimateCharacterController.Character.Abilities;
    // using Opsive.UltimateCharacterController.Demo; // Demo namespace not imported - comment out if not using demo scenes
    using Opsive.UltimateCharacterController.Objects;
    using UnityEngine;

    /// <summary>
    /// The Moving Ladder component will move the ladder when the character mounts on it. 
    /// </summary>
    public class MovingLadder : MonoBehaviour
    {
        private MovingPlatform m_MovingPlatform;
        private Ladder m_Ladder;
        private GameObject m_Character;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            m_MovingPlatform = GetComponent<MovingPlatform>();
            m_Ladder = GetComponentInChildren<Ladder>();

            /* Demo Manager dependency commented out - if you need this functionality, 
             * either import the Demo samples or manually assign the character reference
            #if UNITY_2023_1_OR_NEWER
                        var demoManager = Object.FindFirstObjectByType<DemoManager>();
            #else
                        var demoManager = Object.FindObjectOfType<DemoManager>();
            #endif
                        if (demoManager.Character == null) {
                            Debug.LogError("Error: Unable to find the character.");
                            return;
                        }

                        m_Character = demoManager.Character;
            */

            // Find character by tag as an alternative to DemoManager
            m_Character = GameObject.FindGameObjectWithTag("Player");
            if (m_Character == null) {
                Debug.LogWarning("MovingLadder: Unable to find character with 'Player' tag. This component will not function.");
                return;
            }

            EventHandler.RegisterEvent<Ability, bool>(m_Character, "OnCharacterAbilityActive", OnAbilityActive);
        }

        /// <summary>
        /// The character's ability has been started or stopped.
        /// </summary>
        /// <param name="ability">The ability which was started or stopped.</param>
        /// <param name="active">True if the ability was started, false if it was stopped.</param>
        private void OnAbilityActive(Ability ability, bool active)
        {
            if (!(ability is LadderClimb)) {
                return;
            }

            var ladderClimb = ability as LadderClimb;
            if (ladderClimb.DetectedObject != m_Ladder.gameObject) {
                return;
            }

            m_MovingPlatform.TargetWaypoint = active ? 1 : 0;
            m_Ladder.BottomDismountOffset += active ? 0.05f : -0.05f;
            StateManager.SetState(m_Character, "MovingLadder", active);
        }

        /// <summary>
        /// The object has been destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (m_Character == null) {
                return;
            }

            EventHandler.UnregisterEvent<Ability, bool>(m_Character, "OnCharacterAbilityActive", OnAbilityActive);
        }
    }
}

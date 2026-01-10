using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;

namespace HolidayGJ.Cards
{
    public class AugmentTester : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RunnerAugmentController augmentController;

        [Header("Test Cards")]
        [SerializeField] private CardData testCard1;
        [SerializeField] private CardData testCard2;
        [SerializeField] private CardData testCard3;

        [Header("Test Levels (1-3)")]
        [Range(1, 3)]
        [SerializeField] private int testCard1Level = 1;
        [Range(1, 3)]
        [SerializeField] private int testCard2Level = 1;
        [Range(1, 3)]
        [SerializeField] private int testCard3Level = 1;

        [Header("Display")]
        [SerializeField] private bool showActiveAugments = true;

        [Header("Speed Test")]
        [SerializeField] private bool forceSpeedActive = false;

        private UltimateCharacterLocomotion locomotion;
        private SpeedChange speedChangeAbility;

        private void Start()
        {
            if (augmentController == null)
            {
                augmentController = FindFirstObjectByType<RunnerAugmentController>();
            }

            if (augmentController == null)
            {
                Debug.LogWarning("AugmentTester: No RunnerAugmentController found in scene");
                return;
            }

            locomotion = augmentController.GetComponent<UltimateCharacterLocomotion>();
            if (locomotion != null)
            {
                speedChangeAbility = locomotion.GetAbility<SpeedChange>();
            }
        }

        private void Update()
        {
            if (forceSpeedActive && speedChangeAbility != null && locomotion != null)
            {
                locomotion.InputVector = new UnityEngine.Vector2(0, 1f);
                
                if (!speedChangeAbility.IsActive)
                {
                    bool started = locomotion.TryStartAbility(speedChangeAbility);
                    if (!started)
                    {
                        Debug.LogWarning($"AugmentTester: Failed to start SpeedChange. Enabled: {speedChangeAbility.Enabled}, Grounded: {locomotion.Grounded}");
                    }
                    else
                    {
                        var multiplierField = typeof(SpeedChange).GetField("m_SpeedChangeMultiplier", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        float currentMultiplier = (float)multiplierField.GetValue(speedChangeAbility);
                        Debug.Log($"AugmentTester: SpeedChange ACTIVATED! Multiplier = {currentMultiplier}");
                    }
                }
            }
            else if (!forceSpeedActive && locomotion != null)
            {
                locomotion.InputVector = UnityEngine.Vector2.zero;
            }
        }

        [ContextMenu("Apply Test Cards")]
        public void ApplyTestCards()
        {
            if (augmentController == null)
            {
                Debug.LogError("AugmentTester: No RunnerAugmentController assigned");
                return;
            }

            int cardsApplied = 0;

            if (testCard1 != null)
            {
                CardInstance card1 = CreateCardInstance(testCard1, testCard1Level);
                augmentController.ApplyCard(card1);
                cardsApplied++;
            }

            if (testCard2 != null)
            {
                CardInstance card2 = CreateCardInstance(testCard2, testCard2Level);
                augmentController.ApplyCard(card2);
                cardsApplied++;
            }

            if (testCard3 != null)
            {
                CardInstance card3 = CreateCardInstance(testCard3, testCard3Level);
                augmentController.ApplyCard(card3);
                cardsApplied++;
            }

            Debug.Log($"AugmentTester: Applied {cardsApplied} test cards");

            if (showActiveAugments)
            {
                PrintActiveAugments();
            }
        }

        [ContextMenu("Clear All Effects")]
        public void ClearAllEffects()
        {
            if (augmentController == null)
            {
                Debug.LogError("AugmentTester: No RunnerAugmentController assigned");
                return;
            }

            augmentController.ClearAllEffects();
            Debug.Log("AugmentTester: Cleared all augments");
        }

        [ContextMenu("Toggle Force Speed Active")]
        public void ToggleForceSpeed()
        {
            forceSpeedActive = !forceSpeedActive;
            
            if (forceSpeedActive)
            {
                Debug.Log("AugmentTester: Speed boost FORCED ON - simulating forward movement");
            }
            else
            {
                Debug.Log("AugmentTester: Speed boost force OFF");
                if (speedChangeAbility != null && speedChangeAbility.IsActive)
                {
                    locomotion.TryStopAbility(speedChangeAbility);
                }
                if (locomotion != null)
                {
                    locomotion.InputVector = UnityEngine.Vector2.zero;
                }
            }
        }

        [ContextMenu("Test Speed Boost (3 seconds)")]
        public void TestSpeedBoost()
        {
            if (speedChangeAbility == null)
            {
                Debug.LogError("AugmentTester: SpeedChange ability not found");
                return;
            }

            var multiplierField = typeof(SpeedChange).GetField("m_SpeedChangeMultiplier", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            float currentMultiplier = (float)multiplierField.GetValue(speedChangeAbility);
            Debug.Log($"AugmentTester: Testing speed boost. Current multiplier = {currentMultiplier}");

            StartCoroutine(TestSpeedCoroutine());
        }

        private System.Collections.IEnumerator TestSpeedCoroutine()
        {
            Debug.Log("AugmentTester: Activating speed boost for 3 seconds...");
            forceSpeedActive = true;
            
            yield return new WaitForSeconds(3f);
            
            forceSpeedActive = false;
            if (speedChangeAbility != null && speedChangeAbility.IsActive)
            {
                locomotion.TryStopAbility(speedChangeAbility);
            }
            Debug.Log("AugmentTester: Speed boost test complete");
        }

        [ContextMenu("Print Active Augments")]
        public void PrintActiveAugments()
        {
            if (augmentController == null)
            {
                Debug.LogError("AugmentTester: No RunnerAugmentController assigned");
                return;
            }

            var activeAugments = augmentController.GetActiveAugments();
            var activeAbilities = augmentController.GetActiveAbilities();

            Debug.Log("=== ACTIVE AUGMENTS ===");
            Debug.Log($"Total Passive Augments: {activeAugments.Count}");

            foreach (CardInstance card in activeAugments)
            {
                Debug.Log($"- {card.cardData.cardName} (Level {card.currentLevel}) [{card.cardData.augmentType}] = {card.GetAugmentValue()}");
            }

            Debug.Log($"\nTotal Active Abilities: {activeAbilities.Count}");

            foreach (var kvp in activeAbilities)
            {
                Debug.Log($"- {kvp.Key}: {kvp.Value} charges");
            }
        }

        private CardInstance CreateCardInstance(CardData cardData, int level)
        {
            CardInstance instance = new CardInstance(cardData);

            for (int i = 1; i < level; i++)
            {
                instance.LevelUp();
            }

            return instance;
        }

        private void OnGUI()
        {
            if (!showActiveAugments || augmentController == null) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 450));
            GUILayout.Box("=== AUGMENT TESTER ===");

            if (forceSpeedActive)
            {
                GUI.color = Color.yellow;
                GUILayout.Label("⚡ SPEED BOOST FORCED ON ⚡");
                GUI.color = Color.white;
            }

            GUILayout.Label("Passive Augments:");
            var augments = augmentController.GetActiveAugments();
            if (augments.Count == 0)
            {
                GUILayout.Label("  None");
            }
            else
            {
                foreach (CardInstance card in augments)
                {
                    GUILayout.Label($"  {card.cardData.cardName} ★ Lvl {card.currentLevel}");
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("Active Abilities:");
            var abilities = augmentController.GetActiveAbilities();
            if (abilities.Count == 0)
            {
                GUILayout.Label("  None");
            }
            else
            {
                foreach (var kvp in abilities)
                {
                    GUILayout.Label($"  {kvp.Key}: {kvp.Value} charges");
                }
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Toggle Force Speed"))
            {
                ToggleForceSpeed();
            }
            if (GUILayout.Button("Test Speed (3s)"))
            {
                TestSpeedBoost();
            }

            GUILayout.EndArea();
        }
    }
}

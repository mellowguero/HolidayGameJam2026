using UnityEngine;

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

        private void Start()
        {
            if (augmentController == null)
            {
                augmentController = FindFirstObjectByType<RunnerAugmentController>();
            }

            if (augmentController == null)
            {
                Debug.LogWarning("AugmentTester: No RunnerAugmentController found in scene");
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

            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
            GUILayout.Box("=== AUGMENT TESTER ===");

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

            GUILayout.EndArea();
        }
    }
}

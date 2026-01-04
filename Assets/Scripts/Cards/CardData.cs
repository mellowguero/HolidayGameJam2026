using UnityEngine;

namespace HolidayGJ.Cards
{
    public enum CardCategory
    {
        Movement,
        Environmental,
        Tactical
    }

    public enum CardRarity
    {
        Common,
        Uncommon,
        Rare
    }

    public enum RunnerAugmentType
    {
        JumpHeight,
        TeleportRange,
        SprintSpeed,
        ClimbVaultSpeed,
        LedgeGrabRange,
        DodgeEffectiveness,
        BlindSpotDuration,
        InvisibilityDuration,
        CamouflageLevel,
        Custom
    }

    [CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Card Identity")]
        public string cardName;
        public Sprite cardIcon;
        public Color cardColor = Color.white;
        public CardCategory category;
        public CardRarity rarity;

        [Header("Runner Augment (Passive)")]
        [TextArea(2, 3)]
        public string runnerDescription;
        public RunnerAugmentType augmentType;
        public float[] augmentValuesPerLevel = new float[3];
        public GameObject runnerVFXPrefab;

        [Header("Watcher Ability (Active)")]
        [TextArea(2, 3)]
        public string watcherDescription;
        public GameObject watcherAbilityPrefab;
        public float[] cooldownPerLevel = new float[3];
        public int[] maxSimultaneousUsesPerLevel = new int[3];

        [Header("Leveling")]
        public int maxLevel = 3;

        public float cooldown => cooldownPerLevel != null && cooldownPerLevel.Length > 0 ? cooldownPerLevel[0] : 0f;
        public int maxUses => maxSimultaneousUsesPerLevel != null && maxSimultaneousUsesPerLevel.Length > 0 ? maxSimultaneousUsesPerLevel[0] : 0;
    }
}

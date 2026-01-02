using UnityEngine;

namespace HolidayGJ.Cards
{
    public enum CardCategory
    {
        Environment,
        Enemy,
        Modifier
    }

    public enum CardRarity
    {
        Common,
        Uncommon,
        Rare
    }

    [CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Card Identity")]
        public string cardName;
        [TextArea(3, 5)]
        public string description;
        public Sprite cardIcon;
        public Color cardColor = Color.white;

        [Header("Card Classification")]
        public CardCategory category;
        public CardRarity rarity;

        [Header("Ability Settings")]
        public GameObject abilityPrefab;

        [Header("Usage Settings")]
        [Tooltip("Seconds before card can be used again. 0 = no cooldown")]
        public float cooldown = 0f;
        
        [Tooltip("Maximum uses. 0 = infinite uses")]
        public int maxUses = 0;
    }
}

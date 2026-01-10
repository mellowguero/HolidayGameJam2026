using UnityEngine;

namespace HolidayGJ.Cards
{
    [System.Serializable]
    public class CardInstance
    {
        public CardData cardData;
        public int currentLevel = 1;
        public int remainingUses;
        public float cooldownRemaining;

        public CardInstance(CardData data, int level = 1)
        {
            cardData = data;
            currentLevel = Mathf.Clamp(level, 1, 3);
            remainingUses = data.maxUses;
            cooldownRemaining = 0f;
        }

        public float GetAugmentValue()
        {
            if (cardData == null || cardData.augmentValuesPerLevel == null) return 0f;
            
            int index = Mathf.Clamp(currentLevel - 1, 0, cardData.augmentValuesPerLevel.Length - 1);
            return cardData.augmentValuesPerLevel[index];
        }

        public float GetWatcherCooldown()
        {
            if (cardData == null || cardData.watcherCooldownPerLevel == null) return 0f;
            
            int index = Mathf.Clamp(currentLevel - 1, 0, cardData.watcherCooldownPerLevel.Length - 1);
            return cardData.watcherCooldownPerLevel[index];
        }

        public int GetWatcherUses()
        {
            if (cardData == null || cardData.watcherUsesPerLevel == null) return 0;
            
            int index = Mathf.Clamp(currentLevel - 1, 0, cardData.watcherUsesPerLevel.Length - 1);
            return cardData.watcherUsesPerLevel[index];
        }

        public void LevelUp()
        {
            if (currentLevel < 3)
            {
                currentLevel++;
            }
        }

        public bool CanUse()
        {
            if (cardData.maxUses > 0 && remainingUses <= 0) return false;
            if (cooldownRemaining > 0f) return false;
            return true;
        }

        public void Use()
        {
            if (cardData.maxUses > 0)
            {
                remainingUses--;
            }
            cooldownRemaining = cardData.cooldown;
        }

        public void UpdateCooldown(float deltaTime)
        {
            if (cooldownRemaining > 0f)
            {
                cooldownRemaining -= deltaTime;
                if (cooldownRemaining < 0f) cooldownRemaining = 0f;
            }
        }
    }
}

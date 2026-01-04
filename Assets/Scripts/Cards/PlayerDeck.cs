using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HolidayGJ.Cards
{
    [Serializable]
    public class CardInstance
    {
        public CardData cardData;
        public int currentLevel;
        
        public CardInstance(CardData data)
        {
            cardData = data;
            currentLevel = 1;
        }
        
        public void LevelUp()
        {
            if (currentLevel < cardData.maxLevel)
            {
                currentLevel++;
                Debug.Log($"Card {cardData.cardName} leveled up to {currentLevel}");
            }
        }
        
        public float GetAugmentValue()
        {
            if (cardData.augmentValuesPerLevel != null && currentLevel > 0 && currentLevel <= cardData.augmentValuesPerLevel.Length)
            {
                return cardData.augmentValuesPerLevel[currentLevel - 1];
            }
            return 0f;
        }
        
        public float GetCooldown()
        {
            if (cardData.cooldownPerLevel != null && currentLevel > 0 && currentLevel <= cardData.cooldownPerLevel.Length)
            {
                return cardData.cooldownPerLevel[currentLevel - 1];
            }
            return 0f;
        }
        
        public int GetMaxUses()
        {
            if (cardData.maxSimultaneousUsesPerLevel != null && currentLevel > 0 && currentLevel <= cardData.maxSimultaneousUsesPerLevel.Length)
            {
                return cardData.maxSimultaneousUsesPerLevel[currentLevel - 1];
            }
            return 0;
        }
    }
    
    public class PlayerDeck : MonoBehaviour
    {
        [SerializeField] private List<CardInstance> cards = new List<CardInstance>();
        
        public event Action<CardInstance> OnCardAdded;
        public event Action<CardInstance> OnCardLeveledUp;
        
        public IReadOnlyList<CardInstance> Cards => cards;
        
        public void AddCard(CardData cardData)
        {
            if (cardData == null)
            {
                Debug.LogWarning("Attempted to add null card to player deck");
                return;
            }
            
            CardInstance existingCard = GetCard(cardData);
            
            if (existingCard != null)
            {
                existingCard.LevelUp();
                OnCardLeveledUp?.Invoke(existingCard);
                Debug.Log($"Card {cardData.cardName} already in deck - leveled up to {existingCard.currentLevel}");
            }
            else
            {
                CardInstance newCard = new CardInstance(cardData);
                cards.Add(newCard);
                OnCardAdded?.Invoke(newCard);
                Debug.Log($"New card added to deck: {cardData.cardName} at Level 1");
            }
        }
        
        public CardInstance[] DrawCards(int count)
        {
            if (cards.Count == 0)
            {
                Debug.LogWarning("Cannot draw cards from empty deck");
                return new CardInstance[0];
            }
            
            int actualCount = Mathf.Min(count, cards.Count);
            
            List<CardInstance> shuffledDeck = new List<CardInstance>(cards);
            
            for (int i = 0; i < shuffledDeck.Count; i++)
            {
                CardInstance temp = shuffledDeck[i];
                int randomIndex = UnityEngine.Random.Range(i, shuffledDeck.Count);
                shuffledDeck[i] = shuffledDeck[randomIndex];
                shuffledDeck[randomIndex] = temp;
            }
            
            CardInstance[] drawnCards = shuffledDeck.Take(actualCount).ToArray();
            
            Debug.Log($"Drew {drawnCards.Length} cards from deck");
            return drawnCards;
        }
        
        public CardInstance GetCard(CardData cardData)
        {
            return cards.FirstOrDefault(c => c.cardData == cardData);
        }
        
        public bool HasCard(CardData cardData)
        {
            return GetCard(cardData) != null;
        }
        
        public int GetCardLevel(CardData cardData)
        {
            CardInstance card = GetCard(cardData);
            return card?.currentLevel ?? 0;
        }
        
        public void InitializeWithDefaultCards(CardData[] defaultCards)
        {
            cards.Clear();
            
            foreach (CardData cardData in defaultCards)
            {
                if (cardData != null)
                {
                    CardInstance newCard = new CardInstance(cardData);
                    cards.Add(newCard);
                }
            }
            
            Debug.Log($"Deck initialized with {cards.Count} default cards");
        }
        
        public void ClearDeck()
        {
            cards.Clear();
            Debug.Log("Deck cleared");
        }
        
        [ContextMenu("Debug: Print Deck State")]
        private void DebugPrintDeckState()
        {
            Debug.Log("=== PLAYER DECK STATE ===");
            Debug.Log($"Total cards: {cards.Count}");
            
            foreach (CardInstance card in cards)
            {
                Debug.Log($"- {card.cardData.cardName}: Level {card.currentLevel}");
            }
        }
    }
}

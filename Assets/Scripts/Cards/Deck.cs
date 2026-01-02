using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HolidayGJ.Cards
{
    public class Deck : MonoBehaviour
    {
        [SerializeField] private List<CardData> collectedCards = new List<CardData>();
        
        private Dictionary<CardData, float> cardCooldowns = new Dictionary<CardData, float>();
        private Dictionary<CardData, int> cardUseCounts = new Dictionary<CardData, int>();
        
        public event Action<CardData> OnCardAdded;
        public event Action<CardData> OnCardUsed;
        
        public IReadOnlyList<CardData> CollectedCards => collectedCards;
        
        private void Update()
        {
            UpdateCooldowns();
        }
        
        public void AddCard(CardData card)
        {
            if (card == null)
            {
                Debug.LogWarning("Attempted to add null card to deck");
                return;
            }
            
            collectedCards.Add(card);
            
            if (!cardUseCounts.ContainsKey(card))
            {
                cardUseCounts[card] = card.maxUses;
            }
            
            OnCardAdded?.Invoke(card);
            Debug.Log($"Card added to deck: {card.cardName}");
        }
        
        public CardData[] GetAvailableCards()
        {
            return collectedCards
                .Where(card => CanUseCard(card))
                .Distinct()
                .ToArray();
        }
        
        public CardData[] GetAllCards()
        {
            return collectedCards.Distinct().ToArray();
        }
        
        public bool CanUseCard(CardData card)
        {
            if (card == null || !collectedCards.Contains(card))
            {
                return false;
            }
            
            if (cardCooldowns.ContainsKey(card) && cardCooldowns[card] > 0f)
            {
                return false;
            }
            
            if (card.maxUses > 0)
            {
                if (!cardUseCounts.ContainsKey(card) || cardUseCounts[card] <= 0)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public void UseCard(CardData card)
        {
            if (!CanUseCard(card))
            {
                Debug.LogWarning($"Cannot use card: {card?.cardName ?? "null"}");
                return;
            }
            
            cardCooldowns[card] = card.cooldown;
            
            if (card.maxUses > 0 && cardUseCounts.ContainsKey(card))
            {
                cardUseCounts[card]--;
                
                if (cardUseCounts[card] <= 0)
                {
                    Debug.Log($"Card {card.cardName} has no uses remaining");
                }
            }
            
            OnCardUsed?.Invoke(card);
            Debug.Log($"Card used: {card.cardName}");
        }
        
        public float GetCardCooldown(CardData card)
        {
            if (cardCooldowns.ContainsKey(card))
            {
                return cardCooldowns[card];
            }
            return 0f;
        }
        
        public int GetCardRemainingUses(CardData card)
        {
            if (card.maxUses == 0)
            {
                return -1;
            }
            
            if (cardUseCounts.ContainsKey(card))
            {
                return cardUseCounts[card];
            }
            
            return card.maxUses;
        }
        
        public void ResetDeck()
        {
            collectedCards.Clear();
            cardCooldowns.Clear();
            cardUseCounts.Clear();
            Debug.Log("Deck reset");
        }
        
        public void ResetCooldowns()
        {
            cardCooldowns.Clear();
            
            foreach (var card in collectedCards.Distinct())
            {
                if (card.maxUses > 0)
                {
                    cardUseCounts[card] = card.maxUses;
                }
            }
            
            Debug.Log("Deck cooldowns and uses reset");
        }
        
        private void UpdateCooldowns()
        {
            List<CardData> cardsToUpdate = new List<CardData>(cardCooldowns.Keys);
            
            foreach (CardData card in cardsToUpdate)
            {
                if (cardCooldowns[card] > 0f)
                {
                    cardCooldowns[card] -= Time.deltaTime;
                    
                    if (cardCooldowns[card] <= 0f)
                    {
                        cardCooldowns[card] = 0f;
                    }
                }
            }
        }
        
        [ContextMenu("Debug: Print Deck State")]
        private void DebugPrintDeckState()
        {
            Debug.Log("=== DECK STATE ===");
            Debug.Log($"Total cards collected: {collectedCards.Count}");
            Debug.Log($"Unique cards: {collectedCards.Distinct().Count()}");
            
            foreach (var card in collectedCards.Distinct())
            {
                float cooldown = GetCardCooldown(card);
                int uses = GetCardRemainingUses(card);
                bool canUse = CanUseCard(card);
                
                Debug.Log($"- {card.cardName}: Cooldown={cooldown:F1}s, Uses={uses}, CanUse={canUse}");
            }
        }
        
        [ContextMenu("Debug: Clear Deck")]
        private void DebugClearDeck()
        {
            ResetDeck();
        }
    }
}

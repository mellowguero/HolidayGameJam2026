using System.Collections.Generic;
using UnityEngine;
using HolidayGJ.Cards;

namespace HolidayGameJam.UI
{
    public class WatcherDeckUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CardManager cardManager;
        [SerializeField] private Transform cardSlotContainer;
        [SerializeField] private GameObject cardSlotPrefab;
        
        [Header("Settings")]
        [SerializeField] private int maxCardSlots = 10;
        [SerializeField] private bool autoFindCardManager = true;
        
        private List<CardSlotUI> cardSlots = new List<CardSlotUI>();
        private Dictionary<CardData, CardSlotUI> cardToSlotMap = new Dictionary<CardData, CardSlotUI>();
        
        private void Start()
        {
            if (autoFindCardManager && cardManager == null)
            {
                cardManager = CardManager.Instance;
            }
            
            if (cardManager == null)
            {
                Debug.LogError("WatcherDeckUI: CardManager not found!");
                return;
            }
            
            if (cardSlotContainer == null)
            {
                Debug.LogError("WatcherDeckUI: Card slot container not assigned!");
                return;
            }
            
            if (cardSlotPrefab == null)
            {
                Debug.LogError("WatcherDeckUI: Card slot prefab not assigned!");
                return;
            }
            
            InitializeCardSlots();
            
            if (cardManager.RunnerDeck != null)
            {
                cardManager.RunnerDeck.OnCardAdded += OnCardAdded;
                cardManager.RunnerDeck.OnCardUsed += OnCardUsed;
            }
            
            RefreshAllCards();
        }
        
        private void OnDestroy()
        {
            if (cardManager != null && cardManager.RunnerDeck != null)
            {
                cardManager.RunnerDeck.OnCardAdded -= OnCardAdded;
                cardManager.RunnerDeck.OnCardUsed -= OnCardUsed;
            }
        }
        
        private void Update()
        {
            UpdateCardStates();
        }
        
        private void InitializeCardSlots()
        {
            foreach (Transform child in cardSlotContainer)
            {
                Destroy(child.gameObject);
            }
            
            cardSlots.Clear();
            
            for (int i = 0; i < maxCardSlots; i++)
            {
                GameObject slotObject = Instantiate(cardSlotPrefab, cardSlotContainer);
                CardSlotUI slot = slotObject.GetComponent<CardSlotUI>();
                
                if (slot != null)
                {
                    cardSlots.Add(slot);
                    slot.Clear();
                }
                else
                {
                    Debug.LogError("WatcherDeckUI: Card slot prefab doesn't have CardSlotUI component!");
                }
            }
        }
        
        private void RefreshAllCards()
        {
            if (cardManager == null || cardManager.RunnerDeck == null)
            {
                return;
            }
            
            cardToSlotMap.Clear();
            
            CardData[] uniqueCards = cardManager.RunnerDeck.GetAllCards();
            
            Debug.Log($"[WatcherDeckUI] Refreshing UI with {uniqueCards.Length} unique cards");
            for (int i = 0; i < uniqueCards.Length; i++)
            {
                Debug.Log($"[WatcherDeckUI] Card {i}: {uniqueCards[i].cardName}");
            }
            
            for (int i = 0; i < cardSlots.Count; i++)
            {
                if (i < uniqueCards.Length)
                {
                    cardSlots[i].Setup(uniqueCards[i]);
                    cardToSlotMap[uniqueCards[i]] = cardSlots[i];
                    Debug.Log($"[WatcherDeckUI] Slot {i} setup with card: {uniqueCards[i].cardName}");
                }
                else
                {
                    cardSlots[i].Clear();
                }
            }
            
            UpdateCardStates();
        }
        
        private void UpdateCardStates()
        {
            if (cardManager == null || cardManager.RunnerDeck == null)
            {
                return;
            }
            
            foreach (var kvp in cardToSlotMap)
            {
                CardData card = kvp.Key;
                CardSlotUI slot = kvp.Value;
                
                if (card != null && slot != null)
                {
                    bool canUse = cardManager.RunnerDeck.CanUseCard(card);
                    float cooldown = cardManager.RunnerDeck.GetCardCooldown(card);
                    int remainingUses = cardManager.RunnerDeck.GetCardRemainingUses(card);
                    
                    slot.UpdateState(canUse, cooldown, remainingUses);
                }
            }
        }
        
        private void OnCardAdded(CardData card)
        {
            RefreshAllCards();
        }
        
        private void OnCardUsed(CardData card)
        {
            UpdateCardStates();
        }
        
        [ContextMenu("Debug: Refresh UI")]
        public void DebugRefreshUI()
        {
            RefreshAllCards();
        }
        
        [ContextMenu("Debug: Print Slot Count")]
        public void DebugPrintSlotCount()
        {
            Debug.Log($"Card slots created: {cardSlots.Count}");
            Debug.Log($"Cards displayed: {cardToSlotMap.Count}");
        }
    }
}

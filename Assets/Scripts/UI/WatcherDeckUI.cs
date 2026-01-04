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
        [SerializeField] private int displayPlayerID = 1;
        
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
            SubscribeToDeckEvents();
            RefreshAllCards();
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromDeckEvents();
        }
        
        private void SubscribeToDeckEvents()
        {
            PlayerDeck deck = cardManager.GetPlayerDeck(displayPlayerID);
            if (deck != null)
            {
                deck.OnCardAdded += OnCardAdded;
                deck.OnCardLeveledUp += OnCardLeveledUp;
            }
        }
        
        private void UnsubscribeFromDeckEvents()
        {
            if (cardManager != null)
            {
                PlayerDeck deck = cardManager.GetPlayerDeck(displayPlayerID);
                if (deck != null)
                {
                    deck.OnCardAdded -= OnCardAdded;
                    deck.OnCardLeveledUp -= OnCardLeveledUp;
                }
            }
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
            if (cardManager == null)
            {
                return;
            }
            
            PlayerDeck deck = cardManager.GetPlayerDeck(displayPlayerID);
            if (deck == null)
            {
                return;
            }
            
            cardToSlotMap.Clear();
            
            var cardInstances = deck.Cards;
            
            Debug.Log($"[WatcherDeckUI] Refreshing UI with {cardInstances.Count} cards for Player {displayPlayerID}");
            
            for (int i = 0; i < cardSlots.Count; i++)
            {
                if (i < cardInstances.Count)
                {
                    CardInstance cardInstance = cardInstances[i];
                    cardSlots[i].Setup(cardInstance.cardData);
                    cardToSlotMap[cardInstance.cardData] = cardSlots[i];
                    
                    cardSlots[i].UpdateState(true, 0f, cardInstance.currentLevel);
                    
                    Debug.Log($"[WatcherDeckUI] Slot {i}: {cardInstance.cardData.cardName} (Lvl {cardInstance.currentLevel})");
                }
                else
                {
                    cardSlots[i].Clear();
                }
            }
        }
        
        private void OnCardAdded(CardInstance card)
        {
            RefreshAllCards();
        }
        
        private void OnCardLeveledUp(CardInstance card)
        {
            RefreshAllCards();
        }
        
        public void SetDisplayPlayer(int playerID)
        {
            UnsubscribeFromDeckEvents();
            displayPlayerID = playerID;
            SubscribeToDeckEvents();
            RefreshAllCards();
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

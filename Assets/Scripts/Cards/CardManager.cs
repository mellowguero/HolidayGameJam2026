using System;
using UnityEngine;

namespace HolidayGJ.Cards
{
    public class CardManager : MonoBehaviour
    {
        private static CardManager instance;
        
        public static CardManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("CardManager");
                    instance = managerObject.AddComponent<CardManager>();
                    DontDestroyOnLoad(managerObject);
                }
                return instance;
            }
        }
        
        [Header("Player Decks")]
        [SerializeField] private PlayerDeck player1Deck;
        [SerializeField] private PlayerDeck player2Deck;
        
        [Header("Default Cards")]
        [SerializeField] private CardData[] defaultStartingCards;
        
        [Header("Role Tracking")]
        public int currentRunnerPlayerID = 1;
        public int currentWatcherPlayerID = 2;
        
        public PlayerDeck Player1Deck => player1Deck;
        public PlayerDeck Player2Deck => player2Deck;
        
        public event Action<int, CardInstance> OnCardCollected;
        public event Action<int, CardInstance> OnCardLeveledUp;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            Initialize();
        }
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                UnsubscribeFromDeckEvents();
            }
        }
        
        public void Initialize()
        {
            if (player1Deck == null)
            {
                GameObject player1DeckObj = new GameObject("Player1Deck");
                player1DeckObj.transform.SetParent(transform);
                player1Deck = player1DeckObj.AddComponent<PlayerDeck>();
            }
            
            if (player2Deck == null)
            {
                GameObject player2DeckObj = new GameObject("Player2Deck");
                player2DeckObj.transform.SetParent(transform);
                player2Deck = player2DeckObj.AddComponent<PlayerDeck>();
            }
            
            SubscribeToDeckEvents();
            InitializeDecks();
            
            Debug.Log("CardManager initialized with two player decks");
        }
        
        public void InitializeDecks()
        {
            if (defaultStartingCards != null && defaultStartingCards.Length > 0)
            {
                player1Deck.InitializeWithDefaultCards(defaultStartingCards);
                player2Deck.InitializeWithDefaultCards(defaultStartingCards);
                Debug.Log($"Both decks initialized with {defaultStartingCards.Length} default cards");
            }
            else
            {
                Debug.LogWarning("No default starting cards assigned to CardManager");
            }
        }
        
        public PlayerDeck GetPlayerDeck(int playerID)
        {
            if (playerID == 1)
            {
                return player1Deck;
            }
            else if (playerID == 2)
            {
                return player2Deck;
            }
            
            Debug.LogWarning($"Invalid player ID: {playerID}. Must be 1 or 2");
            return null;
        }
        
        public void AddCardToPlayer(int playerID, CardData card)
        {
            PlayerDeck deck = GetPlayerDeck(playerID);
            
            if (deck != null && card != null)
            {
                deck.AddCard(card);
            }
        }
        
        public void SwapRoles()
        {
            int temp = currentRunnerPlayerID;
            currentRunnerPlayerID = currentWatcherPlayerID;
            currentWatcherPlayerID = temp;
            
            Debug.Log($"Roles swapped - Runner: Player {currentRunnerPlayerID}, Watcher: Player {currentWatcherPlayerID}");
        }
        
        public PlayerDeck GetCurrentRunnerDeck()
        {
            return GetPlayerDeck(currentRunnerPlayerID);
        }
        
        public PlayerDeck GetCurrentWatcherDeck()
        {
            return GetPlayerDeck(currentWatcherPlayerID);
        }
        
        public void ResetForNewMatch()
        {
            if (player1Deck != null)
            {
                player1Deck.ClearDeck();
            }
            
            if (player2Deck != null)
            {
                player2Deck.ClearDeck();
            }
            
            currentRunnerPlayerID = 1;
            currentWatcherPlayerID = 2;
            
            InitializeDecks();
            
            Debug.Log("CardManager reset for new match");
        }
        
        private void SubscribeToDeckEvents()
        {
            if (player1Deck != null)
            {
                player1Deck.OnCardAdded += (card) => OnCardCollected?.Invoke(1, card);
                player1Deck.OnCardLeveledUp += (card) => OnCardLeveledUp?.Invoke(1, card);
            }
            
            if (player2Deck != null)
            {
                player2Deck.OnCardAdded += (card) => OnCardCollected?.Invoke(2, card);
                player2Deck.OnCardLeveledUp += (card) => OnCardLeveledUp?.Invoke(2, card);
            }
        }
        
        private void UnsubscribeFromDeckEvents()
        {
            if (player1Deck != null)
            {
                player1Deck.OnCardAdded -= (card) => OnCardCollected?.Invoke(1, card);
                player1Deck.OnCardLeveledUp -= (card) => OnCardLeveledUp?.Invoke(1, card);
            }
            
            if (player2Deck != null)
            {
                player2Deck.OnCardAdded -= (card) => OnCardCollected?.Invoke(2, card);
                player2Deck.OnCardLeveledUp -= (card) => OnCardLeveledUp?.Invoke(2, card);
            }
        }
        
        [ContextMenu("Debug: Print Manager State")]
        private void DebugPrintManagerState()
        {
            Debug.Log("=== CARD MANAGER STATE ===");
            Debug.Log($"Instance exists: {instance != null}");
            Debug.Log($"Player 1 Deck exists: {player1Deck != null}");
            Debug.Log($"Player 2 Deck exists: {player2Deck != null}");
            Debug.Log($"Current Runner: Player {currentRunnerPlayerID}");
            Debug.Log($"Current Watcher: Player {currentWatcherPlayerID}");
            
            if (player1Deck != null)
            {
                Debug.Log($"Player 1 cards: {player1Deck.Cards.Count}");
            }
            
            if (player2Deck != null)
            {
                Debug.Log($"Player 2 cards: {player2Deck.Cards.Count}");
            }
        }
        
        [ContextMenu("Debug: Swap Roles")]
        private void DebugSwapRoles()
        {
            SwapRoles();
        }
    }
}

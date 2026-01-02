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
        
        [SerializeField] private Deck runnerDeck;
        
        public Deck RunnerDeck => runnerDeck;
        
        public event Action<CardData> OnCardCollected;
        public event Action<CardData> OnCardUsed;
        
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
                if (runnerDeck != null)
                {
                    runnerDeck.OnCardAdded -= NotifyCardCollected;
                    runnerDeck.OnCardUsed -= NotifyCardUsed;
                }
            }
        }
        
        public void Initialize()
        {
            if (runnerDeck == null)
            {
                runnerDeck = gameObject.AddComponent<Deck>();
            }
            
            runnerDeck.OnCardAdded += NotifyCardCollected;
            runnerDeck.OnCardUsed += NotifyCardUsed;
            
            Debug.Log("CardManager initialized");
        }
        
        public void ResetForNewMatch()
        {
            if (runnerDeck != null)
            {
                runnerDeck.ResetDeck();
            }
            
            Debug.Log("CardManager reset for new match");
        }
        
        public void NotifyCardCollected(CardData card)
        {
            OnCardCollected?.Invoke(card);
        }
        
        public void NotifyCardUsed(CardData card)
        {
            OnCardUsed?.Invoke(card);
        }
        
        [ContextMenu("Debug: Print Manager State")]
        private void DebugPrintManagerState()
        {
            Debug.Log("=== CARD MANAGER STATE ===");
            Debug.Log($"Instance exists: {instance != null}");
            Debug.Log($"Deck exists: {runnerDeck != null}");
            
            if (runnerDeck != null)
            {
                Debug.Log($"Cards in deck: {runnerDeck.CollectedCards.Count}");
            }
        }
    }
}

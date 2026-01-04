using UnityEngine;

namespace HolidayGJ.Cards
{
    [RequireComponent(typeof(Collider))]
    public class CardPickup : MonoBehaviour
    {
        [Header("Card Data")]
        [SerializeField] private CardData cardToGive;
        
        [Header("Visual Effects")]
        [SerializeField] private GameObject collectVFX;
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] private float bobHeight = 0.3f;
        [SerializeField] private float bobSpeed = 2f;
        
        [Header("Audio")]
        [SerializeField] private AudioClip collectSound;
        
        private Vector3 startPosition;
        private bool isCollected = false;
        
        private void Start()
        {
            startPosition = transform.position;
            
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }
            
            if (cardToGive == null)
            {
                Debug.LogWarning($"CardPickup on {gameObject.name} has no CardData assigned!", this);
            }
        }
        
        private void Update()
        {
            if (isCollected) return;
            
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return;
            
            if (other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Character"))
            {
                CollectCard();
            }
        }
        
        private void CollectCard()
        {
            if (cardToGive == null)
            {
                Debug.LogError($"Cannot collect card - no CardData assigned on {gameObject.name}");
                Destroy(gameObject);
                return;
            }
            
            isCollected = true;
            
            int runnerPlayerID = CardManager.Instance.currentRunnerPlayerID;
            CardManager.Instance.AddCardToPlayer(runnerPlayerID, cardToGive);
            
            if (collectVFX != null)
            {
                Instantiate(collectVFX, transform.position, Quaternion.identity);
            }
            
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }
            
            Debug.Log($"Player {runnerPlayerID} collected card: {cardToGive.cardName}");
            
            Destroy(gameObject);
        }
    }
}

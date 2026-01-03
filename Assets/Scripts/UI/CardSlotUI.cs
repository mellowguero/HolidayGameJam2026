using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HolidayGJ.Cards;

namespace HolidayGameJam.UI
{
    public class CardSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image cardIcon;
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private TextMeshProUGUI cooldownText;
        [SerializeField] private TextMeshProUGUI useCountText;
        [SerializeField] private Image backgroundImage;
        
        [Header("Visual Settings")]
        [SerializeField] private Color availableColor = Color.white;
        [SerializeField] private Color unavailableColor = Color.gray;
        [SerializeField] private Color cooldownOverlayColor = new Color(0f, 0f, 0f, 0.7f);
        
        private CardData currentCard;
        private Button button;
        
        private void Awake()
        {
            button = GetComponent<Button>();
            if (button == null)
            {
                button = gameObject.AddComponent<Button>();
            }
        }
        
        public void Setup(CardData card)
        {
            currentCard = card;
            
            if (card == null)
            {
                Clear();
                return;
            }
            
            if (cardIcon != null && card.cardIcon != null)
            {
                cardIcon.sprite = card.cardIcon;
                cardIcon.enabled = true;
            }
            
            if (cardNameText != null)
            {
                cardNameText.text = card.cardName;
            }
            
            if (backgroundImage != null)
            {
                backgroundImage.color = card.cardColor;
            }
            
            gameObject.SetActive(true);
        }
        
        public void UpdateState(bool canUse, float cooldownRemaining, int usesRemaining)
        {
            if (currentCard == null)
            {
                return;
            }
            
            if (button != null)
            {
                button.interactable = canUse;
            }
            
            if (backgroundImage != null)
            {
                backgroundImage.color = canUse ? availableColor : unavailableColor;
            }
            
            if (cooldownOverlay != null)
            {
                if (cooldownRemaining > 0f)
                {
                    cooldownOverlay.gameObject.SetActive(true);
                    cooldownOverlay.color = cooldownOverlayColor;
                }
                else
                {
                    cooldownOverlay.gameObject.SetActive(false);
                }
            }
            
            if (cooldownText != null)
            {
                if (cooldownRemaining > 0f)
                {
                    cooldownText.gameObject.SetActive(true);
                    cooldownText.text = cooldownRemaining.ToString("F1") + "s";
                }
                else
                {
                    cooldownText.gameObject.SetActive(false);
                }
            }
            
            if (useCountText != null)
            {
                if (currentCard.maxUses > 0)
                {
                    useCountText.gameObject.SetActive(true);
                    useCountText.text = usesRemaining.ToString();
                }
                else
                {
                    useCountText.gameObject.SetActive(false);
                }
            }
        }
        
        public void Clear()
        {
            currentCard = null;
            
            if (cardIcon != null)
            {
                cardIcon.sprite = null;
                cardIcon.enabled = false;
            }
            
            if (cardNameText != null)
            {
                cardNameText.text = "";
            }
            
            if (cooldownOverlay != null)
            {
                cooldownOverlay.gameObject.SetActive(false);
            }
            
            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(false);
            }
            
            if (useCountText != null)
            {
                useCountText.gameObject.SetActive(false);
            }
            
            gameObject.SetActive(false);
        }
        
        public CardData GetCard()
        {
            return currentCard;
        }
        
        public void OnCardClicked()
        {
            if (currentCard != null && button != null && button.interactable)
            {
                Debug.Log($"Card clicked: {currentCard.cardName}");
            }
        }
    }
}

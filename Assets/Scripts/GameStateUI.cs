using UnityEngine;
using TMPro;

namespace HolidayGameJam
{
    public class GameStateUI : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _losePanel;

        [Header("Text Components")]
        [SerializeField] private TextMeshProUGUI _winText;
        [SerializeField] private TextMeshProUGUI _loseText;

        [Header("Colors")]
        [SerializeField] private Color _winBackgroundColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
        [SerializeField] private Color _loseBackgroundColor = new Color(0.8f, 0.2f, 0.2f, 0.8f);

        private void Start()
        {
            HideAllPanels();
        }

        public void ShowWin()
        {
            HideAllPanels();
            if (_winPanel != null)
            {
                _winPanel.SetActive(true);
            }
        }

        public void ShowLose()
        {
            HideAllPanels();
            if (_losePanel != null)
            {
                _losePanel.SetActive(true);
            }
        }

        public void HideAllPanels()
        {
            if (_winPanel != null)
            {
                _winPanel.SetActive(false);
            }
            if (_losePanel != null)
            {
                _losePanel.SetActive(false);
            }
        }
    }
}

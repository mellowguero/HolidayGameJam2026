using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace HolidayGameJam
{
    public class GameManager : MonoBehaviour
    {
        [Header("Component References")]
        // [SerializeField] private RunnerHealth _runnerHealth; // TODO: Replace with Opsive CharacterHealth
        [SerializeField] private GoalTrigger _goalTrigger;
        [SerializeField] private GameStateUI _gameStateUI;

        [Header("Scene Management")]
        [SerializeField] private string _runnerSceneName = "RunnerScene";

        private bool _roundEnded = false;

        private void Awake()
        {
            int managerCount = FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length;
            if (managerCount > 1)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            FindSceneReferences();
            WireUpEvents();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FindSceneReferences();
            WireUpEvents();
            _roundEnded = false;
        }

        private void FindSceneReferences()
        {
            // TODO: Re-enable when using Opsive CharacterHealth
            // if (_runnerHealth == null)
            // {
            //     _runnerHealth = FindAnyObjectByType<RunnerHealth>();
            // }

            if (_goalTrigger == null)
            {
                _goalTrigger = FindAnyObjectByType<GoalTrigger>();
            }

            if (_gameStateUI == null)
            {
                _gameStateUI = FindAnyObjectByType<GameStateUI>();
            }
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                RestartRound();
            }
        }

        private void WireUpEvents()
        {
            // TODO: Re-enable when using Opsive CharacterHealth
            // if (_runnerHealth != null)
            // {
            //     _runnerHealth.onDeath.RemoveListener(OnPlayerDeath);
            //     _runnerHealth.onDeath.AddListener(OnPlayerDeath);
            // }

            if (_goalTrigger != null)
            {
                _goalTrigger.onGoalReached.RemoveListener(OnGoalReached);
                _goalTrigger.onGoalReached.AddListener(OnGoalReached);
            }
        }

        public void OnPlayerDeath()
        {
            if (_roundEnded) return;
            
            _roundEnded = true;
            Debug.Log("Player died! Press R to restart.");

            if (_gameStateUI != null)
            {
                _gameStateUI.ShowLose();
            }
        }

        public void OnGoalReached()
        {
            if (_roundEnded) return;

            _roundEnded = true;
            Debug.Log("Goal reached! Press R to restart.");
            Debug.Log($"GameManager: _gameStateUI is {(_gameStateUI != null ? "NOT NULL" : "NULL")}");

            if (_gameStateUI != null)
            {
                _gameStateUI.ShowWin();
            }
            else
            {
                Debug.LogError("GameManager: Cannot show win UI - _gameStateUI is null!");
            }
        }

        public void RestartRound()
        {
            _roundEnded = false;
            ClearAllHazards();
            SceneManager.LoadScene(_runnerSceneName);
        }

        private void ClearAllHazards()
        {
            GameObject[] hazards = GameObject.FindGameObjectsWithTag("Hazard");
            foreach (GameObject hazard in hazards)
            {
                Destroy(hazard);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            // TODO: Re-enable when using Opsive CharacterHealth
            // if (_runnerHealth != null)
            // {
            //     _runnerHealth.onDeath.RemoveListener(OnPlayerDeath);
            // }

            if (_goalTrigger != null)
            {
                _goalTrigger.onGoalReached.RemoveListener(OnGoalReached);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

namespace HolidayGameJam
{
    public class GoalTrigger : MonoBehaviour
    {
        [Header("Events")]
        [Tooltip("Triggered when the Runner reaches the goal")]
        public UnityEvent onGoalReached;

        private bool _goalReached = false;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"GoalTrigger: OnTriggerEnter called! Collider: {other.name}, Tag: {other.tag}");
            
            if (_goalReached)
            {
                Debug.Log("GoalTrigger: Goal already reached, ignoring.");
                return;
            }

            if (other.CompareTag("Player"))
            {
                _goalReached = true;
                Debug.Log("Goal reached!");
                onGoalReached?.Invoke();
            }
            else
            {
                Debug.Log($"GoalTrigger: Collider tag '{other.tag}' does not match 'Player'");
            }
        }

        public void ResetGoal()
        {
            _goalReached = false;
        }
    }
}

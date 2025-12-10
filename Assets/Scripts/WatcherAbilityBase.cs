using UnityEngine;

public abstract class WatcherAbilityBase : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] protected GameObject abilityPrefab;
    [SerializeField] protected float cooldownDuration = 1f;
    [SerializeField] protected LayerMask targetLayerMask;

    protected float cooldownTimer = 0f;

    public bool IsReady => cooldownTimer <= 0f;

    protected virtual void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void TryActivate(Vector3 targetPosition)
    {
        if (!IsReady)
        {
            Debug.Log($"{GetType().Name} is on cooldown: {cooldownTimer:F2}s remaining");
            return;
        }

        if (CanActivateAt(targetPosition))
        {
            ActivateAbility(targetPosition);
            cooldownTimer = cooldownDuration;
            OnAbilityActivated(targetPosition);
        }
    }

    protected virtual bool CanActivateAt(Vector3 position)
    {
        return true;
    }

    protected abstract void ActivateAbility(Vector3 targetPosition);

    protected virtual void OnAbilityActivated(Vector3 position)
    {
        Debug.Log($"{GetType().Name} activated at {position}");
    }
}

using UnityEngine;

public class FallingBlockAbility : WatcherAbilityBase
{
    [Header("Falling Block Settings")]
    [SerializeField] private float spawnHeightAboveTarget = 5f;

    protected override void ActivateAbility(Vector3 targetPosition)
    {
        if (abilityPrefab == null)
        {
            Debug.LogError("FallingBlockAbility: No ability prefab assigned!");
            return;
        }

        Vector3 spawnPosition = targetPosition + Vector3.up * spawnHeightAboveTarget;
        GameObject fallingBlock = Instantiate(abilityPrefab, spawnPosition, Quaternion.identity);

        Debug.Log($"Falling block spawned at {spawnPosition}, will hit ground near {targetPosition}");
    }

    protected override bool CanActivateAt(Vector3 position)
    {
        return true;
    }
}

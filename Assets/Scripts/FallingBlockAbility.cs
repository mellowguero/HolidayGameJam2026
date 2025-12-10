using UnityEngine;

public class FallingBlockAbility : WatcherAbilityBase
{
    [Header("Falling Block Settings")]
    [SerializeField] private float spawnHeightAboveTarget = 5f;
    [SerializeField] private float blockLifetime = 10f;

    protected override void ActivateAbility(Vector3 targetPosition)
    {
        if (abilityPrefab == null)
        {
            Debug.LogError("FallingBlockAbility: No ability prefab assigned!");
            return;
        }

        Vector3 spawnPosition = targetPosition + Vector3.up * spawnHeightAboveTarget;
        GameObject fallingBlock = Instantiate(abilityPrefab, spawnPosition, Quaternion.identity);

        Destroy(fallingBlock, blockLifetime);

        Debug.Log($"Falling block spawned at {spawnPosition}, will hit ground near {targetPosition}");
    }

    protected override bool CanActivateAt(Vector3 position)
    {
        return true;
    }
}

using UnityEngine;
using Opsive.Shared.Events;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character;

public class VaultDebugger : MonoBehaviour
{
    private UltimateCharacterLocomotion characterLocomotion;
    private Ability vaultAbility;

    private void Awake()
    {
        characterLocomotion = GetComponent<UltimateCharacterLocomotion>();
        
        var abilities = characterLocomotion.GetAbilities<Ability>();
        foreach (var ability in abilities)
        {
            if (ability.GetType().Name == "Vault")
            {
                vaultAbility = ability;
                Debug.Log($"Found Vault ability! Enabled: {ability.Enabled}, Index: {ability.Index}");
                break;
            }
        }
        
        if (vaultAbility == null)
        {
            Debug.LogError("Vault ability not found on character!");
        }
        
        EventHandler.RegisterEvent<Ability, bool>(gameObject, "OnCharacterAbilityActive", OnAbilityActive);
    }

    private void OnAbilityActive(Ability ability, bool activated)
    {
        if (activated)
        {
            // Debug.Log($"<color=green>Ability STARTED: {ability.GetType().Name}</color>");
        }
        else
        {
            // Debug.Log($"<color=red>Ability STOPPED: {ability.GetType().Name}</color>");
        }
    }

    private void Update()
    {
        if (vaultAbility != null && characterLocomotion.Grounded && characterLocomotion.Moving)
        {
            float rayDistance = 5f;
            Vector3 origin = transform.position + Vector3.up * 1f;
            Vector3 direction = transform.forward;
            
            int vaultableLayer = LayerMask.NameToLayer("Vaultable");
            LayerMask mask = 1 << vaultableLayer;
            
            if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, mask))
            {
                Debug.DrawRay(origin, direction * hit.distance, Color.green);
                Debug.Log($"VAULT RAYCAST HIT! Object: {hit.collider.name}, Distance: {hit.distance:F2}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, Height: {hit.collider.bounds.size.y:F2}");
                
                bool canStart = vaultAbility.CanStartAbility();
                if (canStart)
                {
                    Debug.Log($"<color=cyan>Vault CAN START but isn't starting automatically!</color>");
                }
                else
                {
                    Debug.Log($"<color=orange>Vault CANNOT START - Enabled: {vaultAbility.Enabled}, IsActive: {vaultAbility.IsActive}</color>");
                }
            }
            else
            {
                Debug.DrawRay(origin, direction * rayDistance, Color.red);
            }
        }
    }

    private void OnDestroy()
    {
        EventHandler.UnregisterEvent<Ability, bool>(gameObject, "OnCharacterAbilityActive", OnAbilityActive);
    }
}

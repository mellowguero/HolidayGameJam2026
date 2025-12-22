using UnityEngine;
using UnityEngine.InputSystem;
using RayFire;

public class RayFireDestructionTester : MonoBehaviour
{
    [Header("Target")]
    public RayfireRigid targetRigid;
    
    [Header("Controls")]
    [Tooltip("Press this key to demolish the target")]
    public Key demolishKey = Key.Space;
    
    [Header("Damage Settings")]
    [Tooltip("Press this key to apply damage")]
    public Key damageKey = Key.D;
    [Tooltip("Amount of damage to apply per key press")]
    public float damageAmount = 25f;
    
    [Header("Explosion Settings")]
    [Tooltip("Add explosion force when demolishing")]
    public bool addExplosionForce = true;
    [Tooltip("Strength of the explosion")]
    public float explosionForce = 500f;
    [Tooltip("Radius of the explosion")]
    public float explosionRadius = 5f;
    
    private bool hasBeenDemolished = false;
    
    void Update()
    {
        if (targetRigid == null)
        {
            Debug.LogWarning("Target Rigid is NULL!");
            return;
        }
        
        if (hasBeenDemolished)
        {
            return;
        }
        
        if (Keyboard.current != null && Keyboard.current[demolishKey].wasPressedThisFrame)
        {
            Debug.Log($"[DEMOLISH] Attempting to demolish {targetRigid.name}");
            Debug.Log($"[DEMOLISH] Initialized: {targetRigid.initialized}");
            
            Vector3 explosionCenter = targetRigid.transform.position;
            targetRigid.Demolish();
            hasBeenDemolished = true;
            
            if (addExplosionForce)
            {
                StartCoroutine(ApplyExplosionForceToFragments(explosionCenter));
            }
            
            Debug.Log($"[DEMOLISH] Demolish() called!");
        }
        
        if (Keyboard.current != null && Keyboard.current[damageKey].wasPressedThisFrame && targetRigid.damage.en)
        {
            Debug.Log($"[DAMAGE] Applying {damageAmount} damage to {targetRigid.name}");
            Debug.Log($"[DAMAGE] Current damage before: {targetRigid.damage.cur} / {targetRigid.damage.max}");
            
            Vector3 explosionCenter = targetRigid.transform.position;
            targetRigid.ApplyDamage(damageAmount, transform.position, 1f);
            
            if (targetRigid.damage.cur >= targetRigid.damage.max)
            {
                hasBeenDemolished = true;
                if (addExplosionForce)
                {
                    StartCoroutine(ApplyExplosionForceToFragments(explosionCenter));
                }
            }
            
            Debug.Log($"[DAMAGE] Current damage after: {targetRigid.damage.cur} / {targetRigid.damage.max}");
        }
    }
    
    System.Collections.IEnumerator ApplyExplosionForceToFragments(Vector3 explosionCenter)
    {
        yield return new WaitForFixedUpdate();
        
        if (targetRigid != null && targetRigid.fragments != null && targetRigid.fragments.Count > 0)
        {
            Debug.Log($"[EXPLOSION] Applying force to {targetRigid.fragments.Count} fragments");
            
            foreach (RayfireRigid fragment in targetRigid.fragments)
            {
                if (fragment != null)
                {
                    Rigidbody rb = fragment.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddExplosionForce(
                            explosionForce,
                            explosionCenter,
                            explosionRadius,
                            1f,
                            ForceMode.Impulse
                        );
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MSEExampleSpawner : MonoBehaviour
{
    public ParticlesObj particles;
    int index = 0;
    public GameObject CurParticle;
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.aKey.wasPressedThisFrame)
        {
            index -= 1;
            if(index < 0)
            {
                index = particles.Particles.Count - 1;
            }
            Spawn();
        }
        if (Keyboard.current != null && Keyboard.current.dKey.wasPressedThisFrame)
        {
            index += 1;
            if(index >= particles.Particles.Count)
            {
                index = 0;
            }
            Spawn();
        }
        if (index > particles.Particles.Count - 1)
        {
            index = 0;
        }
        if ((Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) || 
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
        {
            Spawn();
        }
    }
    private void Start()
    {
        Spawn();
    }
    public void Spawn()
    {
        if (CurParticle != null)
        {
            Destroy(CurParticle);
        }
        CurParticle = Instantiate(particles.Particles[index]);
    }
}

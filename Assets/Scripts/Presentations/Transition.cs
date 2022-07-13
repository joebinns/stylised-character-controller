using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transitions between two game objects, despawning one and spawning another with effects.
/// </summary>
[System.Serializable]
public class Transition
{
    public GameObject toEnable;
    public GameObject toDisable;

    public Effect spawnEffect;
    
    public Transform toEnableEffectAnchor;
    public Transform toDisableEffectAnchor;

    /// <summary>
    /// Runs a transition, disabling certain game objects and enabling others, with effects.
    /// </summary>
    /// <returns>Whether or not the transition has occured or not.</returns>
    public bool MakeTransition()
    {
        if (spawnEffect.isPlaying)
        {
            return false;
        }
        
        toDisable.SetActive(false);

        toEnable.SetActive(true);
        spawnEffect.ActivateEffects(toEnableEffectAnchor, toEnable);

        return true;
    }

    /// <summary>
    /// Reverts a transition, enabling certain game objects, with effects, and disabling others (does the opposite of make transition).
    /// </summary>
    /// <returns>Whether or not the transition has occured or not.</returns>
    public bool RevertTransition()
    {
        if (spawnEffect.isPlaying)
        {
            return false;
        }

        toEnable.SetActive(false);

        toDisable.SetActive(true);
        spawnEffect.ActivateEffects(toDisableEffectAnchor, toDisable);

        return true;
    }
}

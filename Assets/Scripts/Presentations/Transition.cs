using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Transition
{
    public GameObject toEnable;
    public GameObject toDisable;

    public Effect spawnEffect;
    
    public Transform toEnableEffectAnchor;
    public Transform toDisableEffectAnchor;

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

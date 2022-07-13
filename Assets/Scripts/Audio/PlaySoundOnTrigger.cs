using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Play a given sound when this rigid body / collider enters a trigger.
/// </summary>
public class PlaySoundOnTrigger : MonoBehaviour
{
    public string audioName;

    /// <summary>
    /// Play the sound when this rigid body / collider enters a trigger.
    /// </summary>
    /// <param name="other">The trigger's rigid body / collider.</param>
    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<AudioManager>().Play(audioName);
    }
}

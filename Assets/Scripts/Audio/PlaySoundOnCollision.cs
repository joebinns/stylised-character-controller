using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Play a given sound when this rigid body / collider collides.
/// </summary>
public class PlaySoundOnCollision : MonoBehaviour
{
    public string audioName;

    /// <summary>
    /// Play the sound when this rigid body / collider collides with another.
    /// </summary>
    /// <param name="other">The other rigid body / collider involved in this collision.</param>
    private void OnCollisionEnter(Collision other)
    {
        FindObjectOfType<AudioManager>().Play(audioName);
    }
}

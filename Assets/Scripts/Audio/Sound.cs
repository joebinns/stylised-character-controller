using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The properties to store for a generic sound.
/// </summary>
[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

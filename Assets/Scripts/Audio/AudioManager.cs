using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// An Audio Manager. Based on Brackey's tutorial https://youtu.be/6OT43pvUyfY.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    /// <summary>
    /// Declare the properties of the sounds.
    /// </summary>
    void Awake()
    {
        
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.playOnAwake = false;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    /// <summary>
    /// Update the audio clips properties of the sounds.
    /// </summary>
    private void Update()
    {
        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    /// <summary>
    /// Beginning playing a chosen sound.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Play();
    }

    /// <summary>
    /// Get whether a sound is playing or not.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    /// <returns>Whether the sound is playing or not.</returns>
    public bool IsPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return false;
        }

        return(s.source.isPlaying);
    }

    /// <summary>
    /// Play two sounds consecutively.
    /// </summary>
    /// <param name="name1">The name of the first sound to be played.</param>
    /// <param name="name2">The name of the sound to be player after the first has finished.</param>
    /// <returns></returns>
    public IEnumerator PlayQueued(string name1, string name2)
    {
        Sound s1 = Array.Find(sounds, sound => sound.name == name1);
        Sound s2 = Array.Find(sounds, sound => sound.name == name2);
        if (s1 == null)
        {
            Debug.LogWarning("Sound: " + name1 + " not found");
            yield return null;
        }
        if (s2 == null)
        {
            Debug.LogWarning("Sound: " + name2 + " not found");
            yield return null;
        }

        if (s1.source.isPlaying | s2.source.isPlaying)
        {
            yield return null;
        }

        s1.source.Play();
        yield return new WaitForSeconds(s1.clip.length);
        s2.source.Play();
    }

    /// <summary>
    /// Stop playing a sound.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Stop();
    }
}
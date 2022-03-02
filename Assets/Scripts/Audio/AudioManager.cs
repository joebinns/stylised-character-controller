using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

//Credit to Brackeys youtube tutorial on Audio managers, as the majority of this code and learning how to use it was made by him.
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;
    //AudioManager

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

    private void Update()
    {
        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

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

    public IEnumerator PlayQueued(string name1, string name2)
    {
        Sound s1 = Array.Find(sounds, sound => sound.name == name1);
        Sound s2 = Array.Find(sounds, sound => sound.name == name2);
        if (s1 == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            yield return null;
        }
        if (s2 == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
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

    //this addition to the code was made by me, the rest was from Brackeys tutorial
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        s.source.Stop();
    }
}
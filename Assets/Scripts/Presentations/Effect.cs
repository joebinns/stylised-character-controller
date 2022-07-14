using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A spawn effect for an object which scales up the object with a bounce, plays a sound, and activates particle systems.
/// </summary>
[System.Serializable]
public class Effect : MonoBehaviour
{
    public ParticleSystem[] particleSystems;

    public GameObject _activatedGameObject;
    public Vector3 _startPos;
    public Vector3 _endPos;
    public Vector3 _endScale;
    public bool isPlaying = false;

    /// <summary>
    /// Plays a sound effect for spawning a game object, and activates particle effects.
    /// </summary>
    /// <param name="anchor">The transform about which the effects should take place.</param>
    /// <param name="activatedGameObject">The game object to spawn.</param>
    public void ActivateEffects(Transform anchor, GameObject activatedGameObject)
    {
        _t = 0f;
        _activatedGameObject = activatedGameObject;
        _startPos = anchor.position;
        _endPos = activatedGameObject.transform.position;
        _endScale = activatedGameObject.transform.localScale;
        isPlaying = true;    

        FindObjectOfType<AudioManager>().Play("Woo");

        SetParent(anchor);

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Play();
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.enabled = true;
        }
    }

    /// <summary>
    /// Updates the scale of the game object when appropriate.
    /// </summary>
    private void Update()
    {
        if (isPlaying)
        {
            ScaleUp(_startPos, _endPos, _endScale);
        }
    }

    /// <summary>
    /// Lerps the scale and position of the spawned game object, with a bounce.
    /// </summary>
    private float _t = 0f;
    private void ScaleUp(Vector3 startPos, Vector3 endPos, Vector3 endScale, float duration = 0.7f)
    {
        _t += Time.deltaTime / duration;
        _t = Mathf.Min(_t, 1f);

        float t = Easing.Bounce.Out(_t);

        // Lerp position from startPos to endPos
        Vector3 position = MathsUtils.LerpVector3(startPos, endPos, t);

        // Lerp scale from Vector3.zero to endScale
        Vector3 scale = MathsUtils.LerpVector3(Vector3.zero, endScale, t);

        _activatedGameObject.transform.position = position;
        _activatedGameObject.transform.localScale = scale;

        if (_t >= 1f)
        {
            isPlaying = false;
        }
    }

    /// <summary>
    /// Sets the transform parent of the particle systems, such as to recycle a single particle system over multiple effects.
    /// </summary>
    /// <param name="parent">The anchor transform about which the particle systems should occur.</param>
    private void SetParent(Transform parent)
    {
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.transform.SetParent(parent, true);
            particleSystem.transform.localPosition = Vector3.zero;
            particleSystem.transform.localScale = Vector3.one;
        }
    }
}

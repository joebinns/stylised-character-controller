using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Squash and stretch effect on the transform scale from an oscillator.
/// </summary>
[DisallowMultipleComponent]
public class SquashAndStretch : MonoBehaviour
{
    public bool constantVolume = true;

    private Vector3 _localEquilibriumScale;

    [Tooltip("The oscillator that reacts to this object's physics.")]
    [SerializeField] private Oscillator _oscillator;
    [Tooltip("The conversion factor from the oscillator's local position to the contribution it has on this object's local scale.")]
    [SerializeField] private float _conversion = 0.15f;

    /// <summary>
    /// Set the equilibrium scale, corresponding to the equilibrium position of the oscillator.
    /// </summary>
    void Start()
    {
        _localEquilibriumScale = transform.localScale;
    }

    /// <summary>
    /// Updates the local scale to be the squashed and stretched scale.
    /// </summary>
    private void FixedUpdate()
    {
        transform.localScale = CalculateSquashedAndStretchedScale();
    }

    /// <summary>
    /// Calculates the squashed and stretched scale.
    /// </summary>
    /// <returns>The volume maintained squashed and stretched local scale.</returns>
    private Vector3 CalculateSquashedAndStretchedScale()
    {
        Vector3 rawOscillatorContribution = GetContributionFromOscillator();
        Vector3 oscillatorContribution = rawOscillatorContribution;
        if (constantVolume)
        {
            oscillatorContribution = MaintainVolume(rawOscillatorContribution);
        }
        Vector3 localScale = _localEquilibriumScale + oscillatorContribution;
        return (localScale);
    }

    /// <summary>
    /// Calculates the contribution of the oscillator's local position, for further processing.
    /// </summary>
    /// <returns>Direct conversion from oscillator position to scale contribution.</returns>
    private Vector3 GetContributionFromOscillator()
    {
        Vector3 oscillatorContribution = _oscillator.transform.localPosition * _conversion;
        oscillatorContribution = -oscillatorContribution; // Take the negative of the contribution, in order to get the desired effect of squash/stretch.
        return (oscillatorContribution);
    }

    /// <summary>
    /// Maintains the volume of the scale in all axes, such that i.e. a compression in the given vector's axis results in expansion in both the perpendicular axes.
    /// CAUTION: Last I checked, this function is not actually maintaining an exactly constant volume, the maths is a little bit off.
    /// But the desired visual effect is there.
    /// </summary>
    /// <param name="primaryDeformation">The primary deformation that would otherwise cause the volume to change.</param>
    /// <returns></returns>
    private Vector3 MaintainVolume(Vector3 primaryDeformation)
    {
        if (primaryDeformation != Vector3.zero)
        {
            Vector3 rightCrossSecondaryDeformation = Vector3.Cross(primaryDeformation, transform.right);
            Vector3 upCrossSecondaryDeformation = Vector3.Cross(primaryDeformation, transform.up);
            Vector3 forwardCrossSecondaryDeformation = Vector3.Cross(primaryDeformation, transform.forward);
            Vector3 totalDeformation = primaryDeformation + 0.5f * (rightCrossSecondaryDeformation + upCrossSecondaryDeformation + forwardCrossSecondaryDeformation);
            return (totalDeformation);
        }
        return (Vector3.zero);
    }
}

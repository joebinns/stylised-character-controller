using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TorsionalOscillator))]
[RequireComponent(typeof(MeshRenderer))]
public class DynamicPendulumColour : MonoBehaviour
{
    private TorsionalOscillator _torsionalOscillator;
    private Renderer _meshRenderer;
    private Material _dynamicPendulumMaterial;

    private void Start()
    {
        _torsionalOscillator = GetComponent<TorsionalOscillator>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _dynamicPendulumMaterial = _meshRenderer.material;
    }

    private void FixedUpdate()
    {
        float angle = _torsionalOscillator.angularDisplacementMagnitude;

        // Color goes from green (0,1,0,0) to yellow (1,1,0,0) to red (1,0,0,0) to pink (1,0,1,0) to blue (0,0,1,0) to cyan (0,1,1,0) to green...
        Color color = Color.green;
        float upperAmplitude = 20f; // Approximately the upper limit of the angle amplitude within regular use
        float ratio = angle / upperAmplitude;

        float r = 2f * (1f - Mathf.Clamp(ratio, 0.5f, 1f));
        float g = 2f * Mathf.Clamp(ratio, 0f, 0.5f);
        float b = 1f;


        /*
        float r = 3f * Mathf.Clamp(ratio, 0f, 1f / 3f);
        float g = 3f * ((2f / 3f) - Mathf.Clamp(ratio, 1f / 3f, 1f));
        float b = 3f * ((1f) - Mathf.Clamp(ratio, 1f / 3f, 1f));
        */

        /*
        float r = 1f;
        float g = 0f;
        float b = 0f;
        if (0f < ratio & ratio < 1f/6f)
        {
            r = 1f;
            g = 6f * ratio;
            b = 0f;
        }
        else if (1f/6f < ratio & ratio < 2f/6f)
        {
            r = 2f - 6f * ratio;
            g = 1f;
            b = 0f;
        }
        else if (2f/6f < ratio & ratio < 3f/6f)
        {
            r = 0f;
            g = 1f;
            b = 6f * ratio - 2f;
        }
        else if (3f/6f < ratio & ratio < 4f/6f)
        {
            r = 0f;
            g = 4f - 6f * ratio;
            b = 1f;
        }
        else if (4f/6f < ratio & ratio < 5f/6f)
        {
            r = 6f * ratio - 4f;
            g = 0f;
            b = 1f;
        }
        else if (5f/6f < ratio & ratio < 1f)
        {
            r = 1f;
            g = 0f;
            b = 1f - 6f * ratio;
        }
        */

        color.r = r;
        color.g = g;
        color.b = b;
        Gizmos.color = color;

        _dynamicPendulumMaterial.color = color;
    }
}

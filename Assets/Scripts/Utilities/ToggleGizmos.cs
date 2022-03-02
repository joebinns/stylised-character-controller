using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Gizmos = Popcron.Gizmos;

[ExecuteAlways]
public class ToggleGizmos : MonoBehaviour
{
    [SerializeField] private Oscillator playerOscillator;

    private void Start()
    {
        // Ensure Gizmos are enabled;
        Gizmos.Enabled = true;

        // Begin with all gizmos off
        //Gizmos.Enabled = !Gizmos.Enabled;
        Oscillator[] oscillators = GameObject.FindObjectsOfType<Oscillator>();
        TorsionalOscillator[] torsionalOscillators = GameObject.FindObjectsOfType<TorsionalOscillator>();

        for (int i = 0; i < oscillators.Length; i++)
        {
            Oscillator oscillator = oscillators[i];
            oscillator.renderGizmos = false;
        }
        for (int i = 0; i < torsionalOscillators.Length; i++)
        {
            TorsionalOscillator torsionalOscillator = torsionalOscillators[i];
            torsionalOscillator.renderGizmos = false;
        }
    }

    // Toggle torsional oscillators
    public void TorsionalOscillatorGizmosToggle(InputAction.CallbackContext context)
    {
        if (context.started) // Button down
        {
            TorsionalOscillator[] torsionalOscillators = GameObject.FindObjectsOfType<TorsionalOscillator>();

            bool baseAround = torsionalOscillators[0].renderGizmos;
            for (int i = 0; i < torsionalOscillators.Length; i++)
            {
                TorsionalOscillator torsionalOscillator = torsionalOscillators[i];
                torsionalOscillator.renderGizmos = !baseAround;
            }
        }
    }

    // Toggle oscillators
    public void OscillatorGizmosToggle(InputAction.CallbackContext context)
    {
        if (context.started) // Button down
        {
            Oscillator[] oscillators = GameObject.FindObjectsOfType<Oscillator>();

            bool baseAround = oscillators[0].renderGizmos;
            for (int i = 0; i < oscillators.Length; i++)
            {
                Oscillator oscillator = oscillators[i];
                oscillator.renderGizmos = !baseAround;
            }
        }
    }

    // Toggle player
    public void PlayerGizmosToggle(InputAction.CallbackContext context)
    {
        if (context.started) // Button down
        {
            playerOscillator.renderGizmos = !playerOscillator.renderGizmos;
        }
    }

    // Toggle all
    public void GizmosToggle(InputAction.CallbackContext context)
    {
        if (context.started) // Button down
        {
            // Toggle gizmo drawing
            //Gizmos.Enabled = !Gizmos.Enabled;
            Oscillator[] oscillators = GameObject.FindObjectsOfType<Oscillator>();
            TorsionalOscillator[] torsionalOscillators = GameObject.FindObjectsOfType<TorsionalOscillator>();

            bool baseAround = oscillators[0].renderGizmos;
            for (int i = 0; i < oscillators.Length; i++)
            {
                Oscillator oscillator = oscillators[i];
                oscillator.renderGizmos = !baseAround;
            }
            for (int i = 0; i < torsionalOscillators.Length; i++)
            {
                TorsionalOscillator torsionalOscillator = torsionalOscillators[i];
                torsionalOscillator.renderGizmos = !baseAround;
            }
        }
    }
}
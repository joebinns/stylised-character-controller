using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

/// <summary>
/// Begin the pendulum creator and switch cameras upon input.
/// </summary>
public class StartPendulums : MonoBehaviour
{
    public PendulumCreator pendulumCreator;
    public CinemachineVirtualCamera vc;

    /// <summary>
    /// Switch camera to that which views the pendulum creator.
    /// </summary>
    /// <param name="context">The input's context.</param>
    public void SwitchCamera(InputAction.CallbackContext context)
    {   
        if (context.started)
        {
            pendulumCreator.gameObject.SetActive(true);
            vc.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Start the pendulum creator.
    /// </summary>
    /// <param name="context">The input's context.</param>
    public void Begin(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            pendulumCreator.started = true;
        }
    }
}

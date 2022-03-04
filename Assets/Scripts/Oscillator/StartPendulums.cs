using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class StartPendulums : MonoBehaviour
{
    public PendulumCreator pendulumCreator;
    public CinemachineVirtualCamera vc;

    public void SwitchCamera(InputAction.CallbackContext context)
    {   
        if (context.started)
        {
            pendulumCreator.gameObject.SetActive(true);
            vc.gameObject.SetActive(true);
        }
    }

    public void Begin(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            pendulumCreator.started = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

/// <summary>
/// Switch consecutively between Cinemachine cameras.
/// </summary>
public class SwitchCamera : MonoBehaviour
{
    public List<CinemachineVirtualCamera> cinemachineVirtualCameras = new List<CinemachineVirtualCamera>();

    private int _currentCameraIndex = 0;

    /// <summary>
    /// Ensure only the first Cinemachine camera is active.
    /// </summary>
    private void Start()
    {
        foreach (CinemachineVirtualCamera camera in cinemachineVirtualCameras)
        {
            camera.gameObject.SetActive(false);
        }

        cinemachineVirtualCameras[0].gameObject.SetActive(true);
    }

    /// <summary>
    /// Switch to the next consecutive Cinemachine camera.
    /// </summary>
    /// <param name="context">The input's context.</param>
    public void SwitchToNextCamera(InputAction.CallbackContext context)
    {   
        if (context.started)
        {
            cinemachineVirtualCameras[_currentCameraIndex].gameObject.SetActive(false);
            _currentCameraIndex += 1;
            if (_currentCameraIndex >= cinemachineVirtualCameras.Count)
            {
                _currentCameraIndex = 0;
            }
            cinemachineVirtualCameras[_currentCameraIndex].gameObject.SetActive(true);
        }
    }
}

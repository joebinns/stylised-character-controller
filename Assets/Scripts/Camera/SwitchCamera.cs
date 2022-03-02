using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class SwitchCamera : MonoBehaviour
{
    public List<CinemachineVirtualCamera> cinemachineVirtualCameras = new List<CinemachineVirtualCamera>();

    private int _currentCameraIndex = 0;

    private void Start()
    {
        foreach (CinemachineVirtualCamera camera in cinemachineVirtualCameras)
        {
            camera.gameObject.SetActive(false);
        }

        cinemachineVirtualCameras[0].gameObject.SetActive(true);
    }

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

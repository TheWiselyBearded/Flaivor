using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class IntroUI : MonoBehaviour
{
    public event Action OnCameraRequestInputReceived;
    public event Action OnVoiceRequestInputReceived;
    public UnityEvent voiceRequest;
    public UnityEvent cameraRequest;

    // Reference to the main camera
    public Camera mainCamera;

    // The distance between the camera and the object
    public float distance = 0.6f;

    private void Start() {
        if (mainCamera == null) mainCamera = Camera.main;
        // If the main camera is found
        if (mainCamera != null) {
            // Calculate the position based on the camera's forward direction and the specified distance
            Vector3 newPosition = mainCamera.transform.position + mainCamera.transform.forward * distance;

            // Adjust y-coordinate to be at head level
            newPosition.y = -1* mainCamera.transform.position.y;

            // Set the position of the GameObject to the calculated position
            transform.position = newPosition;
        }
    }

    public void InvokeVoiceRequest()
    {
        OnVoiceRequestInputReceived?.Invoke();
        voiceRequest?.Invoke();
    }
    public void InvokeCameraRequest()
    {
        OnCameraRequestInputReceived?.Invoke();
        cameraRequest?.Invoke();
    }
}

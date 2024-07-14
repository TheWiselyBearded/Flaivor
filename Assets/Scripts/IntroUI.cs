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

    public GameObject vrPanel;

    // Reference to the main camera
    public Camera mainCamera;

    // The distance between the camera and the object
    public float distance = 0.6f;

    private void Start()
    {
        vrPanel.SetActive(false);
        Invoke("SetHeadLevel", 1.5f);
    }

    private void SetHeadLevel()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        // If the main camera is found
        if (mainCamera != null)
        {
            // Calculate the position based on the camera's forward direction and the specified distance
            Vector3 newPosition = mainCamera.transform.position + mainCamera.transform.forward * distance;

            // Adjust y-coordinate to be at head level
            newPosition.y = mainCamera.transform.position.y;

            // Set the position of the GameObject to the calculated position
            transform.position = newPosition;

            // Make the GameObject face the camera
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0.0f, 180.0f, 0.0f);
            // zero out the rotation on the x and z axis
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        vrPanel.SetActive(true);
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

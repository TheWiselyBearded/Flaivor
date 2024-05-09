using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntroUI : MonoBehaviour
{
    public event Action OnCameraRequestInputReceived;
    public event Action OnVoiceRequestInputReceived;
    public UnityEvent voiceRequest;
    public UnityEvent cameraRequest;

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

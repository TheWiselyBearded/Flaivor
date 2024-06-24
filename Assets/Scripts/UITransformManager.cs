using System;
using UnityEngine;

public class UITransformManager : MonoBehaviour {
    public static UITransformManager Instance { get; private set; }


    // Field to store the most recently set Vector3
    private Transform _latestTransform;

    private void Awake() {
        // Singleton pattern: Ensure only one instance exists
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Preserve across scenes
        }
        else {
            Destroy(gameObject);
        }
    }

    // Method to set the Vector3 value and invoke the event
    public void SetVector3(Transform newTransform) {
        _latestTransform = newTransform;
        Debug.Log("UITransformManager: Vector3 updated to " + newTransform);
    }

    // Method to obtain the most recently set Vector3
    public Transform GetLatestTransform() {
        return _latestTransform;
    }
}

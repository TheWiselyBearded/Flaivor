using System;
using System.Collections.Generic;
using UnityEngine;

public class Dispatcher : MonoBehaviour {
    public static Dispatcher Instance { get; private set; }
    private static readonly Queue<Action> executionQueue = new Queue<Action>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void Enqueue(Action action) {
        lock (executionQueue) {
            executionQueue.Enqueue(action);
        }
    }

    private void Update() {
        lock (executionQueue) {
            while (executionQueue.Count > 0) {
                executionQueue.Dequeue().Invoke();
            }
        }
    }
}

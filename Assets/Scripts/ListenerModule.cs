using Meta.WitAi.Dictation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ListenerModule : MonoBehaviour
{
    // Event for user input received
    public event Action<string> OnUserInputReceived;
    [FormerlySerializedAs("dictation")]
    [SerializeField] private DictationService witDictation;
    protected Animator mAnimator;


    public void Start()
    {
        if (witDictation != null) witDictation = FindObjectOfType<DictationService>();
        Debug.Log($"Wit status {witDictation.Active}");
        if (!witDictation.Active) witDictation.Activate();
        Debug.Log($"Wit status {witDictation.Active}");
    }

    private void StopListening()
    {
        witDictation.Deactivate();
    }

    private void OnEnable()
    {
        witDictation.DictationEvents.OnFullTranscription.AddListener(OnFullTranscription);
    }

    private void OnDisable()
    {
        witDictation.DictationEvents.OnFullTranscription.RemoveListener(OnFullTranscription);
    }

    private void OnDestroy()
    {
        StopListening();
    }

    private void OnFullTranscription(string text)
    {
        Debug.Log($"Full transcripiton {text}");
        OnUserInputReceived?.Invoke(text);
        ToggleDictation(false);
    }

    public void ToggleDictation(bool state)
    {
        if (state) witDictation.Activate();
        else witDictation.Deactivate();
    }

}
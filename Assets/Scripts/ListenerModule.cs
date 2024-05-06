using Meta.Voice.Samples.Dictation;
using Meta.WitAi.Dictation;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ListenerModule : MonoBehaviour
{
    // Event for user input received
    public event Action<string> OnUserInputReceived;
    [FormerlySerializedAs("dictation")]
    [SerializeField] private DictationService witDictation;
    public DictationActivation dicatationActivation;
    protected Animator mAnimator;
    public TextMeshProUGUI inputText;
    public bool debug;

    public void Start()
    {
        if (witDictation != null) witDictation = FindObjectOfType<DictationService>();
        Debug.Log($"Wit status {witDictation.Active}");
        //if (!witDictation.Active) witDictation.Activate();
        //Debug.Log($"Wit status {witDictation.Active}");
    }

    private void StopListening()
    {
        witDictation.Deactivate();
    }

    private void OnEnable()
    {
        //witDictation.DictationEvents.OnFullTranscription.AddListener(OnFullTranscription);
    }

    private void OnDisable()
    {
        //witDictation.DictationEvents.OnFullTranscription.RemoveListener(OnFullTranscription);
    }

    private void OnDestroy()
    {
        StopListening();
    }

    /// <summary>
    /// invoked via external ui
    /// </summary>
    public void SubmitVoiceTranscription()
    {
        string text = inputText.text;
        if (debug) text = "I have the following ingredients with me, Eggs, spices, green onions, steak, potatoes, tortilla, bread, sugar, and milk";
        Debug.Log($"Full transcripiton {text}");
        OnUserInputReceived?.Invoke(text);
        ToggleDictation(false);
    }

    private void OnFullTranscription(string text)
    {
        Debug.Log($"Full transcripiton {text}");
        OnUserInputReceived?.Invoke(text);
        ToggleDictation(false);
    }

    public void ToggleDictation(bool state)
    {
        dicatationActivation.ToggleActivation();
        Debug.Log($"Desired state {state}, Wit dicatation state {witDictation.IsRequestActive}");
    }

}
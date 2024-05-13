using Meta.Voice.Samples.Dictation;
using Meta.WitAi.Dictation;
using Meta.WitAi.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ListenerModule : MonoBehaviour
{
    public UnityEvent onDictationComplete;
    // Event for user input received
    public event Action<string> OnUserInputReceived;
    public event Action<string> OnUserHelpInputReceived;
    [FormerlySerializedAs("dictation")]
    [SerializeField] private DictationService witDictation;
    public DictationActivation dicatationActivation;
    protected Animator mAnimator;
    public TextMeshProUGUI recipeInputText;
    public bool debug;

    public enum ListeningMode {
        Recipe,
        Help
    }

    public ListeningMode listeningMode = ListeningMode.Recipe;

    public void Start()
    {
        if (witDictation != null) witDictation = FindObjectOfType<DictationService>();
        Debug.Log($"Wit status {witDictation.Active}");
        //if (!witDictation.Active) witDictation.Activate();
        //Debug.Log($"Wit status {witDictation.Active}");
        //witDictation.DictationEvents.OnComplete.AddListener(OnDictationComplete);
    }

    private void OnDictationComplete(VoiceServiceRequest dictationResult)
    {
        Debug.Log($"end dictation {dictationResult.Results.Transcription.ToString()}");
        onDictationComplete?.Invoke();

    }

    public void SetListeningMode(int mode) {
        listeningMode = (ListeningMode)mode;
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

    /// <summary>
    /// invoked via external ui
    /// </summary>
    public void SubmitVoiceTranscription()
    {
        string text = recipeInputText.text;
        if (debug) text = "I have the following ingredients with me, Eggs, spices, green onions, steak, potatoes, tortilla, bread, sugar, and milk";
        Debug.Log($"Full transcripiton {text}");
        OnUserInputReceived?.Invoke(text);
        ToggleDictation(false);
    }


    public void SubmitHelpTranscription(string helpText) {
        Debug.Log($"Help Text {helpText}");
        OnUserHelpInputReceived?.Invoke(helpText);
    }

    private void OnFullTranscription(string text)
    {
        if (listeningMode == ListeningMode.Recipe) onDictationComplete?.Invoke();
        else if (listeningMode == ListeningMode.Help) SubmitHelpTranscription(text);
        //Debug.Log($"Full transcripiton {text}");
        //OnUserInputReceived?.Invoke(text);
        ToggleDictation(false);
    }

    public void ToggleDictation(bool state)
    {
        dicatationActivation.ToggleActivation(state);
        Debug.Log($"Desired state {state}, Wit dicatation state {witDictation.IsRequestActive}");
    }

}
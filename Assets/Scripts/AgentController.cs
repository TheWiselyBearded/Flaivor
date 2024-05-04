using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Events;

public class AgentController : MonoBehaviour
{
    public CookSessionController cookSessionController;

    public SpeechModule speechModule;
    public ListenerModule listenerModule;
    public ThinkerModule thinkerModule;

    public UnityEvent[] thinkerEvents;

    [System.Serializable]
    public enum AgentState
    {
        Listening,
        Thinking,
        Speaking
    }
    public AgentState agentState;

    private void Awake()
    {
        agentState = AgentState.Listening;
    }

    private void Start()
    {
        listenerModule.OnUserInputReceived += ListenerModule_OnUserInputReceived;
        thinkerModule.OnChatGPTInputReceived += ThinkerModule_OnChatGPTInputReceived;
    }

    

    private void ListenerModule_OnUserInputReceived(string obj)
    {
        thinkerModule.SubmitChat(obj);
        SetMode(AgentState.Thinking);
    }

    private void OnDestroy()
    {
        thinkerModule.OnChatGPTInputReceived -= ThinkerModule_OnChatGPTInputReceived;
    }

    private void ThinkerModule_OnChatGPTInputReceived(string obj)
    {
        Debug.Log($"Thinker Mode response fed to chef");
        SetMode(AgentState.Speaking);
        cookSessionController.CreateReceipe(obj);
    }

    public void SetMode(AgentState newState)
    {
        agentState = newState;
        switch (agentState)
        {
            case AgentState.Listening:
                ApplyListeningModeSettings();
                break;
            case AgentState.Thinking:
                ApplyThinkingModeSettings();
                break;
            case AgentState.Speaking:
                ApplySpeakingModeSettings();
                break;
        }
    }

    void ApplyListeningModeSettings()
    {
        listenerModule.ToggleDictation(true);
    }

    void ApplyThinkingModeSettings()
    {
        listenerModule.ToggleDictation(false);
        InvokeEvents(thinkerEvents);
        // generate ui objects and such for response queue?
    }

    private void InvokeEvents(UnityEvent[] events)
    {
        foreach (UnityEvent uev in events)
        {
            uev?.Invoke();
        }
    }

    void ApplySpeakingModeSettings()
    {

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    public CookSessionController cookSessionController;

    public SpeechModule speechModule;
    public ListenerModule listenerModule;
    public ThinkerModule thinkerModule;

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
        SetMode(AgentState.Speaking);
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
        // generate ui objects and such for response queue?
    }

    void ApplySpeakingModeSettings()
    {

    }

}

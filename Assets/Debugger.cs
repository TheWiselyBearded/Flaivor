using OpenAI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AgentController;

public class Debugger : MonoBehaviour
{
    public TMP_InputField userInput;
    public AgentController agent;

    [SerializeField]
    private RectTransform contentArea;

    [SerializeField]
    private ScrollRect scrollView;

    private void Start()
    {
        agent.thinkerModule.OnChatGPTInputReceived += ThinkerModule_OnChatGPTInputReceived;
    }

    private void ThinkerModule_OnChatGPTInputReceived(string obj)
    {
        var assistantMessageContent = AddNewTextMessageContent();
        assistantMessageContent.text = "Assistant:\n";
        assistantMessageContent.text += obj;
    }

    public void SetAgentThinkingMode() => agent.SetMode(AgentState.Thinking);

    public void SubmitChatRequest()
    {
        agent.thinkerModule.SubmitChatJSON(userInput.text); 
    }

    private TextMeshProUGUI AddNewTextMessageContent()
    {
        var textObject = new GameObject($"{contentArea.childCount + 1}");
        textObject.transform.SetParent(contentArea, false);
        var textMesh = textObject.AddComponent<TextMeshProUGUI>();
        textMesh.fontSize = 24;
#if UNITY_2023_1_OR_NEWER
            textMesh.textWrappingMode = TextWrappingModes.Normal;
#else
        textMesh.enableWordWrapping = true;
#endif
        return textMesh;
    }

}

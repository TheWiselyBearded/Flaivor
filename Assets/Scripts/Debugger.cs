using OpenAI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AgentController;

public class Debugger : MonoBehaviour
{
    public RawImage rawImage;
    public Texture2D debugFridgeTexture;
    public TMP_InputField userInput;
    public AgentController agent;
    public string recipeBookJSON;

    [SerializeField]
    private RectTransform contentArea;

    [SerializeField]
    private ScrollRect scrollView;


    private void Start()
    {
        //agent.thinkerModule.OnChatGPTInputReceived += ThinkerModule_OnChatGPTInputReceived;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AssignTextureToRawImage();
            agent.SetAgentMode(1);
            agent.SubmitChatImageRequest();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            agent.SetAgentMode(1);
            agent.thinkerModule.SubmitChatJSON("I have the following ingredients with me, Eggs, spices, green onions, steak, potatoes, tortilla, bread, sugar, and milk");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadRecipeBookJSON();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            agent.SetAgentMode(1);
            agent.thinkerModule.SubmitChatHelpJSON("How long are avocados good for? I bought these like a week ago");
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            agent.cookSessionController.SwipeRecipesLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            agent.cookSessionController.SwipeRecipesRight();
        }

    }

    public void LoadRecipeBookJSON()
    {
        string recipeJSON = agent.cookSessionController.LoadJsonFromFile(recipeBookJSON);
        if (recipeJSON != null)
        {
            agent.ThinkerModule_OnChatGPTInputReceived(recipeJSON);
        }
    }

    private void ThinkerModule_OnChatGPTInputReceived(string obj)
    {
        var assistantMessageContent = AddNewTextMessageContent();
        assistantMessageContent.text = "Assistant:\n";
        assistantMessageContent.text += obj;
    }

    // Method to assign a Texture2D to the RawImage
    public void AssignTextureToRawImage()
    {
        if (rawImage != null && debugFridgeTexture != null)
        {
            rawImage.texture = debugFridgeTexture;
        }
        else
        {
            Debug.LogError("RawImage or debugFridgeTexture is not assigned!");
        }
    }


    public void SetAgentThinkingMode() => agent.SetMode(AgentState.Thinking);

    // example query: I have the following ingredients with me, Eggs, spices, green onions, steak, potatoes, tortilla, bread, sugar, and milk
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

    public void SetRecipeFirst()
    {
        agent.cookSessionController.SetRecipe(0);
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class AgentController : MonoBehaviour
{
    public CookSessionController cookSessionController;
    public GameObject PFB_HelpResponse;
    public GameObject avatar;

    public SpeechModule speechModule;
    public ListenerModule listenerModule;
    public ThinkerModule thinkerModule;


    private bool requestRecipes = false;
    private string requestString = "";

    public UnityEvent[] thinkerEvents;

    [System.Serializable]
    public enum AgentState
    {
        Listening,
        Thinking,
        Speaking
    }
    public AgentState agentState;
    public UnityEvent<AgentState> OnAgentStateChanged;

    [System.Serializable]
    public enum ThinkingMode
    {
        Recipes,
        Help
    }
    public ThinkingMode thinkingMode = ThinkingMode.Recipes;

    private void Awake()
    {
        agentState = AgentState.Listening;
        avatar.SetActive(false);
    }

    private void Start()
    {
        listenerModule.OnUserInputReceived += ListenerModule_OnUserInputReceived;
        listenerModule.OnUserHelpInputReceived += ListenerModule_OnUserHelpInputReceived;
        thinkerModule.OnChatGPTInputReceived += ThinkerModule_OnChatGPTInputReceived;
        ThinkerModule.OnChatGPTHelpInputReceived += ThinkerModule_OnChatGPTHelpInputReceived;
        thinkerModule.OnTextureGenerated += HandleTextureGenerated;
    }

    /// <summary>
    /// instantiate help prefab window with text
    /// </summary>
    /// <param name="obj"></param>
    private void ThinkerModule_OnChatGPTHelpInputReceived(string obj)
    {
        //GameObject instance = Instantiate(PFB_HelpResponse, null);
        //ResponseWindow window = instance.GetComponent<ResponseWindow>();
        //window.SetResponseText(obj);
    }

    private void ListenerModule_OnUserHelpInputReceived(string obj)
    {
        Debug.Log($"Would submit help text {obj}");
        thinkerModule.SubmitChatHelpJSON(obj);
        
        thinkingMode = ThinkingMode.Help;
        SetMode(AgentState.Thinking);
    }

    private void Update()
    {
        if (requestRecipes)
        {
            requestRecipes = false;
            RecipeChatRequest();
        }
    }

    /// <summary>
    /// invoked via unity button press events
    /// </summary>
    /// <param name="state"></param>
    public void SetAgentMode(int state)
    {
        switch (state)
        {
            case 0:
                SetMode(AgentState.Listening);
                break;
            case 1:
                SetMode(AgentState.Thinking);
                break;
            case 2:
                SetMode(AgentState.Speaking);
                break;
        }
    }

    private string listenerModuleInput;
    private bool listenerModuleInputReceived = false;
    public UnityEvent<string> OnListenerModuleInputReceived;

    private void ListenerModule_OnUserInputReceived(string obj)
    {
        // with new UX, we will need to not immediately switch to thinking mode
        listenerModuleInput = obj;
        listenerModuleInputReceived = true;
        OnListenerModuleInputReceived.Invoke(obj);

        Debug.Log("Listener input received: " + obj);
        SubmitChatRequest();
        // thinkerModule.SubmitChatJSON(obj);
        //thinkerModule.SubmitChat(obj);
        // SetMode(AgentState.Thinking);
    }

    public void SubmitChatRequest()
    {
        if (!listenerModuleInputReceived)
        {
            Debug.LogError("No listener input received");
            return;
        }
        thinkerModule.SubmitChatJSON(listenerModuleInput);
        thinkingMode = ThinkingMode.Recipes;
        SetMode(AgentState.Thinking);

        listenerModuleInputReceived = false;
    }

    private void OnDestroy()
    {
        listenerModule.OnUserInputReceived -= ListenerModule_OnUserInputReceived;
        listenerModule.OnUserHelpInputReceived -= ListenerModule_OnUserHelpInputReceived;
        thinkerModule.OnChatGPTInputReceived -= ThinkerModule_OnChatGPTInputReceived;
        ThinkerModule.OnChatGPTHelpInputReceived -= ThinkerModule_OnChatGPTHelpInputReceived;
        thinkerModule.OnTextureGenerated -= HandleTextureGenerated;
    }

    /// <summary>
    /// called from update for now, set from event of chatgpt input received, saves string as global variable
    /// </summary>
    public void RecipeChatRequest()
    {
        //Task.Run(async () => await ThinkerModule_OnChatGPTInputReceivedTask(obj));
        ThinkerModule_OnChatGPTInputReceivedTask(requestString);
    }

    public void ThinkerModule_OnChatGPTInputReceived(string obj)
    {
        requestString = obj;
        requestRecipes = true;
    }

    int recipeCounter = 0;
    private void HandleTextureGenerated(Texture2D texture) {
        // Handle the texture, e.g., create recipe objects
        Debug.Log("Received texture from ThinkerModule.");
        // Assuming you have a way to identify which recipe the texture belongs to
        Recipe associatedRecipe = cookSessionController.recipeBook.Recipes[recipeCounter]; // Replace with actual logic
        cookSessionController.CreateRecipeObjects(associatedRecipe, texture);
        recipeCounter++;
        if (recipeCounter >3) {
            avatar.SetActive(true);
            SetMode(AgentState.Speaking);
        }
    }

    private async void ThinkerModule_OnChatGPTInputReceivedTask(string obj) {
        Debug.Log("Thinker Mode response fed to chef");
        cookSessionController.CreateRecipeBook(obj);

        if (cookSessionController.recipeBook.Recipes.Count > 0) {
            var tasks = new List<Task<Texture2D>>();
            var semaphore = new SemaphoreSlim(1);  // Adjust the number to limit concurrent requests

            foreach (var recipe in cookSessionController.recipeBook.Recipes) {
                tasks.Add(thinkerModule.SubmitImageGeneratorRequestAsync(recipe.RecipeName, recipe.Description, semaphore));
            }

            var results = await Task.WhenAll(tasks).ConfigureAwait(true);

            for (int i = 0; i < results.Length; i++) {
                if (results[i] != null) {
                    cookSessionController.CreateRecipeObjects(cookSessionController.recipeBook.Recipes[i], results[i]);
                }
                else {
                    Debug.LogError($"Failed to generate image for {cookSessionController.recipeBook.Recipes[i].RecipeName}");
                }
            }
        }

        avatar.SetActive(true);
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
        OnAgentStateChanged.Invoke(agentState);
    }

    void ApplyListeningModeSettings()
    {
        Debug.Log("Listen mode settings");
        listenerModule.ToggleDictation(true);
    }

    void ApplyThinkingModeSettings()
    {
        listenerModule.ToggleDictation(false);
        InvokeEvents(thinkerEvents);
        // generate ui objects and such for response queue?
    }


    /// <summary>
    /// invoked via unity button event
    /// </summary>
    public void SubmitChatImageRequest() => thinkerModule.SubmitScreenshotChatRequest();

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

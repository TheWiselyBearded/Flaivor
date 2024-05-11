using Newtonsoft.Json;
using OVRSimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CookSessionController : MonoBehaviour
{
    [SerializeField] public RecipeBook recipeBook;
    public Recipe recipe;
    public int StepIndex;
    public int SubStepIndex;


    [SerializeField] private GameObject recipeStepUIPrefab; // Drag your prefab here in the Inspector
    [SerializeField] private GameObject recipeMediumUIPrefab; // Drag your prefab here in the Inspector
    [SerializeField] private GameObject recipeFullUIPrefab; // Drag your prefab here in the Inspector
    [SerializeField] private Transform uiParent; // Assign a parent transform to control the layout
    public GameObject userObject;

    private Vector3 spawnOffset = new Vector3(0f, 0f, 0.25f); // Adjust the offset as needed
    private Vector3 spawnDirection;

    public GameObject[] fullMenuUIs = new GameObject[3];
    public GameObject[] mediumMenuUIs = new GameObject[3];
    public GameObject recipeProgressUI;
    protected int numCallsMedium = 1;
    protected int numCallsFull = 1;
    public bool debugMode;

    private void Awake()
    {
        //recipes = new List<Recipe>();
        uiParent = null;
    }

    private void Start()
    {
        RecipeMediumUI.OnChooseDishMediumReceived += RecipeMediumUI_OnChooseDishMediumReceived;
        InstructionStepProgressUI.OnProgressNextStepReceived += InstructionStepProgressUI_OnProgressStepReceived;
        InstructionStepProgressUI.OnProgressPrevStepReceived += InstructionStepProgressUI_OnProgressPrevStepReceived;
    }

    

    private void OnDestroy() {
        RecipeMediumUI.OnChooseDishMediumReceived -= RecipeMediumUI_OnChooseDishMediumReceived;
        InstructionStepProgressUI.OnProgressNextStepReceived -= InstructionStepProgressUI_OnProgressStepReceived;
        InstructionStepProgressUI.OnProgressPrevStepReceived -= InstructionStepProgressUI_OnProgressPrevStepReceived;
    }

    private void InstructionStepProgressUI_OnProgressPrevStepReceived() {
        Debug.Log($"Step index {StepIndex}, substep index {SubStepIndex}");
        if (StepIndex <= 0 && SubStepIndex <= 0) {
            Debug.Log("Reached min steps");
            return;
        }

        SubStepIndex--;
        if (SubStepIndex < 0) {
            StepIndex--;
            if (StepIndex < 0) {
                StepIndex = 0;
                SubStepIndex = 0;
            } else {
                SubStepIndex = recipe.Instructions[StepIndex].SubSteps.Count - 1;
            }
            Debug.Log($"Step index now {StepIndex} and substep now {SubStepIndex}");
        }
        recipeProgressUI.GetComponent<InstructionStepProgressUI>().SetInstructionSubstep(recipe.Instructions[StepIndex].Description, recipe.Instructions[StepIndex].SubSteps[SubStepIndex]);
    }

    private void InstructionStepProgressUI_OnProgressStepReceived() {
        if (StepIndex > recipe.Instructions.Count - 1) {
            Debug.Log("Reached max steps");
            return;
        }

        SubStepIndex++;
        if (SubStepIndex >= recipe.Instructions[StepIndex].SubSteps.Count) {
            StepIndex++;
            SubStepIndex = 0;
        }
        if (StepIndex >= recipe.Instructions.Count) {
            Debug.Log("Reached max steps");
            return;
        }
        recipeProgressUI.GetComponent<InstructionStepProgressUI>().SetInstructionSubstep(recipe.Instructions[StepIndex].Description, recipe.Instructions[StepIndex].SubSteps[SubStepIndex]);
    }



    public void GoBackStep() {
        recipe.currentStepIndex--;
    }

    public void GoNextStep() {
        recipe.currentStepIndex++;
    }
    

    /// <summary>
    /// match string of recipe name to index of recipe in recipe book list
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private void RecipeMediumUI_OnChooseDishMediumReceived(string obj)
    {
        for (int i = 0; i < recipeBook.Recipes.Count; i++) {
            if (ContainsMostOfSequence(recipeBook.Recipes[i].RecipeName, obj)) {
                SetRecipe(i);
            }
        }
    }

    /// <summary>
    /// Checks if a string contains most of the characters in sequence from another string.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="sequence"></param>
    /// <returns></returns>
    private bool ContainsMostOfSequence(string str, string sequence)
    {
        int m = str.Length;
        int n = sequence.Length;

        // Create a 2D array to store the lengths of LCS
        int[,] LCSuff = new int[m + 1, n + 1];
        int result = 0; // To store length of the longest common substring

        // Build LCSuff[m+1][n+1] in bottom-up fashion
        for (int i = 0; i <= m; i++)
        {
            for (int j = 0; j <= n; j++)
            {
                if (i == 0 || j == 0)
                    LCSuff[i, j] = 0;
                else if (str[i - 1] == sequence[j - 1])
                {
                    LCSuff[i, j] = LCSuff[i - 1, j - 1] + 1;
                    result = Math.Max(result, LCSuff[i, j]);
                }
                else
                    LCSuff[i, j] = 0;
            }
        }

        // Check if the length of LCS is at least half of the length of the sequence
        return result >= sequence.Length / 2;
    }


    public void CreateRecipeBook(string jsonRecipes)
    {
        //recipes = JsonConvert.DeserializeObject<List<Recipe>>(jsonRecipes);

        try
        {
            //jsonRecipes = EnsureJsonWrappedWithRecipesKey(jsonRecipes);
            //Debug.Log(jsonRecipes);
            recipeBook = JsonConvert.DeserializeObject<RecipeBook>(jsonRecipes);
            if (debugMode)
            {
                foreach (Recipe recipe in recipeBook.Recipes)
                {
                    Debug.Log($"Recipe: {recipe.RecipeName}");
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        Debug.Log($"Ingredient: {ingredient.Key}, Quantity: {ingredient.Value}");
                    }
                    foreach (Instruction instruction in recipe.Instructions)
                    {
                        Debug.Log($"Step {instruction.StepNumber}: {instruction.Description}");
                        foreach (string subStep in instruction.SubSteps)
                        {
                            Debug.Log($"Sub-step: {subStep}");
                        }
                    }
                }
            }

            if (debugMode) SaveRecipesToJson(jsonRecipes);

        }
        catch (JsonSerializationException ex)
        {
            Debug.LogError("JsonSerializationException: " + ex.Message);
        }
    }

    public void SaveRecipesToJson(string json)
    {
        if (debugMode)
        {
            string fileName = $"recipes_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.json";
            string directoryPath = Application.streamingAssetsPath;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string path = Path.Combine(directoryPath, fileName);

            File.WriteAllText(path, json);
            Debug.Log($"Recipes saved to: {path}");
        }
    }

    public string LoadJsonFromFile(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        else
        {
            Debug.LogError($"File not found: {path}");
            return null;
        }
    }


    public void SetRecipe(int _recipeIndex) {

        for (int i = 0; i < mediumMenuUIs.Length; i++) {
            GameObject gRef = mediumMenuUIs[i];
            // delete if not selected
            if (i == _recipeIndex) {
                SetRecipe(recipeBook.Recipes[_recipeIndex]);
            }
            Destroy(gRef);
            mediumMenuUIs[i] = null;
        }

    }
    public void SetRecipe(Recipe _recipe)
    {
        recipe = _recipe;
        CreateRecipeFullUI(recipe);
        CreateRecipeStepUI(0);        
    }

    public void CreateRecipeObjects(Recipe _recipe)
    {
        // Reset spawnDirection for each new set of recipe objects
        spawnDirection = userObject.transform.forward;

        // instantiate prefabs
        CreateRecipeMediumUI(_recipe);
        //CreateRecipeFullUI(_recipe);
    }

    
    public void CreateRecipeMediumUI(Recipe recipe)
    {
        if (recipeMediumUIPrefab == null)
        {
            Debug.LogError("RecipeMediumUIPrefab is not assigned in the inspector!");
            return;
        }

        // Calculate the spawn position in front of the user
        Vector3 spawnPosition = userObject.transform.position + spawnDirection * 1.1f + (spawnOffset * numCallsMedium); // Adjust the distance as needed
        // Instantiate the prefab as a child of uiParent
        GameObject instance = Instantiate(recipeMediumUIPrefab, spawnPosition, Quaternion.identity, uiParent);

        numCallsMedium++;

        // Get the RecipeMediumUI component and set the recipe details
        RecipeMediumUI recipeUI = instance.GetComponent<RecipeMediumUI>();
        if (recipeUI != null)
        {
            string ingredientsText = FormatIngredients(recipe.Ingredients);
            recipeUI.SetRecipeUI(recipe.RecipeName, recipe.Description, ingredientsText);
        }
        else
        {
            Debug.LogError("RecipeMediumUI component not found on the instantiated prefab!");
        }

        mediumMenuUIs[numCallsMedium - 2] = instance;
    }

    public void CreateRecipeFullUI(Recipe recipe)
    {
        if (recipeFullUIPrefab == null)
        {
            Debug.LogError("RecipeMediumUIPrefab is not assigned in the inspector!");
            return;
        }

        Vector3 spawnPosition = userObject.transform.position + spawnDirection * 1.1f + (spawnOffset * numCallsFull); // Adjust the distance as needed

        GameObject instance = Instantiate(recipeFullUIPrefab, spawnPosition, Quaternion.identity, uiParent);

        numCallsFull++;

        // Get the RecipeMediumUI component and set the recipe details
        RecipeFullUI recipeUI = instance.GetComponent<RecipeFullUI>();
        if (recipeUI != null)
        {
            string ingredientsText = FormatIngredients(recipe.Ingredients);
            recipeUI.SetRecipeUI(recipe.RecipeName, recipe.Description, ingredientsText);
            recipeUI.SetInstructionsUI(recipe.Instructions);
        }
        else
        {
            Debug.LogError("RecipeMediumUI component not found on the instantiated prefab!");
        }

        fullMenuUIs[numCallsFull - 2] = instance;
    }

    public void CreateRecipeStepUI(int stepIndex)
    {
        if (recipeStepUIPrefab == null)
        {
            Debug.LogError("RecipeMediumUIPrefab is not assigned in the inspector!");
            return;
        }
        Vector3 spawnPosition = userObject.transform.position + spawnDirection * 0.2f + (spawnOffset); // Adjust the distance as needed
        // Instantiate the prefab as a child of uiParent
        GameObject instance = Instantiate(recipeStepUIPrefab, spawnPosition, Quaternion.identity, uiParent);
        recipeProgressUI = instance;
        // Get the RecipeMediumUI component and set the recipe details
        InstructionStepProgressUI recipeUI = instance.GetComponent<InstructionStepProgressUI>();
        if (recipeUI != null)
        {

            recipeUI.SetInstructionStepUI(recipe.Instructions[stepIndex].StepNumber.ToString() + " " + recipe.Instructions[stepIndex].Description,
                recipe.Instructions[stepIndex].SubSteps);
        }
        else
        {
            Debug.LogError("RecipeMediumUI component not found on the instantiated prefab!");
        }
    }

    // Helper method to format dictionary ingredients into a single string
    private string FormatIngredients(Dictionary<string, string> ingredients)
    {
        string ingredientsText = "";
        foreach (KeyValuePair<string, string> ingredient in ingredients)
        {
            ingredientsText += $"{ingredient.Key}: {ingredient.Value}\n";
        }
        return ingredientsText.TrimEnd(); // Remove the last newline character for cleaner formatting
    }

    public static string EnsureJsonWrappedWithRecipesKey(string jsonString) {
        // Trim any whitespace that might affect the check
        jsonString = jsonString.Trim();

        // Check if the JSON string starts with an array indicator '['
        if (!jsonString.Contains("recipes")) {
            // The JSON is not wrapped with "recipes" key, wrap it
            jsonString = "{\"recipes\":" + jsonString + "}";
        }

        return jsonString;
    }

}

[System.Serializable]
public class RecipeBook
{
    [JsonProperty("recipes")]
    public List<Recipe> Recipes { get; set; }
}

[System.Serializable]
public class Recipe
{
    [JsonProperty("recipe_name")]
    public string RecipeName { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    public int currentStepIndex;

    public Dictionary<string, string> Ingredients { get; set; }

    public List<Instruction> Instructions { get; set; }
}

[System.Serializable]
public class Instruction
{
    [JsonProperty("step_number")]
    public int StepNumber { get; set; }

    public string Description { get; set; }

    [JsonProperty("sub_steps")]
    public List<string> SubSteps { get; set; }
}
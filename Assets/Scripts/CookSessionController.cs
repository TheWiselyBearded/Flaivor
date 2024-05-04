using Newtonsoft.Json;
using OVRSimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookSessionController : MonoBehaviour
{
    [SerializeField] public RecipeBook recipeBook;
    public Recipe recipe;

    [SerializeField] private GameObject recipeMediumUIPrefab; // Drag your prefab here in the Inspector
    [SerializeField] private Transform uiParent; // Assign a parent transform to control the layout

    private void Awake()
    {
        //recipes = new List<Recipe>();
        uiParent = null;
    }

    public void SequenceSteps()
    {
        recipe.currentStepIndex++;
    }

    public void GoBackStep()
    {
        recipe.currentStepIndex--;
    }

    public void GoNextStep()
    {
        recipe.currentStepIndex++;
    }

    public void CreateReceipe(string jsonRecipes)
    {
        //recipes = JsonConvert.DeserializeObject<List<Recipe>>(jsonRecipes);

        try
        {
            recipeBook = JsonConvert.DeserializeObject<RecipeBook>(jsonRecipes);

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
        catch (JsonSerializationException ex)
        {
            Debug.LogError("JsonSerializationException: " + ex.Message);
        }
    }


    public void SetRecipe(int _recipe) => SetRecipe(recipeBook.Recipes[0]);
    public void SetRecipe(Recipe _recipe)
    {
        recipe = _recipe;
        // instantiate prefabs
        CreateRecipeUI(recipe);

    }

    // Example method to create and set up the recipe UI
    public void CreateRecipeUI(Recipe recipe)
    {
        if (recipeMediumUIPrefab == null)
        {
            Debug.LogError("RecipeMediumUIPrefab is not assigned in the inspector!");
            return;
        }

        // Instantiate the prefab as a child of uiParent
        GameObject instance = Instantiate(recipeMediumUIPrefab, uiParent);

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
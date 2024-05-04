using Newtonsoft.Json;
using OVRSimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookSessionController : MonoBehaviour
{
    [SerializeField] public RecipeBook RecipeBook;
    public Recipe recipe;

    private void Awake()
    {
        //recipes = new List<Recipe>();
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
            RecipeBook recipeBook = JsonConvert.DeserializeObject<RecipeBook>(jsonRecipes);

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



    public void SetRecipe(Recipe _recipe) => recipe= _recipe;


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
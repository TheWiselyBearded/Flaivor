using Newtonsoft.Json;
using OVRSimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookSessionController : MonoBehaviour
{
    public List<Recipe> recipes;
    public Recipe recipe;

    private void Awake()
    {
        recipes = new List<Recipe>();
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
        recipes = JsonConvert.DeserializeObject<List<Recipe>>(jsonRecipes);

        foreach (Recipe recipe in recipes)
        {
            Debug.Log($"Recipe: {recipe.RecipeName}");
            foreach (KeyValuePair<string, string> ingredient in recipe.Ingredients)
            {
                Debug.Log($"Ingredient: {ingredient.Key}, Amount: {ingredient.Value}");
            }

            foreach (Instruction instruction in recipe.Instructions)
            {
                Debug.Log($"Step {instruction.StepNumber}: {instruction.Description}");
                foreach (string subStep in instruction.SubSteps)
                {
                    Debug.Log($"Sub-step: {subStep}");
                }
            }
            Debug.Log($"Successfully created recipe {recipe.RecipeName}");
        }
    }

    public void SetRecipe(Recipe _recipe) => recipe= _recipe;


}

[System.Serializable]
public class Recipe
{
    public string Description { get; set; }
    public int currentStepIndex { get; set; }
    public string RecipeName { get; set; }
    public Dictionary<string, string> Ingredients { get; set; }
    public List<Instruction> Instructions { get; set; }
}
[System.Serializable]
public class Instruction
{
    public int StepNumber { get; set; }
    public string Description { get; set; }
    public List<string> SubSteps { get; set; }
}
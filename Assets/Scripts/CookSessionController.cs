using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookSessionController : MonoBehaviour
{
    public Recipe recipe;


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

    public void SetRecipe(Recipe _recipe) => recipe= _recipe;


}

[System.Serializable]
public class Recipe
{
    public string Name;
    public string Description;
    public int currentStepIndex;
    public RecipeStep[] recipeSteps;
}
[System.Serializable]
public class Ingredients
{

}
[System.Serializable]
public class RecipeStep
{

}
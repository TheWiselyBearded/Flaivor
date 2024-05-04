using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecipeFullUI : MonoBehaviour
{
    public TextMeshProUGUI recipeName;
    public TextMeshProUGUI recipeDescription;
    public TextMeshProUGUI ingredientDescription;
    public InstructionUI[] instructionUIs;

    public void SetRecipeUI(string _recipeName, string _recipeDescription, string _ingredientDescription)
    {
        recipeName.text = _recipeName;
        recipeDescription.text = _recipeDescription;
        ingredientDescription.text = _ingredientDescription;
    }

    public void SetInstructionsUI(string _instructions)
    {
        Debug.Log($"Instructions to be implemented {_instructions}");
    }
}

[System.Serializable]
public class InstructionUI
{
    public TextMeshProUGUI instructionName;
    public TextMeshProUGUI instructionDescription;
}

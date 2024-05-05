using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecipeMediumUI : MonoBehaviour
{
    public TextMeshProUGUI recipeName;
    public TextMeshProUGUI recipeDescription;
    public TextMeshProUGUI ingredientDescription;
    

    public void SetRecipeUI(string _recipeName, string _recipeDescription, string _ingredientDescription)
    {
        recipeName.text = _recipeName;
        recipeDescription.text = _recipeDescription;
        ingredientDescription.text = _ingredientDescription;
    }
}

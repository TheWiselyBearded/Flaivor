using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RecipeMediumUI : MonoBehaviour
{
    public TextMeshProUGUI recipeName;
    public TextMeshProUGUI recipeDescription;
    public TextMeshProUGUI ingredientDescription;
    
    public static event Action<string> OnChooseDishMediumReceived;

    public void SetRecipeUI(string _recipeName, string _recipeDescription, string _ingredientDescription)
    {
        recipeName.text = _recipeName;
        recipeDescription.text = _recipeDescription;
        ingredientDescription.text = _ingredientDescription;
    }

    /// <summary>
    /// wired via unity gui
    /// </summary>
    public void ChooseDishButtonPress()
    {
        OnChooseDishMediumReceived?.Invoke(recipeName.text);
    }
}

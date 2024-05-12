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
    public RawImage dishImage;

    public CanvasGroup canvasGroup;

    public static event Action<string> OnChooseDishMediumReceived;

    public void SetRecipeUI(string _recipeName, string _recipeDescription, string _ingredientDescription, Texture _recipeImage)
    {
        recipeName.text = _recipeName;
        recipeDescription.text = _recipeDescription;
        ingredientDescription.text = _ingredientDescription;
        dishImage.texture = _recipeImage;
    }

    /// <summary>
    /// wired via unity gui
    /// </summary>
    public void ChooseDishButtonPress()
    {
        OnChooseDishMediumReceived?.Invoke(recipeName.text);
    }
}

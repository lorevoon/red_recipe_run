using UnityEngine;
using TMPro;

public class RecipeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ingredientText;

    public void Initialize(EIngredient ingredient)
    {
        if (ingredientText != null)
        {
            ingredientText.text = "• " + ingredient.ToString();
        }
        else
        {
            Debug.LogError("RecipeUI: ingredientText reference is missing!");
        }
    }
} 
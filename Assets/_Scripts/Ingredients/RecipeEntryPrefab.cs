using UnityEngine;
using TMPro;

public class RecipeEntryPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ingredientText;
    
    public void SetIngredient(EIngredient ingredient)
    {
        if (ingredientText != null)
        {
            ingredientText.text = "• " + ingredient.ToString();
        }
        else
        {
            Debug.LogError("RecipeEntryPrefab: ingredientText reference is missing!");
        }
    }
} 
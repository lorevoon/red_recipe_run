using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IngredientBadgeUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text ingredientName;
    [SerializeField] private TMP_Text countText; // New field for quantity

    public void Initialize(EIngredient ingredient, int count)
    {
        // Existing sprite loading logic
        string path = $"FoodSprites/{ingredient}";
        Sprite sprite = Resources.Load<Sprite>(path);
        
        if(sprite == null) 
            Debug.LogError($"Missing sprite: {path}");
        else
            icon.sprite = sprite;

        // Existing name formatting
        ingredientName.text = FormatIngredientName(ingredient.ToString());
        
        // New quantity display
        countText.text = $"x{count}";
    }

    private string FormatIngredientName(string rawName)
    {
        return System.Text.RegularExpressions.Regex.Replace(
            rawName,
            "(\\B[A-Z])",
            " $1"
        );
    }
}
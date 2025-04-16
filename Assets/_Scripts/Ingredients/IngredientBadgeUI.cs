using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IngredientBadgeUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text ingredientName;

    public void Initialize(EIngredient ingredient)
    {
        string path = $"FoodSprites/{ingredient}";
        Sprite sprite = Resources.Load<Sprite>(path);
    
        if(sprite == null) 
            Debug.LogError($"Missing sprite: {path}");
        else
            icon.sprite = sprite;

        ingredientName.text = FormatIngredientName(ingredient.ToString());
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
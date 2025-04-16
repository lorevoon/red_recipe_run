using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IngredientBadgeUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text ingredientName;

    public void Initialize(EIngredient ingredient)
    {
        string path = $"FreePixelFood/Sprite/Food/{ingredient}";
        icon.sprite = Resources.Load<Sprite>(path);
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
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public EIngredient IngredientType;
    [SerializeField] private float weight = 1.0f;

    // Public properties for controlled access
    // public Ingredients IngredientType => ingredientType;
    // public float Weight => weight;
}
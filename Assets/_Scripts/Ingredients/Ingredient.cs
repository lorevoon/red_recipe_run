using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField] private Ingredients ingredientType;
    [SerializeField] private float weight = 1.0f; 

    // Public properties for controlled access
    public Ingredients IngredientType => ingredientType;
    public float Weight => weight;
}
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class RecipeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text recipeNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Transform ingredientsContainer;
    [SerializeField] private GameObject ingredientBadgePrefab;

    public void Initialize(RecipeList.Recipe recipe)
    {
        // Set basic info
        recipeNameText.text = recipe.RecipeName;
        descriptionText.text = recipe.Description;

        // Clear existing ingredients
        foreach (Transform child in ingredientsContainer)
            Destroy(child.gameObject);

        // Create ingredient badges
        foreach (var ingredient in recipe.Ingredients)
        {
            var badge = Instantiate(ingredientBadgePrefab, ingredientsContainer);
            badge.GetComponent<IngredientBadgeUI>().Initialize(ingredient);
        }
    }
}

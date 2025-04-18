using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipeManager : Singleton<RecipeManager>
{

    [Header("UI Configuration")]
    [SerializeField] private GameObject recipeEntryPrefab;
    [SerializeField] private GameObject RecipePanel;
    [SerializeField] private Transform scrollViewContent;


    private SRecipe currentRecipe;
    private bool isRecipeOpen = false;

    void Start()
    {
        GenerateNewRecipe();
    }

    public void GenerateNewRecipe()
    {
        int randomIndex = Random.Range(0, RecipeList.AllRecipes.Count);
        currentRecipe = RecipeList.AllRecipes[randomIndex];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            isRecipeOpen = !isRecipeOpen;
            ToggleRecipe(isRecipeOpen);
        }
    }

    public void ToggleRecipe(bool isOpen = true)
    {
        RecipePanel.SetActive(isOpen);

        if (isOpen)
        {
            UpdateRecipeUI();
        }
    }

    private void UpdateRecipeUI()
    {
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }
        var entry = Instantiate(recipeEntryPrefab, scrollViewContent);
        entry.GetComponent<RecipeUI>().Initialize(currentRecipe);
    }

    public SRecipe GetCurrentRecipe()
    {
        return currentRecipe;
    }

    public bool IsInRecipe(EIngredient ingredient)
    {
        return currentRecipe.Ingredients.TryGetValue(ingredient, out int count) && count > 0;
    }

    public bool SubtractFromRecipe(EIngredient ingredient)
    {
        if (!(currentRecipe.Ingredients.TryGetValue(ingredient, out int count) && count > 0)) return false;
        currentRecipe.Ingredients[ingredient] = count - 1;
        return true;
    }
}


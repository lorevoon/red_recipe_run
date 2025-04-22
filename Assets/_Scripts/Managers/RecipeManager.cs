using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Random = UnityEngine.Random;

public class RecipeManager : Singleton<RecipeManager>
{

    [Header("UI Configuration")]
    [SerializeField] private GameObject recipeEntryPrefab;
    [SerializeField] private GameObject RecipePanel;
    [SerializeField] private Transform scrollViewContent;


    private SRecipe currentRecipe;
    public Dictionary<EIngredient, int> unspawnedIngredientsInRecipe;
    private int ingredientsLeft;
    private bool isRecipeOpen = false;
    public int difficulty = 0;
 


    void Start()
    {
        GenerateNewRecipe();
    }

    public void GenerateNewRecipe()
    {
        List<SRecipe> possibleRecipes = new List<SRecipe>();
        
        switch(difficulty)
        {
            case 0: // 8-11 ingredients
                possibleRecipes = RecipeList.AllRecipes
                    .Where(recipe => GetTotalIngredients(recipe) >= 8 && GetTotalIngredients(recipe) <= 11)
                    .ToList();
                break;
                
            case 1: // Exactly 12 ingredients
                possibleRecipes = RecipeList.AllRecipes
                    .Where(recipe => GetTotalIngredients(recipe) == 12)
                    .ToList();
                break;
                
            case 2: // 13-15 ingredients
                possibleRecipes = RecipeList.AllRecipes
                    .Where(recipe => GetTotalIngredients(recipe) >= 13 && GetTotalIngredients(recipe) <= 15)
                    .ToList();
                break;
        }

        // Fallback to all recipes if none match difficulty
        if(!possibleRecipes.Any())
        {
            Debug.LogWarning($"No recipes found for difficulty {difficulty}. Using all recipes.");
            possibleRecipes = RecipeList.AllRecipes;
        }

        // Select random recipe
        int randomIndex = Random.Range(0, possibleRecipes.Count);
        currentRecipe = possibleRecipes[randomIndex];
        ingredientsLeft = GetTotalIngredients(currentRecipe);
        unspawnedIngredientsInRecipe = currentRecipe.Ingredients;
        Debug.Log(currentRecipe.RecipeName);
    }


    private int GetTotalIngredients(SRecipe recipe)
    {
        return recipe.Ingredients.Sum(pair => pair.Value);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            isRecipeOpen = !isRecipeOpen;
            ToggleRecipe(isRecipeOpen);
        }

        // if (InventoryManager.Instance.CheckRecipeComplete())
        // {
        //     if (difficulty < 2){
        //         difficulty += 1;
        //     }
        //     GenerateNewRecipe();
        //     UpdateRecipeUI();
        // }
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
        if (scrollViewContent.TryGetComponent<RectTransform>(out var rectTransform))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
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

    public EIngredient GetRandomIngredientInRecipe()
    {
        List<EIngredient> weightedList = new List<EIngredient>();

        foreach (var pair in unspawnedIngredientsInRecipe)
        {
            for (int i = 0; i < pair.Value; i++)
            {
                weightedList.Add(pair.Key);
            }
        }
        
        if (ingredientsLeft <= 0 || weightedList.Count <= 0)
        {
            Debug.Log("no ingredients left. generating a random one");
            Array values = Enum.GetValues(typeof(EIngredient));
            return (EIngredient)values.GetValue(Random.Range(0, values.Length-1));
        }
        
        EIngredient selected = weightedList[Random.Range(0, weightedList.Count)];
        
        unspawnedIngredientsInRecipe[selected]--;
        if (unspawnedIngredientsInRecipe[selected] <= 0)
        {
            unspawnedIngredientsInRecipe.Remove(selected);
        }

        ingredientsLeft--;

        Debug.Log("generating " + selected);
        return selected;
    }
}


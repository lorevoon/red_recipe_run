using System;
using System.Collections.Generic;
using UnityEngine;
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

    void Start()
    {
        GenerateNewRecipe();
    }

    public void GenerateNewRecipe()
    {
        int randomIndex = Random.Range(0, RecipeList.AllRecipes.Count);
        currentRecipe = RecipeList.AllRecipes[randomIndex];
        unspawnedIngredientsInRecipe = currentRecipe.Ingredients;
        foreach (var pair in unspawnedIngredientsInRecipe)
        {
            ingredientsLeft += pair.Value;
        }
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

    public EIngredient GetRandomIngredientInRecipe()
    {
        if (ingredientsLeft <= 0)
        {
            Array values = Enum.GetValues(typeof(EIngredient));
            return (EIngredient)values.GetValue(Random.Range(0, values.Length-1));
        }
        
        List<EIngredient> weightedList = new List<EIngredient>();

        foreach (var pair in unspawnedIngredientsInRecipe)
        {
            for (int i = 0; i < pair.Value; i++)
            {
                weightedList.Add(pair.Key);
            }
        }
        
        EIngredient selected = weightedList[Random.Range(0, weightedList.Count)];
        
        unspawnedIngredientsInRecipe[selected]--;
        if (unspawnedIngredientsInRecipe[selected] <= 0)
        {
            unspawnedIngredientsInRecipe.Remove(selected);
        }

        ingredientsLeft--;

        return selected;
    }
}


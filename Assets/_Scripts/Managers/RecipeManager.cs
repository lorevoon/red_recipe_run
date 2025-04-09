using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    private List<EIngredient> currentRecipe = new List<EIngredient>();
    private List<EIngredient> playerInventory = new List<EIngredient>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GenerateNewRecipe()
    {
        currentRecipe.Clear();
        int recipeLength = Random.Range(2, 5); // Random length between 2 and 4 ingredients
        
        // Get all possible ingredients
        EIngredient[] allIngredients = (EIngredient[])System.Enum.GetValues(typeof(EIngredient));
        
        // Shuffle the ingredients
        for (int i = 0; i < allIngredients.Length; i++)
        {
            int randomIndex = Random.Range(i, allIngredients.Length);
            EIngredient temp = allIngredients[i];
            allIngredients[i] = allIngredients[randomIndex];
            allIngredients[randomIndex] = temp;
        }
        
        // Take the first recipeLength ingredients
        currentRecipe = allIngredients.Take(recipeLength).ToList();
    }

    public List<EIngredient> GetCurrentRecipe()
    {
        return new List<EIngredient>(currentRecipe);
    }

    public void AddItemToInventory(EIngredient item)
    {
        int maxInventorySize = (int)PlayerProgress.Instance.GetUpgradeValue(EUpgradeType.InventorySpace);
        if (playerInventory.Count < maxInventorySize)
        {
            playerInventory.Add(item);
        }
        else
        {
            Debug.Log("Inventory is full!");
        }
    }

    public void RemoveItemFromInventory(EIngredient item)
    {
        if (playerInventory.Contains(item))
        {
            playerInventory.Remove(item);
        }
    }

    public List<EIngredient> GetInventory()
    {
        return new List<EIngredient>(playerInventory);
    }

    public bool HasAllRecipeIngredients()
    {
        foreach (var ingredient in currentRecipe)
        {
            if (!playerInventory.Contains(ingredient))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearInventory()
    {
        playerInventory.Clear();
    }

    public int GetInventorySpace()
    {
        return (int)PlayerProgress.Instance.GetUpgradeValue(EUpgradeType.InventorySpace);
    }

    public int GetUsedInventorySpace()
    {
        return playerInventory.Count;
    }
} 
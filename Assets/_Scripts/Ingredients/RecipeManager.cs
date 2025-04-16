using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    [Header("UI Configuration")]
    [SerializeField] private GameObject recipeEntryPrefab;
    [SerializeField] private Transform scrollViewContent;

    private RecipeList.Recipe currentRecipe;
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

    void Start()
    {
        GenerateNewRecipe();
        UpdateRecipeUI();
    }

    public void GenerateNewRecipe()
    {
        int randomIndex = Random.Range(0, RecipeList.AllRecipes.Count);
        currentRecipe = RecipeList.AllRecipes[randomIndex];
    }

    private void UpdateRecipeUI()
    {
        // Clear existing entries
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // Create a single entry for the current recipe
        var entry = Instantiate(recipeEntryPrefab, scrollViewContent);
        entry.GetComponent<RecipeUI>().Initialize(currentRecipe); // Pass the entire recipe
    }

    public RecipeList.Recipe GetCurrentRecipe()
    {
        return currentRecipe;
    }

    public bool HasAllRecipeIngredients()
    {
        foreach (var ingredient in currentRecipe.Ingredients)
        {
            if (!playerInventory.Contains(ingredient))
            {
                return false;
            }
        }
        return true;
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
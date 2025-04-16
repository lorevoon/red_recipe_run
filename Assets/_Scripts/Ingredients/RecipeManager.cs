using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    [Header("UI Configuration")]
    [SerializeField] private GameObject recipeEntryPrefab;
    [SerializeField] private Transform scrollViewContent;

    private List<EIngredient> currentRecipe = new List<EIngredient>();
    private RecipeList.Recipe selectedRecipe;
    private List<EIngredient> playerInventory = new List<EIngredient>();
    private int Difficulty;

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
    }

    public void GenerateNewRecipe()
    {
        currentRecipe.Clear();
        int randomIndex = Random.Range(0, RecipeList.AllRecipes.Count);
        RecipeList.Recipe selectedRecipe = RecipeList.AllRecipes[randomIndex];
        currentRecipe = new List<EIngredient>(selectedRecipe.Ingredients);
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
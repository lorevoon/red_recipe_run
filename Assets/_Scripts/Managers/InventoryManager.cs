using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventoryContent;
    [SerializeField] private Transform recipeContent;
    [SerializeField] private GameObject itemPrefab;
    
    private List<EIngredient> inventory = new List<EIngredient>();
    private List<EIngredient> currentRecipe = new List<EIngredient>();
    private bool isInventoryOpen = false;

    // Reference to the IngredientItem prefab for dropping
    [SerializeField] private GameObject ingredientPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(GameObject panel, Transform invContent, Transform recContent, GameObject prefab)
    {
        Debug.Log("InventoryManager: Initialize called");
        inventoryPanel = panel;
        inventoryContent = invContent;
        recipeContent = recContent;
        itemPrefab = prefab;
        
        if (inventoryPanel != null)
        {
            Debug.Log("InventoryManager: Setting panel inactive initially");
            inventoryPanel.SetActive(false);
            GenerateNewRecipe();
        }
        else
        {
            Debug.LogError("InventoryManager: inventoryPanel is null during initialization!");
        }
    }

    private void GenerateNewRecipe()
    {
        currentRecipe.Clear();
        
        // Get all available ingredients
        EIngredient[] allIngredients = (EIngredient[])System.Enum.GetValues(typeof(EIngredient));
        List<EIngredient> availableIngredients = new List<EIngredient>(allIngredients);
        
        // Randomly select 2-4 ingredients
        int recipeSize = Random.Range(2, 5);
        for (int i = 0; i < recipeSize && availableIngredients.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableIngredients.Count);
            currentRecipe.Add(availableIngredients[randomIndex]);
            availableIngredients.RemoveAt(randomIndex);
        }

        Debug.Log($"InventoryManager: Generated new recipe with {currentRecipe.Count} ingredients");
        foreach (var ingredient in currentRecipe)
        {
            Debug.Log($"Recipe ingredient: {ingredient}");
        }

        UpdateRecipeUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("InventoryManager: I key detected");
            ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.Q) && inventory.Count > 0)
        {
            DropItem();
        }
    }

    private void ToggleInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("InventoryManager: inventoryPanel is null when trying to toggle!");
            return;
        }

        isInventoryOpen = !isInventoryOpen;
        Debug.Log($"InventoryManager: Setting panel {(isInventoryOpen ? "ACTIVE" : "INACTIVE")}");
        inventoryPanel.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            UpdateInventoryUI();
            UpdateRecipeUI();
        }
    }

    private void UpdateInventoryUI()
    {
        if (inventoryContent == null)
        {
            Debug.LogError("InventoryManager: inventoryContent is null!");
            return;
        }

        // Clear existing items
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

        Debug.Log($"InventoryManager: Updating inventory UI with {inventory.Count} items");
        
        // Add title
        GameObject titleObj = Instantiate(itemPrefab, inventoryContent);
        TextMeshProUGUI titleText = titleObj.GetComponentInChildren<TextMeshProUGUI>();
        if (titleText != null)
        {
            titleText.text = "Your Inventory:";
            titleText.fontSize = 20;
            titleText.fontStyle = FontStyles.Bold;
        }

        // Add current inventory items
        foreach (EIngredient item in inventory)
        {
            GameObject itemObj = Instantiate(itemPrefab, inventoryContent);
            TextMeshProUGUI text = itemObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = "• " + item.ToString();
            }
        }
    }

    private void UpdateRecipeUI()
    {
        if (recipeContent == null)
        {
            Debug.LogError("InventoryManager: recipeContent is null!");
            return;
        }

        // Clear existing items
        foreach (Transform child in recipeContent)
        {
            Destroy(child.gameObject);
        }

        Debug.Log($"InventoryManager: Updating recipe UI with {currentRecipe.Count} items");
        
        // Add title
        GameObject titleObj = Instantiate(itemPrefab, recipeContent);
        TextMeshProUGUI titleText = titleObj.GetComponentInChildren<TextMeshProUGUI>();
        if (titleText != null)
        {
            titleText.text = "Recipe Needed:";
            titleText.fontSize = 20;
            titleText.fontStyle = FontStyles.Bold;
        }

        // Add current recipe items
        foreach (EIngredient item in currentRecipe)
        {
            GameObject itemObj = Instantiate(itemPrefab, recipeContent);
            TextMeshProUGUI text = itemObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                bool isCollected = inventory.Contains(item);
                text.text = "• " + item.ToString();
                text.fontStyle = isCollected ? FontStyles.Strikethrough : FontStyles.Normal;
                text.color = isCollected ? new Color(0.5f, 0.5f, 0.5f) : Color.white;
            }
        }
    }

    public void AddItem(EIngredient item)
    {
        inventory.Add(item);
        Debug.Log($"InventoryManager: Added {item} to inventory");
        if (isInventoryOpen)
        {
            UpdateInventoryUI();
            UpdateRecipeUI(); // Update recipe UI to show collected items
        }
    }

    public bool CheckRecipeComplete()
    {
        foreach (EIngredient requiredItem in currentRecipe)
        {
            if (!inventory.Contains(requiredItem))
            {
                return false;
            }
        }
        return true;
    }

    private void DropItem()
    {
        if (inventory.Count == 0) return;

        // Get the first item in the inventory
        EIngredient itemToDrop = inventory[0];
        
        // Remove it from inventory
        inventory.RemoveAt(0);
        Debug.Log($"InventoryManager: Dropped {itemToDrop}");

        // Create the dropped item in the world
        if (ingredientPrefab != null)
        {
            // Get the player's position
            Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            
            // Create the item slightly offset from the player
            Vector3 dropPosition = playerPos + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            GameObject droppedItem = Instantiate(ingredientPrefab, dropPosition, Quaternion.identity);
            
            // Set the ingredient type
            IngredientItem ingredientComponent = droppedItem.GetComponent<IngredientItem>();
            if (ingredientComponent != null)
            {
                ingredientComponent.IngredientType = itemToDrop;
            }
        }
        else
        {
            Debug.LogWarning("InventoryManager: No ingredient prefab assigned for dropping items!");
        }

        // Update UI if inventory is open
        if (isInventoryOpen)
        {
            UpdateInventoryUI();
            UpdateRecipeUI();
        }
    }
} 
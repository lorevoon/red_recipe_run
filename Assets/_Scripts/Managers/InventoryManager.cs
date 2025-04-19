using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
    private List<EIngredient> inventory = new List<EIngredient>();

    // Reference to the IngredientItem prefab for dropping
    [SerializeField] private GameObject ingredientPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("InventoryManager: Instance created and set to persist");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("InventoryManager: Duplicate instance destroyed");
        }
    }

    private void Start()
    {
        Debug.Log("InventoryManager: Started with empty inventory");
    }

    // Static method to ensure an instance exists
    public static InventoryManager EnsureExists()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("InventoryManager");
            Instance = go.AddComponent<InventoryManager>();
            Debug.Log("InventoryManager: Created new instance");
        }
        return Instance;
    }

    public void Initialize(GameObject panel, Transform invContent, Transform recContent, GameObject prefab)
    {
        Debug.Log("InventoryManager: Initialize called");
    }

    public List<EIngredient> GetInventory()
    {
        return new List<EIngredient>(inventory);
    }

    public void AddItem(EIngredient item)
    {
        inventory.Add(item);
        Debug.Log($"InventoryManager: Added {item} to inventory");
    }

    public void RemoveItem(EIngredient item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            Debug.Log($"InventoryManager: Removed {item} from inventory");
        }
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
            Ingredient ingredientComponent = droppedItem.GetComponent<Ingredient>();
            if (ingredientComponent != null)
            {
                ingredientComponent.IngredientType = itemToDrop;
            }
        }
        else
        {
            Debug.LogWarning("InventoryManager: No ingredient prefab assigned for dropping items!");
        }
    }
} 
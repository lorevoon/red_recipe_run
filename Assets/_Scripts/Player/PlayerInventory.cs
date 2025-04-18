using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<GameObject> InventoryList = new List<GameObject>();
    
    private PlayerController _playerController;
    private float _pickupRadius = 2f; // Radius for picking up ingredients
    private int _maxIngredients = 5; // Base inventory size, will be modified by upgrades
    private int _currIngredients;

    private void Start()
    {
        // Initialize the current ingredient count
        _currIngredients = 0;
        
        // Get player controller reference
        _playerController = GetComponent<PlayerController>();
        
        // Update max ingredients from upgrade manager
        UpdateMaxIngredientsFromUpgrades();
    }
    
    private void Update()
    {
        // Update max ingredients based on upgrades each frame
        UpdateMaxIngredientsFromUpgrades();
    }
    
    private void UpdateMaxIngredientsFromUpgrades()
    {
        if (UpgradeManager.Instance != null)
        {
            _maxIngredients = UpgradeManager.Instance.GetCurrentInventorySize();
        }
    }

    public void AddItem(GameObject ingredient)
    {
        InventoryList.Add(ingredient);
        _currIngredients++;
        
        // Update UI if the toolkit manager exists
        if (UIToolkitManager.Instance != null)
        {
            UIToolkitManager.Instance.RefreshInventoryUI();
        }
    }

    public GameObject RemoveItem()
    {
        if (InventoryList.Count == 0) return null;
        
        GameObject ingredient = InventoryList[^1];
        InventoryList.RemoveAt(InventoryList.Count - 1);
        _currIngredients--;
        
        // Update UI if the toolkit manager exists
        if (UIToolkitManager.Instance != null)
        {
            UIToolkitManager.Instance.RefreshInventoryUI();
        }
        
        return ingredient;
    }

    public bool IsIngredientInInventory(GameObject ingredient)
    {
        return InventoryList.Contains(ingredient);
    }
    
    
    public void TryPickUpIngredient()
    {
        // Don't try to pick up if inventory is full
        if (_currIngredients >= _maxIngredients)
        {
            Debug.Log("Inventory full, can't pick up more items");
            return;
        }
            
        Collider2D[] nearbyIngredients = Physics2D.OverlapCircleAll(transform.position, _pickupRadius);
        GameObject closestIngredient = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in nearbyIngredients)
        {
            if (col.CompareTag("Ingredient") && !InventoryList.Contains(col.gameObject))
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIngredient = col.gameObject;
                }
            }
        }

        if (closestIngredient != null)
        {
            PickUpIngredient(closestIngredient); // pick up ingredient
            Debug.Log($"Picked up {closestIngredient.name}, inventory count: {_currIngredients}");
        }
        else
        {
            Debug.Log("No ingredients found to pick up");
        }
    }


   public void PickUpIngredient(GameObject ingredient)
   { 
       AddItem(ingredient);
       Transform basketTransform = PlayerController.Instance.Basket.transform; 
       basketTransform.localScale *= 1.05f; // Increase basket size by 5%

       ingredient.transform.SetParent(basketTransform, true);
       ingredient.SetActive(false);

       // Add the ingredient to the InventoryManager
       Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
       if (ingredientComponent != null)
       {
           InventoryManager.Instance.AddItem(ingredientComponent.IngredientType);
       }
    }

    public void DropIngredient()
    { 
        if (InventoryList.Count <= 0)
        {
            Debug.Log("No items to drop");
            return;
        }
        
        GameObject mostRecent = RemoveItem();  // This already decrements _currIngredients
        if (mostRecent == null)
        {
            Debug.LogError("RemoveItem returned null even though inventory has items");
            return;
        }
        
        Debug.Log($"Dropping {mostRecent.name}, inventory count now: {_currIngredients}");
        
        // Store the world position before unparenting
        Vector3 worldPosition = transform.position + new Vector3(0, -1, 0); // Drop slightly below player

        // Unparent the object
        mostRecent.transform.SetParent(null);

        // Restore the world position
        mostRecent.transform.position = worldPosition;

        // Ensure the rotation is reset to avoid unwanted rotation from the parent
        mostRecent.transform.rotation = Quaternion.identity;

        mostRecent.SetActive(true);
        Ingredient ingredientComponent = mostRecent.GetComponent<Ingredient>();
        
        if (ingredientComponent != null)
        {
            ingredientComponent.OnDrop();
            
            // Remove the ingredient from the InventoryManager
            InventoryManager.Instance.RemoveItem(ingredientComponent.IngredientType);
        }

        // Play throw sound effect
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayThrow();
        }
        
        // Force refresh the UI
        if (UIToolkitManager.Instance != null)
        {
            UIToolkitManager.Instance.RefreshInventoryUI();
        }
    }
    
    public int GetMaxIngredients()
    {
        return _maxIngredients;
    }
    
    public int GetCurrentIngredients()
    {
        return _currIngredients;
    }
}
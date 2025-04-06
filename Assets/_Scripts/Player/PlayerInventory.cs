using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<GameObject> InventoryList = new List<GameObject>();
    
    private PlayerController _playerController;
    private float _pickupRadius = 2f; // Radius for picking up ingredients
    private int _max_ingredients = 25;
    private int _curr_ingredients;


    public void AddItem(GameObject ingredient)
    {
        InventoryList.Add(ingredient);
    }

    public GameObject RemoveItem()
    {
        GameObject ingredient = InventoryList[^1];
        InventoryList.RemoveAt(InventoryList.Count - 1);
        return ingredient;
    }

    public bool IsIngredientInInventory(GameObject ingredient)
    {
        return InventoryList.Contains(ingredient);
    }
    
    
    public void TryPickUpIngredient()
    {
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


        if (closestIngredient != null && _curr_ingredients < _max_ingredients)
        {
            PickUpIngredient(closestIngredient); // pick up ingredient
            _curr_ingredients += 1;
        }
    }


   public void PickUpIngredient(GameObject ingredient)
   { 
       AddItem(ingredient);
       Transform basketTransform = PlayerController.Instance.Basket.transform; 
       basketTransform.localScale *= 1.05f; // Increase basket size by 5%

       ingredient.transform.SetParent(basketTransform, true);
       ingredient.SetActive(false);
    }

    public void DropIngredient()
    { 
        if (InventoryList.Count <= 0) return;
        
        GameObject mostRecent = RemoveItem();
        
       // Store the world position before unparenting
        Vector3 worldPosition = mostRecent.transform.position;

        // Unparent the object
        mostRecent.transform.SetParent(null);

        // Restore the world position
        mostRecent.transform.position = worldPosition;

        // Ensure the rotation is reset to avoid unwanted rotation from the parent
        mostRecent.transform.rotation = Quaternion.identity;

        mostRecent.SetActive(true);
        mostRecent.GetComponent<Ingredient>().OnDrop();
    }
}
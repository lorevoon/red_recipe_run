using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<GameObject> InventoryList = new List<GameObject>();

    private PlayerController _playerController;
    private float _pickupRadius = 2f; // Radius for picking up ingredients

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

        if (closestIngredient != null)
        {
            PickUpIngredient(closestIngredient); // pick up ingredient
        }
    }

    public void PickUpIngredient(GameObject ingredient)
    {
        AddItem(ingredient);
        ingredient.transform.SetParent(PlayerController.Instance.Basket.transform);
        ingredient.GetComponent<Collider2D>().enabled = false;
        ingredient.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void DropIngredient()
    {
        GameObject mostRecent = RemoveItem();

        mostRecent.transform.SetParent(null);
        mostRecent.GetComponent<SpriteRenderer>().enabled = true;
        mostRecent.GetComponent<Collider2D>().enabled = true;
    }
}
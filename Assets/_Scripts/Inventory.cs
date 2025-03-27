using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<EIngredient.EIngredientType> inventory = new List<EIngredient.EIngredientType>();

    public void AddItemToInventory(EIngredient.EIngredientType ingredient)
    {
        inventory.Add(ingredient);
    }

    public void RemoveItemFromInventory(EIngredient.EIngredientType ingredient)
    {
        inventory.Remove(ingredient);
    }

    public bool CheckInventory(EIngredient.EIngredientType ingredient)
    {
        return inventory.Contains(ingredient);
    }
}
using UnityEngine;
using TMPro;

public class InventoryItemPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    
    public void SetItemName(string name)
    {
        itemNameText.text = name;
    }
} 
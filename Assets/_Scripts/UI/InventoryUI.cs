using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventoryContent;
    [SerializeField] private Transform recipeContent;
    [SerializeField] private GameObject itemPrefab;
    
    public void Initialize(GameObject panel, Transform invContent, Transform recContent, GameObject prefab)
    {
        inventoryPanel = panel;
        inventoryContent = invContent;
        recipeContent = recContent;
        itemPrefab = prefab;
    }

    private void Start()
    {
        if (inventoryPanel != null)
        {
            // Set up the inventory panel
            inventoryPanel.SetActive(false);
            
            // Set up the layout
            SetupLayout();
        }
    }
    
    private void SetupLayout()
    {
        // Add a title for the inventory section
        GameObject inventoryTitle = new GameObject("InventoryTitle");
        inventoryTitle.transform.SetParent(inventoryPanel.transform, false);
        TextMeshProUGUI inventoryTitleText = inventoryTitle.AddComponent<TextMeshProUGUI>();
        inventoryTitleText.text = "Inventory";
        inventoryTitleText.fontSize = 24;
        inventoryTitleText.alignment = TextAlignmentOptions.Center;
        
        // Add a title for the recipe section
        GameObject recipeTitle = new GameObject("RecipeTitle");
        recipeTitle.transform.SetParent(inventoryPanel.transform, false);
        TextMeshProUGUI recipeTitleText = recipeTitle.AddComponent<TextMeshProUGUI>();
        recipeTitleText.text = "Current Recipe";
        recipeTitleText.fontSize = 24;
        recipeTitleText.alignment = TextAlignmentOptions.Center;
        
        // Set up the panel background
        Image panelImage = inventoryPanel.GetComponent<Image>();
        if (panelImage == null)
        {
            panelImage = inventoryPanel.AddComponent<Image>();
        }
        panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
    }
} 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class UIToolkitManager : MonoBehaviour
{
    public static UIToolkitManager Instance { get; private set; }
    
    [Header("UI Documents")]
    [SerializeField] private UIDocument inventoryDocument;
    [SerializeField] private UIDocument upgradesDocument;
    
    [Header("UI Assets")]
    [SerializeField] private string inventoryUXMLPath = "UI/InventoryUI";
    [SerializeField] private string upgradesUXMLPath = "UI/UpgradesUI";
    [SerializeField] private string inventoryItemTemplatePath = "UI/InventoryItemTemplate";
    [SerializeField] private string styleSheetPath = "UI/GameUI";
    [SerializeField] private string panelSettingsPath = "UI/GameUIPanelSettings";
    
    // UI Element references
    private VisualElement inventoryRoot;
    private VisualElement upgradesRoot;
    private VisualElement inventoryContent;
    private Label coinsDisplay;
    
    // Upgrade elements
    private Label speedLevelText;
    private Label inventoryLevelText;
    private Label healthLevelText;
    private Label lanternLevelText;
    private Label speedCostText;
    private Label inventoryCostText;
    private Label healthCostText;
    private Label lanternCostText;
    private Button speedButton;
    private Button inventoryButton;
    private Button healthButton;
    private Button lanternButton;
    
    // State
    private bool isInventoryVisible = false;
    private bool isUpgradesVisible = false;
    
    // Templates
    private VisualTreeAsset inventoryItemTemplate;
    
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
            return;
        }
        
        // Load templates
        inventoryItemTemplate = Resources.Load<VisualTreeAsset>(inventoryItemTemplatePath);
        
        // Create UI Documents if not assigned
        SetupUIDocuments();
    }
    
    private void Start()
    {
        // Initialize UI
        InitializeInventoryUI();
        InitializeUpgradesUI();
        
        // Hide UI panels initially
        SetInventoryVisibility(false);
        SetUpgradesVisibility(false);
    }
    
    private void Update()
    {
        // Toggle Inventory with I key
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleInventory();
        }
        
        // Toggle Upgrades with U key
        if (Input.GetKeyDown(KeyCode.S))
        {
            ToggleUpgrades();
        }
    }
    
    private void SetupUIDocuments()
    {
        // Load panel settings
        PanelSettings panelSettings = Resources.Load<PanelSettings>(panelSettingsPath);
        if (panelSettings == null)
        {
            Debug.LogError("Could not find PanelSettings asset! UI will not display correctly.");
        }

        // Create inventory document if not assigned
        if (inventoryDocument == null)
        {
            GameObject invUIObj = new GameObject("InventoryUI");
            invUIObj.transform.SetParent(transform);
            inventoryDocument = invUIObj.AddComponent<UIDocument>();
            inventoryDocument.visualTreeAsset = Resources.Load<VisualTreeAsset>(inventoryUXMLPath);
            if (panelSettings != null)
            {
                inventoryDocument.panelSettings = panelSettings;
            }
        }
        
        // Create upgrades document if not assigned
        if (upgradesDocument == null)
        {
            GameObject upgradesUIObj = new GameObject("UpgradesUI");
            upgradesUIObj.transform.SetParent(transform);
            upgradesDocument = upgradesUIObj.AddComponent<UIDocument>();
            upgradesDocument.visualTreeAsset = Resources.Load<VisualTreeAsset>(upgradesUXMLPath);
            if (panelSettings != null)
            {
                upgradesDocument.panelSettings = panelSettings;
            }
        }
        
        // Load and apply stylesheet
        StyleSheet styleSheet = Resources.Load<StyleSheet>(styleSheetPath);
        if (styleSheet != null)
        {
            // Add stylesheet to the root visual elements
            if (inventoryDocument.rootVisualElement != null)
            {
                inventoryDocument.rootVisualElement.styleSheets.Add(styleSheet);
            }
            
            if (upgradesDocument.rootVisualElement != null)
            {
                upgradesDocument.rootVisualElement.styleSheets.Add(styleSheet);
            }
        }
    }
    
    private void InitializeInventoryUI()
    {
        if (inventoryDocument.rootVisualElement == null)
        {
            Debug.LogError("Inventory document root is null!");
            return;
        }
        
        inventoryRoot = inventoryDocument.rootVisualElement.Q("inventory-container");
        
        if (inventoryRoot == null)
        {
            Debug.LogError("Failed to find inventory container element!");
            return;
        }
        
        inventoryContent = inventoryRoot.Q("inventory-content");
        
        if (inventoryContent == null)
        {
            Debug.LogError("Failed to find inventory content element!");
        }
    }
    
    private void InitializeUpgradesUI()
    {
        if (upgradesDocument.rootVisualElement == null)
        {
            Debug.LogError("Upgrades document root is null!");
            return;
        }
        
        upgradesRoot = upgradesDocument.rootVisualElement.Q("upgrades-container");
        
        if (upgradesRoot == null)
        {
            Debug.LogError("Failed to find upgrades container element!");
            return;
        }
        
        // Get coin display
        coinsDisplay = upgradesRoot.Q<Label>("coins-display");
        
        // Get upgrade level elements
        speedLevelText = upgradesRoot.Q<Label>("speed-level");
        inventoryLevelText = upgradesRoot.Q<Label>("inventory-level");
        healthLevelText = upgradesRoot.Q<Label>("health-level");
        lanternLevelText = upgradesRoot.Q<Label>("lantern-level");
        
        // Get upgrade cost elements
        speedCostText = upgradesRoot.Q<Label>("speed-cost");
        inventoryCostText = upgradesRoot.Q<Label>("inventory-cost");
        healthCostText = upgradesRoot.Q<Label>("health-cost");
        lanternCostText = upgradesRoot.Q<Label>("lantern-cost");
        
        // Get upgrade buttons
        speedButton = upgradesRoot.Q<Button>("speed-button");
        inventoryButton = upgradesRoot.Q<Button>("inventory-button");
        healthButton = upgradesRoot.Q<Button>("health-button");
        lanternButton = upgradesRoot.Q<Button>("lantern-button");
        
        // Add button event handlers
        if (speedButton != null) speedButton.clicked += () => PurchaseUpgrade(EUpgradeType.MovementSpeed);
        if (inventoryButton != null) inventoryButton.clicked += () => PurchaseUpgrade(EUpgradeType.InventorySpace);
        if (healthButton != null) healthButton.clicked += () => PurchaseUpgrade(EUpgradeType.MaxHearts);
        if (lanternButton != null) lanternButton.clicked += () => PurchaseUpgrade(EUpgradeType.LanternPower);
        
        // Initial refresh
        RefreshUpgradesUI();
    }
    
    #region Inventory Methods
    
    public void ToggleInventory()
    {
        SetInventoryVisibility(!isInventoryVisible);
        
        // Play sound effect
        if (isInventoryVisible)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayInventoryOpen();
        }
        else
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayInventoryClose();
        }
    }
    
    private void SetInventoryVisibility(bool visible)
    {
        isInventoryVisible = visible;
        
        if (inventoryRoot == null) return;
        
        if (visible)
        {
            inventoryRoot.AddToClassList("visible");
            RefreshInventoryUI();
        }
        else
        {
            inventoryRoot.RemoveFromClassList("visible");
        }
    }
    
    public void RefreshInventoryUI()
    {
        if (inventoryContent == null) return;
        
        // Clear existing items
        inventoryContent.Clear();
        
        // Get inventory items from manager
        if (InventoryManager.Instance == null) return;
        
        List<EIngredient> inventoryList = InventoryManager.Instance.GetInventory();
        
        if (inventoryList.Count == 0)
        {
            // Display empty message
            Label emptyLabel = new Label("Inventory is Empty");
            emptyLabel.AddToClassList("item-name");
            emptyLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            emptyLabel.style.marginTop = 20;
            inventoryContent.Add(emptyLabel);
        }
        else
        {
            // Group items by type and count them
            var groupedItems = inventoryList
                .GroupBy(item => item)
                .Select(group => new { Ingredient = group.Key, Count = group.Count() });
                
            foreach (var itemGroup in groupedItems)
            {
                if (inventoryItemTemplate != null)
                {
                    // Instantiate the template
                    TemplateContainer itemContainer = inventoryItemTemplate.Instantiate();
                    
                    // Get and update the name and count labels
                    Label nameLabel = itemContainer.Q<Label>("item-name");
                    Label countLabel = itemContainer.Q<Label>("item-count");
                    
                    if (nameLabel != null && countLabel != null)
                    {
                        nameLabel.text = itemGroup.Ingredient.ToString();
                        countLabel.text = $"x{itemGroup.Count}";
                    }
                    
                    // Add item to the inventory content
                    inventoryContent.Add(itemContainer);
                }
                else
                {
                    // Fallback if template isn't loaded
                    // Create item container
                    VisualElement itemContainer = new VisualElement();
                    itemContainer.AddToClassList("inventory-item");
                    
                    // Create icon element
                    VisualElement iconElement = new VisualElement();
                    iconElement.AddToClassList("item-icon");
                    
                    // Create details container
                    VisualElement detailsContainer = new VisualElement();
                    detailsContainer.AddToClassList("item-details");
                    
                    // Create name and count labels
                    Label nameLabel = new Label(itemGroup.Ingredient.ToString());
                    nameLabel.AddToClassList("item-name");
                    
                    Label countLabel = new Label($"x{itemGroup.Count}");
                    countLabel.AddToClassList("item-count");
                    
                    // Add elements to containers
                    detailsContainer.Add(nameLabel);
                    detailsContainer.Add(countLabel);
                    
                    itemContainer.Add(iconElement);
                    itemContainer.Add(detailsContainer);
                    
                    // Add item to content
                    inventoryContent.Add(itemContainer);
                }
            }
        }
        
        // Update capacity indicator
        if (PlayerController.Instance != null && PlayerController.Instance.GetComponent<PlayerInventory>() != null)
        {
            PlayerInventory playerInventory = PlayerController.Instance.GetComponent<PlayerInventory>();
            int current = playerInventory.GetCurrentIngredients();
            int max = playerInventory.GetMaxIngredients();
            
            Label capacityLabel = new Label($"Capacity: {current}/{max}");
            capacityLabel.AddToClassList("item-count");
            capacityLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            capacityLabel.style.marginTop = 10;
            inventoryContent.Add(capacityLabel);
        }
    }
    
    #endregion
    
    #region Upgrades Methods
    
    public void ToggleUpgrades()
    {
        SetUpgradesVisibility(!isUpgradesVisible);
        
        // Play sound effect
        if (isUpgradesVisible)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayUpgradesOpen();
        }
        else
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayUpgradesClose();
        }
    }
    
    private void SetUpgradesVisibility(bool visible)
    {
        isUpgradesVisible = visible;
        
        if (upgradesRoot == null) return;
        
        if (visible)
        {
            upgradesRoot.AddToClassList("visible");
            RefreshUpgradesUI();
        }
        else
        {
            upgradesRoot.RemoveFromClassList("visible");
        }
    }
    
    public void RefreshUpgradesUI()
    {
        if (UpgradeManager.Instance == null || coinsDisplay == null) return;
        
        // Update coins display
        int coins = UpgradeManager.Instance.GetCoins();
        coinsDisplay.text = $"Coins: {coins}";
        
        // Update upgrade levels
        int speedLevel = UpgradeManager.Instance.GetSpeedLevel();
        int inventoryLevel = UpgradeManager.Instance.GetInventoryLevel();
        int healthLevel = UpgradeManager.Instance.GetHealthLevel();
        int lanternLevel = UpgradeManager.Instance.GetLanternLevel();
        
        if (speedLevelText != null) speedLevelText.text = $"Level: {speedLevel}";
        if (inventoryLevelText != null) inventoryLevelText.text = $"Level: {inventoryLevel}";
        if (healthLevelText != null) healthLevelText.text = $"Level: {healthLevel}";
        if (lanternLevelText != null) lanternLevelText.text = $"Level: {lanternLevel}";
        
        // Update upgrade costs
        int speedCost = UpgradeManager.Instance.GetUpgradeCost(EUpgradeType.MovementSpeed);
        int inventoryCost = UpgradeManager.Instance.GetUpgradeCost(EUpgradeType.InventorySpace);
        int healthCost = UpgradeManager.Instance.GetUpgradeCost(EUpgradeType.MaxHearts);
        int lanternCost = UpgradeManager.Instance.GetUpgradeCost(EUpgradeType.LanternPower);
        
        if (speedCostText != null) speedCostText.text = $"Cost: {speedCost}";
        if (inventoryCostText != null) inventoryCostText.text = $"Cost: {inventoryCost}";
        if (healthCostText != null) healthCostText.text = $"Cost: {healthCost}";
        if (lanternCostText != null) lanternCostText.text = $"Cost: {lanternCost}";
        
        // Update button states
        if (speedButton != null) UpdateButtonState(speedButton, coins >= speedCost && speedCost > 0);
        if (inventoryButton != null) UpdateButtonState(inventoryButton, coins >= inventoryCost && inventoryCost > 0);
        if (healthButton != null) UpdateButtonState(healthButton, coins >= healthCost && healthCost > 0);
        if (lanternButton != null) UpdateButtonState(lanternButton, coins >= lanternCost && lanternCost > 0);
    }
    
    private void UpdateButtonState(Button button, bool interactable)
    {
        if (button == null) return;
        
        button.SetEnabled(interactable);
        
        if (interactable)
        {
            button.RemoveFromClassList("button-disabled");
        }
        else
        {
            button.AddToClassList("button-disabled");
        }
    }
    
    private void PurchaseUpgrade(EUpgradeType upgradeType)
    {
        if (UpgradeManager.Instance == null) return;
        
        bool success = UpgradeManager.Instance.PurchaseUpgrade(upgradeType);
        
        if (success)
        {
            // Refresh UI
            RefreshUpgradesUI();
        }
    }
    
    #endregion
} 
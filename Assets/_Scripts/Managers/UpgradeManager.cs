using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject upgradePanel;
    public TextMeshProUGUI coinText;
    
    [Header("Upgrade Buttons")]
    public Button speedUpgradeButton;
    public Button inventoryUpgradeButton;
    public Button healthUpgradeButton;
    
    [Header("Cost Texts")]
    public TextMeshProUGUI speedCostText;
    public TextMeshProUGUI inventoryCostText;
    public TextMeshProUGUI healthCostText;
    
    [Header("Value Texts")]
    public TextMeshProUGUI speedValueText;
    public TextMeshProUGUI inventoryValueText;
    public TextMeshProUGUI healthValueText;

    [Header("Upgrade Settings")]
    public int baseCost = 10;
    public float costMultiplier = 1.5f;
    
    private int coins = 0;
    private int speedLevel = 0;
    private int inventoryLevel = 0;
    private int healthLevel = 0;

    // Maximum upgrade levels
    private const int MAX_SPEED_LEVEL = 5;
    private const int MAX_INVENTORY_LEVEL = 5;
    private const int MAX_HEALTH_LEVEL = 5;

    // Player stats
    private float baseSpeed = 5f;
    private int baseInventorySize = 5;
    private int baseHealth = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadUpgrades();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize UI
        UpdateUI();
        
        // Add button listeners
        speedUpgradeButton.onClick.AddListener(() => PurchaseUpgrade(EUpgradeType.MovementSpeed));
        inventoryUpgradeButton.onClick.AddListener(() => PurchaseUpgrade(EUpgradeType.InventorySpace));
        healthUpgradeButton.onClick.AddListener(() => PurchaseUpgrade(EUpgradeType.MaxHearts));
        
        // Initially hide the panel
        upgradePanel.SetActive(false);
    }

    private void Update()
    {
        // Toggle panel with 'U' key
        if (Input.GetKeyDown(KeyCode.U))
        {
            upgradePanel.SetActive(!upgradePanel.activeSelf);
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveUpgrades();
        UpdateUI();
    }

    private void PurchaseUpgrade(EUpgradeType upgradeType)
    {
        int cost = CalculateCost(upgradeType);
        if (coins < cost) return;

        coins -= cost;
        
        switch (upgradeType)
        {
            case EUpgradeType.MovementSpeed:
                speedLevel++;
                // Apply speed upgrade to player
                break;
            case EUpgradeType.InventorySpace:
                inventoryLevel++;
                // Apply inventory upgrade
                break;
            case EUpgradeType.MaxHearts:
                healthLevel++;
                // Apply health upgrade
                break;
        }
        
        SaveUpgrades();
        UpdateUI();
    }

    private int CalculateCost(EUpgradeType upgradeType)
    {
        int level = upgradeType switch
        {
            EUpgradeType.MovementSpeed => speedLevel,
            EUpgradeType.InventorySpace => inventoryLevel,
            EUpgradeType.MaxHearts => healthLevel,
            _ => 0
        };
        
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, level));
    }

    private void UpdateUI()
    {
        coinText.text = $"Coins: {coins}";
        
        // Update costs
        speedCostText.text = $"Cost: {CalculateCost(EUpgradeType.MovementSpeed)}";
        inventoryCostText.text = $"Cost: {CalculateCost(EUpgradeType.InventorySpace)}";
        healthCostText.text = $"Cost: {CalculateCost(EUpgradeType.MaxHearts)}";
        
        // Update values
        speedValueText.text = $"Level: {speedLevel}";
        inventoryValueText.text = $"Level: {inventoryLevel}";
        healthValueText.text = $"Level: {healthLevel}";
        
        // Update button interactability
        speedUpgradeButton.interactable = coins >= CalculateCost(EUpgradeType.MovementSpeed);
        inventoryUpgradeButton.interactable = coins >= CalculateCost(EUpgradeType.InventorySpace);
        healthUpgradeButton.interactable = coins >= CalculateCost(EUpgradeType.MaxHearts);
    }

    private void SaveUpgrades()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("SpeedLevel", speedLevel);
        PlayerPrefs.SetInt("InventoryLevel", inventoryLevel);
        PlayerPrefs.SetInt("HealthLevel", healthLevel);
        PlayerPrefs.SetInt("SpeedCost", CalculateCost(EUpgradeType.MovementSpeed));
        PlayerPrefs.SetInt("InventoryCost", CalculateCost(EUpgradeType.InventorySpace));
        PlayerPrefs.SetInt("HealthCost", CalculateCost(EUpgradeType.MaxHearts));
        PlayerPrefs.Save();
    }

    private void LoadUpgrades()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
        speedLevel = PlayerPrefs.GetInt("SpeedLevel", 0);
        inventoryLevel = PlayerPrefs.GetInt("InventoryLevel", 0);
        healthLevel = PlayerPrefs.GetInt("HealthLevel", 0);
    }

    public float GetCurrentSpeed()
    {
        return baseSpeed * (1f + (speedLevel * 0.2f));
    }

    public int GetCurrentInventorySize()
    {
        return baseInventorySize + (inventoryLevel * 2);
    }

    public int GetCurrentHealth()
    {
        return baseHealth + healthLevel;
    }
} 
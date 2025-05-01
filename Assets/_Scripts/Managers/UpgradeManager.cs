using UnityEngine;
using System;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("Upgrade Settings")]
    [SerializeField] private int initialCoins = 200; // Starting mana for testing
    
    // Fixed upgrade costs
    private readonly int[] upgradeCosts = new int[] { 10, 25, 50, 100, 200 };
    
    private int coins = 0;
    private int speedLevel = 0;
    private int inventoryLevel = 0; 
    private int healthLevel = 0;
    private int lanternLevel = 0;
    private int spiritPowerLevel = 0;

    // Maximum upgrade levels
    private const int MAX_UPGRADE_LEVEL = 5;

    // Base player stats
    //private float baseSpeed = _playerController._maxSpeed;
    private PlayerController _playerController;
    private int baseInventorySize = 5;
    private int baseHealth = 3;
    private float baseLanternRange = 5f;
    private float baseToolSpeed = 3f;
    
    // Speed multiplier per level (1.25 = 25% increase)
    private const float SPEED_MULTIPLIER = 0.25f;
    
    // Items added per inventory upgrade
    private const int INVENTORY_INCREMENT = 1;
    
    // Hearts added per health upgrade
    private const int HEALTH_INCREMENT = 1;
    
    // Lantern range multiplier
    private const float LANTERN_MULTIPLIER = 0.25f;
    
    // Tool speed multiplier
    private const float TOOL_SPEED_MULTIPLIER = 0.3f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ResetUpgrades(); // Reset upgrades on start
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        // Make sure we have proper values on start
        ResetUpgrades();
    }
    
    // Reset upgrades and give initial mana
    public void ResetUpgrades()
    {
        coins = initialCoins;
        speedLevel = 0;
        inventoryLevel = 0;
        healthLevel = 0;
        lanternLevel = 0;
        spiritPowerLevel = 0;
        SaveUpgrades();
        
        Debug.Log($"Upgrades reset. Starting with {coins} mana.");
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveUpgrades();
    }

    public bool PurchaseUpgrade(EUpgradeType upgradeType)
    {
        int level = GetUpgradeLevel(upgradeType);
        
        // Check if already at max level
        if (level >= MAX_UPGRADE_LEVEL)
        {
            Debug.Log($"{upgradeType} already at max level ({MAX_UPGRADE_LEVEL})");
            return false;
        }
        
        // Get cost for the next level
        int cost = GetUpgradeCost(upgradeType);
        
        if (coins < cost)
        {
            Debug.Log($"Not enough mana. Need {cost}, have {coins}");
            return false;
        }

        // Subtract mana
        coins -= cost;
        
        // Increment the appropriate level
        switch (upgradeType)
        {
            case EUpgradeType.MovementSpeed:
                speedLevel++;
                Debug.Log($"Agile Fairy Spell upgraded to level {speedLevel}. New speed: {GetCurrentSpeed()}");
                break;
            case EUpgradeType.InventorySpace:
                inventoryLevel++;
                Debug.Log($"Basket Enlargement upgraded to level {inventoryLevel}. New capacity: {GetCurrentInventorySize()}");
                break;
            case EUpgradeType.MaxHearts:
                healthLevel++;
                Debug.Log($"Blood Pact upgraded to level {healthLevel}. New health: {GetCurrentHealth()}");
                // Update the player's heart display
                UpdatePlayerHealth();
                break;
            case EUpgradeType.LanternPower:
                lanternLevel++;
                Debug.Log($"Sacred Flame upgraded to level {lanternLevel}. New range: {GetCurrentLanternRange()}");
                // Update the lantern if it exists
                UpdateLantern();
                break;
            case EUpgradeType.SpiritPower:
                spiritPowerLevel++;
                Debug.Log($"Dark Power upgraded to level {spiritPowerLevel}. New tool speed: {GetCurrentToolSpeed()}");
                // Update player's tool speed
                UpdateToolSpeed();
                break;
        }
        
        SaveUpgrades();
        return true;
    }
    
    private void UpdateToolSpeed()
    {
        // Update player's tool speed
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.ToolSpeed = GetCurrentToolSpeed();
            Debug.Log($"Updated player tool speed to {PlayerController.Instance.ToolSpeed}");
        }
    }
    
    private void UpdateLantern()
    {
        // Find the lantern in the scene
        GameObject lanternObj = GameObject.FindWithTag("Lantern");
        if (lanternObj != null)
        {
            LanternController lanternController = lanternObj.GetComponent<LanternController>();
            if (lanternController != null)
            {
                // Update the lantern radius
                lanternController.UpdateLightRadius();
                Debug.Log("Updated lantern with new range value");
            }
        }
    }

    private void UpdatePlayerHealth()
    {
        // Find the player in the scene
        PlayerController player = PlayerController.Instance;
        if (player != null)
        {
            // Update the player's health display
            player.UpgradeHealth();
            Debug.Log("Updated player health display");
        }
    }

    // Get the current level for a specific upgrade type
    private int GetUpgradeLevel(EUpgradeType upgradeType)
    {
        return upgradeType switch
        {
            EUpgradeType.MovementSpeed => speedLevel,
            EUpgradeType.InventorySpace => inventoryLevel,
            EUpgradeType.MaxHearts => healthLevel,
            EUpgradeType.LanternPower => lanternLevel,
            EUpgradeType.SpiritPower => spiritPowerLevel,
            _ => 0
        };
    }

    // Get the cost for the next level of an upgrade
    public int GetUpgradeCost(EUpgradeType upgradeType)
    {
        int level = GetUpgradeLevel(upgradeType);
        
        // Already at max level
        if (level >= MAX_UPGRADE_LEVEL)
        {
            return -1; // Indicates max level reached
        }
        
        // Return the cost for the next level
        return upgradeCosts[level];
    }

    private void SaveUpgrades()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("SpeedLevel", speedLevel);
        PlayerPrefs.SetInt("InventoryLevel", inventoryLevel);
        PlayerPrefs.SetInt("HealthLevel", healthLevel);
        PlayerPrefs.SetInt("LanternLevel", lanternLevel);
        PlayerPrefs.SetInt("SpiritPowerLevel", spiritPowerLevel);
        PlayerPrefs.Save();
    }

    private void LoadUpgrades()
    {
        coins = PlayerPrefs.GetInt("Coins", initialCoins);
        speedLevel = PlayerPrefs.GetInt("SpeedLevel", 0);
        inventoryLevel = PlayerPrefs.GetInt("InventoryLevel", 0);
        healthLevel = PlayerPrefs.GetInt("HealthLevel", 0);
        lanternLevel = PlayerPrefs.GetInt("LanternLevel", 0);
        spiritPowerLevel = PlayerPrefs.GetInt("SpiritPowerLevel", 0);
    }

    // Getters for UIToolkitManager
    public int GetCoins() => coins;
    public int GetSpeedLevel() => speedLevel;
    public int GetInventoryLevel() => inventoryLevel;
    public int GetHealthLevel() => healthLevel;
    public int GetLanternLevel() => lanternLevel;
    public int GetSpiritPowerLevel() => spiritPowerLevel;

    // Getters for game mechanics
    public float GetCurrentSpeed()
    {
        if (_playerController == null)
            _playerController = PlayerController.Instance;

        // Return just the speed level since actual speed calculation is handled in PlayerController
        return speedLevel;
    }

    public int GetCurrentInventorySize()
    {
        return baseInventorySize + (inventoryLevel * INVENTORY_INCREMENT);
    }

    public int GetCurrentHealth()
    {
        return baseHealth + (healthLevel * HEALTH_INCREMENT);
    }
    
    public float GetCurrentLanternRange()
    {
        return baseLanternRange * (1f + (lanternLevel * LANTERN_MULTIPLIER));
    }
    
    public float GetCurrentToolSpeed()
    {
        return baseToolSpeed * (1f + (spiritPowerLevel * TOOL_SPEED_MULTIPLIER));
    }
} 
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeData
{
    public EUpgradeType type;
    public int level;
    public int cost;
    public float value;
}

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }

    [SerializeField] private List<UpgradeData> upgrades = new List<UpgradeData>();
    private int completedLevels = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUpgrades();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeUpgrades()
    {
        // Movement Speed upgrades
        upgrades.Add(new UpgradeData { type = EUpgradeType.MovementSpeed, level = 0, cost = 1, value = 1.0f });
        upgrades.Add(new UpgradeData { type = EUpgradeType.MovementSpeed, level = 1, cost = 2, value = 1.2f });
        upgrades.Add(new UpgradeData { type = EUpgradeType.MovementSpeed, level = 2, cost = 3, value = 1.4f });
        upgrades.Add(new UpgradeData { type = EUpgradeType.MovementSpeed, level = 3, cost = 4, value = 1.6f });

        // Inventory Space upgrades
        upgrades.Add(new UpgradeData { type = EUpgradeType.InventorySpace, level = 0, cost = 1, value = 5 });
        upgrades.Add(new UpgradeData { type = EUpgradeType.InventorySpace, level = 1, cost = 2, value = 8 });
        upgrades.Add(new UpgradeData { type = EUpgradeType.InventorySpace, level = 2, cost = 3, value = 12 });
        upgrades.Add(new UpgradeData { type = EUpgradeType.InventorySpace, level = 3, cost = 4, value = 15 });

        // Max Hearts upgrades
        upgrades.Add(new UpgradeData { type = EUpgradeType.MaxHearts, level = 0, cost = 1, value = 3 });
        upgrades.Add(new UpgradeData { type = EUpgradeType.MaxHearts, level = 1, cost = 2, value = 4 });
        upgrades.Add(new UpgradeData { type = EUpgradeType.MaxHearts, level = 2, cost = 3, value = 5 });
        upgrades.Add(new UpgradeData { type = EUpgradeType.MaxHearts, level = 3, cost = 4, value = 6 });
    }

    public void CompleteLevel()
    {
        completedLevels++;
    }

    public int GetCompletedLevels()
    {
        return completedLevels;
    }

    public List<UpgradeData> GetAvailableUpgrades(EUpgradeType type)
    {
        return upgrades.FindAll(u => u.type == type && u.cost <= completedLevels);
    }

    public UpgradeData GetCurrentUpgrade(EUpgradeType type)
    {
        return upgrades.Find(u => u.type == type && u.cost <= completedLevels);
    }

    public bool CanPurchaseUpgrade(EUpgradeType type, int level)
    {
        var upgrade = upgrades.Find(u => u.type == type && u.level == level);
        return upgrade != null && upgrade.cost <= completedLevels;
    }

    public float GetUpgradeValue(EUpgradeType type)
    {
        var upgrade = GetCurrentUpgrade(type);
        return upgrade?.value ?? 0f;
    }
} 
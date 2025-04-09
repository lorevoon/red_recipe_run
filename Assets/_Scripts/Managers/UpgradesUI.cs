using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradesUI : MonoBehaviour
{
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private Transform upgradesContent;
    [SerializeField] private GameObject upgradePrefab;
    [SerializeField] private TextMeshProUGUI levelsText;

    private bool isUpgradesVisible = false;
    private Dictionary<EUpgradeType, GameObject> upgradeUIElements = new Dictionary<EUpgradeType, GameObject>();

    private void Start()
    {
        upgradesPanel.SetActive(false);
        InitializeUpgradeUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleUpgrades();
        }
    }

    private void ToggleUpgrades()
    {
        isUpgradesVisible = !isUpgradesVisible;
        upgradesPanel.SetActive(isUpgradesVisible);

        if (isUpgradesVisible)
        {
            UpdateUpgradesDisplay();
        }
    }

    private void InitializeUpgradeUI()
    {
        foreach (EUpgradeType type in System.Enum.GetValues(typeof(EUpgradeType)))
        {
            GameObject upgradeUI = Instantiate(upgradePrefab, upgradesContent);
            upgradeUIElements[type] = upgradeUI;
        }
    }

    private void UpdateUpgradesDisplay()
    {
        levelsText.text = $"Completed Levels: {PlayerProgress.Instance.GetCompletedLevels()}";

        foreach (var kvp in upgradeUIElements)
        {
            EUpgradeType type = kvp.Key;
            GameObject upgradeUI = kvp.Value;

            // Get current upgrade data
            var currentUpgrade = PlayerProgress.Instance.GetCurrentUpgrade(type);
            var availableUpgrades = PlayerProgress.Instance.GetAvailableUpgrades(type);

            // Update UI elements
            TextMeshProUGUI titleText = upgradeUI.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = upgradeUI.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI costText = upgradeUI.transform.Find("Cost").GetComponent<TextMeshProUGUI>();
            Button upgradeButton = upgradeUI.transform.Find("UpgradeButton").GetComponent<Button>();

            // Set title and description based on upgrade type
            switch (type)
            {
                case EUpgradeType.MovementSpeed:
                    titleText.text = "Movement Speed";
                    descriptionText.text = $"Current: {currentUpgrade.value}x";
                    break;
                case EUpgradeType.InventorySpace:
                    titleText.text = "Inventory Space";
                    descriptionText.text = $"Current: {currentUpgrade.value} slots";
                    break;
                case EUpgradeType.MaxHearts:
                    titleText.text = "Max Hearts";
                    descriptionText.text = $"Current: {currentUpgrade.value} hearts";
                    break;
            }

            // Update cost and button state
            if (availableUpgrades.Count > 0)
            {
                var nextUpgrade = availableUpgrades[0];
                costText.text = $"Cost: {nextUpgrade.cost} levels";
                upgradeButton.interactable = PlayerProgress.Instance.GetCompletedLevels() >= nextUpgrade.cost;
                upgradeButton.onClick.RemoveAllListeners();
                upgradeButton.onClick.AddListener(() => PurchaseUpgrade(type, nextUpgrade.level));
            }
            else
            {
                costText.text = "Max Level Reached";
                upgradeButton.interactable = false;
            }
        }
    }

    private void PurchaseUpgrade(EUpgradeType type, int level)
    {
        if (PlayerProgress.Instance.CanPurchaseUpgrade(type, level))
        {
            // Apply the upgrade
            float newValue = PlayerProgress.Instance.GetUpgradeValue(type);
            
            // Update the UI
            UpdateUpgradesDisplay();
        }
    }
} 
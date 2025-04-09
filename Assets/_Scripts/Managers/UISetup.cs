using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISetup : MonoBehaviour
{
    private void Start()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("UICanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Get the CanvasScaler and set its properties
        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Create Upgrade Panel
        GameObject panelObj = new GameObject("UpgradePanel", typeof(RectTransform));
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.9f);

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(400, 600);
        panelRect.anchoredPosition = Vector2.zero;

        // Create Title
        GameObject titleObj = CreateTextObject("TitleText", panelObj.transform, "UPGRADES", 
            new Vector2(0, 250), new Vector2(400, 50), 36, Color.white);

        // Create Coins Display
        GameObject coinsObj = CreateTextObject("CoinText", panelObj.transform, "Coins: 0",
            new Vector2(0, 200), new Vector2(400, 40), 28, Color.yellow);

        // Create Upgrade Buttons
        CreateUpgradeButton("SpeedButton", panelObj.transform, "Speed Upgrade", 
            new Vector2(0, 100), EUpgradeType.MovementSpeed);
        CreateUpgradeButton("InventoryButton", panelObj.transform, "Inventory Upgrade", 
            new Vector2(0, 0), EUpgradeType.InventorySpace);
        CreateUpgradeButton("HealthButton", panelObj.transform, "Health Upgrade", 
            new Vector2(0, -100), EUpgradeType.MaxHearts);

        // Create UpgradeManager
        GameObject upgradeManagerObj = new GameObject("UpgradeManager");
        UpgradeManager upgradeManager = upgradeManagerObj.AddComponent<UpgradeManager>();

        // Set references in UpgradeManager
        upgradeManager.upgradePanel = panelObj;
        upgradeManager.coinText = coinsObj.GetComponent<TextMeshProUGUI>();

        // Find and assign button references
        upgradeManager.speedUpgradeButton = GameObject.Find("SpeedButton").GetComponent<Button>();
        upgradeManager.inventoryUpgradeButton = GameObject.Find("InventoryButton").GetComponent<Button>();
        upgradeManager.healthUpgradeButton = GameObject.Find("HealthButton").GetComponent<Button>();

        // Find and assign text references
        upgradeManager.speedCostText = GameObject.Find("SpeedButton/CostText").GetComponent<TextMeshProUGUI>();
        upgradeManager.inventoryCostText = GameObject.Find("InventoryButton/CostText").GetComponent<TextMeshProUGUI>();
        upgradeManager.healthCostText = GameObject.Find("HealthButton/CostText").GetComponent<TextMeshProUGUI>();

        upgradeManager.speedValueText = GameObject.Find("SpeedButton/ValueText").GetComponent<TextMeshProUGUI>();
        upgradeManager.inventoryValueText = GameObject.Find("InventoryButton/ValueText").GetComponent<TextMeshProUGUI>();
        upgradeManager.healthValueText = GameObject.Find("HealthButton/ValueText").GetComponent<TextMeshProUGUI>();

        // Set initial panel state
        panelObj.SetActive(false);
    }

    private GameObject CreateTextObject(string name, Transform parent, string text, 
        Vector2 position, Vector2 size, int fontSize, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        return obj;
    }

    private void CreateUpgradeButton(string name, Transform parent, string text, 
        Vector2 position, EUpgradeType upgradeType)
    {
        // Create button object
        GameObject buttonObj = new GameObject(name, typeof(RectTransform));
        buttonObj.transform.SetParent(parent, false);
        
        // Add button component and image
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        
        // Set button colors
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.3f, 0.8f, 1f);
        colors.highlightedColor = new Color(0.3f, 0.4f, 0.9f, 1f);
        colors.pressedColor = new Color(0.4f, 0.5f, 1f, 1f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        button.colors = colors;

        // Set button size and position
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(360, 80);
        buttonRect.anchoredPosition = position;

        // Create name text
        GameObject nameText = CreateTextObject("NameText", buttonObj.transform, text,
            new Vector2(0, 15), new Vector2(360, 30), 24, Color.white);

        // Create value text
        GameObject valueText = CreateTextObject("ValueText", buttonObj.transform, "Current: 0",
            new Vector2(0, -5), new Vector2(360, 25), 20, new Color(0.68f, 0.85f, 0.9f));

        // Create cost text
        GameObject costText = CreateTextObject("CostText", buttonObj.transform, "Cost: 0",
            new Vector2(0, -25), new Vector2(360, 25), 18, Color.yellow);
    }
} 
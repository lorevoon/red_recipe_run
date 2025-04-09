using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySetup : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject inventoryPanelPrefab;
    [SerializeField] private GameObject inventoryItemPrefab;
    private InventoryManager inventoryManager;
    private GameObject inventoryPanel;

    private void Awake()
    {
        Debug.Log("InventorySetup: Awake called");
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            Debug.Log("InventorySetup: Found canvas: " + (canvas != null));
        }

        // Add InventoryManager to this GameObject
        inventoryManager = gameObject.AddComponent<InventoryManager>();
    }

    private void Start()
    {
        Debug.Log("InventorySetup: Start called");
        SetupInventory();
    }

    private void SetupInventory()
    {
        if (inventoryPanelPrefab == null)
        {
            Debug.LogError("InventorySetup: inventoryPanelPrefab is not assigned!");
            return;
        }

        if (inventoryItemPrefab == null)
        {
            Debug.LogError("InventorySetup: inventoryItemPrefab is not assigned!");
            return;
        }

        // Create the main inventory panel
        inventoryPanel = Instantiate(inventoryPanelPrefab, canvas.transform);
        inventoryPanel.name = "InventoryPanel";
        Debug.Log("InventorySetup: Created inventory panel");

        // Set up the panel's RectTransform
        RectTransform panelRect = inventoryPanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.anchorMin = new Vector2(0.1f, 0.1f);
            panelRect.anchorMax = new Vector2(0.9f, 0.9f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
        }

        // Create the title
        CreateTitle(inventoryPanel.transform, "Inventory", new Vector2(0.5f, 0.9f));
        
        // Create scrollable content areas
        GameObject inventoryContent = CreateScrollView("InventoryContent", inventoryPanel.transform, new Vector2(0.1f, 0.45f), new Vector2(0.9f, 0.85f));
        GameObject recipeContent = CreateScrollView("RecipeContent", inventoryPanel.transform, new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.4f));
        
        // Initialize the InventoryManager
        inventoryManager.Initialize(inventoryPanel, inventoryContent.transform, recipeContent.transform, inventoryItemPrefab);
        Debug.Log("InventorySetup: Initialized InventoryManager");
    }

    private GameObject CreateScrollView(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
    {
        // Create scroll view container
        GameObject scrollView = new GameObject(name + "ScrollView", typeof(ScrollRect));
        scrollView.transform.SetParent(parent, false);
        
        RectTransform scrollRectTransform = scrollView.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = anchorMin;
        scrollRectTransform.anchorMax = anchorMax;
        scrollRectTransform.offsetMin = Vector2.zero;
        scrollRectTransform.offsetMax = Vector2.zero;

        // Create viewport
        GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
        viewport.transform.SetParent(scrollView.transform, false);
        viewport.GetComponent<Image>().color = new Color(1, 1, 1, 0.1f);
        
        RectTransform viewportRectTransform = viewport.GetComponent<RectTransform>();
        viewportRectTransform.anchorMin = Vector2.zero;
        viewportRectTransform.anchorMax = Vector2.one;
        viewportRectTransform.offsetMin = Vector2.zero;
        viewportRectTransform.offsetMax = Vector2.zero;

        // Create content container
        GameObject content = new GameObject(name, typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);
        
        RectTransform contentRectTransform = content.GetComponent<RectTransform>();
        contentRectTransform.anchorMin = new Vector2(0, 1);
        contentRectTransform.anchorMax = Vector2.one;
        contentRectTransform.pivot = new Vector2(0.5f, 1);
        contentRectTransform.offsetMin = Vector2.zero;
        contentRectTransform.offsetMax = Vector2.zero;

        // Configure layout group
        VerticalLayoutGroup layoutGroup = content.GetComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 10;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        // Configure content size fitter
        ContentSizeFitter sizeFitter = content.GetComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Configure scroll rect
        ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
        scrollRect.content = contentRectTransform;
        scrollRect.viewport = viewportRectTransform;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        return content;
    }

    private void CreateTitle(Transform parent, string text, Vector2 anchorPosition)
    {
        GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
        titleObj.transform.SetParent(parent, false);
        
        RectTransform rectTransform = titleObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorPosition;
        rectTransform.anchorMax = anchorPosition;
        rectTransform.sizeDelta = new Vector2(200, 50);
        rectTransform.anchoredPosition = Vector2.zero;

        TextMeshProUGUI tmpText = titleObj.GetComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 24;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.white;
    }
} 
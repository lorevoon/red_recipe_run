using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private UIToolkitManager uiToolkitManager;

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
        }
    }

    private void Start()
    {
        // Create UI Toolkit Manager if it doesn't exist
        if (uiToolkitManager == null)
        {
            GameObject uiToolkitObj = new GameObject("UIToolkitManager");
            uiToolkitManager = uiToolkitObj.AddComponent<UIToolkitManager>();
            uiToolkitObj.transform.SetParent(transform);
        }
    }

    public void ToggleInventory()
    {
        if (uiToolkitManager != null)
        {
            uiToolkitManager.ToggleInventory();
        }
    }

    public void ToggleUpgrades()
    {
        if (uiToolkitManager != null)
        {
            uiToolkitManager.ToggleUpgrades();
        }
    }

    public void RefreshInventoryUI()
    {
        if (uiToolkitManager != null)
        {
            uiToolkitManager.RefreshInventoryUI();
        }
    }

    public void RefreshUpgradesUI()
    {
        if (uiToolkitManager != null)
        {
            uiToolkitManager.RefreshUpgradesUI();
        }
    }
}

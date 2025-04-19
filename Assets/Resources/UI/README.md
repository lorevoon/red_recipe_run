# UI Toolkit Implementation

This directory contains UI assets for the UI Toolkit-based inventory and upgrades UI system.

## Files

- **InventoryUI.uxml**: The UXML layout for the inventory panel
- **UpgradesUI.uxml**: The UXML layout for the upgrades panel
- **InventoryItemTemplate.uxml**: Template for inventory items
- **GameUI.uss**: Unified stylesheet for all UI elements

## Usage

The UI system is managed by the `UIToolkitManager` class, which is created by the `UIManager`. The entire UI system is initialized by the `UISetup` class, which creates the necessary manager objects if they don't exist.

### Key Components

1. **UIToolkitManager**: Main class that handles UI creation, updates, and user interaction
2. **UIManager**: Lightweight wrapper that provides access to the UIToolkitManager
3. **UISetup**: Ensures all required managers are created

### Keyboard Shortcuts

- Press `I` to toggle the Inventory panel
- Press `U` to toggle the Upgrades panel

## Customization

The UI can be customized by modifying:

- UXML files to change layout structure
- USS file to change styling
- UIToolkitManager.cs to change behavior

## Integration with Game Systems

The UI integrates with:

- **InventoryManager**: For displaying collected ingredients
- **UpgradeManager**: For displaying and purchasing upgrades
- **PlayerInventory**: For showing current/maximum capacity

## Dependencies

- Requires Unity 2020.1 or newer with UI Toolkit package 
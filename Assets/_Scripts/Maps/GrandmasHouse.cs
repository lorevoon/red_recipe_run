using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandmasHouse : MonoBehaviour
{
    private bool _isPlayerInRange;
    private Vector3 _doorPosition;
    private Vector3 _doorEndPosition;

    private PlayerController _playerController;
    private InventoryManager _inventoryManager;
    private RecipeManager _recipeManager;
    
    // [SerializeField] private Transform _handTransform;
    private Animator _animator;
    
    private void Start()
    {
        CameraManager.Instance.AllVirtualCameras[(int)ECamera.GrandmasHouse].Follow = transform;
        _doorPosition = transform.position + new Vector3(0.8f, -3.6f, 0);
        _doorEndPosition = transform.position + new Vector3(0.8f, -2.5f, 0);

        _playerController = PlayerController.Instance;
        _inventoryManager = InventoryManager.Instance;
        _recipeManager = RecipeManager.Instance;
        
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!_isPlayerInRange) return;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = true;
            CameraManager.Instance.SwapCamera(CameraManager.Instance.AllVirtualCameras[(int)ECamera.GrandmasHouse]);
            // UIToolkitManager.Instance.ToggleUpgrades();
        }
        else if (other.CompareTag("Ingredient"))
        {
            EIngredient ingredient = other.gameObject.GetComponent<Ingredient>().IngredientType;
            Debug.Log("trying to put: " + ingredient);
            
            // Only accept ingredients that are part of the recipe
            if (_recipeManager.IsInRecipe(ingredient))
            {
                // First start the animation to pull the ingredient to the door
                StartCoroutine(PullIngredientToDoorRoutine(other.gameObject, ingredient));
                // hand grab animation
            }
            else
            {
                // Optional: Add some visual feedback that this ingredient is not needed
                Debug.Log("Ingredient not needed for current recipe: " + ingredient);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _isPlayerInRange = false;
        CameraManager.Instance.SwapCamera(CameraManager.Instance.AllVirtualCameras[(int)ECamera.RecipeRun]);

        // if (UIToolkitManager.Instance != null)
        // {
        //     UIToolkitManager.Instance.ToggleUpgrades();
        // }
        // else
        // {
        //     Debug.LogWarning("UIToolkitManager.Instance is null during OnTriggerExit2D!");
        // }
    }


    private IEnumerator PullIngredientToDoorRoutine(GameObject ingredient, EIngredient ingredientType)
    {
        float duration = 1.2f; // time to reach the door
        float elapsed = 0f;
        Vector3 start = ingredient.transform.position;
        Vector3 end = _doorPosition;
        
        Rigidbody2D rb = ingredient.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            t = t * t * (3f - 2f * t);

            ingredient.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        
        ingredient.transform.position = end;
        
        _animator.SetTrigger("TakeItem");
        
        // Only subtract from recipe after the witch has taken the ingredient
        // The SubtractFromRecipe method now handles showing the popup and won't immediately generate a new recipe
        _recipeManager.SubtractFromRecipe(ingredientType);

        yield return new WaitForSeconds(3f);
        duration = 2f;
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            t = t * t * (3f - 2f * t);

            ingredient.transform.position = Vector3.Lerp(end, _doorEndPosition, t);
            yield return null;
        }
        
        // Destroy the ingredient after it's been taken
        Destroy(ingredient);
    }

    private IEnumerator CelebrateCompletedRecipeRoutine()
    {
        _playerController.EnableMovement(false);
        _recipeManager.ToggleRecipe();
        
        _playerController.EnableMovement();
        yield return null;
    }
}
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
        if (!this.isActiveAndEnabled) return;

        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = true;
            if (CameraManager.Instance != null && 
                CameraManager.Instance.AllVirtualCameras != null && 
                CameraManager.Instance.AllVirtualCameras[(int)ECamera.GrandmasHouse] != null)
            {
                CameraManager.Instance.SwapCamera(CameraManager.Instance.AllVirtualCameras[(int)ECamera.GrandmasHouse]);
            }
        }
        else if (other.CompareTag("Ingredient") && other.gameObject != null)
        {
            Ingredient ingredientComponent = other.gameObject.GetComponent<Ingredient>();
            if (ingredientComponent == null || _recipeManager == null) return;
            
            EIngredient ingredient = ingredientComponent.IngredientType;
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
        else if (other.CompareTag("LostKid") && other.gameObject != null)
        {
            // Only accept lost kid if the current recipe is Lost Kid
            if (_recipeManager != null && _recipeManager.GetCurrentRecipe().RecipeName == "Lost Kid")
            {
                // Use a larger detection range for the lost kid
                float detectionRange = 3.5f;
                Vector3 kidPos = other.transform.position;
                Vector3 doorPos = _doorPosition;
                float distToDoor = Vector3.Distance(kidPos, doorPos);
                if (distToDoor <= detectionRange)
                {
                    StartCoroutine(PullLostKidToDoorRoutine(other.gameObject));
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!this.isActiveAndEnabled || CameraManager.Instance == null) return;
        
        _isPlayerInRange = false;
        
        // Only try to swap camera if the CameraManager and its cameras still exist
        if (CameraManager.Instance != null && 
            CameraManager.Instance.AllVirtualCameras != null && 
            CameraManager.Instance.AllVirtualCameras[(int)ECamera.RecipeRun] != null)
        {
            CameraManager.Instance.SwapCamera(CameraManager.Instance.AllVirtualCameras[(int)ECamera.RecipeRun]);
        }
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

    private IEnumerator PullLostKidToDoorRoutine(GameObject lostKid)
    {
        // 1. Immediately disable player movement
        if (_playerController != null)
            _playerController.EnableMovement(false);

        // 2. If the player is blocking the door, swap positions
        Vector3 doorPos = _doorPosition;
        if (_playerController != null)
        {
            float playerDist = Vector3.Distance(_playerController.transform.position, doorPos);
            if (playerDist < 0.7f)
            {
                Vector3 temp = _playerController.transform.position;
                _playerController.transform.position = lostKid.transform.position;
                lostKid.transform.position = temp;
                // Zero out player velocity if Rigidbody2D exists
                Rigidbody2D playerRb = _playerController.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = Vector2.zero;
                }
                // Ensure movement is still disabled
                _playerController.EnableMovement(false);
            }
        }

        // 3. Wait a frame to let physics/colliders settle
        yield return null;

        // 4. Animate lost kid to the door
        float duration = 1.2f;
        float elapsed = 0f;
        Vector3 start = lostKid.transform.position;
        Vector3 end = _doorPosition;

        Rigidbody2D rb = lostKid.GetComponent<Rigidbody2D>();
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
            lostKid.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        lostKid.transform.position = end;

        // 5. Trigger the hand animation
        _animator.SetTrigger("TakeItem");

        // 6. Complete the lost kid recipe (shows popup)
        _recipeManager.ForceCompleteLostKidRecipe();

        // 7. Wait for popup to close
        while (_recipeManager.IsWaitingForPopupClosure)
        {
            yield return null;
        }

        // Animate lost kid into the house
        yield return new WaitForSeconds(0.5f);
        duration = 2f;
        elapsed = 0f;
        Vector3 end2 = _doorEndPosition;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);
            lostKid.transform.position = Vector3.Lerp(end, end2, t);
            yield return null;
        }
        lostKid.transform.position = end2;

        // Destroy the lost kid after it's been taken
        Destroy(lostKid);

        // 8. Re-enable player movement
        if (_playerController != null)
            _playerController.EnableMovement(true);
    }

    private IEnumerator CelebrateCompletedRecipeRoutine()
    {
        _playerController.EnableMovement(false);
        _recipeManager.ToggleRecipe();
        
        _playerController.EnableMovement();
        yield return null;
    }
}
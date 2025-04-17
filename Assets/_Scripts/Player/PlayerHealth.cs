using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerHealth : MonoBehaviour
{
    public int Health = 3;
    private Transform _playerTransform;
    private Image _heartImage; 
    private TMP_Text _healthText;
    private GameObject _damageEffectPrefab; 

    void Start()
    {
        // PlayerEvents.HpChanged += OnPlayerHPChanged;
        _playerTransform = PlayerController.Instance.gameObject.transform;
        _heartImage = GameManager.Instance.gameObject.GetComponentInChildren<Image>();
        _healthText = GameManager.Instance.gameObject.GetComponentInChildren<TMP_Text>();
        _damageEffectPrefab = Resources.Load("Prefabs/VFXs/TakeDamageVFX").GameObject();
        
        UpdateHealthDisplay();
    }

    public void TakeDamage(int damage, Vector3 wolfPosition)
    {
        PlayerEvents.HpChanged.Invoke(-damage);
        Health -= damage;
        Health = Mathf.Max(Health, 0); 
        UpdateHealthDisplay();

        if (_damageEffectPrefab != null && Health > 0)
        {
            var playerPosition = _playerTransform.position;
            Vector3 direction = wolfPosition - playerPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Instantiate(_damageEffectPrefab, playerPosition, Quaternion.Euler(angle, 90f, 0));
        }

        if (Health <= 0)
        {
            Debug.Log("Player is dead!");
        }
    }

    void UpdateHealthDisplay()
    {
        _healthText.text = Health.ToString();
    }
}

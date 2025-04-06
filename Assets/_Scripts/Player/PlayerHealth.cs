using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PlayerHealth : MonoBehaviour
{
    public int health = 3; 
    // public Image heartImage; 
    // public TMP_Text healthText;
    public GameObject damageEffectPrefab; 

    void Start()
    {
        // UpdateHealthDisplay();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Max(health, 0); 
        // UpdateHealthDisplay();

        if (damageEffectPrefab != null && health > 0)
        {
            Instantiate(damageEffectPrefab, transform.position, Quaternion.identity);
        }

        if (health <= 0)
        {
            Debug.Log("Player is dead!");
        }
    }

    // void UpdateHealthDisplay()
    // {
    //     healthText.text = health.ToString();
    // }
}

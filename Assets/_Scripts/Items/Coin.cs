using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private float bobHeight = 0.2f;
    
    private Vector3 startPosition;
    private float timeOffset;

    private void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    private void Update()
    {
        // Rotate the coin
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        
        // Bob up and down
        float yOffset = Mathf.Sin((Time.time + timeOffset) * bobSpeed) * bobHeight;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.AddCoins(value);
                Debug.Log($"Collected {value} coins!");
            }
            Destroy(gameObject);
        }
    }
} 
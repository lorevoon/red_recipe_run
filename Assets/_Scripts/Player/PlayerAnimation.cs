using UnityEngine;

// Define the enum outside the class at namespace level
public enum Direction { Up, Down, Left, Right }

public class NewMonoBehaviourScript : MonoBehaviour
{
    //public float moveSpeed = 10f;
    [Header("non-moving Sprites")]
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    
    [Header("Walking Sprites")]
    public Sprite walkDownLeftFoot;
    public Sprite walkDownRightFoot;
    public Sprite walkUpLeftFoot;
    public Sprite walkUpRightFoot;
    public Sprite walkLeftFootForward;
    public Sprite walkLeftFootBack;
    public Sprite walkRightFootForward;
    public Sprite walkRightFootBack;

    [Header("Settings")]
    public float stepInterval = 0.1f;
    public float moveThreshold = 0.1f;
    private float stepTimer;

    private Direction currentDirection;
    private bool isLeftFootForward; // Tracks which foot is forward


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer.sprite = backSprite;
        currentDirection = Direction.Up;
        stepTimer = 0;
        isLeftFootForward = false;
    }

    void Update()
    {


        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // rb.velocity = new Vector2(moveX, moveY).normalized * moveSpeed;

        // Change sprite based on direction
        if (moveY > 0) // Moving Up
        {
            spriteRenderer.sprite = backSprite;
            currentDirection = Direction.Up;
            moveFeet();
            //transform.Rotate(0, 0, 90); // Immediate 90° turn
        }
        else if (moveY < 0) // Moving Down
        {
            spriteRenderer.sprite = frontSprite;
            currentDirection = Direction.Down;
            moveFeet();
            //transform.Rotate(0, 0, 90); // Immediate 90° turn
        }
        else if (moveX < 0) // Moving Left
        {
            spriteRenderer.sprite = leftSprite;
            currentDirection = Direction.Left;
            moveFeet();
            //transform.Rotate(0, 0, 90); // Immediate 90° turn
        }
        else if (moveX > 0) // Moving Right
        {
            spriteRenderer.sprite = rightSprite;
            currentDirection = Direction.Right;
            moveFeet();
            //transform.Rotate(0, 0, 90); // Immediate 90° turn
        }
    }


    void moveFeet()
    {
        bool isMoving = rb.linearVelocity.magnitude > moveThreshold;

        if (isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                isLeftFootForward = !isLeftFootForward;
                stepTimer = stepInterval;
            }

            switch (currentDirection)
            {
                case Direction.Up:
                    spriteRenderer.sprite = isLeftFootForward ? walkUpLeftFoot : walkUpRightFoot;
                    break;
                case Direction.Down:
                    spriteRenderer.sprite = isLeftFootForward ? walkDownLeftFoot : walkDownRightFoot;
                    break;
                case Direction.Left:
                    spriteRenderer.sprite = isLeftFootForward ? walkLeftFootForward : walkLeftFootBack;
                    break;
                case Direction.Right:
                    spriteRenderer.sprite = isLeftFootForward ? walkRightFootForward : walkRightFootBack;
                    break;
            }
        }
        else
        {
            // Return to idle pose
            switch (currentDirection)
            {
                case Direction.Up: spriteRenderer.sprite = backSprite; break;
                case Direction.Down: spriteRenderer.sprite = frontSprite; break;
                case Direction.Left: 
                    spriteRenderer.sprite = leftSprite; 
                    break;
                case Direction.Right: 
                    spriteRenderer.sprite = rightSprite; // Reuse left sprite
                    break;
            }
        }
    }
}

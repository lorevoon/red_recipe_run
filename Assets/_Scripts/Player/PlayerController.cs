using UnityEngine;
using System.Collections.Generic; // Add this line to fix the error


public class PlayerController : Singleton<PlayerController>
{
   private PlayerInventory _playerInventory;
   private AudioSource _audioSource;
  
   public GameObject Tool;
   public GameObject Basket;
  
   private float _playerSpeed = 10; // speed player moves
   private float _toolSpeed = 3;


   void Start()
   {
       _playerInventory = GetComponent<PlayerInventory>();
       _audioSource = GetComponent<AudioSource>();
      
       if (Tool == null)
           Tool = GameObject.FindWithTag("Tool");
   }
    void Update() {
       Move();
       HandlePickup();
   }


   void HandlePickup()
   {
       if (Input.GetKeyDown(KeyCode.Z))
       {
           // Try to pick up an ingredient
           _playerInventory.TryPickUpIngredient();
          
       }


       if (Input.GetKeyDown(KeyCode.X))
       {
           _playerInventory.DropIngredient();
          
       }
   }
    void Move()
   {
       if (Tool == null) return; // Ensure the Tool is not null before moving it


       if (Input.GetKey("down")) // Press up arrow key to move forward on the Y AXIS
       {
           transform.eulerAngles = new Vector3(0, 0, 0); // Left
       }
      
       if (Input.GetKey("up")) // Press down arrow key to move backward on the Y AXIS
       {
           transform.eulerAngles = new Vector3(0, 0, 180); // Left
       }
      
       if (Input.GetKey("right")) // Press left arrow key to move left on the X AXIS
       {
           transform.eulerAngles = new Vector3(0, 0, 90); // Right
       }
      
       if (Input.GetKey("left")) // Press right arrow key to move right on the X AXIS
       {
           transform.eulerAngles = new Vector3(0, 0, -90); // Left
       }


       // Then handle movement if any arrow key is pressed
       if (Input.GetKey("down") || Input.GetKey("up") ||
           Input.GetKey("right") || Input.GetKey("left"))
       {
           // Move in the direction the player is facing
           transform.Translate(Vector3.up * -_playerSpeed * Time.deltaTime, Space.Self);
       }


       if (Input.GetKeyDown("space"))
       {
           // Rotate the Tool to a specific angle (e.g., 45 degrees around the Z-axis)
           Quaternion targetRotation = Quaternion.Euler(0f, 0f, 45f);
           Tool.transform.rotation = Quaternion.RotateTowards(Tool.transform.rotation, targetRotation, _toolSpeed * Time.deltaTime);
       }
   }
}

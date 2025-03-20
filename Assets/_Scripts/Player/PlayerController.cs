using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private float playerSpeed = 10; // speed player moves
    private float toolRotateSpeed = 3;
    public GameObject tool;

    void Start() {
        if (tool == null)
            tool = GameObject.FindWithTag("Tool");
        
    }
  
    void Update () {
        Move(); // Player Movement 
    }
  
    void Move()
    {
        if (tool == null) return; // Ensure the tool is not null before moving it

        if (Input.GetKey("up")) // Press up arrow key to move forward on the Y AXIS
        {
            transform.Translate(0, playerSpeed * Time.deltaTime, 0);
            //tool.transform.Translate(0, playerSpeed * Time.deltaTime, 0);
        }
        
        if (Input.GetKey("down")) // Press down arrow key to move backward on the Y AXIS
        {
            transform.Translate(0, -playerSpeed * Time.deltaTime, 0);
            // tool.transform.Translate(0, -playerSpeed * Time.deltaTime, 0);
        }
        
        if (Input.GetKey("left")) // Press left arrow key to move left on the X AXIS
        {
            transform.Translate(-playerSpeed * Time.deltaTime, 0, 0);
            // tool.transform.Translate(-playerSpeed * Time.deltaTime, 0, 0);
        }
        
        if (Input.GetKey("right")) // Press right arrow key to move right on the X AXIS
        {
            transform.Translate(playerSpeed * Time.deltaTime, 0, 0);
            // tool.transform.Translate(playerSpeed * Time.deltaTime, 0, 0);
        }

        if (Input.GetKeyDown("space"))
        {
            // Rotate the tool to a specific angle (e.g., 45 degrees around the Z-axis)
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 45f);
            tool.transform.rotation = Quaternion.RotateTowards(tool.transform.rotation, targetRotation, toolRotateSpeed * Time.deltaTime);
        }
    }
}
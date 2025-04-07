using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Game : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log("Button clicked!"); // Check Unity Console for this message
        if (gameObject.CompareTag("StartButton"))
        {
            SceneManager.LoadScene("PlayerScene");
        }
    }

}
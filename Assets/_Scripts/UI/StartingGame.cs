using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingGame : MonoBehaviour
{
    // public string targetScene = "PlayerScene"; // Assign in Inspector
    private string targetScene = "StartingCutscene";

    private void OnMouseDown()
    {
        if (GetComponent<Collider2D>() != null)
        {
            // SceneManager.LoadScene(targetScene);
            SceneTransitionManager.Instance.TransitionTo(targetScene);
        }
    }
}
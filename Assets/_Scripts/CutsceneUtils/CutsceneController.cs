using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    private string targetScene = "PlayerScene";
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            SceneTransitionManager.Instance.TransitionTo(targetScene);
    }
}
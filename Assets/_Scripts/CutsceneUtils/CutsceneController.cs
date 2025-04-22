using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    private string _targetScene = "PlayerScene";
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private GameObject _tutorialPage;
    [SerializeField] private GameObject _skipSceneText;

    private void Start()
    {
        StartCoroutine(WaitForTimeline());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            SceneTransitionManager.Instance.TransitionTo(_targetScene);
    }

    private IEnumerator WaitForTimeline()
    {
        yield return new WaitForSeconds(5f);
        _dialogueManager.gameObject.SetActive(true);
        _dialogueManager.StartDialogue();
    }

    public void DisplayTutorial()
    {
        _dialogueManager.gameObject.SetActive(false);
        _skipSceneText.SetActive(false);
        _tutorialPage.SetActive(true);
    }
}
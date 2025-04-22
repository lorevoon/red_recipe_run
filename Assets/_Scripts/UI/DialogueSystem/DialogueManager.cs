using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private CutsceneController _cutsceneController;
    
    public Dialogue dialogue;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    // private Animator _animator;
    private Queue<string> _names;
    private Queue<string> _sentences;

    private void Start()
    {
        // _animator = GetComponent<Animator>();
        _names = new Queue<string>();
        _sentences = new Queue<string>();
        StartDialogue();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            DisplayNextSentence();
    }

    public void StartDialogue()
    {
        // _animator.SetBool("IsOpen", true);
        
        _names.Clear();
        _sentences.Clear();

        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            _names.Enqueue(dialogue.names[i]);
            _sentences.Enqueue(dialogue.sentences[i]);
        }
        
        DisplayNextSentence();
    }

    private void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = _sentences.Dequeue();
        StopAllCoroutines();
        nameText.text = _names.Dequeue();
        StartCoroutine(TypeSentence(sentence));
    }

    private void EndDialogue()
    {
        // _animator.SetBool("IsOpen", false);
        _cutsceneController.DisplayTutorial();
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
    }
    
}
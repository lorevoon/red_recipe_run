using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CreditsScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject credit_panel;
    void Start()
    {
        // Set the panel to be initially disabled
        credit_panel.SetActive(false);
    }

    public void OpenPanel()
    {
        if (credit_panel != null) 
        {
            credit_panel.SetActive(true);
        }
        
    }

    public void ClosePanel()
    {
        if (credit_panel != null) 
        {
            credit_panel.SetActive(false);
        }
        // Debug.Log("Button clicked!"); // Check Unity Console for this message
        // bool isActive = credit_panel.activeSelf;
        // if (gameObject.CompareTag("ClosePanel") && isActive)
        // {
        //     credit_panel.SetActive(false);
        // }
    }

}
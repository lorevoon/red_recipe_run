using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandmasHouse : MonoBehaviour
{
    private bool _isPlayerInRange;
    
    private void Start()
    {
        CameraManager.Instance.AllVirtualCameras[1].transform.position = 
            new Vector3(transform.position.x, transform.position.y-1, -10);
    }

    private void Update()
    {
        if (!_isPlayerInRange) return;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _isPlayerInRange = true;
        CameraManager.Instance.SwapCamera(CameraManager.Instance.AllVirtualCameras[1]);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _isPlayerInRange = false;
        CameraManager.Instance.SwapCamera(CameraManager.Instance.AllVirtualCameras[0]);
    }
}
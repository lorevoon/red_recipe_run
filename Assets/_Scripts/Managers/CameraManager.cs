using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public CinemachineVirtualCamera[] AllVirtualCameras; // serialized
    public Camera InGameMainCamera;
    public Camera UICamera;
    
    public CinemachineVirtualCamera CurrentCamera;
    private CinemachineFramingTransposer _framingTransposer;
    
    private bool _isFramingTransposed = false;
    
    void Start()
    {
        for (int i = 0; i < AllVirtualCameras.Length; i++)
        {
            if (AllVirtualCameras[i].enabled)
            {
                CurrentCamera = AllVirtualCameras[i];
                _framingTransposer = CurrentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                CurrentCamera.Follow = PlayerController.Instance.gameObject.transform;
                break;
            }
        }
    
        // _normalYPanAmount = _framingTransposer.m_YDamping;
        if (_framingTransposer == null) return;
        _isFramingTransposed = true;
    }
    
    // swaps from camera1 (initial) to camera2 (final)
    public void SwapCamera(CinemachineVirtualCamera camera2)
    {
        CinemachineVirtualCamera camera1 = CurrentCamera;
        if (!isActiveAndEnabled) return;
        
        // turn camera1 off, turn camera2 on
        if (camera1) camera1.enabled = false;
        camera2.enabled = true;
        StartCoroutine(AssignCollider(Array.IndexOf(AllVirtualCameras, camera2)));
        CurrentCamera = camera2;
        
        // _framingTransposer = CurrentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        // _isFramingTransposed = _framingTransposer != null;
        // GameObject followObject = GameObject.Find("CameraFollowingObject");
        // if (followObject != null) CurrentCamera.Follow = followObject.transform;
    }
    
    private IEnumerator AssignCollider(int cameraIndex)
    {
        if (cameraIndex == (int)ECamera.MainMenu) yield break;
        yield return null;
        var cam = GameObject.Find(cameraIndex.ToString());
        if (cam)
        {
            var collider = cam.GetComponent<CompositeCollider2D>();
            if (collider)
                AllVirtualCameras[cameraIndex].GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = collider;
        }
        if (cameraIndex is (int)ECamera.RecipeRun or (int)ECamera.GrandmasHouse)
            CurrentCamera.Follow = PlayerController.Instance.gameObject.transform;
    }
}

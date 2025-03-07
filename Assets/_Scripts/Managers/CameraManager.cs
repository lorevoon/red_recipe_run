using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    // public CinemachineVirtualCamera[] AllVirtualCameras;
    // public Camera RecipeRunMapCamera;
    
    // public CinemachineVirtualCamera CurrentCamera;
    // private CinemachineFramingTransposer _framingTransposer;
    
    // private bool _isFramingTransposed = false;
    
    // TODO this needs to swap between cameras depending on the scene
    // TODO index stuff doesn't make sense for now gotta edit to enums after making Cinemachine cameras
    
    // void Start()
    // {
    //     for (int i = 0; i < AllVirtualCameras.Length; i++)
    //     {
    //         if (AllVirtualCameras[i].enabled)
    //         {
    //             CurrentCamera = AllVirtualCameras[i];
    //             _framingTransposer = CurrentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    //         }
    //     }
    //
    //     // _normalYPanAmount = _framingTransposer.m_YDamping;
    //     if (_framingTransposer == null) return;
    //     
    //     _normalYOffsetAmount = _framingTransposer.m_TrackedObjectOffset.y;
    //     _isFramingTransposed = true;
    // }
    
    // public void SwapCamera(CinemachineVirtualCamera camera2)
    // {
    //     CinemachineVirtualCamera camera1 = CurrentCamera;
    //     if (!isActiveAndEnabled) return;
    //     if (camera1) camera1.enabled = false;
    //     camera2.enabled = true;
    //     StartCoroutine(AssignCollider(Array.IndexOf(AllVirtualCameras, camera2)));
    //     CurrentCamera = camera2;
    //     _framingTransposer = CurrentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    //     _isFramingTransposed = _framingTransposer != null;
    //     GameObject followObject = GameObject.Find("CameraFollowingObject");
    //     if (followObject != null) CurrentCamera.Follow = followObject.transform;
    // }
    //
    // private IEnumerator AssignCollider(int cameraIndex)
    // {
    //     if (cameraIndex == 1) yield break;
    //     yield return null;
    //     var cam = GameObject.Find(cameraIndex.ToString());
    //     if (cam)
    //     {
    //         var collider = cam.GetComponent<CompositeCollider2D>();
    //         if (collider)
    //             AllVirtualCameras[cameraIndex].GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = collider;
    //     }
    //     if (cameraIndex is 5 or 6)
    //         CurrentCamera.Follow = PlayerController.Instance.gameObject.transform;
    // }
}

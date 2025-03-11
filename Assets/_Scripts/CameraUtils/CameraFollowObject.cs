using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    private Transform _playerTransform;
    
    // TODO we might not need this if we don't do fancy stuff with the camera idk
    
    private void Start()
    {
        _playerTransform = PlayerController.Instance.transform;
        if (CameraManager.Instance.CurrentCamera == CameraManager.Instance.AllVirtualCameras[(int)ECamera.RecipeRun])
        {
            CameraManager.Instance.CurrentCamera.Follow = _playerTransform;
            Destroy(gameObject);
        }
    }
    
    private void Update()
    {
        transform.position = _playerTransform.position;
    }
}

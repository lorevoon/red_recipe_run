using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera _vCam;
    private float _shakeIntensity = 1.3f;
    private float _shakeTime = 0.2f;

    private float _remainingTime;
    private CinemachineBasicMultiChannelPerlin _cbmcp;
    
    private void Awake()
    {
        PlayerEvents.HpChanged += OnPlayerHPChanged;
        _vCam = GetComponent<CinemachineVirtualCamera>();
        _cbmcp = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = 0.0f;
        _remainingTime = 0.0f;
    }
    
    private void OnPlayerHPChanged(int changeAmount)
    {
        if (changeAmount < 0) // when damaged
        {
            _cbmcp.m_AmplitudeGain = Mathf.Max(_cbmcp.m_AmplitudeGain, 
                _shakeIntensity * Mathf.Clamp(Mathf.Abs(changeAmount), 1, 3.5f));
            _remainingTime += (0.2f - _remainingTime);
        }
    }
    
    public void StopShake()
    {
        _cbmcp.m_AmplitudeGain = 0.0f;
        _remainingTime = 0;
    }
    
    private void LateUpdate()
    {
        if (_remainingTime > 0)
        {
            _remainingTime -= Time.unscaledDeltaTime;
            if (_remainingTime <= 0)
            {
                StopShake();
            }
        }
    }
}

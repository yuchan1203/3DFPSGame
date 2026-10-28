using UnityEngine;

// 카메라 반동 값을 누적하고 시간에 따라 0으로 복귀시킨다.
public class CameraRecoil
{
    private float _pitch = 0f;
    private float _yaw = 0f;

    public float Pitch => _pitch;
    public float Yaw => _yaw;

    public void Add(float pitch, float yaw)
    {
        _pitch += pitch;
        _yaw += yaw;
    }

    public void Recover(float recoverySpeed, float deltaTime)
    {
        _pitch = Mathf.Lerp(_pitch, 0f, recoverySpeed * deltaTime);
        _yaw = Mathf.Lerp(_yaw, 0f, recoverySpeed * deltaTime);
    }
}

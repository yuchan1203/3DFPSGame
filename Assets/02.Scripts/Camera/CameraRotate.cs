using UnityEngine;

// 역할: 마우스를 움직이면 카메라를 그 방향으로 회전
public class CameraRotate : MonoBehaviour
{
    private float _rotationSpeed = 200f;
    private float _accumulationX = 0f;
    private float _accumulationY = 0f;

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        //GameLogger.Log($"Mouse X: {mouseX}, Mouse Y: {mouseY}");
        _accumulationX += mouseX * _rotationSpeed * Time.deltaTime;
        _accumulationY -= mouseY * _rotationSpeed * Time.deltaTime;
        _accumulationY = Mathf.Clamp(_accumulationY, -90, 90);
        transform.eulerAngles = new Vector3(_accumulationY, _accumulationX, 0f);
    }
}
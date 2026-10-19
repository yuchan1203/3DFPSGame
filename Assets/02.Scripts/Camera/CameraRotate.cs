using UnityEngine;

// 역할: 마우스를 움직이면 카메라를 그 방향으로 회전
public class CameraRotate : MonoBehaviour
{
    private float _rotateSpeed = 200f;

    private void Update()
    {
        // 1. 마우스 입력을 받는다.
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Debug.Log($"Mouse X: {mouseX}, MouseY: {mouseY}");
        // 2. 마우스 입력에 따라 회전할 방향을 구한다.
        Vector3 direction = new Vector3(-mouseY, mouseX, 0);
        // 3. 그 방향으로 회전시킨다.
        // 새로운 각도 = 현재 각도 + 회전 속도 * 시간
        transform.eulerAngles += direction * (_rotateSpeed * Time.deltaTime);
    }
}
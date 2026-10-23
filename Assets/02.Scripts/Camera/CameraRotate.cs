using UnityEngine;
public class CameraRotate : MonoBehaviour
{
    private float _rotationSpeed = 200f;
    private float _accumulationX = 0f;
    private float _accumulationY = 0f;
    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Playing)
        {
            return;
        }
        CameraMove();
    }
    private void CameraMove()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        _accumulationX += mouseX * _rotationSpeed * Time.deltaTime;
        _accumulationY -= mouseY * _rotationSpeed * Time.deltaTime;
        _accumulationY = Mathf.Clamp(_accumulationY, -90, 90);
        transform.eulerAngles = new Vector3(_accumulationY, _accumulationX, 0f);
    }
}
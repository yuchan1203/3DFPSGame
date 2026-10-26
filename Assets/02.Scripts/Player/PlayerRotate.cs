using UnityEngine;
public class PlayerRotate : MonoBehaviour
{
    private float _rotationSpeed = 200f;
    private float _accumulationX = 0f;

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Playing)
        {
            return;
        }
        // 커서가 풀려 있으면(UI를 누르는 중이면) 시선을 돌리지 않는다.
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }
        float mouseX = Input.GetAxis("Mouse X");
        _accumulationX += mouseX * _rotationSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0f, _accumulationX, 0f);
    }
}
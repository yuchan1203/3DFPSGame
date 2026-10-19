using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private float _moveSpeed = 20f;

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(h, 0f, v).normalized;
        direction = transform.TransformDirection(direction);
        transform.position += direction * (_moveSpeed * Time.deltaTime);
    }
}
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpPower = 5f;
    private const float Gravity = -20f;
    private float _yVelocity = 0f;
    private void Awake()
    {
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
            if (_characterController == null)
            {
                Debug.LogError("CharacterController가 PlayerMove에 없습니다", this);
            }
        }
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(h, 0f, v).normalized;
        direction = transform.TransformDirection(direction);
        if (_characterController.isGrounded == false)
        {
            _yVelocity += Gravity * Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump") && _characterController.isGrounded)
        {
            _yVelocity = _jumpPower;
        }
        Vector3 velocity = direction * _moveSpeed;
        velocity.y = _yVelocity;
        _characterController.Move(velocity * Time.deltaTime);
    }
}
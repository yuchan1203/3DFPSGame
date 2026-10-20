using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpPower = 5f;
    [SerializeField] private float _sprintSpeed = 10f;
    [SerializeField] private float _staminaRecovery = 15f;
    [SerializeField] private float _sprintCost = 10f;
    [SerializeField] private float _jumpCost = 20f;

    [SerializeField] private CharacterController _characterController;

    private const float Gravity = -20f;
    private const int MaxJumps = 2;
    private const float MaxStamina = 100f;

    private float _yVelocity = 0f;
    private int _jumpCount = 0;
    private float _stamina = 0f;

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
        else
        {
            _yVelocity = 0;
        }
        if (Input.GetButtonDown("Jump") && _characterController.isGrounded)
        {
            _yVelocity = _jumpPower;
        }
        Vector3 velocity = direction * _moveSpeed;
        if (Input.GetButtonDown("Shift"))
        {
            velocity = direction * (_moveSpeed + 5f);
        }
        else
        {
            velocity = direction * _moveSpeed;
        }

        velocity.y = _yVelocity;
        _characterController.Move(velocity * Time.deltaTime);
    }
}
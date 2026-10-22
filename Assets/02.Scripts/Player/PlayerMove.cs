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

        _stamina = MaxStamina;
    }

    private void Update()
    {
        _stamina += Mathf.Min(MaxStamina, _stamina + Time.deltaTime * _staminaRecovery);
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
            _yVelocity = -1f;
            _jumpCount = 0;
        }
        if (Input.GetButtonDown("Jump") && _jumpCount < MaxJumps && _stamina >= _jumpCost)
        {
            _yVelocity = _jumpPower;
            _jumpCount++;
            _stamina -= _jumpCost;
        }
        float requiredStamina = _sprintCost * Time.deltaTime;
        bool isSprint = Input.GetButton("Sprint") && _stamina >= requiredStamina;
        float currentSpeed = _moveSpeed;

        if (isSprint)
        {
            currentSpeed = _sprintSpeed;
            _stamina -= requiredStamina;
        }
        Vector3 velocity = direction * currentSpeed;
        velocity.y = _yVelocity;
        _characterController.Move(velocity * Time.deltaTime);
    }
}
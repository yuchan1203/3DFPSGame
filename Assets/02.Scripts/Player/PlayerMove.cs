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

    private Animator _animator;

    public float Stamina => _stamina;
    public float MMaxStamina => MaxStamina;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _stamina = MaxStamina;
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameState != GameState.Playing)
        {
            return;
        }
        _stamina = Mathf.Min(MaxStamina, _stamina + Time.deltaTime * _staminaRecovery);
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction;
        CameraViewSwitcher viewSwitcher = CameraViewSwitcher.Instance;
        bool isFixedView = viewSwitcher != null && viewSwitcher.IsFixedView == true;
        if (isFixedView == true)
        {
            // 고정 시점에서는 카메라 기준으로 이동 방향을 계산한다.
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 forward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
                Vector3 right = Vector3.ProjectOnPlane(mainCamera.transform.right, Vector3.up).normalized;
                direction = (right * h + forward * v).normalized;
            }
            else
            {
                direction = new Vector3(h, 0f, v).normalized;
            }
        }
        else
        {
            direction = new Vector3(h, 0f, v).normalized;
            direction = transform.TransformDirection(direction);
        }
        if (_animator != null)
        {
            _animator.SetFloat(AnimatorParams.MoveMotion, direction.magnitude);
        }
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

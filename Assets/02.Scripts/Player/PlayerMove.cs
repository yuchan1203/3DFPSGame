using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    private float _moveSpeed = 20f;

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
        _characterController.Move(direction * (_moveSpeed * Time.deltaTime));
    }
}
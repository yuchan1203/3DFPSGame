using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 2f; // 위로 상승하는 속도
    [SerializeField] private float _duration = 1f;  // 유지되는 시간 (N초)

    private float _elapsedTime = 0f;

    void Update()
    {
        _elapsedTime += Time.deltaTime;

        // 위로 이동
        transform.position += Vector3.up * (_moveSpeed * Time.deltaTime);

        // N초가 지나면 삭제
        if (_elapsedTime >= _duration)
        {
            Destroy(gameObject);
        }
    }
}
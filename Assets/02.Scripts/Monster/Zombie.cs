using System.Collections;
using UnityEngine;

// FSM(유한 상태 머신) : 유한개의 상태가 있고, 상태마다 행동을 다르게 함으로써 AI를 구현하는 유명한 방법
// - "상태": 객체가 가질 수 있는 어떤 조건이나 상황
// - "전이": 조건에 따라 현재 상태에서 다른 상태로 넘어가는 것
// - 그리고 위 방식에 따라 규칙을 세워둔 것을 "머신"

public class Zombie : MonoBehaviour, IDamageAble
{
    [Header("스탯")]
    [SerializeField] private float _detectDistance = 8; // 플레이어를 발견하는 거리 
    [SerializeField] private float _comebackDistance = 15; // 처음 자리에서 이만큼 
    [SerializeField] private float _attackRange = 1.5f; // 공격 범위
    [SerializeField] private float _attackInterval = 1.2f; // 공격 속도
    [SerializeField] private float _moveSpeed = 4; // 공격 간격
    [SerializeField] private float _maxHealth = 100f;
    private float _health = 30; // 체력

    [Header("이펙트")]
    [SerializeField] private GameObject _bloodSprayPrefab;

    private CharacterController _controller;
    private Vector3 _startPosition;
    private GameObject _player;
    private ZombieState _state = ZombieState.Idle;
    private float _attackTimer = 0f;

    public float Health => _health;
    public float MaxHealth => _maxHealth;

    private void Start()
    {
        _health = _maxHealth;
        _controller = GetComponent<CharacterController>();
        _startPosition = transform.position;
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        switch (_state)
        {
            case ZombieState.Idle:
                {
                    Idle();
                    break;
                }
            case ZombieState.Trace:
                {
                    Trace();
                    break;
                }
            case ZombieState.Comeback:
                {
                    Comeback();
                    break;
                }
            case ZombieState.Attack:
                {
                    Attack();
                    break;
                }
        }
    }

    // 대미지를 받는다
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_state == ZombieState.Death)
        {
            return;
        }

        _health -= damageInfo.Value;

        StopAllCoroutines();

        if (_health > 0f)
        {
            GameObject bloodSpray = Instantiate(_bloodSprayPrefab, transform);
            //bloodSpray.transform.SetParent(transform);
            bloodSpray.transform.position = damageInfo.HitPoint;
            bloodSpray.transform.forward = damageInfo.HitNormal;
            Debug.Log($"{_state} -> Hit");
            _state = ZombieState.Hit;
            StartCoroutine(Hit_Coroutine());
        }
        else
        {
            Debug.Log($"{_state} -> Dead");
            _state = ZombieState.Death;
            StartCoroutine(Death_Coroutine());
        }
    }

    private IEnumerator Hit_Coroutine()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log($"{_state} -> Trace");
        _state = ZombieState.Trace;
    }

    private IEnumerator Death_Coroutine()
    {
        _controller.enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    private void Idle()
    {
        if (_player == null || _controller == null) return;

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance <= _detectDistance)
        {
            Debug.Log($"{_state} -> Trace");
            _state = ZombieState.Trace;
        }
    }

    private void Trace()
    {
        if (_player == null || _controller == null) return;

        if (Vector3.Distance(transform.position, _startPosition) > _comebackDistance)
        {
            Debug.Log($"{_state} -> Comeback");
            _state = ZombieState.Comeback;
            return;
        }
        if (Vector3.Distance(transform.position, _player.transform.position) <= _attackRange)
        {
            _attackTimer = 0f;
            Debug.Log($"{_state} -> Attack");
            _state = ZombieState.Attack;
            return;
        }
        Vector3 direction = _player.transform.position - transform.position;
        direction.y = 0f;
        direction.Normalize();
        transform.forward = direction;
        _controller.SimpleMove(direction * _moveSpeed);
    }

    private void Comeback()
    {
        // 처음 자리에 도착하면 Idle
        if (Vector3.Distance(transform.position, _startPosition) < 1f)
        {
            Debug.Log($"{_state} -> Idle");
            _state = ZombieState.Idle;
            return;
        }
        // 처음 자리로 돌아간다
        Vector3 direction = _startPosition - transform.position;
        direction.y = 0f;
        direction.Normalize();

        transform.forward = direction;
        _controller.SimpleMove(direction * _moveSpeed);
    }

    private void Attack()
    {
        if (_player == null || _controller == null) return;
        _attackTimer += Time.deltaTime;
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > _detectDistance)
        {
            Debug.Log($"{_state} -> Trace");
            _state = ZombieState.Trace;
        }
        else
        {
            if (_attackTimer > _attackInterval)
            {
                Debug.Log("플레이어를 때렸습니다");
                _attackTimer = 0;
            }
        }
    }
}
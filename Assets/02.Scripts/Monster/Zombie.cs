using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, IDamageAble
{
    [Header("스탯")]
    [SerializeField] private float _detectDistance = 8;
    [SerializeField] private float _comebackDistance = 15;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackInterval = 1.2f;
    [SerializeField] private float _moveSpeed = 4;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _attackDamage = 10f;

    [Header("연결")]
    [SerializeField] private GameObject _bloodSprayPrefab;
    [SerializeField] private GameObject _damageTextPrefab;
    [SerializeField] private Transform _textSpawnPoint;

    private NavMeshAgent _agent;
    private float _health = 30;
    private Vector3 _startPosition;
    private GameObject _player;
    private ZombieState _state = ZombieState.Idle;
    private float _attackTimer = 0f;
    public float Health => _health;
    public float MaxHealth => _maxHealth;
    private void Start()
    {
        _health = _maxHealth;
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;
        _startPosition = transform.position;
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameState != GameState.Playing)
        {
            return;
        }
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
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_state == ZombieState.Death)
        {
            return;
        }
        _health -= damageInfo.Value;
        StopAllCoroutines();
        GameObject textObj = Instantiate(_damageTextPrefab, _textSpawnPoint.position, Quaternion.identity);
        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = damageInfo.Value.ToString();
        }
        if (_health > 0f)
        {
            GameObject bloodSpray = Instantiate(_bloodSprayPrefab, transform);
            bloodSpray.transform.position = damageInfo.HitPoint;
            bloodSpray.transform.forward = damageInfo.HitNormal;
            _state = ZombieState.Hit;
            StartCoroutine(Hit_Coroutine());
        }
        else
        {
            _state = ZombieState.Death;
            StartCoroutine(Death_Coroutine());
        }
    }
    private IEnumerator Hit_Coroutine()
    {
        yield return new WaitForSeconds(0.3f);
        _state = ZombieState.Trace;
    }
    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
    private void Idle()
    {
        if (_player == null) return;
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance <= _detectDistance)
        {
            _state = ZombieState.Trace;
        }
    }
    private void Trace()
    {
        if (_player == null) return;

        if (Vector3.Distance(transform.position, _startPosition) > _comebackDistance)
        {
            _state = ZombieState.Comeback;
            return;
        }
        if (Vector3.Distance(transform.position, _player.transform.position) <= _attackRange)
        {
            _attackTimer = 0f;
            _state = ZombieState.Attack;
            return;
        }
        _agent.SetDestination(_player.transform.position);
    }

    private void Comeback()
    {
        if (Vector3.Distance(transform.position, _startPosition) < 1f)
        {
            _state = ZombieState.Idle;
            return;
        }
        _agent.SetDestination(_startPosition);
    }

    private void Attack()
    {
        if (_player == null) return;
        _attackTimer += Time.deltaTime;
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > _detectDistance)
        {
            _state = ZombieState.Trace;
        }
        else if (_attackTimer > _attackInterval)
        {
            _attackTimer = 0;
            Collider[] colliders = Physics.OverlapSphere(transform.position, _attackRange, LayerMask.GetMask("Player"));
            foreach (var col in colliders)
            {
                DamageInfo damageInfo = new()
                {
                    Value = _attackDamage,
                    HitPoint = col.ClosestPoint(transform.position),
                    HitNormal = (transform.position - col.ClosestPoint(transform.position)).normalized,
                };
                col.GetComponent<IDamageAble>()?.TakeDamage(damageInfo);
            }
        }
    }
}
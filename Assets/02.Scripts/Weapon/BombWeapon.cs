using UnityEngine;

public class BombWeapon : MonoBehaviour, IWeapon
{
    public WeaponState State => WeaponState.Bomb;

    [Header("프리팹")]
    [Tooltip("구체 모양의 bomb 프리팹 (BombProjectile에 Explosion 이펙트를 지정)")]
    [SerializeField] private GameObject _bombPrefab;

    [Header("스탯")]
    [SerializeField] private int _maxCount = 5;
    [SerializeField] private float _throwForce = 12f;
    [SerializeField] private float _upForce = 4f;
    [SerializeField] private float _fireInterval = 0.8f;

    [Header("옵션 1: 차징")]
    [Tooltip("켜면 버튼을 누르는 동안 힘을 모으고, 뗄 때 던진다")]
    [SerializeField] private bool _useCharge = false;
    [SerializeField] private float _maxChargeTime = 1.5f;
    [SerializeField] private float _minForceMultiplier = 0.5f;
    [SerializeField] private float _maxForceMultiplier = 2f;

    [Header("옵션 2: 시간차 폭발")]
    [Tooltip("켜면 충돌해도 터지지 않고, 던진 뒤 일정 시간이 지나면 터진다")]
    [SerializeField] private bool _useFuse = false;
    [SerializeField] private float _fuseTime = 3f;

    private int _count;
    private bool _isCharging;
    private float _chargeTime;

    public int CurrentAmmo => _count;
    public int MaxAmmo => _maxCount;
    public bool CanReload => false;
    public float FireInterval => _fireInterval;

    public bool UseCharge => _useCharge;
    public bool IsCharging => _isCharging;
    public float ChargeRatio => _maxChargeTime > 0f ? Mathf.Clamp01(_chargeTime / _maxChargeTime) : 1f;

    private void Awake()
    {
        _count = _maxCount;
    }

    public void Shoot()
    {
        Throw(1f);
    }

    public void BeginCharge()
    {
        if (_isCharging || _count <= 0)
        {
            return;
        }
        _isCharging = true;
        _chargeTime = 0f;
    }

    public void UpdateCharge(float deltaTime)
    {
        if (_isCharging)
        {
            _chargeTime += deltaTime;
        }
    }

    // 던졌으면 true를 반환한다.
    public bool ReleaseCharge()
    {
        if (!_isCharging)
        {
            return false;
        }
        float multiplier = Mathf.Lerp(_minForceMultiplier, _maxForceMultiplier, ChargeRatio);
        CancelCharge();
        return Throw(multiplier);
    }

    public void CancelCharge()
    {
        _isCharging = false;
        _chargeTime = 0f;
    }

    private bool Throw(float forceMultiplier)
    {
        if (_count <= 0)
        {
            return false;
        }

        Camera camera = Camera.main;
        if (camera == null)
        {
            return false;
        }

        if (_bombPrefab == null)
        {
            GameLogger.LogWarning("BombWeapon: 폭탄 프리팹이 지정되지 않았습니다.");
            return false;
        }
        _count--;

        GameObject bombObject = Instantiate(
            _bombPrefab,
            camera.transform.position + camera.transform.forward * 1f,
            Quaternion.identity);

        if (!bombObject.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody = bombObject.AddComponent<Rigidbody>();
        }
        rigidbody.linearVelocity = (camera.transform.forward * _throwForce + Vector3.up * _upForce) * forceMultiplier;

        if (!bombObject.TryGetComponent(out BombProjectile projectile))
        {
            projectile = bombObject.AddComponent<BombProjectile>();
        }
        projectile.Setup(_useFuse, _fuseTime);
        return true;
    }

    public void Reload()
    {
        // 폭탄은 재장전할 수 없습니다.
    }
}

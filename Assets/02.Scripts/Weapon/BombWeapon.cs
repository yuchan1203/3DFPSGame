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

    private int _count;

    public int CurrentAmmo => _count;
    public int MaxAmmo => _maxCount;
    public bool CanReload => false;
    public float FireInterval => _fireInterval;

    private void Awake()
    {
        _count = _maxCount;
    }

    public void Shoot()
    {
        if (_count <= 0)
        {
            return;
        }
        _count--;

        Camera camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        if (_bombPrefab == null)
        {
            GameLogger.LogWarning("BombWeapon: 폭탄 프리팹이 지정되지 않았습니다.");
            return;
        }

        GameObject bombObject = Instantiate(
            _bombPrefab,
            camera.transform.position + camera.transform.forward * 1f,
            Quaternion.identity);

        if (!bombObject.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody = bombObject.AddComponent<Rigidbody>();
        }
        rigidbody.linearVelocity = camera.transform.forward * _throwForce + Vector3.up * _upForce;

        if (!bombObject.TryGetComponent(out BombProjectile _))
        {
            bombObject.AddComponent<BombProjectile>();
        }
    }

    public void Reload()
    {
        // 폭탄은 재장전할 수 없습니다.
    }
}

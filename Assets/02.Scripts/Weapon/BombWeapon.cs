using UnityEngine;

public class BombWeapon : MonoBehaviour, IWeapon
{
    public WeaponState State => WeaponState.Bomb;

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

        GameObject bombObject = new GameObject("Bomb");
        bombObject.transform.position = camera.transform.position + camera.transform.forward * 1f;
        bombObject.transform.rotation = Quaternion.identity;

        SphereCollider collider = bombObject.AddComponent<SphereCollider>();
        collider.radius = 0.2f;

        Rigidbody rigidbody = bombObject.AddComponent<Rigidbody>();
        rigidbody.linearVelocity = camera.transform.forward * _throwForce + Vector3.up * _upForce;

        bombObject.AddComponent<BombProjectile>();
    }

    public void Reload()
    {
        // 폭탄은 재장전할 수 없습니다.
    }
}

using UnityEngine;

public class ShotgunWeapon : MonoBehaviour, IWeapon
{
    public WeaponState State => WeaponState.Shotgun;

    [Header("이펙트")]
    [Tooltip("탄도 5개 각각의 도착 지점에 재생할 이펙트 (왼쪽 → 오른쪽 순서)")]
    [SerializeField] private ParticleSystem[] _hitEffectPrefabs = new ParticleSystem[5];

    [Header("스탯")]
    [SerializeField] private int _maxAmmo = 5;
    [SerializeField] private float _damage = 5f;
    [SerializeField] private float _range = 100f;
    [SerializeField] private float _spreadAngle = 2f;
    [SerializeField] private float _fireInterval = 0.5f;

    private int _ammo;

    public int CurrentAmmo => _ammo;
    public int MaxAmmo => _maxAmmo;
    public bool CanReload => true;
    public float FireInterval => _fireInterval;

    private void Awake()
    {
        _ammo = _maxAmmo;
    }

    public void Shoot()
    {
        if (_ammo <= 0)
        {
            return;
        }
        _ammo--;

        CameraViewSwitcher viewSwitcher = CameraViewSwitcher.Instance;
        if (viewSwitcher != null)
        {
            viewSwitcher.ApplyRecoil(1f);
        }

        Camera camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        for (int i = -2; i <= 2; i++)
        {
            float yawOffset = i * _spreadAngle;
            Vector3 direction = Quaternion.Euler(0f, yawOffset, 0f) * camera.transform.forward;
            Ray ray = new Ray(camera.transform.position, direction);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, _range))
            {
                PlayHitEffect(i + 2, hitInfo);
                hitInfo.collider.GetComponent<IDamageAble>()?.TakeDamage(new DamageInfo
                {
                    Value = _damage,
                    HitPoint = hitInfo.point,
                    HitNormal = hitInfo.normal
                });
            }
        }
    }

    private void PlayHitEffect(int index, RaycastHit hitInfo)
    {
        if (index < 0 || index >= _hitEffectPrefabs.Length || _hitEffectPrefabs[index] == null)
        {
            return;
        }
        ParticleSystem effect = Instantiate(_hitEffectPrefabs[index]);
        effect.transform.position = hitInfo.point;
        effect.transform.forward = hitInfo.normal;
        effect.Play();
    }

    public void Reload()
    {
        _ammo = _maxAmmo;
    }
}

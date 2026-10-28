using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class GunWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private ParticleSystem _hitEffectPrefab;
    [SerializeField] private TextMeshProUGUI _reloadText;
    [SerializeField] private RectTransform _crossHair;

    [Header("스탯")]
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _fireInterval = 0.1f;
    [SerializeField] private int _maxBulletCount = 30;
    [SerializeField] private float _reloadTime = 2f;
    [SerializeField] private float _rotationSpeed = 360f;

    [Header("거리 비례 대미지 감소")]
    [SerializeField] private bool _useDistanceFalloff = false;
    [Tooltip("사거리 끝에서 적용되는 대미지 비율 (0~1)")]
    [SerializeField, Range(0f, 1f)] private float _minDamageRatio = 0.3f;

    private Animator _animator;

    private int _bulletCount = 30;
    private bool _isReloading = false;
    private float _range = 100f;

    public int MaxBulletCount => _maxBulletCount;
    public int BulletCount => _bulletCount;

    public WeaponState State => WeaponState.Gun;
    public int CurrentAmmo => _bulletCount;
    public int MaxAmmo => _maxBulletCount;
    public bool CanReload => true;
    public float FireInterval => _fireInterval;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnDisable()
    {
        // 비활성화되면 코루틴이 멈추므로 재장전 상태가 남지 않게 초기화한다.
        _isReloading = false;
    }

    private void Start()
    {
        if (_reloadText != null)
        {
            _reloadText.gameObject.SetActive(false);
        }
        SetZRotation(0);
    }

    public void Reload()
    {
        // 이미 재장전 중이면 코루틴을 중복 실행하지 않는다.
        if (_isReloading == true)
        {
            return;
        }
        StartCoroutine(Reload_Coroutine());
    }

    public void SetZRotation(float targetAngle)
    {
        if (_crossHair == null)
        {
            return;
        }
        Vector3 rotation = _crossHair.localEulerAngles;
        rotation.z = targetAngle;
        _crossHair.localEulerAngles = rotation;
    }

    public void AddZRotation(float angleAmount)
    {
        if (_crossHair == null)
        {
            return;
        }
        _crossHair.Rotate(0, 0, angleAmount);
    }

    private IEnumerator Reload_Coroutine()
    {
        _isReloading = true;
        if (_reloadText != null)
        {
            _reloadText.gameObject.SetActive(true);
        }
        float elapsedTime = 0f;
        while (elapsedTime < _reloadTime)
        {
            elapsedTime += Time.deltaTime;
            if (_crossHair != null)
            {
                float currentAngle = _crossHair.localEulerAngles.z - (_rotationSpeed * Time.deltaTime);
                _crossHair.localEulerAngles = new Vector3(0, 0, currentAngle);
            }
            yield return null;
        }
        _bulletCount = _maxBulletCount;
        if (_crossHair != null)
        {
            _crossHair.localEulerAngles = Vector3.zero;
        }
        if (_reloadText != null)
        {
            _reloadText.gameObject.SetActive(false);
        }
        _isReloading = false;
    }

    public void Shoot()
    {
        if (_isReloading == true || _bulletCount <= 0)
        {
            return;
        }
        _bulletCount--;
        if (_animator != null && _animator.GetFloat(AnimatorParams.MoveMotion) == 0f)
        {
            _animator.SetTrigger(AnimatorParams.Attack);
        }

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
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, _range))
        {
            if (_hitEffectPrefab != null)
            {
                ParticleSystem effect = Instantiate(_hitEffectPrefab);
                effect.transform.position = hitInfo.point;
                effect.transform.forward = hitInfo.normal;
                effect.Play();
            }
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.Value = _useDistanceFalloff
                ? DamageFalloff.Calculate(_damage, hitInfo.distance, _range, _minDamageRatio)
                : _damage;
            damageInfo.HitPoint = hitInfo.point;
            damageInfo.HitNormal = hitInfo.normal;
            hitInfo.collider.GetComponent<IDamageAble>()?.TakeDamage(damageInfo);
        }
    }
}

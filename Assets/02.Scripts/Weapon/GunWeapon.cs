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
    [SerializeField] private int _maxBulletCount = 30;
    [SerializeField] private float _reloadTime = 2f;
    [SerializeField] private float _rotationSpeed = 360f;

    private int _bulletCount = 30;
    private float _range = 100f;

    public int MaxBulletCount => _maxBulletCount;
    public int BulletCount => _bulletCount;

    public WeaponState State => WeaponState.Gun;
    public int CurrentAmmo => _bulletCount;
    public int MaxAmmo => _maxBulletCount;
    public bool CanReload => true;

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
    }

    public void Shoot()
    {
        if (_bulletCount <= 0)
        {
            return;
        }
        _bulletCount--;

        CameraViewSwitcher viewSwitcher = FindAnyObjectByType<CameraViewSwitcher>();
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
            damageInfo.Value = _damage;
            damageInfo.HitPoint = hitInfo.point;
            damageInfo.HitNormal = hitInfo.normal;
            hitInfo.collider.GetComponent<IDamageAble>()?.TakeDamage(damageInfo);
        }
    }
}

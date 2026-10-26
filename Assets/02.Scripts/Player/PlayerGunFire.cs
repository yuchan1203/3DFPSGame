using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitEffectPrefab;
    [SerializeField] private TextMeshProUGUI _reloadText;
    [SerializeField] private RectTransform _crossHair;

    [Header("스탯")]
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _fireInterval = 0.1f;
    [SerializeField] private int _maxBulletCount = 100;
    [SerializeField] private float _reloadTime = 2f;
    [SerializeField] private float _rotationSpeed = 360f;

    public int MaxBulletCount => _maxBulletCount;
    public int BulletCount => _bulletCount;

    private int _bulletCount = 10;
    private float _range = 100f;
    private float _fireTimer = 1f;

    private void Start()
    {
        _reloadText.gameObject.SetActive(false);
        SetZRotation(0);
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameState != GameState.Playing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && _maxBulletCount != _bulletCount)
        {
            StartCoroutine(Reload_Coroutine());
        }
        _fireTimer -= Time.deltaTime;
        // 2. 버튼을 누르고 있고, 마우스가 UI 위에 있지 않고, 쏠 때가 되었고, 총알이 남아 있으면 쏜다.
        bool isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        if (Input.GetButton("Fire1") && !isPointerOverUI && _fireTimer <= 0f && _bulletCount > 0)
        {
            _fireTimer = _fireInterval;
            _bulletCount--;
            Shoot();
        }
    }
    public void SetZRotation(float targetAngle)
    {
        Vector3 rotation = _crossHair.localEulerAngles;
        rotation.z = targetAngle;
        _crossHair.localEulerAngles = rotation;
    }

    // 현재 Z축 각도에서 추가로 회전
    public void AddZRotation(float angleAmount)
    {
        _crossHair.Rotate(0, 0, angleAmount);
    }
    private IEnumerator Reload_Coroutine()
    {
        _reloadText.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < _reloadTime)
        {
            elapsedTime += Time.deltaTime;
            float currentAngle = _crossHair.localEulerAngles.z - (_rotationSpeed * Time.deltaTime);
            _crossHair.localEulerAngles = new Vector3(0, 0, currentAngle);
            yield return null;
        }
        //yield return new WaitForSeconds(_reloadTime);
        _bulletCount = _maxBulletCount;
        _crossHair.localEulerAngles = Vector3.zero;
        _reloadText.gameObject.SetActive(false);
    }
    private void Shoot()
    {
        Transform cameraTransform = Camera.main.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hitInfo = new RaycastHit();
        bool isHit = Physics.Raycast(ray, out hitInfo, _range);
        if (isHit)
        {
            ParticleSystem effect = Instantiate(_hitEffectPrefab);
            effect.transform.position = hitInfo.point;
            effect.transform.forward = hitInfo.normal;
            effect.Play();
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.Value = _damage;
            damageInfo.HitPoint = hitInfo.point;
            damageInfo.HitNormal = hitInfo.normal;
            hitInfo.collider.GetComponent<IDamageAble>()?.TakeDamage(damageInfo);
        }
    }
}
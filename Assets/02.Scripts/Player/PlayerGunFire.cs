using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitEffectPrefab;

    [Header("스탯")]
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _fireInterval = 0.1f;
    [SerializeField] private int _maxBulletCount = 100;
    [SerializeField] private float _reloadTime = 2f;

    public int MaxBulletCount => _maxBulletCount;
    public int BulletCount => _bulletCount;

    private int _bulletCount = 10;
    private float _range = 100f;
    private float _fireTimer = 1f;

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
    private IEnumerator Reload_Coroutine()
    {
        yield return new WaitForSeconds(_reloadTime);
        _bulletCount = _maxBulletCount;
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
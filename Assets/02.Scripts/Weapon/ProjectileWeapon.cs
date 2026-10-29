using UnityEngine;

// 투사체를 던지는 무기. 폭발 이펙트와 범위는 투사체 프리팹이, 대미지는 WeaponData가 정한다.
public class ProjectileWeapon : WeaponBase, IChargeable
{
    private bool _isCharging;
    private float _chargeTime;
    private float _forceMultiplier = 1f;

    public bool UseCharge => _data.UseCharge;
    public bool IsCharging => _isCharging;
    public float ChargeRatio => _data.MaxChargeTime > 0f ? Mathf.Clamp01(_chargeTime / _data.MaxChargeTime) : 1f;

    public ProjectileWeapon(WeaponData data) : base(data)
    {
    }

    public void BeginCharge()
    {
        if (_isCharging || IsReloading || CurrentAmmo <= 0)
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

    public bool ReleaseCharge()
    {
        if (_isCharging == false)
        {
            return false;
        }
        _forceMultiplier = Mathf.Lerp(_data.MinForceMultiplier, _data.MaxForceMultiplier, ChargeRatio);
        CancelCharge();
        bool thrown = Shoot();
        _forceMultiplier = 1f;
        return thrown;
    }

    public void CancelCharge()
    {
        _isCharging = false;
        _chargeTime = 0f;
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        CancelCharge();
    }

    protected override bool Fire()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            return false;
        }

        if (_data.ProjectilePrefab == null)
        {
            GameLogger.LogWarning($"{_data.Type}: 투사체 프리팹이 지정되지 않았습니다.");
            return false;
        }

        GameObject projectileObject = Object.Instantiate(
            _data.ProjectilePrefab,
            camera.transform.position + camera.transform.forward * 1f,
            Quaternion.identity);

        if (projectileObject.TryGetComponent(out Rigidbody rigidbody) == false)
        {
            rigidbody = projectileObject.AddComponent<Rigidbody>();
        }
        rigidbody.linearVelocity = (camera.transform.forward * _data.ThrowForce + Vector3.up * _data.UpForce) * _forceMultiplier;

        if (projectileObject.TryGetComponent(out BombProjectile projectile) == false)
        {
            projectile = projectileObject.AddComponent<BombProjectile>();
        }
        projectile.Setup(_data);
        return true;
    }
}

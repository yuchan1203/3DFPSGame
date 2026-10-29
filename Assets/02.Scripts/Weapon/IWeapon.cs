public interface IWeapon
{
    WeaponData Data { get; }
    int CurrentAmmo { get; }
    int MaxAmmo { get; }
    // false면 탄약 없이 계속 공격할 수 있다. (근접 무기)
    bool UsesAmmo { get; }
    bool CanReload { get; }
    bool IsReloading { get; }
    float ReloadRatio { get; }
    float FireInterval { get; }

    // 실제로 발사했으면 true를 반환한다.
    bool Shoot();
    void Reload();
    void Tick(float deltaTime);
    void OnUnequip();
}

public interface IWeapon
{
    WeaponData Data { get; }
    int CurrentAmmo { get; }
    int MaxAmmo { get; }
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

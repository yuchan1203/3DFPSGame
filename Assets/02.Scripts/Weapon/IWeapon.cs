public interface IWeapon
{
    WeaponState State { get; }
    int CurrentAmmo { get; }
    int MaxAmmo { get; }
    bool CanReload { get; }
    float FireInterval { get; }
    void Shoot();
    void Reload();
}
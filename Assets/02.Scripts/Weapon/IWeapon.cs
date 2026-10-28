public interface IWeapon
{
    WeaponState State { get; }
    int CurrentAmmo { get; }
    int MaxAmmo { get; }
    bool CanReload { get; }
    void Shoot();
    void Reload();
}
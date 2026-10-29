// 무기의 동작 방식. 같은 유형의 무기는 같은 클래스가 WeaponData 값만 바꿔서 처리한다.
public enum WeaponCategory
{
    Gun,        // 레이캐스트 즉시 판정 (HitscanWeapon)
    Projectile, // 투사체 발사 (ProjectileWeapon)
}

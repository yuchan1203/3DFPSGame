// 버튼을 누르는 동안 힘을 모았다가 떼면 발사하는 무기.
public interface IChargeable
{
    bool UseCharge { get; }
    bool IsCharging { get; }
    float ChargeRatio { get; }
    void BeginCharge();
    void UpdateCharge(float deltaTime);
    // 발사했으면 true를 반환한다.
    bool ReleaseCharge();
    void CancelCharge();
}

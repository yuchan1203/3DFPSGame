// 좀비가 가질 수 있는 상태

public enum ZombieState
{
    Idle, // 대기: 제자리에서 플레이어를 기다린다
    Trace, // 추격: 플레이어를 쫒아간다
    Comeback, // 복귀: 처음 자리로 돌아간다
    Attack, // 공격: 일정 시간마다 플레이어를 때린다
    Hit, // 피격: 공격을 받아 잠깐 멈칫한다
    Death, // 죽음: 쓰러지고 잠시 뒤 사라진다 
}
using UnityEngine;

// FSM(유한 상태 머신) : 유한개의 상태가 있고, 상태마다 행동을 다르게 함으로써 AI를 구현하는 유명한 방법
// - "상태": 객체가 가질 수 있는 어떤 조건이나 상황
// - "전이": 조건에 따라 현재 상태에서 다른 상태로 넘어가는 것
// - 그리고 위 방식에 따라 규칙을 세워둔 것을 "머신"

// 좀비가 가질 수 있는 상태
public enum ZombieState
{
    Idle, // 대기: 제자리에서 플레이어를 기다린다
    Trace, // 추격: 플레이어를 쫒아간다
    Comback, // 복귀: 처음 자리로 돌아간다
    Attack, // 공격: 일정 시간마다 플레이어를 때린다
    Hit, // 피격: 공격을 받아 잠깐 멈칫한다
    Death, // 죽음: 쓰러지고 잠시 뒤 사라진다 
}
public class Zombie : MonoBehaviour
{
    private void Start()
    {

    }

    private void Update()
    {

    }
}
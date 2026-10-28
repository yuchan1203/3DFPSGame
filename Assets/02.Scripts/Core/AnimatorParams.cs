using UnityEngine;

// 애니메이터 파라미터 해시를 한곳에서 관리한다.
public static class AnimatorParams
{
    public static readonly int MoveMotion = Animator.StringToHash("MoveMotion");
    public static readonly int Attack = Animator.StringToHash("Attack");
}

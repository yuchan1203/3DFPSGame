using UnityEngine;

// 애니메이터 파라미터 해시를 한곳에서 관리한다.
public static class AnimatorParams
{
    public static readonly int MoveMotion = Animator.StringToHash("MoveMotion");
    public static readonly int Attack = Animator.StringToHash("Attack");
    public static readonly int Hit = Animator.StringToHash("Hit");
    public static readonly int Death = Animator.StringToHash("Death");
    public static readonly int Melee = Animator.StringToHash("Melee");
}

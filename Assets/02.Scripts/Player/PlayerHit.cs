using UnityEngine;
public class PlayerHit : MonoBehaviour, IDamageAble
{
    [SerializeField] private float _maxHealth = 100f;
    private float _health;
    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _health;
    private void Awake()
    {
        _health = _maxHealth;
    }
    public void TakeDamage(DamageInfo damageInfo)
    {
        _health -= damageInfo.Value;
        if (_health <= 0f)
        {
            //Debug.Log("Player 사망");
            Destroy(gameObject);
        }
        else
        {
            //Debug.Log($"플레이어 체력: {_health}");
        }
    }
}
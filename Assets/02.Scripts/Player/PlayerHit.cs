using System;
using UnityEngine;
public class PlayerHit : MonoBehaviour, IDamageAble
{
    [SerializeField] private float _maxHealth = 100f;
    private float _health;
    public event Action HealthChanged;
    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _health;
    private void Awake()
    {
        _health = _maxHealth;
    }
    public void TakeDamage(DamageInfo damageInfo)
    {
        _health -= damageInfo.Value;
        // 사망으로 오브젝트가 파괴되기 전에 알려야 체력바가 0까지 갱신된다.
        HealthChanged?.Invoke();
        if (_health <= 0f)
        {
            //Debug.Log("Player 사망");
            Destroy(gameObject);
            GameManager.Instance.GameOver();
        }
        else
        {
            //Debug.Log($"플레이어 체력: {_health}");
        }
    }
}
using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffectPrefab;
    [SerializeField] private float _explosionRange = 4.5f;
    [SerializeField] private float _damage = 100f;

    [Header("거리 비례 대미지 감소")]
    [Tooltip("켜면 폭발 중심에서 멀수록 대미지가 줄어든다")]
    [SerializeField] private bool _useDistanceFalloff = false;
    [Tooltip("폭발 범위 끝에서 적용되는 대미지 비율 (0~1)")]
    [SerializeField, Range(0f, 1f)] private float _minDamageRatio = 0.3f;

    private bool _useFuse;
    private bool _exploded;

    public void Setup(bool useFuse, float fuseTime)
    {
        _useFuse = useFuse;
        if (_useFuse)
        {
            Invoke(nameof(Explode), fuseTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_useFuse)
        {
            return;
        }
        Explode();
    }

    private void Explode()
    {
        if (_exploded)
        {
            return;
        }
        _exploded = true;

        if (_explosionEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
            effect.Play();
        }
        ApplyAreaDamage();
        Destroy(gameObject);
    }

    private void ApplyAreaDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRange, LayerMask.GetMask("Drum", "Zombie"));
        foreach (var col in colliders)
        {
            Vector3 closestPoint = col.ClosestPoint(transform.position);
            float damage = _damage;
            if (_useDistanceFalloff)
            {
                float distance = Vector3.Distance(transform.position, closestPoint);
                damage = DamageFalloff.Calculate(_damage, distance, _explosionRange, _minDamageRatio);
            }
            DamageInfo damageInfo = new DamageInfo
            {
                Value = damage,
                HitPoint = closestPoint,
                HitNormal = (transform.position - closestPoint).normalized,
            };
            col.GetComponent<IDamageAble>()?.TakeDamage(damageInfo);
        }
    }
}

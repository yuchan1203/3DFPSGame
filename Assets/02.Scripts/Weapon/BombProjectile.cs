using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffectPrefab;
    [SerializeField] private float _explosionRange = 4.5f;
    [SerializeField] private float _damage = 100f;

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
            DamageInfo damageInfo = new DamageInfo
            {
                Value = _damage,
                HitPoint = col.ClosestPoint(transform.position),
                HitNormal = (transform.position - col.ClosestPoint(transform.position)).normalized,
            };
            col.GetComponent<IDamageAble>()?.TakeDamage(damageInfo);
        }
    }
}

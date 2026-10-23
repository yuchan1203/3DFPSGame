using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
public class Drum : MonoBehaviour, IDamageAble
{
    [SerializeField] private ParticleSystem _explosionEffectPrefab;
    [Header("스탯")]
    [SerializeField] private float _health = 30f;
    [SerializeField] private float _launchPower = 20f;
    [SerializeField] private float _spinPower = 5f;
    [SerializeField] private float _destroyDelay = 3f;
    [SerializeField] private float _explosionRange = 4.5f;
    [SerializeField] private float _damage = 100f;
    [SerializeField] private float _explosionDelay = 0.2f;
    private Rigidbody _rigidbody;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_health <= 0)
        {
            return;
        }
        _health -= damageInfo.Value;
        if (_health <= 0)
        {
            StartCoroutine(Explode_Coroutine());
        }
    }
    private IEnumerator Explode_Coroutine()
    {
        yield return new WaitForSeconds(_explosionDelay);
        ParticleSystem effect = Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
        effect.Play();
        _rigidbody.AddForce(Vector3.up * _launchPower, ForceMode.Impulse);
        _rigidbody.AddTorque(UnityEngine.Random.insideUnitSphere * _spinPower, ForceMode.Impulse);
        ApplyAreaDamage();
        yield return new WaitForSeconds(_destroyDelay);
        Destroy(gameObject);
    }
    private void ApplyAreaDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRange, LayerMask.GetMask("Drum", "Zombie"));
        foreach (var col in colliders)
        {
            DamageInfo damageInfo = new()
            {
                Value = _damage,
                HitPoint = col.ClosestPoint(transform.position),
                HitNormal = (transform.position - col.ClosestPoint(transform.position)).normalized,
            };
            col.GetComponent<IDamageAble>()?.TakeDamage(damageInfo);
        }
    }
}
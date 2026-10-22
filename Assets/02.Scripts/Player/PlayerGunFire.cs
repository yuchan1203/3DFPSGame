using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hiteffectPrefab;
    private float _range = 100f;
    private float _damage = 10f;
    private float _fireInterval = 0.1f;
    private float _fireTimer = 0f;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Transform cameraTransform = Camera.main.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hitInfo = new RaycastHit();
        bool isHit = Physics.Raycast(ray, out hitInfo, _range);
        if (isHit)
        {
            // 맞은 자리에 총알 자국 이펙트 만들기 
            ParticleSystem effect = Instantiate(_hiteffectPrefab);
            effect.transform.position = hitInfo.point;
            effect.transform.forward = hitInfo.normal;
            effect.Play();

            // 맞은 대상이 좀비라면 TakeDamage
            if (hitInfo.collider.TryGetComponent(out Zombie zombie))
            {
                zombie.TakeDamage(_damage);
            }
            else if (hitInfo.collider.TryGetComponent(out Drum drum))
            {
                drum.TakeDamage(_damage);
            }
        }
    }
}
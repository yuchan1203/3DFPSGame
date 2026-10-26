using TMPro;
using UnityEngine;

public class BulletText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _bulletText;
    private PlayerGunFire _playerGunFire;

    private void Start()
    {
        _playerGunFire = GetComponent<PlayerGunFire>();
    }

    private void Update()
    {
        _bulletText.text = $"{_playerGunFire.BulletCount} / {_playerGunFire.MaxBulletCount}";
    }
}
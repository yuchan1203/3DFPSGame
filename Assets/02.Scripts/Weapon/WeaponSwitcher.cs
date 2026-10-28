using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private GunWeapon _gun;
    [SerializeField] private ShotgunWeapon _shotgun;
    [SerializeField] private BombWeapon _bomb;

    [SerializeField] private float _fireInterval = 0.1f;
    [SerializeField] private TextMeshProUGUI _weaponStateText;
    [SerializeField] private TextMeshProUGUI _bulletText;

    private IWeapon[] _weapons;
    private int _currentIndex = 0;
    private float _fireTimer = 0f;

    private IWeapon CurrentWeapon => _weapons[_currentIndex];

    public int CurrentAmmo => CurrentWeapon.CurrentAmmo;
    public int CurrentMaxAmmo => CurrentWeapon.MaxAmmo;

    private void Awake()
    {
        if (_gun == null)
        {
            _gun = GetComponent<GunWeapon>();
            if (_gun == null)
            {
                _gun = gameObject.AddComponent<GunWeapon>();
            }
        }
        if (_shotgun == null)
        {
            _shotgun = GetComponent<ShotgunWeapon>();
            if (_shotgun == null)
            {
                _shotgun = gameObject.AddComponent<ShotgunWeapon>();
            }
        }
        if (_bomb == null)
        {
            _bomb = GetComponent<BombWeapon>();
            if (_bomb == null)
            {
                _bomb = gameObject.AddComponent<BombWeapon>();
            }
        }

        _weapons = new IWeapon[] { _gun, _shotgun, _bomb };
        UpdateWeaponStateText();
        UpdateBulletText();
    }

    public void Reload()
    {
        CurrentWeapon.Reload();
    }

    public void ChangeWeapon()
    {
        _currentIndex = (_currentIndex + 1) % _weapons.Length;
        UpdateWeaponStateText();
        UpdateBulletText();
    }

    private void UpdateWeaponStateText()
    {
        if (_weaponStateText == null)
        {
            return;
        }
        _weaponStateText.text = "현재 무기: " + CurrentWeapon.State.ToString();
    }

    private void UpdateBulletText()
    {
        if (_bulletText == null)
        {
            return;
        }
        _bulletText.text = "Bullet: " + CurrentWeapon.CurrentAmmo + " / " + CurrentWeapon.MaxAmmo;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameState != GameState.Playing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeWeapon();
        }

        if (Input.GetKeyDown(KeyCode.R) && CurrentWeapon.CanReload)
        {
            Reload();
            UpdateBulletText();
        }

        _fireTimer -= Time.deltaTime;
        bool isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (Input.GetButton("Fire1") && !isPointerOverUI && _fireTimer <= 0f)
        {
            _fireTimer = _fireInterval;
            CurrentWeapon.Shoot();
            UpdateBulletText();
        }
    }
}

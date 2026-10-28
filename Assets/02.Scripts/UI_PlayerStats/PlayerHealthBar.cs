using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Slider _healthSlider;
    private PlayerMove _playerMove;
    private PlayerHit _playerHit;
    private void Awake()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerHit = GetComponent<PlayerHit>();
    }
    private void OnEnable()
    {
        _playerMove.StaminaChanged += RefreshStamina;
        _playerHit.HealthChanged += RefreshHealth;
    }
    private void OnDisable()
    {
        _playerMove.StaminaChanged -= RefreshStamina;
        _playerHit.HealthChanged -= RefreshHealth;
    }
    private void Start()
    {
        RefreshStamina();
        RefreshHealth();
    }
    private void RefreshStamina()
    {
        _staminaSlider.value = _playerMove.Stamina / _playerMove.MMaxStamina;
    }
    private void RefreshHealth()
    {
        _healthSlider.value = _playerHit.CurrentHealth / _playerHit.MaxHealth;
    }
}

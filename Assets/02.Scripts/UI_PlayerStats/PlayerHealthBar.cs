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
    private void LateUpdate()
    {
        _staminaSlider.value = _playerMove.Stamina / _playerMove.MMaxStamina;
        _healthSlider.value = _playerHit.CurrentHealth / _playerHit.MaxHealth;
    }
}
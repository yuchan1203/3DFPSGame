using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Zombie))]
public class ZombieHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private Zombie _zombie;
    private void Awake()
    {
        _zombie = GetComponent<Zombie>();
        if (_slider == null)
        {
            _slider = GetComponentInChildren<Slider>();
        }
    }
    private void LateUpdate()
    {
        if (_zombie == null || _slider == null || Camera.main == null) return;
        _slider.value = _zombie.Health / _zombie.MaxHealth;
        _slider.transform.forward = Camera.main.transform.forward;
    }
}
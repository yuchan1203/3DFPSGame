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
    private void OnEnable()
    {
        _zombie.HealthChanged += Refresh;
    }
    private void OnDisable()
    {
        _zombie.HealthChanged -= Refresh;
    }
    private void Start()
    {
        Refresh();
    }
    private void Refresh()
    {
        if (_slider == null)
        {
            return;
        }
        _slider.value = _zombie.Health / _zombie.MaxHealth;
    }
    // 체력바가 항상 카메라를 보도록 회전은 매 프레임 갱신한다.
    private void LateUpdate()
    {
        if (_slider == null || Camera.main == null) return;
        _slider.transform.forward = Camera.main.transform.forward;
    }
}

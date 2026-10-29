using UnityEngine;
using UnityEngine.UI;

// 플레이어가 데미지를 입으면 화면 전체를 잠깐 빨갛게 덮었다가 서서히 걷어낸다.
public class DamageScreenFlash : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private PlayerHit _playerHit;
    [Tooltip("화면 전체를 덮는 빨간 이미지. 레이캐스트는 꺼 둔다")]
    [SerializeField] private Image _flashImage;

    [Header("연출")]
    [Tooltip("맞은 순간의 불투명도 (0~1)")]
    [SerializeField, Range(0f, 1f)] private float _maxAlpha = 0.4f;
    [Tooltip("빨간 화면이 사라지는 데 걸리는 시간(초)")]
    [SerializeField] private float _fadeDuration = 0.3f;

    private float _lastHealth;
    private float _alpha = 0f;

    private void Start()
    {
        _lastHealth = _playerHit.CurrentHealth;
        SetAlpha(0f);
    }

    private void OnEnable()
    {
        _playerHit.HealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        _playerHit.HealthChanged -= OnHealthChanged;
    }

    // 체력이 줄었을 때만 깜빡인다.
    private void OnHealthChanged()
    {
        float health = _playerHit.CurrentHealth;
        if (health < _lastHealth)
        {
            SetAlpha(_maxAlpha);
        }
        _lastHealth = health;
    }

    private void Update()
    {
        if (_alpha <= 0f)
        {
            return;
        }
        float fadeSpeed = _fadeDuration > 0f ? _maxAlpha / _fadeDuration : float.MaxValue;
        // 일시정지(timeScale 0) 중에도 빨간 화면이 남지 않도록 실제 시간으로 걷어낸다.
        SetAlpha(Mathf.Max(0f, _alpha - fadeSpeed * Time.unscaledDeltaTime));
    }

    private void SetAlpha(float alpha)
    {
        _alpha = alpha;
        Color color = _flashImage.color;
        color.a = alpha;
        _flashImage.color = color;
    }
}

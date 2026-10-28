using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CameraViewSettings
{
    public Vector3 positionOffset;
    public Vector3 rotationEuler;
    public bool inheritPlayerYaw;
}

public class CameraViewSwitcher : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _viewStateText;
    [SerializeField] private Transform _target;
    [SerializeField] private CameraViewSettings[] _views;
    [SerializeField] private CameraState _currentMode = CameraState.FirstPerson;
    [SerializeField] private float _rotationSpeed = 200f;

    [Header("Recoil")]
    [SerializeField] private bool _useRecoil = true;
    [SerializeField] private float _recoilPitchAmount = 2f;
    [SerializeField] private float _recoilYawAmount = 1f;
    [SerializeField] private float _recoilRecoverySpeed = 8f;

    [Header("Fade")]
    [SerializeField] private bool _useFade = false;
    [SerializeField] private float _fadeDuration = 0.5f;

    private float _accumulationX = 0f;
    private float _accumulationY = 0f;

    // 시점별로 누적값을 따로 보관합니다.
    private float[] _accumulationXPerView;
    private float[] _accumulationYPerView;
    private bool[] _accumulationInitialized;

    private bool _isTransitioning = false;

    private float _recoilPitch = 0f;
    private float _recoilYaw = 0f;

    private Canvas _fadeCanvas;
    private CanvasGroup _fadeCanvasGroup;
    private Image _fadeImage;

    private void Awake()
    {
        if (_target == null)
        {
            Debug.LogError("No target assigned!");
        }

        if (_useFade)
        {
            CreateFadeCanvas();
        }
    }

    private void Start()
    {
        int viewCount = _views.Length;
        _accumulationXPerView = new float[viewCount];
        _accumulationYPerView = new float[viewCount];
        _accumulationInitialized = new bool[viewCount];

        for (int i = 0; i < viewCount; i++)
        {
            if (_views[i].inheritPlayerYaw == true)
            {
                _accumulationXPerView[i] = _views[i].rotationEuler.y;
                _accumulationYPerView[i] = _views[i].rotationEuler.x;
                _accumulationInitialized[i] = true;
            }
        }

        if (_accumulationInitialized[(int)_currentMode] == true)
        {
            _accumulationX = _accumulationXPerView[(int)_currentMode];
            _accumulationY = _accumulationYPerView[(int)_currentMode];
        }
        else
        {
            _accumulationX = _views[(int)_currentMode].rotationEuler.y;
            _accumulationY = _views[(int)_currentMode].rotationEuler.x;
        }

        UpdateViewStateText();
    }

    public void SwitchView()
    {
        SwitchTo(NextMode(_currentMode));
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameState != GameState.Playing)
        {
            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            // 커서가 풀려 있으면 입력을 처리하지 않는다.
        }

        if (Input.GetKeyDown(KeyCode.V) && _isTransitioning == false)
        {
            SwitchView();
        }

        if (IsOrbitView(_currentMode) == true && Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            _accumulationX += mouseX * _rotationSpeed * Time.deltaTime;
            _accumulationY -= mouseY * _rotationSpeed * Time.deltaTime;
            _accumulationY = Mathf.Clamp(_accumulationY, -90f, 90f);
        }

        // 반동 복귀
        _recoilPitch = Mathf.Lerp(_recoilPitch, 0f, _recoilRecoverySpeed * Time.deltaTime);
        _recoilYaw = Mathf.Lerp(_recoilYaw, 0f, _recoilRecoverySpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        if (_views == null || _views.Length == 0)
        {
            return;
        }

        CameraViewSettings view = _views[(int)_currentMode];

        Vector3 targetPosition;
        if (view.inheritPlayerYaw == true)
        {
            targetPosition = _target.position + _target.rotation * view.positionOffset;
        }
        else
        {
            targetPosition = _target.position + view.positionOffset;
        }

        Quaternion targetRotation;
        if (view.inheritPlayerYaw == true)
        {
            targetRotation = Quaternion.Euler(_accumulationY + _recoilPitch, _accumulationX + _recoilYaw, 0f);
        }
        else
        {
            targetRotation = Quaternion.Euler(view.rotationEuler.x + _recoilPitch, view.rotationEuler.y + _recoilYaw, view.rotationEuler.z);
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    private CameraState NextMode(CameraState current)
    {
        int nextIndex = ((int)current + 1) % System.Enum.GetValues(typeof(CameraState)).Length;
        return (CameraState)nextIndex;
    }

    private bool IsOrbitView(CameraState mode)
    {
        CameraViewSettings view = _views[(int)mode];
        return view.inheritPlayerYaw;
    }

    public bool IsFixedView
    {
        get
        {
            if (_views == null || _views.Length == 0)
            {
                return false;
            }
            return _views[(int)_currentMode].inheritPlayerYaw == false;
        }
    }

    private void SwitchTo(CameraState next)
    {
        CameraState previous = _currentMode;
        bool previousIsOrbit = IsOrbitView(previous);
        bool nextIsOrbit = IsOrbitView(next);

        if (previousIsOrbit == true)
        {
            _accumulationXPerView[(int)previous] = _accumulationX;
            _accumulationYPerView[(int)previous] = _accumulationY;
            _accumulationInitialized[(int)previous] = true;
        }

        if (nextIsOrbit == true)
        {
            if (_accumulationInitialized[(int)next] == false)
            {
                _accumulationX = _views[(int)next].rotationEuler.y;
                _accumulationY = _views[(int)next].rotationEuler.x;
                _accumulationXPerView[(int)next] = _accumulationX;
                _accumulationYPerView[(int)next] = _accumulationY;
                _accumulationInitialized[(int)next] = true;
            }
            else if (previousIsOrbit == true)
            {
                // 오빗 시점끼리 전환: 사용자 시선 오프셋을 보존하며 새 기준각에 맞춤
                float oldBaseX = _views[(int)previous].rotationEuler.y;
                float oldBaseY = _views[(int)previous].rotationEuler.x;
                float newBaseX = _views[(int)next].rotationEuler.y;
                float newBaseY = _views[(int)next].rotationEuler.x;

                float offsetX = _accumulationX - oldBaseX;
                float offsetY = _accumulationY - oldBaseY;

                _accumulationX = newBaseX + offsetX;
                _accumulationY = Mathf.Clamp(newBaseY + offsetY, -90f, 90f);
            }
            else
            {
                // 고정 시점에서 복귀: 해당 시점에 저장된 누적값 복원
                _accumulationX = _accumulationXPerView[(int)next];
                _accumulationY = _accumulationYPerView[(int)next];
            }
        }

        if (_useFade == true)
        {
            StartCoroutine(FadeTransition(next));
        }
        else
        {
            _currentMode = next;
            UpdateViewStateText();
        }
    }

    private void UpdateViewStateText()
    {
        if (_viewStateText == null)
        {
            return;
        }
        _viewStateText.text = "현재 뷰: " + _currentMode.ToString();
    }

    private IEnumerator FadeTransition(CameraState next)
    {
        _isTransitioning = true;

        float halfDuration = _fadeDuration * 0.5f;
        float timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            SetFadeAlpha(Mathf.Clamp01(timer / halfDuration));
            yield return null;
        }

        SetFadeAlpha(1f);
        _currentMode = next;
        UpdateViewStateText();

        timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            SetFadeAlpha(1f - Mathf.Clamp01(timer / halfDuration));
            yield return null;
        }

        SetFadeAlpha(0f);
        _isTransitioning = false;
    }

    public void AddRecoil(float pitch, float yaw)
    {
        if (_useRecoil == false)
        {
            return;
        }
        _recoilPitch += pitch;
        _recoilYaw += yaw;
    }

    public void ApplyRecoil(float strength)
    {
        if (_useRecoil == false)
        {
            return;
        }
        strength = Mathf.Clamp01(strength);
        AddRecoil(_recoilPitchAmount * strength, Random.Range(-_recoilYawAmount, _recoilYawAmount) * strength);
    }

    private void CreateFadeCanvas()
    {
        GameObject canvasObject = new GameObject("FadeCanvas");
        canvasObject.transform.SetParent(transform, false);

        _fadeCanvas = canvasObject.AddComponent<Canvas>();
        _fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _fadeCanvas.sortingOrder = 999;

        canvasObject.AddComponent<CanvasScaler>();

        _fadeCanvasGroup = canvasObject.AddComponent<CanvasGroup>();
        _fadeCanvasGroup.alpha = 0f;
        _fadeCanvasGroup.blocksRaycasts = false;

        GameObject imageObject = new GameObject("FadeImage");
        imageObject.transform.SetParent(canvasObject.transform, false);

        _fadeImage = imageObject.AddComponent<Image>();
        _fadeImage.color = Color.black;
        _fadeImage.raycastTarget = false;

        RectTransform rectTransform = _fadeImage.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private void SetFadeAlpha(float alpha)
    {
        if (_fadeCanvasGroup != null)
        {
            _fadeCanvasGroup.alpha = alpha;
        }
    }
}

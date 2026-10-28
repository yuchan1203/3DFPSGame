using System.Collections;
using UnityEngine;
using TMPro;

public class CameraViewSwitcher : MonoBehaviour
{
    private static CameraViewSwitcher _instance = null;
    public static CameraViewSwitcher Instance => _instance;

    private static readonly int CameraStateCount = System.Enum.GetValues(typeof(CameraState)).Length;

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

    private CameraOrbitAngles _orbitAngles;
    private readonly CameraRecoil _recoil = new CameraRecoil();
    private CameraScreenFader _fader;

    private bool _isTransitioning = false;

    // 전환 가능한 시점 개수 (인스펙터 배열과 열거형 중 작은 쪽)
    private int ViewCount
    {
        get
        {
            if (_views == null)
            {
                return 0;
            }
            return Mathf.Min(_views.Length, CameraStateCount);
        }
    }

    public bool IsFixedView
    {
        get
        {
            CameraViewSettings view = GetView(_currentMode);
            if (view == null)
            {
                return false;
            }
            return view.inheritPlayerYaw == false;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            GameLogger.LogWarning("CameraViewSwitcher가 씬에 둘 이상 있습니다.");
        }
        _instance = this;

        if (_target == null)
        {
            GameLogger.LogError("No target assigned!");
        }

        if (ViewCount == 0)
        {
            GameLogger.LogError("시점 설정(_views)이 비어 있습니다.");
        }
        else if ((int)_currentMode >= ViewCount)
        {
            GameLogger.LogWarning($"{_currentMode} 시점 설정이 없어 첫 번째 시점으로 시작합니다.");
            _currentMode = (CameraState)0;
        }

        if (_useFade)
        {
            _fader = new CameraScreenFader(transform);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void Start()
    {
        if (ViewCount > 0)
        {
            _orbitAngles = new CameraOrbitAngles(_views, _currentMode);
        }
        UpdateViewStateText();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameState != GameState.Playing)
        {
            return;
        }

        if (_orbitAngles == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.V) && _isTransitioning == false)
        {
            SwitchView();
        }

        if (IsOrbitView(_currentMode) == true && Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            _orbitAngles.AddInput(mouseX * _rotationSpeed * Time.deltaTime, mouseY * _rotationSpeed * Time.deltaTime);
        }

        // 반동 복귀
        _recoil.Recover(_recoilRecoverySpeed, Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        CameraViewSettings view = GetView(_currentMode);
        if (view == null || _orbitAngles == null)
        {
            return;
        }

        Vector3 targetPosition;
        Quaternion targetRotation;
        if (view.inheritPlayerYaw == true)
        {
            targetPosition = _target.position + _target.rotation * view.positionOffset;
            targetRotation = Quaternion.Euler(_orbitAngles.Y + _recoil.Pitch, _orbitAngles.X + _recoil.Yaw, 0f);
        }
        else
        {
            targetPosition = _target.position + view.positionOffset;
            targetRotation = Quaternion.Euler(view.rotationEuler.x + _recoil.Pitch, view.rotationEuler.y + _recoil.Yaw, view.rotationEuler.z);
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    public void SwitchView()
    {
        SwitchTo(NextMode(_currentMode));
    }

    public void AddRecoil(float pitch, float yaw)
    {
        if (_useRecoil == false)
        {
            return;
        }
        _recoil.Add(pitch, yaw);
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

    private CameraState NextMode(CameraState current)
    {
        // 인스펙터에 설정된 시점까지만 순환한다.
        int nextIndex = ((int)current + 1) % ViewCount;
        return (CameraState)nextIndex;
    }

    private CameraViewSettings GetView(CameraState mode)
    {
        int index = (int)mode;
        if (index < 0 || index >= ViewCount)
        {
            return null;
        }
        return _views[index];
    }

    private bool IsOrbitView(CameraState mode)
    {
        CameraViewSettings view = GetView(mode);
        return view != null && view.inheritPlayerYaw;
    }

    private void SwitchTo(CameraState next)
    {
        _orbitAngles.OnViewChanged(_currentMode, next);

        if (_useFade == true && _fader != null)
        {
            StartCoroutine(FadeTransition(next));
        }
        else
        {
            ApplyMode(next);
        }
    }

    private IEnumerator FadeTransition(CameraState next)
    {
        _isTransitioning = true;
        yield return _fader.Fade(_fadeDuration, () => ApplyMode(next));
        _isTransitioning = false;
    }

    private void ApplyMode(CameraState next)
    {
        _currentMode = next;
        UpdateViewStateText();
    }

    private void UpdateViewStateText()
    {
        if (_viewStateText == null)
        {
            return;
        }
        _viewStateText.text = "현재 뷰: " + _currentMode.ToString();
    }
}

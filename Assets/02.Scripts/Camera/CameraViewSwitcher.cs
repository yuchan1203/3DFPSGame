using System.Collections;
using UnityEngine;
using TMPro;

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

    private CameraOrbitAngles _orbitAngles;
    private readonly CameraRecoil _recoil = new CameraRecoil();
    private CameraScreenFader _fader;

    private bool _isTransitioning = false;

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

    private void Awake()
    {
        if (_target == null)
        {
            Debug.LogError("No target assigned!");
        }

        if (_useFade)
        {
            _fader = new CameraScreenFader(transform);
        }
    }

    private void Start()
    {
        _orbitAngles = new CameraOrbitAngles(_views, _currentMode);
        UpdateViewStateText();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameState != GameState.Playing)
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

        if (_views == null || _views.Length == 0)
        {
            return;
        }

        CameraViewSettings view = _views[(int)_currentMode];

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
        int nextIndex = ((int)current + 1) % System.Enum.GetValues(typeof(CameraState)).Length;
        return (CameraState)nextIndex;
    }

    private bool IsOrbitView(CameraState mode)
    {
        return _views[(int)mode].inheritPlayerYaw;
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

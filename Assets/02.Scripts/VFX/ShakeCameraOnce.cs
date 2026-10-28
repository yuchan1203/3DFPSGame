using UnityEngine;
public class ShakeCameraOnce : MonoBehaviour
{
    [SerializeField] private float _shakePower = 1f;
    private CameraViewSwitcher _viewSwitcher;
    private void Awake()
    {
        _viewSwitcher = FindAnyObjectByType<CameraViewSwitcher>();
    }
    private void Start()
    {
        if (_viewSwitcher != null)
        {
            _viewSwitcher.ApplyRecoil(_shakePower);
        }
    }
}
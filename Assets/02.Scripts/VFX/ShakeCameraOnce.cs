using UnityEngine;
public class ShakeCameraOnce : MonoBehaviour
{
    [SerializeField] private float _shakePower = 1f;
    private void Start()
    {
        CameraViewSwitcher viewSwitcher = CameraViewSwitcher.Instance;
        if (viewSwitcher != null)
        {
            viewSwitcher.ApplyRecoil(_shakePower);
        }
    }
}
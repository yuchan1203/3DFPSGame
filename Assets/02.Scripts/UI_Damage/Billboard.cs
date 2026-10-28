using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _camera;

    private void LateUpdate()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                return;
            }
        }
        // 카메라와 같은 회전을 사용해 항상 정면으로 보이게 한다.
        transform.rotation = _camera.transform.rotation;
    }
}

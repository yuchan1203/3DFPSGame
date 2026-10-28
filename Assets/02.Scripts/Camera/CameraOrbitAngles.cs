using UnityEngine;

// 오빗(마우스 회전) 시점의 누적 회전값을 관리한다.
// 시점별로 누적값을 따로 보관해, 다른 시점에 다녀와도 시선을 복원한다.
public class CameraOrbitAngles
{
    private const float MinPitch = -90f;
    private const float MaxPitch = 90f;

    private readonly CameraViewSettings[] _views;

    private float _x = 0f;
    private float _y = 0f;

    private readonly float[] _xPerView;
    private readonly float[] _yPerView;
    private readonly bool[] _initialized;

    public float X => _x;
    public float Y => _y;

    public CameraOrbitAngles(CameraViewSettings[] views, CameraState initialMode)
    {
        _views = views;

        int viewCount = views.Length;
        _xPerView = new float[viewCount];
        _yPerView = new float[viewCount];
        _initialized = new bool[viewCount];

        for (int i = 0; i < viewCount; i++)
        {
            if (views[i].inheritPlayerYaw == true)
            {
                _xPerView[i] = views[i].rotationEuler.y;
                _yPerView[i] = views[i].rotationEuler.x;
                _initialized[i] = true;
            }
        }

        int initialIndex = (int)initialMode;
        if (_initialized[initialIndex] == true)
        {
            _x = _xPerView[initialIndex];
            _y = _yPerView[initialIndex];
        }
        else
        {
            _x = views[initialIndex].rotationEuler.y;
            _y = views[initialIndex].rotationEuler.x;
        }
    }

    public void AddInput(float deltaX, float deltaY)
    {
        _x += deltaX;
        _y -= deltaY;
        _y = Mathf.Clamp(_y, MinPitch, MaxPitch);
    }

    public void OnViewChanged(CameraState previous, CameraState next)
    {
        int previousIndex = (int)previous;
        int nextIndex = (int)next;
        bool previousIsOrbit = _views[previousIndex].inheritPlayerYaw;
        bool nextIsOrbit = _views[nextIndex].inheritPlayerYaw;

        if (previousIsOrbit == true)
        {
            _xPerView[previousIndex] = _x;
            _yPerView[previousIndex] = _y;
            _initialized[previousIndex] = true;
        }

        if (nextIsOrbit == false)
        {
            return;
        }

        if (_initialized[nextIndex] == false)
        {
            _x = _views[nextIndex].rotationEuler.y;
            _y = _views[nextIndex].rotationEuler.x;
            _xPerView[nextIndex] = _x;
            _yPerView[nextIndex] = _y;
            _initialized[nextIndex] = true;
        }
        else if (previousIsOrbit == true)
        {
            // 오빗 시점끼리 전환: 사용자 시선 오프셋을 보존하며 새 기준각에 맞춤
            float offsetX = _x - _views[previousIndex].rotationEuler.y;
            float offsetY = _y - _views[previousIndex].rotationEuler.x;

            _x = _views[nextIndex].rotationEuler.y + offsetX;
            _y = Mathf.Clamp(_views[nextIndex].rotationEuler.x + offsetY, MinPitch, MaxPitch);
        }
        else
        {
            // 고정 시점에서 복귀: 해당 시점에 저장된 누적값 복원
            _x = _xPerView[nextIndex];
            _y = _yPerView[nextIndex];
        }
    }
}

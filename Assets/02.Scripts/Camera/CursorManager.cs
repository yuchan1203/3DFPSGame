using UnityEngine;
public class CursorManager : MonoBehaviour
{
    private bool _wasPaused;

    private void Awake()
    {
        LockCursor();
    }
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // 일시정지 중에는 커서를 보여 주고, 해제되면 다시 잠근다.
        GameState state = GameManager.Instance.GameState;
        if (state == GameState.Pause)
        {
            _wasPaused = true;
            UnlockCursor();
            return;
        }
        if (_wasPaused)
        {
            _wasPaused = false;
            LockCursor();
        }

        // 게임 중이 아니면 커서를 건드리지 않는다.
        if (state != GameState.Playing)
        {
            return;
        }

        // 왼쪽 Alt를 누르고 있는 동안만 커서를 보여 준다.
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            UnlockCursor();
        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            LockCursor();
        }
    }
}
using UnityEngine;
public class CursorManager : MonoBehaviour
{
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
        // 게임 중이 아니면 커서를 건드리지 않는다.
        if (GameManager.Instance.GameState != GameState.Playing)
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
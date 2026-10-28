using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;
    private GameState _gameState = GameState.Ready;
    public GameState GameState => _gameState;

    [SerializeField] private TextMeshProUGUI _stateText;
    [SerializeField] private Animator _playerAnimator;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        StartCoroutine(StartSequence_Coroutine());
    }
    private IEnumerator StartSequence_Coroutine()
    {
        _stateText.gameObject.SetActive(true);
        _stateText.color = Color.white;
        _stateText.text = "3";
        yield return new WaitForSeconds(0.5f);
        _stateText.text = "2";
        yield return new WaitForSeconds(0.5f);
        _stateText.text = "1";
        yield return new WaitForSeconds(0.5f);
        _stateText.text = "시작!";
        yield return new WaitForSeconds(0.5f);
        _stateText.text = "";
        _stateText.gameObject.SetActive(false);
        _gameState = GameState.Playing;
    }

    public void GameOver()
    {
        _stateText.gameObject.SetActive(true);
        _gameState = GameState.GameOver;
        _stateText.text = "게임오버";
        _stateText.color = Color.red;
        if (_playerAnimator != null)
        {
            _playerAnimator.SetFloat(AnimatorParams.MoveMotion, 0f);
        }
    }
}
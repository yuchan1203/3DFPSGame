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
        gameObject.SetActive(true);
        _stateText.color = Color.white;
        _stateText.text = "준비";
        yield return new WaitForSeconds(2f);
        _stateText.text = "시작";
        yield return new WaitForSeconds(1f);
        _stateText.text = "";
        gameObject.SetActive(false);
        _gameState = GameState.Playing;
    }

    public void GameOver()
    {
        _gameState = GameState.GameOver;
        _stateText.text = "게임오버";
        _stateText.color = Color.red;
    }
}
using UnityEngine;
using System.Collections;
public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;
    private GameState _gameState = GameState.Ready;
    public GameState GameState => _gameState;
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    private void Start()
    {
        StartCoroutine(StartSequence_Coroutine());
    }
    private IEnumerator StartSequence_Coroutine()
    {
        yield return new WaitForSeconds(2f);
        _gameState = GameState.Playing;
    }
}
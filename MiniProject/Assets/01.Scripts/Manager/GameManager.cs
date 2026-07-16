using UnityEngine;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public enum gameState
    {
        Playing,
        Paused,
        Victory,
        GameOver
    }

    public static GameManager instance;

    public gameState CurrentState { get; private set; }

    public bool IsPlaying => CurrentState == gameState.Playing;

    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            
            Destroy(gameObject);
            return;
        }

        Time.timeScale = 1f;

        CurrentState = gameState.Playing;
    }

    //게임 오버
    public void GameOver()
    {
        if(CurrentState == gameState.GameOver)
        {
            return;
        }

        CurrentState = gameState.GameOver;

        Debug.Log("Game Over");
        Time.timeScale = 0f;
    }

    //일시정지
    public void PuaseGame()
    {
        if(CurrentState != gameState.Playing)
        {
            return;
        }

        CurrentState =gameState.Paused;

        Time.timeScale = 0f;
    }

    //일시정지 해제
    public void ResumeGame()
    {
        if(CurrentState != gameState.Paused)
        {
            return;
        }

        CurrentState = gameState.Playing;
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        if (instance == null)
        { 
            instance = this;
        }
    }
}

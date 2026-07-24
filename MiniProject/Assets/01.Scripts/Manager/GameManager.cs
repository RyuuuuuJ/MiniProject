using UnityEngine;
using System;
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
    public event Action<gameState> OnGameStateChanged;
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

    private void SetState(gameState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }

        CurrentState = newState;

        OnGameStateChanged?.Invoke(CurrentState);
    }


    //게임 오버
    public void GameOver()
    {
        if(CurrentState == gameState.GameOver)
        {
            return;
        }

        SetState(gameState.GameOver);

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

        SetState(gameState.Paused);

        Time.timeScale = 0f;
    }

    //일시정지 해제
    public void ResumeGame()
    {
        if(CurrentState != gameState.Paused)
        {
            return;
        }

        SetState(gameState.Playing);
        Time.timeScale = 1f;
    }

    // 게임 승리
    public void Victory()
    {       
        if (CurrentState == gameState.Victory || CurrentState == gameState.GameOver)
        {
            return;
        }

        SetState(gameState.Victory);

        Debug.Log("Victory");

        //승리 화면 나올때 시간 정지
        Time.timeScale = 0f;
    }

    private void OnDestroy()
    {
        if (instance == null)
        { 
            instance = this;
        }
    }
}

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

    // 일반 속도와 2배속 값
    private const float NormalSpeed = 1f;
    private const float FastSpeed = 2f;

    private float currentPlaySpeed = NormalSpeed;

    public float CurrentPlaySpeed => currentPlaySpeed;

    public bool IsFastSpeed => Mathf.Approximately(currentPlaySpeed, FastSpeed);

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

        currentPlaySpeed = NormalSpeed;
        Time.timeScale = currentPlaySpeed;

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

        if (PlaySound.instance != null)
        {
            PlaySound.instance.StopGameBGM();
            PlaySound.instance.PlayGameOver();
        }


        Time.timeScale = 0f;
    }

    //일시정지 버튼 클릭
    public void TogglePause()
    {
        if (CurrentState == gameState.Victory || CurrentState == gameState.GameOver)
        {
            return;
        }

        if (CurrentState == gameState.Paused)
        {
            ResumeGame();
        }
        else if (CurrentState == gameState.Playing)
        {
            PauseGame();
        }
    }

    //일시정지
    public void PauseGame()
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
        Time.timeScale = currentPlaySpeed;
    }

    //배속 전환
    public void ToggleGameSpeed()
    {
        if (CurrentState != gameState.Playing)
        {
            return;
        }

        currentPlaySpeed = IsFastSpeed? NormalSpeed: FastSpeed;

        Time.timeScale = currentPlaySpeed;
    }

    // 게임 승리
    public void Victory()
    {       
        if (CurrentState == gameState.Victory || CurrentState == gameState.GameOver)
        {
            return;
        }

        SetState(gameState.Victory);

        if (PlaySound.instance != null)
        {
            PlaySound.instance.StopGameBGM();
            PlaySound.instance.PlayVictory();
        }


        //승리 화면 나올때 시간 정지
        Time.timeScale = 0f;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}

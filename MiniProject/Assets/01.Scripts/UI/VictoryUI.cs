using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUI : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameObject victoryPanel;

    private bool isVictoryShown;

    private void Awake()
    {
        isVictoryShown = false;

        // 게임 시작 시 승리 화면을 숨깁니다.
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }
    private void OnEnable()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared += ShowVictory;
        }
    }

    private void OnDisable()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared -= ShowVictory;
        }
    }

    private void ShowVictory()
    {
        if (isVictoryShown)
        {
            return;
        }

        isVictoryShown = true;

        if (GameManager.instance != null)
        {
            GameManager.instance.Victory();
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }

    // 현재 스테이지 재시작
    public void RetryGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 메인 메뉴로 이동
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 오버시 나타나는 UI
public class GameOverUI : MonoBehaviour
{
    // 재시작
    public void RetryGame()
    {
        
        Time.timeScale = 1f;

        // 현재 열려 있는 씬을 다시 불러오기
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 메인 메뉴 씬으로 이동
    public void GoToMainMenu()
    {
        
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}

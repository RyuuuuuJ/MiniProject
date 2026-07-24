using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject settingPanel;

    private void Awake()
    {
        Time.timeScale = 1.0f;

        settingPanel.SetActive(false);
    }

    //게임 시작/ 씬으로 이동
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // 설정 창 열기
    public void OpenSettings()
    {
        settingPanel.SetActive(true);
    }

    // 설정 창 닫기
    public void CloseSettings()
    {
        settingPanel.SetActive(false);
    }

    // 게임 종료
    public void QuitGame()
    {
#if UNITY_EDITOR
        // Unity Editor에서 테스트할 때 Play Mode를 종료합니다.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

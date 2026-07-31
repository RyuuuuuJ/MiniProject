using UnityEngine;
using UnityEngine.UI;

public class GameControlUI : MonoBehaviour
{

    [SerializeField] private Image pausePlayIcon;
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite playSprite;

    [SerializeField] private Image speedIcon;

    [SerializeField] private Color normalColor;

    [SerializeField] private Color activeColor;
    [SerializeField] private GameObject soundSettingPanel;
    // 아이콘 이미지 크기가 달라서 임의로 크기 설정
    [SerializeField] private Vector2 pauseIconSize = new Vector2(52f, 52f);
    [SerializeField] private Vector2 playIconSize = new Vector2(44f, 44f);
    public void OnClickPause()
    {
        if (GameManager.instance == null)
        {
            return;
        }

        GameManager.instance.TogglePause();

        RefreshButtonVisual();
    }

    public void OnClickSpeed()
    {
        if (GameManager.instance == null)
        {
            return;
        }

        GameManager.instance.ToggleGameSpeed();

        RefreshButtonVisual();
    }

    // 인게임 사운드 설정
    public void ToggleSoundSettings()
    {
        if (soundSettingPanel == null)
        {
            return;
        }

        soundSettingPanel.SetActive(!soundSettingPanel.activeSelf);
    }

    public void CloseSoundSettings()
    {
        if (soundSettingPanel != null)
        {
            soundSettingPanel.SetActive(false);
        }

        PlayerPrefs.Save();
    }

    private void Start()
    {
        RefreshButtonVisual();
    }

    private void RefreshButtonVisual()
    {
        if (GameManager.instance == null)
        {
            return;
        }

        bool isPaused = GameManager.instance.CurrentState ==  GameManager.gameState.Paused;

        if (pausePlayIcon != null)
        {
            //상태마다 아이콘 변경
            pausePlayIcon.sprite =isPaused ? playSprite : pauseSprite;

            pausePlayIcon.rectTransform.sizeDelta = isPaused ? playIconSize : pauseIconSize;

            // 아이콘 중앙에 배치
            pausePlayIcon.rectTransform.anchoredPosition = Vector2.zero;
        }

        if (speedIcon != null)
        {
            speedIcon.color =GameManager.instance.IsFastSpeed ? activeColor  : normalColor;
        }
    }
}
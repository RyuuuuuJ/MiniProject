using TMPro;
using UnityEngine;
using UnityEngine.UI;


// Wave 시작 UI
public class WaveUI : MonoBehaviour
{

    [SerializeField] private WaveManager waveManager;

    [SerializeField] private TMP_Text waveText;

    [SerializeField] private TMP_Text remainingMonsterText;

    [SerializeField] private Button startWaveButton;

    [SerializeField] private TMP_Text startWaveButtonText;

    [SerializeField] private GameManager gameManager;

    private void OnEnable()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveChanged += UpdateWaveText;
            waveManager.OnRemainingMonsterChanged += UpdateRemainingMonsterText;
            waveManager.OnWaveStateChanged += UpdateWaveState;
        }

        // 게임 승리와 패배 상태 감지
        if (gameManager != null)
        {
            gameManager.OnGameStateChanged += UpdateGameState;
        }
    }

    private void Start()
    {
        RefreshUI();

        if (gameManager != null)
        {
            UpdateGameState(gameManager.CurrentState);
        }
    }

    private void OnDisable()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveChanged -= UpdateWaveText;
            waveManager.OnRemainingMonsterChanged -= UpdateRemainingMonsterText;
            waveManager.OnWaveStateChanged -= UpdateWaveState;
        }

        if (gameManager != null)
        {
            gameManager.OnGameStateChanged -= UpdateGameState;
        }
    }

    public void OnClickStartWave()
    {
        if (waveManager == null)
        {
            Debug.LogError(
                "WaveUI에 WaveManager가 연결되지 않았습니다."
            );

            return;
        }

        waveManager.StartNextWave();
    }

    private void RefreshUI()
    {
        if (waveManager == null)
        {
            return;
        }

        UpdateWaveText(waveManager.CurrentWaveNumber,waveManager.TotalWaveCount);

        UpdateRemainingMonsterText(waveManager.RemainingMonsterCount);

        UpdateWaveState(waveManager.CurrentState);
    }

    private void UpdateWaveText(int currentWave, int totalWave)
    {
        if (waveText == null)
        {
            return;
        }

        waveText.text = $"WAVE {currentWave} / {totalWave}";
    }

    private void UpdateRemainingMonsterText(int remainingCount)
    {
        if (remainingMonsterText == null)
        {
            return;
        }

        remainingMonsterText.text = $"ENEMY {remainingCount}";
    }

    private void UpdateWaveState(WaveManager.WaveState state)
    {
        if (startWaveButton == null)
        {
            return;
        }

        switch (state)
        {
            case WaveManager.WaveState.Waiting:
                startWaveButton.interactable = true;
                SetButtonText("START WAVE");
                break;

            case WaveManager.WaveState.Spawning:

            case WaveManager.WaveState.Fighting:
                startWaveButton.interactable = false;
                SetButtonText("IN PROGRESS");
                break;

            case WaveManager.WaveState.Cleared:
                bool hasNextWave =waveManager.CurrentWaveNumber <waveManager.TotalWaveCount;

                startWaveButton.interactable = hasNextWave;

                SetButtonText(hasNextWave ? "NEXT WAVE" : "ALL CLEAR");
                break;
        }
    }

    private void UpdateGameState(GameManager.gameState state)
    {
        if (startWaveButton == null)
        {
            return;
        }

        bool gameFinished =state == GameManager.gameState.Victory || state == GameManager.gameState.GameOver;

        startWaveButton.gameObject.SetActive(!gameFinished);
    }

    private void SetButtonText(string text)
    {
        if (startWaveButtonText != null)
        {
            startWaveButtonText.text = text;
        }
    }
}
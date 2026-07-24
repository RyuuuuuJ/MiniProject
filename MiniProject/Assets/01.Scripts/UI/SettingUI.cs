using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    private const string VolumeSaveKey = "MasterVolume";

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeValueText;
    [SerializeField] private Image soundIcon;

    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;

    private void Awake()
    {
        float savedVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeSaveKey, 1f)
   );

        volumeSlider.SetValueWithoutNotify(savedVolume);

        PlayerPrefs.SetFloat(VolumeSaveKey, savedVolume);

        ApplyVolume(savedVolume);
    }

    // 볼륨 세팅
    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        ApplyVolume(volume);

        //값 저장
        PlayerPrefs.SetFloat(VolumeSaveKey, volume);
    }

    private void ApplyVolume(float volume)
    {  
        AudioListener.volume = volume;

        volumeValueText.text =$"{Mathf.RoundToInt(volume * 100f)}%";

        // 볼륨이 0이면 음소거 아이콘을 표시합니다.
        soundIcon.sprite = volume <= 0.001f? soundOffSprite: soundOnSprite;
    }

    private void OnDestroy()
    {
        // 게임 저장
        PlayerPrefs.Save();
    }
}

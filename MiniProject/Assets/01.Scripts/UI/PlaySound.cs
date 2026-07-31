using UnityEngine;

public enum TowerSoundType
{
    Fire,
    Ice,
    Lightning
}

public class PlaySound : MonoBehaviour
{
    public static PlaySound instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource gameBgmSource;
    [SerializeField] private AudioClip fireAttackClip;
    [SerializeField] private AudioClip iceAttackClip;
    [SerializeField] private AudioClip lightningAttackClip;
    [SerializeField] private AudioClip[] monsterHitClips;
    [SerializeField] private AudioClip monsterDeathClip;
    [SerializeField] private AudioClip towerBuildClip;
    [SerializeField] private AudioClip towerUpgradeClip;
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private float towerVolume = 0.3f;

    [SerializeField] private float monsterHitVolume = 0.2f;

    [SerializeField] private float monsterDeathVolume = 0.35f;
    [SerializeField] private float towerBuildVolume = 0.6f;
    [SerializeField] private float towerUpgradeVolume = 0.7f;
    [SerializeField] private float victoryVolume = 0.8f;
    [SerializeField] private float gameOverVolume = 0.8f;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }
    }

    // 타워 종류에 맞는 공격음을 재생
    public void PlayTowerAttack(TowerSoundType soundType)
    {
        AudioClip selectedClip = null;

        switch (soundType)
        {
            case TowerSoundType.Fire:
                selectedClip = fireAttackClip;
                break;

            case TowerSoundType.Ice:
                selectedClip = iceAttackClip;
                break;

            case TowerSoundType.Lightning:
                selectedClip = lightningAttackClip;
                break;
        }

        PlayClip(selectedClip, towerVolume);
    }

    // 두 개의 몬스터 피격음 중 하나를 무작위 재생
    public void PlayMonsterHit()
    {
        if (monsterHitClips == null || monsterHitClips.Length == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, monsterHitClips.Length);

        PlayClip( monsterHitClips[randomIndex], monsterHitVolume);
    }

    public void PlayMonsterDeath()
    {
        PlayClip(monsterDeathClip, monsterDeathVolume);
    }

    // 타워 건설시 재생
    public void PlayTowerBuild()
    {
        PlayClip(towerBuildClip, towerBuildVolume);
    }

    // 타워 업그레이드시 재생
    public void PlayTowerUpgrade()
    {
        PlayClip(towerUpgradeClip, towerUpgradeVolume);
    }

    // 승리시 재생
    public void PlayVictory()
    {
        PlayClip(victoryClip, victoryVolume);
    }

    // 패배시 재생
    public void PlayGameOver()
    {
        PlayClip(gameOverClip, gameOverVolume);
    }

    // 승리 또는 패배 시 게임 BGM 정지
    public void StopGameBGM()
    {
        if (gameBgmSource == null)
        {
            return;
        }

        if (gameBgmSource.isPlaying)
        {
            gameBgmSource.Stop();
        }
    }

    private void PlayClip(AudioClip clip, float volume)
    {
        if (sfxSource == null || clip == null)
        {
            return;
        }

        // 음소거 상태일때 재생 X
        if (AudioListener.volume <= 0.001f)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, volume);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
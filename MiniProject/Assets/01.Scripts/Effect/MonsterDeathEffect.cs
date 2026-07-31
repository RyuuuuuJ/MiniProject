using UnityEngine;

public class MonsterDeathEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer effectRenderer;
    [SerializeField] private Sprite[] frames;

    [SerializeField] private float frameDuration = 0.08f;

    private float elapsedTime;
    private int currentFrameIndex;

    private void Awake()
    {
        if (effectRenderer == null)
        {
            effectRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        elapsedTime = 0f;
        currentFrameIndex = 0;

        if (effectRenderer != null && frames != null && frames.Length > 0)
        {
            effectRenderer.sprite = frames[0];
        }
    }

    private void Update()
    {
        if (effectRenderer == null || frames == null || frames.Length == 0)
        {
            ReturnToPool();
            return;
        }

        elapsedTime += Time.deltaTime;

        int nextFrameIndex = Mathf.FloorToInt(elapsedTime / frameDuration);


        if (nextFrameIndex >= frames.Length)
        {
            ReturnToPool();
            return;
        }

        // 같은 프레임을 반복해서 안넣기
        if (nextFrameIndex == currentFrameIndex)
        {
            return;
        }

        currentFrameIndex = nextFrameIndex;
        effectRenderer.sprite = frames[currentFrameIndex];
    }

    private void ReturnToPool()
    {
        if (ObjectPoolManager.instance != null)
        {
            ObjectPoolManager.instance.ReturnObject(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

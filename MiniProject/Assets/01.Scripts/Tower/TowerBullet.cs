using System.Collections;
using UnityEngine;

public class TowerBullet : MonoBehaviour
{
    private MonsterHp target;
    private int damage;
    private float speed;
    private bool isInitialized;

    [SerializeField] Sprite hitSprite;
    [SerializeField] float hitEffectDuration;
    [SerializeField] float hitScale;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector3 originalScale;
    private Color originalColor;

    private bool isHitting;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        originalScale = transform.localScale;

        if(spriteRenderer != null )
        {
            originalColor = spriteRenderer.color;
        }
    }
    private void OnEnable()
    {
        //정보 초기화
        target = null;
        damage = 0;
        speed = 0f;

        isInitialized = false;
        isHitting = false;

        transform.localScale = originalScale;

        if(spriteRenderer != null )
        {
            spriteRenderer.color = originalColor;
        }

        if(animator != null )
        {
            animator.enabled = true;
        }
    }

    //타워 공격 시 공격대상,데미지,속도를 불러옴
    public void Initialize(MonsterHp newTarget,int newDamage,float newSpeed)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;

        isInitialized = true;
    }

    private void Update()
    {
        if(!isInitialized)
        {
            return;
        }

        //공격 중 적이 죽거나 사라지면 풀로 반환
        if (target == null || target.isDead)
        {
            ReturnToPool();
            return; 
        }

        MoveToTarget();
    }

    //적한테 공격 발사
    private void MoveToTarget()
    {
        Vector3 targetPosition = target.transform.position;

        Vector3 direction = targetPosition - transform.position;


        if (direction.sqrMagnitude > 0.001f)
        {
            float angle =
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation =
                Quaternion.Euler(0f, 0f, angle);
        }

        //현재 위치에서 몬스터 위치로 발사
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed*Time.deltaTime);

        //적과의 거리 계산
        float distance = Vector3.Distance(transform.position, targetPosition);

        //사정거리 안에 들어오면 공격
        if(distance <= 0.05f)
        {
            HitTarget();
        }
    }

    //몬스터 적중
    private void HitTarget()
    {
        if(isHitting)
        {
            return;
        }

        isHitting = true;
        isInitialized = false;

        if(target != null && !target.isDead)
        {
            target.TakeDamage(damage);
        }
        target = null;

        StartCoroutine(PlayHitEffect());
    }

    private IEnumerator PlayHitEffect()
    {
        if (animator != null)
        {
            animator.enabled = false;
        }

        // 명중 순간 표시할 스프라이트로 변경
        if (spriteRenderer != null && hitSprite != null)
        {
            spriteRenderer.sprite = hitSprite;
        }

        float elapsedTime = 0f;

        while (elapsedTime < hitEffectDuration)
        {
            elapsedTime += Time.deltaTime;

            float ratio =
                Mathf.Clamp01(elapsedTime / hitEffectDuration);

            // 명중 순간 총알이 커짐
            transform.localScale = Vector3.Lerp(
                originalScale,
                originalScale * hitScale,
                ratio
            );

            // 명중 동시에 투명해짐
            if (spriteRenderer != null)
            {
                Color color = originalColor;
                color.a = 1f - ratio;

                spriteRenderer.color = color;
            }

            yield return null;
        }

        ReturnToPool();
    }
    private void ReturnToPool()
    {
        isInitialized = false;
        target = null;

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

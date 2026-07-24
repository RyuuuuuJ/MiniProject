using UnityEngine;

public class LightningAttack : MonoBehaviour
{
    [SerializeField] private float baseLength = 3.6f;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private static readonly int LightningAttackHash = Animator.StringToHash("Base Layer.LightningAttack");

    private void Awake()
    {
        // GetComponent는 처음 한 번만 실행합니다.
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // 게임이 시작되자마자 애니메이션이 자동 재생되는 것을 방지합니다.
        animator.enabled = false;

        SetAlpha(0f);
    }

    public void Play(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        // 두 위치가 사실상 같으면 번개를 표시하지 않습니다.
        if (distance <= 0.001f)
        {
            return;
        }

        // 시작점과 끝점의 중앙으로 이동합니다.
        transform.position = (start + end) * 0.5f;

        // 번개 이미지의 오른쪽이 몬스터를 향하도록 회전합니다.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        float parentScaleX = 1f;
        float parentScaleY = 1f;

        if (transform.parent != null)
        {
            parentScaleX = Mathf.Max(Mathf.Abs(transform.parent.lossyScale.x), 0.001f);

            parentScaleY = Mathf.Max(Mathf.Abs(transform.parent.lossyScale.y), 0.001f);
        }

        transform.localScale = new Vector3(distance / baseLength / parentScaleX,1f / parentScaleY,1f);

        // 첫 재생 때 비활성화해 둔 Animator를 켭니다.
        animator.enabled = true;

        // 항상 애니메이션 첫 프레임부터 다시 재생합니다.
        animator.Play(LightningAttackHash, 0, 0f);
        animator.Update(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}

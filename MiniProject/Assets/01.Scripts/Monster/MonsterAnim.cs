using UnityEngine;

public class MonsterAnim : MonoBehaviour
{
    [SerializeField] private SpriteRenderer monsterRenderer;

    //원본 이미지가 오른쪽을 바라보면 체크
    [SerializeField] private bool spriteFacesRight = true;

    [SerializeField, Min(0.0001f)] private float minimumMoveDistance = 0.001f;

    private Vector3 previousPosition;
    private bool hasPreviousPosition;

    private void Awake()
    {
        if (monsterRenderer == null)
        {
            monsterRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        // 오브젝트 풀에서 다시 생성됐을 때 위치 초기화
        hasPreviousPosition = false;
    }

    private void LateUpdate()
    {
        if (monsterRenderer == null)
        {
            return;
        }

        Vector3 currentPosition = transform.position;

        if (!hasPreviousPosition)
        {
            previousPosition = currentPosition;
            hasPreviousPosition = true;
            return;
        }

        float horizontalMovement =
            currentPosition.x - previousPosition.x;

        // 세로로 이동하거나 거의 움직이지 않았다면
        // 마지막으로 바라보던 방향 유지
        if (Mathf.Abs(horizontalMovement) >= minimumMoveDistance)
        {
            bool movingRight = horizontalMovement > 0f;

            monsterRenderer.flipX = spriteFacesRight ? !movingRight : movingRight;
        }

        previousPosition = currentPosition;
    }
}

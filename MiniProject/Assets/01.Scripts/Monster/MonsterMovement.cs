using UnityEngine;

public class MonsterMovement : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private float arrivalDistance = 0.05f;

    private Transform[] wayPoints;

    //적이 이동하고 있는 목표 웨이포인트의 번호
    private int currentWaypointIndex;

    bool canMove;

    private void Awake()
    {
        FindWayPoints();
    }

    private void OnEnable()
    {

        if (wayPoints == null || wayPoints.Length < 2)
        {
            FindWayPoints();
        }
        if (wayPoints == null || wayPoints.Length < 2)
        {
            canMove = false;
            return;
        }

        transform.position = wayPoints[0].position;
        currentWaypointIndex = 1;
        canMove = true;
    }

    private void OnDisable()
    {
        canMove = false ;
    }  

    void Update()
    {
        if (!canMove)
        {
            return;
        }

        MoveToNextWaypoint();
    }
    void FindWayPoints()
    {
        GameObject waypointRoot = GameObject.Find("Waypoints");

        if (waypointRoot == null)
        {
            Debug.LogError("Waypoints 오브젝트를 찾을 수 없습니다.");
            return;
        }

        int waypointCount = waypointRoot.transform.childCount;

        if (waypointCount < 2)
        {
            Debug.LogError("Waypoint가 최소 2개 필요합니다.");
            return;
        }

        wayPoints = new Transform[waypointCount];

        for (int i = 0; i < waypointCount; i++)
        {
            wayPoints[i] = waypointRoot.transform.GetChild(i);
        }
    }

    void MoveToNextWaypoint()
    {
        if (currentWaypointIndex >= wayPoints.Length)
        {
            return;
        }

        Transform targetWaypoint = wayPoints[currentWaypointIndex];

        //현재 위치에서 목표 위치로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        //현재 위치와 목표의 거리 구하기
        float distanceToTarget = Vector3.Distance(transform.position, targetWaypoint.position);

        //목표 지점에 가까워 졋는지 확인 
        if (distanceToTarget <= arrivalDistance)
        {
            transform.position = targetWaypoint.position;

            currentWaypointIndex++;

            if (currentWaypointIndex >= wayPoints.Length)
            {
                ReachBase();
            }
        }
    }

    void ReachBase()
    {
        Debug.Log($"{gameObject.name}이 기지에 도착했습니다.");

        //pool에 반환
        if (ObjectPoolManager.instance != null)
        {
            ObjectPoolManager.instance.ReturnObject(gameObject);
        }
        else
        {
            Debug.LogError("ObjectPoolManager를 찾을 수 없습니다.");
            gameObject.SetActive(false);
        }
    }

   
}

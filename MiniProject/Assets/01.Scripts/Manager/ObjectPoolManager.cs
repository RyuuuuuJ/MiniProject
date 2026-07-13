using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{

    public static ObjectPoolManager instance;

    [SerializeField] List<GameObject> objList = new List<GameObject>();

    Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, Transform> poolParents = new Dictionary<string, Transform>();

    [SerializeField]
    private int poolSize = 5;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        CreatePool();
    }

    void CreatePool()
    {
        foreach (GameObject prefab in objList)
        {
            // 프리팹 이름을 풀의 고유 키로 사용.
            string key = prefab.name;

            // 중복 확인        
            if (pools.ContainsKey(key))
            {
                Debug.LogWarning(
                    $"{key} 프리팹이 ObjectPoolManager에 중복 등록되었습니다."
                );

                continue;
            }

            pools.Add(key, new Queue<GameObject>());

            prefabs.Add(key, prefab);

            GameObject parentPool = new GameObject($"{key}_Pool");

            parentPool.transform.SetParent(transform);

            poolParents.Add(key, parentPool.transform);

            // 설정한 poolSize만큼 오브젝트를 미리 생성합니다.
            for (int i = 0; i < poolSize; i++)
            {
                GameObject go = CreateNewObject(key);
            
                pools[key].Enqueue(go);
            }
        }
    }

    private GameObject CreateNewObject(string key)
    {
        GameObject go = Instantiate(prefabs[key],poolParents[key]);

        go.name = key;

        go.SetActive(false);

        return go;
    }

    public GameObject GetObject(string key,Vector3 position,Quaternion rotation)
    {
        GameObject go;
        if (pools[key].Count > 0)
        {
            go = pools[key].Dequeue();
        }
        else
        {
            go = CreateNewObject(key);
        }

        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);

        return go;
    }


    public void ReturnObject(GameObject go)
    {
        if(go == null)
        {
            return;
        }

        string key = go.name;

        if(!pools.ContainsKey(key))
        {
            Destroy(go);
            return;
        }

        go.SetActive(false);

        go.transform.SetParent(poolParents[key]);

        pools[key].Enqueue(go);
    }
}

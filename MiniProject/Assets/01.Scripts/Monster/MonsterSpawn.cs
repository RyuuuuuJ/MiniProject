using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] GameObject Slime;
    //[SerializeField] GameObject Goblin;
    //[SerializeField] GameObject Orc;
    //[SerializeField] GameObject Boss;

    //생성할 몬스터 수
    [SerializeField] int Spawncount = 5;

    //첫번째 몬스터가 시작되기 전까지 시간 (1초)
    [SerializeField] float StartDelay = 1f;

    //몬스터 생성 간격
    [SerializeField] float spawnInterval = 1f;

    void Start()
    {
        StartCoroutine(SpawnMonster());
    }

    IEnumerator SpawnMonster()
    {

        //생성 갯수가 0 이하면 종료
        if(Spawncount <= 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(StartDelay);

        for (int i = 0;i<Spawncount;i++)
        {
            ObjectPoolManager.instance.GetObject("Slime", transform.position, Quaternion.identity);
            //ObjectPoolManager.instance.GetObject("Goblin", transform.position, Quaternion.identity);
            //ObjectPoolManager.instance.GetObject("Orc", transform.position, Quaternion.identity);
            //ObjectPoolManager.instance.GetObject("Boss", transform.position, Quaternion.identity);

            if (i<Spawncount-1)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }

       
    }
}

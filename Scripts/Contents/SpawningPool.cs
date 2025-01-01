using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    [SerializeField]
    int _monsterCount = 0;

    [SerializeField]
    int _potionCount = 0;

    int _reserveMonsterCount = 0;
    int _reservePotionCount = 0;

    [SerializeField]
    int _keepMonsterCount = 0;

    [SerializeField]
    int _keepPotionCount = 0;

    [SerializeField]
    Vector3 _spawnPos;

    [SerializeField]
    float _spawnRadius = 500.0f;

    [SerializeField]
    float _spawnTime = 5.0f;

    public GameObject Parent;

    public void AddMonsterCount(int value) 
    {
        _monsterCount += value; 
    }
    public void AddPotionCount(int value)
    {
        _potionCount += value;
    }

    public void SetKeepMonsterCount(int count) { _keepMonsterCount = count; }
    public void SetKeepPotionCount(int count) { _keepPotionCount = count; }

    void Start()
    {
        Managers.Game.OnSpawnMonsterEvent -= AddMonsterCount;
        Managers.Game.OnSpawnMonsterEvent += AddMonsterCount;

        Managers.Game.OnSpawnPotionEvent -= AddPotionCount;
        Managers.Game.OnSpawnPotionEvent += AddPotionCount;
    }

    void Update()
    {
        if (_keepMonsterCount > 0)
        {
            while (_reserveMonsterCount + _monsterCount < _keepMonsterCount)
            {
                StartCoroutine(ReserveMonsterSpawn());
            }
        }
        if (_keepPotionCount > 0)
        {
            while (_reservePotionCount + _potionCount < _keepPotionCount)
            {
                StartCoroutine(ReservePotionSpawn());
            }
        }
    }

    // 사용할 문자열 배열
    private string[] monsterArray = {
        "Lizard",
        "LizardWarrior",
        "EnemyCreature",
        "EnemyGoblin",
        "PlantMonster",
        "StoneGolem",
        "WoodMonster",
    };

    // 각 문자열에 대한 확률 (문자열 순서와 동일)
    private float[] monsterProbabilities = {
        0.3f, // Lizard 30%
        0.15f, // LizardWarrior 15%
        0.3f, // EnemyCreature 30%
        0.05f, // EnemyGoblin 5%
        0.1f, // PlantMonster 10%
        0.05f, // StoneGolem 5%
        0.05f // WoodMonster 5%
    };

    private string[] postionArray = {
        "HP_Large",
        "HP_Middle",
        "HP_Large"
    };

    // 각 문자열에 대한 확률 (문자열 순서와 동일)
    private float[] potionProbabilities = {
        0.1f, // HP_Large 10%
        0.2f, // HP_Middle 20%
        0.8f, // HP_Large 80%
    };

    // 랜덤한 문자열을 반환하는 함수
    public string GetRandomMonsterString()
    {
        // 확률 총합 계산
        float totalProbability = 0f;
        foreach (float prob in monsterProbabilities)
        {
            totalProbability += prob;
        }

        // 랜덤 값 생성 (0부터 확률 총합까지)
        float randomPoint = Random.Range(0f, totalProbability);

        // 확률 누적 합을 비교하여 문자열 선택
        float cumulativeProbability = 0f;
        for (int i = 0; i < monsterArray.Length; i++)
        {
            cumulativeProbability += monsterProbabilities[i];
            if (randomPoint <= cumulativeProbability)
            {
                return monsterArray[i];
            }
        }

        // 기본 반환값 (예외 상황 처리)
        return "No Match Found";
    }

    public string GetRandomPotionString()
    {
        // 확률 총합 계산
        float totalProbability = 0f;
        foreach (float prob in potionProbabilities)
        {
            totalProbability += prob;
        }

        // 랜덤 값 생성 (0부터 확률 총합까지)
        float randomPoint = Random.Range(0f, totalProbability);

        // 확률 누적 합을 비교하여 문자열 선택
        float cumulativeProbability = 0f;
        for (int i = 0; i < postionArray.Length; i++)
        {
            cumulativeProbability += potionProbabilities[i];
            if (randomPoint <= cumulativeProbability)
            {
                return postionArray[i];
            }
        }

        // 기본 반환값 (예외 상황 처리)
        return "No Match Found";
    }


    public GameObject monsterObjParent;
    IEnumerator ReserveMonsterSpawn()
    {
        _reserveMonsterCount++;
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));
        GameObject obj;

        obj = Managers.Game.Spawn(Define.WorldObject.Monster, GetRandomMonsterString());
        obj.transform.SetParent(monsterObjParent.transform);

        if (obj == null)
            yield return new WaitForSeconds(0.01f);
        
        NavMeshAgent nma = obj.GetOrAddComponent<NavMeshAgent>();
        
        Vector3 randPos;
        while (true)
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
            randDir.y = 0;
            randPos = _spawnPos + randDir;
        
            // 갈 수 있나
            NavMeshPath path = new NavMeshPath();
            if (nma.CalculatePath(randPos, path))
                break;
        }
        
        obj.transform.position = randPos;
        _reserveMonsterCount--;
    }

    public GameObject potionObjParent;
    IEnumerator ReservePotionSpawn()
    {
        _reservePotionCount++;
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));
        GameObject obj;

        obj = Managers.Game.Spawn(Define.WorldObject.Potion, GetRandomPotionString());
        obj.transform.SetParent(potionObjParent.transform);
        
        if (obj == null)
            yield return new WaitForSeconds(0.01f);
        
        NavMeshAgent nma = obj.GetOrAddComponent<NavMeshAgent>();
        
        Vector3 randPos;
        while (true)
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
            randDir.y = 0;
            randPos = _spawnPos + randDir;
        
            // 갈 수 있나
            NavMeshPath path = new NavMeshPath();
            if (nma.CalculatePath(randPos, path))
                break;
        }
        
        obj.transform.position = randPos;
        _reservePotionCount--;
    }
}

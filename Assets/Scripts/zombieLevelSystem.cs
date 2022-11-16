using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(zombiePoolSystem))]
public class zombieLevelSystem : MonoBehaviour
{
    [SerializeField] private int poolCount;
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private int spawnCount;
    [SerializeField] private float spawnDelay;
    [SerializeField] private Transform[] spawnPoints;
    
    
    private GameObject[] zombiePool;
    private zombiePoolSystem _zombiePoolSystem;

    private float spawnTimer;
    private bool isInitialized;
    
    void Start()
    {
        Init();
    }


    void Init()
    {
        _zombiePoolSystem = GetComponent<zombiePoolSystem>();
        _zombiePoolSystem.GeneratePool(ref zombiePool, poolCount, this.transform, zombiePrefab);
        isInitialized = true;
    }


    void SpawnZombieBlock()
    {
        print("SPAWN ZOMBIE BLOCK");
        if (spawnCount > spawnPoints.Length) spawnCount = spawnPoints.Length;
        List<int> points = new List<int>(spawnPoints.Length);
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            points.Add(i);
        }

        int raz = spawnPoints.Length - spawnCount;
        for (int i = 0; i < raz; i++)
        {
            points.RemoveAt(Random.Range(0, points.Count-1));
        }

        for (int i = 0; i < points.Count; i++)
        {
            GameObject z =_zombiePoolSystem.GetPoolElement(ref zombiePool);
            if (z != null)
            {
                z.transform.position = spawnPoints[points[i]].position;
                z.transform.rotation = spawnPoints[points[i]].rotation;
                z.gameObject.SetActive(true);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isInitialized)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer>=spawnDelay)
            {
                spawnTimer = 0;
                SpawnZombieBlock();
            }
        }
    }
}

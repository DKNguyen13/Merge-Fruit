using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private bool _canSpawn = true;
    [SerializeField] private float _spawnDelay = 1f;
    private float _spawnCountdownTimer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        SpawnNextFruit();
    }

    void Update()
    {
        if (!_canSpawn &&  _spawnCountdownTimer >= 0f)
        {
            _spawnCountdownTimer -= Time.deltaTime;
        }
    }

    #region Spawn
    public void SpawnNextFruit()
    {
        if (!_canSpawn) return;

        _canSpawn = false;
        _spawnCountdownTimer = _spawnDelay;

        //FruitType randomType = GetRandomFruitType();
        FruitPooling.Instance.GetFruitFromPool(FruitType.Fruit_1, _spawnPoint.position);
    }

    private FruitType GetNextFruitType(FruitType current)
    {
        int nextIndex = (int)current + 1;

        if (nextIndex >= System.Enum.GetValues(typeof(FruitType)).Length) return current;

        return (FruitType)nextIndex;
    }

    private FruitType GetRandomFruitType()
    {
        int maxStartLevel = 4;
        int randomIndex = UnityEngine.Random.Range(0, maxStartLevel);

        return (FruitType)randomIndex;
    }
    #endregion

    #region Merge Fruit
    public void MergeFruit(Fruit a, Fruit b)
    {
        Vector3 mergePos = (a.transform.position + b.transform.position) / 2f;

        FruitType nextType = GetNextFruitType(a.Type);

        FruitPooling.Instance.ReturnFruitFromPool(a.gameObject);
        FruitPooling.Instance.ReturnFruitFromPool(b.gameObject);

        FruitPooling.Instance.GetFruitFromPool(nextType, mergePos);

        //AddScore(nextType);

        //SpawnNextFruit();
    }

    #endregion
}

using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _spawnDelay = 2f;
    [SerializeField] private float _spawnCountdownTimer;
    [SerializeField] private bool _canSpawn = false;
    [SerializeField] private bool _isDrag = false;
    private float _minX, _maxX;
    private Fruit _currentFruit;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Setup();
    }

    void Start()
    {
        SpawnNextFruit();
    }

    void Update()
    {
        HandleInput();
        
        if (!_canSpawn)
        {
            if (!_isDrag) return;

            if (_spawnCountdownTimer > 0f)
            {
                _spawnCountdownTimer -= Time.deltaTime;
            }
            else
            {
                _canSpawn = true;
                _isDrag = false;
                SpawnNextFruit();
            }
        }
    }

    #region Setup
    private void Setup()
    {
        float height = Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;
        _minX = - width;
        _maxX = width;
    }
    #endregion

    #region Spawn
    public void SpawnNextFruit()
    {
        if (!_canSpawn) return;

        _canSpawn = false;

        //FruitType randomType = GetRandomFruitType();
        _currentFruit = FruitPooling.Instance.GetFruitFromPool(FruitType.Fruit_1, _spawnPoint.position).GetComponent<Fruit>();
    }

    private FruitType GetRandomFruitType()
    {
        int maxStartLevel = 4;
        int randomIndex = UnityEngine.Random.Range(0, maxStartLevel);

        return (FruitType)randomIndex;
    }
    #endregion

    #region Input
    private void HandleInput()
    {
        if (_currentFruit == null) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                MoveFruit(touch.position);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                DropFruit();
            }
        }

    #if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            MoveFruit(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            DropFruit();
        }
    #endif
    }

    private void MoveFruit(Vector2 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        float fruitRadius = _currentFruit.GetComponent<CircleCollider2D>().radius * _currentFruit.transform.localScale.x;
        float clampedX = Mathf.Clamp(worldPos.x, _minX + fruitRadius, _maxX - fruitRadius);

        _currentFruit.transform.position = new Vector3(clampedX, _currentFruit.transform.position.y, 0f);
        _currentFruit.ShowDivideLine();
    }

    private void DropFruit()
    {
        Rigidbody2D rb = _currentFruit.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        _currentFruit.HideDivideLine();
        _currentFruit = null;
        _spawnCountdownTimer = _spawnDelay;
        _isDrag = true;
    }
    #endregion

    #region Merge Fruit
    private FruitType GetNextFruitType(FruitType current)
    {
        int nextIndex = (int)current + 1;

        if (nextIndex >= System.Enum.GetValues(typeof(FruitType)).Length) return current;
        return (FruitType)nextIndex;
    }

    public void MergeFruit(Fruit a, Fruit b)
    {
        Vector3 mergePos = (a.transform.position + b.transform.position) / 2f;

        FruitType nextType = GetNextFruitType(a.Type);

        FruitPooling.Instance.ReturnFruitFromPool(a.gameObject);
        FruitPooling.Instance.ReturnFruitFromPool(b.gameObject);

        FruitPooling.Instance.GetFruitMergeFromPool(nextType, mergePos);

        //AddScore(nextType);
    }

    #endregion
}

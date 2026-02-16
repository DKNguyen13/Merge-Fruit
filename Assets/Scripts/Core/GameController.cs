using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    public static System.Action<FruitType> OnFruitMerged;

    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _deathZoneTransform;
    [SerializeField] private float _spawnDelay = 2f;
    [SerializeField] private float _spawnCountdownTimer;
    [SerializeField] private int _score;

    private float _minX, _maxX;
    private Fruit _currentFruit;
    private FruitType _currentType;
    private FruitType _nextType;

    // Bool
    private bool _isDrag = false;
    private bool _canSpawn = true;
    private bool _isPaused = false;
    private bool _isPlaySound = true;
    private bool _isDragging = false;
    private bool _isGameOver = false;

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
        _currentType = GetRandomFruitType();
        _nextType = GetRandomFruitType();
        
        UIManager.Instance.UpdateNextFoodUI(_nextType);

        SpawnCurrentFruit();
    }

    void Update()
    {
        if (_isGameOver || _isPaused) return;

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

                _currentType = _nextType;
                _nextType = GetRandomFruitType();

                UIManager.Instance.UpdateNextFoodUI(_nextType);

                SpawnCurrentFruit();
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
    public void SpawnCurrentFruit()
    {
        if (!_canSpawn) return;

        _canSpawn = false;

        _currentFruit = FruitPooling.Instance.GetFruitFromPool(_currentType, _spawnPoint.position).GetComponent<Fruit>();
    }

    private FruitType GetRandomFruitType()
    {
        int maxStartLevel = 3;
        int randomIndex = UnityEngine.Random.Range(0, maxStartLevel);

        return (FruitType)randomIndex;
    }
    #endregion

    #region Input
    private void HandleInput()
    {
        if (_currentFruit == null) return;

    // #if UNITY_EDITOR
    //     if (EventSystem.current.IsPointerOverGameObject()) return;

    //     if (Input.GetMouseButton(0))
    //     {
    //         MoveFruit(Input.mousePosition);
    //     }

    //     if (Input.GetMouseButtonUp(0))
    //     {
    //         DropFruit();
    //     }
    // #elif UNITY_ANDROID
    //     if (EventSystem.current.IsPointerOverGameObject()) return;

    //     if (Input.touchCount > 0)
    //     {
    //         Touch touch = Input.GetTouch(0);

    //         if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
    //         {
    //             MoveFruit(touch.position);
    //         }

    //         if (touch.phase == TouchPhase.Ended)
    //         {
    //             DropFruit();
    //         }
    //     }

    // #endif

    #if UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
        }

        if (Input.GetMouseButton(0) && _isDragging)
        {
            MoveFruit(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            DropFruit();
            _isDragging = false;
        }

    #elif UNITY_ANDROID

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

            if (touch.phase == TouchPhase.Began)
            {
                _isDragging = true;
            }

            if (_isDragging && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
            {
                MoveFruit(touch.position);
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (_isDragging)
                {
                    DropFruit();
                    _isDragging = false;
                }
            }
        }
        else if (_isDragging)
        {
            DropFruit();
            _isDragging = false;
        }
    #endif
    }
    #endregion

    #region Move/Drop
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
        rb.angularVelocity = Random.Range(-60f, 60f);

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
        
        VFXPooling.Instance.PlayVFX(mergePos);
        _audioManager.PlaySfx(SoundType.Pop);

        OnFruitMerged?.Invoke(nextType);
        FruitPooling.Instance.GetFruitMergeFromPool(nextType, mergePos);
    }
    #endregion

    #region Score
    public void AddScore(int amount)
    {
        _score += amount;
        UIManager.Instance.UpdateScore(_score);
    }
    #endregion

    #region Handler gameover
    public void GameOver()
    {
        if (_isGameOver) return;

        _isGameOver = true;
        UIManager.Instance.ShowResultUI();

        Debug.Log("GAME OVER!");
    }
    #endregion

    #region Pause game
    public void PauseGame(bool pause)
    {
        _isPaused = pause;
        Time.timeScale = pause ? 0 : 1;
    }
    #endregion

    // Getter, setter
    public bool IsGameOver => _isGameOver;
    public bool IsPause => _isPaused;
    public bool IsPLaySound
    {
        set => _isPlaySound = value;
        get => _isPlaySound;
    }
    public float DeathY => _deathZoneTransform.position.y;
}
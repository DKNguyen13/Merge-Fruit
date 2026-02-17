using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Fruit infor")]
    [SerializeField] private FruitType _type;
    [SerializeField] private GameObject _devideLine;
    [SerializeField] private bool _isMerging;
    [SerializeField] private int _scoreValue;
    
    private Vector3 _originalScale;
    private float _overDeathTimer = 0f;
    private float _deathDelay = 1.5f;

    private Rigidbody2D _rb;

    void OnEnable()
    {
        _isMerging = false;
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _originalScale = transform.localScale;
    }

    void Update()
    {
        if (GameController.Instance.IsGameOver) return;
        if (_rb.bodyType != RigidbodyType2D.Dynamic) return;

        float deathY = GameController.Instance.DeathY;

        if (transform.position.y > deathY && GetComponent<Rigidbody2D>().velocity.magnitude < 0.1f)
        {
            _overDeathTimer += Time.deltaTime;

            if (_overDeathTimer >= _deathDelay)
            {
                GameController.Instance.GameOver();
            }
        }
        else
        {
            _overDeathTimer = 0f;
        }
    }

    #region Show/Hide divideLine
    public void ShowDivideLine()
    {
        _devideLine.SetActive(true);
    }

    public void HideDivideLine()
    {
        _devideLine.SetActive(false);
    }
    #endregion

    #region Reset
    public void ResetScale()
    {
        transform.localScale = _originalScale;
    }
    #endregion

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_type == FruitType.Fruit_8) return;

        if (collision.gameObject.CompareTag("Food"))
        {
            var fruit = collision.gameObject.GetComponent<Fruit>();
            if (fruit && fruit.Type == _type && _isMerging == false && !fruit.IsMerging && GetInstanceID() < fruit.GetInstanceID())
            {
                _isMerging = true;
                fruit.IsMerging = true;
                GameController.Instance.AddScore(_scoreValue * 2);
                GameController.Instance.MergeFruit(this, fruit);
            }
        }
    }

    // Getter, setter
    public FruitType Type => _type;
    public Rigidbody2D Rb => _rb;
    public Vector3 OriginalScale => _originalScale;
    public bool IsMerging
    {
        set => _isMerging = value;
        get => _isMerging;   
    }
    public int ScoreValue => _scoreValue;
}

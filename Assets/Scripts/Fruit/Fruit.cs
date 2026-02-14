using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Fruit infor")]
    [SerializeField] private FruitType _type;
    [SerializeField] private GameObject _devideLine;
    [SerializeField] private bool _isMerging;
    [SerializeField] private int _scoreValue;

    private Rigidbody2D _rb;

    void OnEnable()
    {
        _isMerging = false;
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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

    void OnCollisionEnter2D(Collision2D collision)
    {
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
    public bool IsMerging
    {
        set => _isMerging = value;
        get => _isMerging;   
    }
    public int ScoreValue => _scoreValue;
}

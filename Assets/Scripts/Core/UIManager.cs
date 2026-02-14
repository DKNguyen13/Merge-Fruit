using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Button")]
    [SerializeField] private Button _shopBtn;
    [SerializeField] private Button _settingBtn;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _scoreText;

    [Header("Next spawn food")]
    [SerializeField] private Image _nextFruitImage;
    [SerializeField] private Sprite[] _fruitSprites;

    private int _displayScore;
    private Coroutine _scoreRoutine;

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
        if (!_shopBtn || !_settingBtn)
        {
            Debug.LogError("Button null!");
            return;
        }

        if (!_scoreText)
        {
            Debug.LogError("Text null!");
            return;
        }
    }
    
    #region Score
    public void UpdateScore(int targetScore)
    {
        if (_scoreRoutine != null)
            StopCoroutine(_scoreRoutine);

        _scoreRoutine = StartCoroutine(AnimateScore(targetScore));
    }

    private IEnumerator AnimateScore(int target)
    {
        float duration = 0.4f;
        float timer = 0f;
        int startScore = _displayScore;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            _displayScore = Mathf.RoundToInt(Mathf.Lerp(startScore, target, t));
            _scoreText.text = _displayScore.ToString("N0");
            yield return null;
        }

        _displayScore = target;
        _scoreText.text = target.ToString("N0");
    }
    #endregion

    #region Next food
    public void UpdateNextFoodUI(FruitType type)
    {
        _nextFruitImage.sprite = _fruitSprites[(int)type];
    }
    #endregion
}
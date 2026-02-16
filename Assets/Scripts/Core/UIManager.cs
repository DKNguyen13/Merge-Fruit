using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("Setting UI")]
    [SerializeField] private GameObject _settingUI;
    [SerializeField] private Button _homeBtn;
    [SerializeField] private Button _soundBtn;
    [SerializeField] private Button _closeSettingUIBtn;

    [Header("Result UI")]
    [SerializeField] private GameObject _resultUI;
    [SerializeField] private CanvasGroup _scoreGroup;
    [SerializeField] private CanvasGroup _buttonResultGroup;
    [SerializeField] private Button _homeResultUIBtn;
    [SerializeField] private Button _closeResultUIBtn;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    [SerializeField] private float _fadeDuration = 0.3f;
    private Coroutine _resultRoutine;

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
        if (!_shopBtn || !_settingBtn || !_homeBtn || !_soundBtn || !_closeSettingUIBtn || !_closeResultUIBtn || !_homeResultUIBtn)
        {
            Debug.LogError("Button null!");
            return;
        }

        if (!_scoreText || !_highScoreText || !_currentScoreText)
        {
            Debug.LogError("Text null!");
            return;
        }

        if (!_settingUI || !_resultUI)
        {
            Debug.LogError("UI null!");
            return;
        }

        RegisterEventButton();
    }

    #region Handle button
    private void RegisterEventButton()
    {
        _settingBtn.onClick.AddListener(() =>
        {
            GameController.Instance.PauseGame(true);
            _settingUI.SetActive(true);
        });

        _soundBtn.onClick.AddListener(() => GameController.Instance.IsPLaySound = !GameController.Instance.IsPLaySound);

        // Home button
        _homeBtn.onClick.AddListener(() =>
        {
            GameController.Instance.PauseGame(false);
            SceneManager.LoadScene("MainScene");
        });

        _homeResultUIBtn.onClick.AddListener(() =>
        {
            GameController.Instance.PauseGame(false);
            SceneManager.LoadScene("MainScene");
        });

        // Close button
        _closeSettingUIBtn.onClick.AddListener(() =>
        {
            GameController.Instance.PauseGame(false);
            _settingUI.SetActive(false);
        });

        _closeResultUIBtn.onClick.AddListener(() =>
        {
            GameController.Instance.PauseGame(false);
            SceneManager.LoadScene("GameplayScene");
        });
    }
    #endregion

    #region Score 
    public void UpdateScore(int targetScore)
    {
        if (_scoreRoutine != null) StopCoroutine(_scoreRoutine);

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

    #region Result UI
    public void ShowResultUI(int currentScore, int highScore)
    {
        if (_resultRoutine != null) StopCoroutine(_resultRoutine);
        _resultRoutine = StartCoroutine(ResultRoutine(currentScore, highScore));

        if (currentScore > highScore)
        {
            GameManager.Instance.SaveData(currentScore);
        }
    }

    private IEnumerator ResultRoutine(int current, int high)
    {
        _resultUI.SetActive(true);

        _scoreGroup.alpha = 0;
        _currentScoreText.text = "Score : 0 <sprite=1>";
        _highScoreText.text = "High score: 0 <sprite=1>";

        float timer = 0;

        while (timer < _fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            _scoreGroup.alpha = timer / _fadeDuration;
            yield return null;
        }
        _scoreGroup.alpha = 1;

        yield return StartCoroutine(CountScore(_highScoreText, high, "High score: "));
        
        yield return new WaitForSecondsRealtime(0.2f);

        yield return StartCoroutine(CountScore(_currentScoreText, current, "Score: "));

        yield return new WaitForSecondsRealtime(0.2f);

        timer = 0;
        _buttonResultGroup.alpha = 0;
        _buttonResultGroup.blocksRaycasts = false;
        _buttonResultGroup.interactable = false;

        while (timer < _fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            _buttonResultGroup.alpha = timer / _fadeDuration;
            yield return null;
        }

        _buttonResultGroup.alpha = 1;
        _buttonResultGroup.blocksRaycasts = true;
        _buttonResultGroup.interactable = true;
    }

    private IEnumerator CountScore(TextMeshProUGUI text, int target, string titleText = "Score: ")
    {
        float duration = 0.8f;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;

            int value = Mathf.RoundToInt(Mathf.Lerp(0, target, t));
            text.text = titleText + value.ToString("N0") + " <sprite=1>";

            yield return null;
        }

        text.text = titleText + target.ToString("N0") + " <sprite=1>";
    }
    #endregion
}
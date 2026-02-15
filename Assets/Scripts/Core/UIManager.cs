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
        if (!_shopBtn || !_settingBtn || !_homeBtn || !_soundBtn || !_closeSettingUIBtn)
        {
            Debug.LogError("Button null!");
            return;
        }

        if (!_scoreText)
        {
            Debug.LogError("Text null!");
            return;
        }

        if (!_settingUI)
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
        _homeBtn.onClick.AddListener(() =>
        {
            GameController.Instance.PauseGame(false);
            SceneManager.LoadScene("MainScene");
        });
        _closeSettingUIBtn.onClick.AddListener(() =>
        {
            GameController.Instance.PauseGame(false);
            _settingUI.SetActive(false);
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
}
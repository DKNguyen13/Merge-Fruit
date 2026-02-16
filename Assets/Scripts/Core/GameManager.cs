using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;
    }

    #region Data
    public void SaveData(int score)
    {
        int currentHighScore = GetHighScore();

        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(Constants.HIGH_SCORE_KEY, score);
            PlayerPrefs.Save();
        }
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(Constants.HIGH_SCORE_KEY, 0);
    }
    #endregion
}
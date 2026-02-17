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

    #region Score Data
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

    #region Item Data
    public void AddItem(int amount)
    {
        int current = GetQuantityItem();
        current += amount;

        PlayerPrefs.SetInt(Constants.PORTAL_ITEM_KEY, current);
        PlayerPrefs.Save();
    }

    public bool UseItem()
    {
        int current = GetQuantityItem();
        if (current <= 0) return false;

        current--;

        PlayerPrefs.SetInt(Constants.PORTAL_ITEM_KEY, current);
        PlayerPrefs.Save();

        return true;
    }

    public int GetQuantityItem()
    {
        return PlayerPrefs.GetInt(Constants.PORTAL_ITEM_KEY, 0);
    }
    #endregion
}
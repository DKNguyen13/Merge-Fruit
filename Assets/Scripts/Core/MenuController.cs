using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button _playGameBtn;
    void Awake()
    {
    }

    void Start()
    {
        _playGameBtn.onClick.AddListener(() => SceneManager.LoadScene("GameplayScene"));
    }
}
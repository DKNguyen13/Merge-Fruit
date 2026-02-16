using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera shake infor")]
    [SerializeField] private float _strength = 1f;
    [SerializeField] private float _duration = 0.2f;
    private Vector3 _originalPos;
    private Tween _shakeTween;

    private void Awake()
    {
        _originalPos = transform.localPosition;
    }

    private void OnEnable()
    {
        GameController.OnFruitMerged += HandleMerge;
    }

    #region Handle merge
    private void HandleMerge(FruitType type)
    {
        int level = (int)type;
        if (level < 4) return;
        
        Shake(_duration, _strength);
    }
    #endregion

    #region Shake
    public void Shake(float duration, float strength)
    {
        _shakeTween?.Kill();

        transform.localPosition = _originalPos;

        _shakeTween = transform
            .DOShakePosition(duration, strength, 15, 90, false, true)
            .OnComplete(() =>
            {
                transform.localPosition = _originalPos;
            });
    }
    #endregion

    private void OnDisable()
    {
        GameController.OnFruitMerged -= HandleMerge;
    }
}
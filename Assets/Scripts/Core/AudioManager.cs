using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SoundData
{
    public SoundType type;
    public AudioSource src;
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Header("Bgm")]
    [SerializeField] private AudioClip[] _bgmClips;
    private int _currentIndex = -1;

    [Header("Vfx")]
    [SerializeField] private SoundData[] _sounds;

    private AudioSource _auSrc;
    private Dictionary<SoundType, AudioSource> _soundDict;

    void Awake()
    {
        Setup();
    }

    void Update()
    {
        if (GameController.Instance.IsGameOver) return;

        if (!_auSrc.isPlaying && GameController.Instance.IsPLaySound)
        {
            PlayRandomBgm();
        }
        else if (_auSrc.isPlaying && !GameController.Instance.IsPLaySound)
        {
            _auSrc.Stop();
        }
    }

    #region Setup
    private void Setup()
    {
        _auSrc = GetComponent<AudioSource>();
        _soundDict = new Dictionary<SoundType, AudioSource>();

        foreach (var sound in _sounds)
        {
            if (sound.src == null) continue;

            if (!_soundDict.ContainsKey(sound.type))
            {
                _soundDict.Add(sound.type, sound.src);
            }
        }
    }
    #endregion

    #region BGM
    private void PlayRandomBgm()
    {
        if (_bgmClips == null || _bgmClips.Length == 0)
        {
            Debug.LogWarning("No BGM clips assigned!");
            return;
        }

        int newIndex;

        do
        {
            newIndex = Random.Range(0, _bgmClips.Length);
        }
        while (_bgmClips.Length > 1 && newIndex == _currentIndex);

        _currentIndex = newIndex;

        _auSrc.clip = _bgmClips[_currentIndex];
        _auSrc.Play();

    }

    public void StopBgm()
    {
        if (_auSrc.isPlaying)
        {
            _auSrc.Stop();
        }
    }
    #endregion

    #region SFX
    public void PlaySfx(SoundType type)
    {
        if (GameController.Instance.IsPLaySound == false) return;
        if (!_soundDict.TryGetValue(type, out var source)) return;

        source.pitch = Random.Range(0.93f, 1.07f);
        source.PlayOneShot(source.clip);
        source.pitch = 1f;
    }

    public void PlayBubbleCounterSfx()
    {
        if (!_soundDict.TryGetValue(SoundType.BubbleCounter, out var source)) return;
        StopBubbleCounterSfx();
        source.Play();
    }

    public void StopBubbleCounterSfx()
    {
        if (!_soundDict.TryGetValue(SoundType.BubbleCounter, out var source)) return;
        source.Stop();
    }
    #endregion
}
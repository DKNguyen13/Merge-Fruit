using System.Collections.Generic;
using UnityEngine;

public class VFXPooling : MonoBehaviour
{
    public static VFXPooling Instance { get; private set; }

    [SerializeField] private GameObject _mergeVFXPrefab;
    [SerializeField] private int _poolSize = 10;

    private Queue<GameObject> _pool = new Queue<GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject vfx = Instantiate(_mergeVFXPrefab, transform);
            vfx.SetActive(false);
            _pool.Enqueue(vfx);
        }
    }

    public void PlayVFX(Vector3 position)
    {
        GameObject vfx = _pool.Count > 0 ? _pool.Dequeue() : Instantiate(_mergeVFXPrefab);

        vfx.transform.position = position;
        vfx.SetActive(true);

        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        ps.Play();

        StartCoroutine(ReturnToPool(ps, vfx));
    }

    private System.Collections.IEnumerator ReturnToPool(ParticleSystem ps, GameObject vfx)
    {
        yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);

        vfx.SetActive(false);
        _pool.Enqueue(vfx);
    }
}
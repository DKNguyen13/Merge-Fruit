using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FruitPool
{
    public FruitType type;
    public GameObject fruitPrefab;
    public int size = 10;
}

public class FruitPooling : MonoBehaviour
{
    public static FruitPooling Instance { get; private set; }

    [SerializeField] private List<FruitPool> _fruits = new();
    private Dictionary<FruitType, Queue<GameObject>> _fruitDict = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    
        Setup();
    }

    #region Setup data
    private void Setup()
    {
        if (_fruits.Count == 0)
        {
            Debug.Log("List fruit null!");
            return;
        }

        foreach (var item in _fruits)
        {
            Queue<GameObject> queue = new();

            GameObject parent = new GameObject(item.type.ToString());
            parent.transform.SetParent(transform);

            for (int i = 0; i < item.size; i++)
            {
                GameObject obj = Instantiate(item.fruitPrefab, parent.transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            _fruitDict.Add(item.type, queue);
        }
    }
    #endregion

    #region Get/Return fruit
    public GameObject GetFruitFromPool(FruitType type, Vector3 position)
    {
        if (!_fruitDict.TryGetValue(type, out var queue))  return null;
        if (queue.Count == 0) return null;
        
        GameObject obj = queue.Dequeue();
        obj.GetComponent<Fruit>().Rb.bodyType = RigidbodyType2D.Kinematic;
        obj.transform.position = position;
        obj.SetActive(true);

        return obj;
    }

    public void ReturnFruitFromPool(GameObject obj)
    {
        var fruit = obj.GetComponent<Fruit>();
        
        if (!fruit)
        {
            Destroy(obj);
            return;
        }

        if (!_fruitDict.TryGetValue(fruit.Type, out var queue))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        queue.Enqueue(obj);
    }
    #endregion
}
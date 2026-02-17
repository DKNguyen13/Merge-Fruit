using UnityEngine;
using DG.Tweening;

public class PortalEffect : MonoBehaviour
{
    private Animator _anim;

    [SerializeField] private float _suckDuration = 0.5f;

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        _anim.Rebind();
        _anim.Update(0f);
        _anim.Play(0, 0, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Fruit fruit = other.GetComponent<Fruit>();

        if (fruit != null)
        {
            AbsorbFruit(fruit);
        }
    }

    private void AbsorbFruit(Fruit fruit)
    {
        Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
        Transform tf = fruit.transform;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        tf.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Join(
            tf.DOMove(transform.position, _suckDuration)
            .SetEase(Ease.InQuad)
        );

        seq.Join(
            tf.DOScale(Vector3.zero, _suckDuration)
            .SetEase(Ease.InBack)
        );

        seq.OnComplete(() =>
        {
            ResetFruitBeforePool(fruit);
            FruitPooling.Instance.ReturnFruitFromPool(fruit.gameObject);
        });
    }
    
    private void ResetFruitBeforePool(Fruit fruit)
    {
        Transform tf = fruit.transform;
        Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();

        tf.DOKill();

        fruit.ResetScale();
        tf.rotation = Quaternion.identity;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void ReturnPortalToPool()
    {
        gameObject.SetActive(false);
    }
}
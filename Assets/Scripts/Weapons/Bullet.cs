using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private float speed = 10;
    private float damage;
    private string targetTag;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void Move(Transform target)
    {
        Debug.Log($"Bullet is moving toward {target.name} to do {damage} damage!");
    }

    public void SetBullet(float _damage, string _targetTag, float _speed = 10)
    {
        damage = _damage;
        targetTag = _targetTag;
        speed = _speed;
    }

    float GetDamage()
    {
        return damage;
    }

    void Damage(IDamageable damageable)
    {
        if (damageable != null)
        {
            damageable.GetDamage(damage);

            if (targetTag == "Enemy")
            {
               AddScore(); 
            }
            Destroy(gameObject);
        }
    }

    void AddScore()
    {
        GameManager.GetInstance().GetScoreManager().IncrementScore();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(targetTag) )
        {
            IDamageable damageable = collider.GetComponent<IDamageable>();
            Damage(damageable);
        }
        if (collider.gameObject.CompareTag("Shield"))
        {
            Shield shield = collider.GetComponent<Shield>();

            if (shield != null)
            {
                if (shield.GetParentTag() == targetTag)
                {
                    shield.GetDamage(damage);
                    Destroy(gameObject);
                }
            }
        }
    }
}

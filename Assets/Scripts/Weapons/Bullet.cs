using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private float speed = 10;
    private float damage;
    private string targetTag;
    private Camera _camera;

    private void Update()
    {
        _camera = Camera.main;
        Move();
        DestroyWhenOffScreen();
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

    private void DestroyWhenOffScreen()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        if (screenPosition.x < 0 
        || screenPosition.x > _camera.pixelWidth
        || screenPosition.y < 0 
        || screenPosition.y > _camera.pixelHeight)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(targetTag) || collider.gameObject.CompareTag("Pickup"))
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

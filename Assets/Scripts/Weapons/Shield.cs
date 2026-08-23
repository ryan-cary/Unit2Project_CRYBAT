using UnityEngine;

public class Shield : MonoBehaviour, IDamageable
{
    [SerializeField] private Color damageColor;

    private SpriteRenderer spriteRenderer;
    private ShieldBehavior shieldBehavior;
    private Health health;
    private Color startColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            startColor = spriteRenderer.color;
        }
    }

    public void GetDamage(float damage)
    {
        if (health != null)
        {
            health.DeductHealth(damage);
            UpdateColor();

            if (health.GetHealth() <= 0)
        {
            if (shieldBehavior != null)
            {
                shieldBehavior.ReorderShieldList(this);
            }
            Destroy(gameObject);
        }
        }
    }

    public void SetShieldBehavior(ShieldBehavior _shieldBehavior)
    {
        shieldBehavior = _shieldBehavior;
        health = new Health(_shieldBehavior.GetShieldHealth());

    }

    public string GetParentTag()
    {
        return shieldBehavior.GetPlayableObject().gameObject.tag;
    }

    private void UpdateColor()
    {
        spriteRenderer.color = Color.Lerp(damageColor, startColor, health.GetHealth() / health.GetMaxHealth());
    }
}

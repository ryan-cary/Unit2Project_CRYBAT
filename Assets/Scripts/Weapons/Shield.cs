using UnityEngine;

public class Shield : MonoBehaviour, IDamageable
{
    private ShieldBehavior shieldBehavior;
    private Health health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = new Health(10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetDamage(float damage)
    {
        health.DeductHealth(damage);

        if (health.GetHealth() <= 0)
        {
            // TODO: reset shield list in behavior class
            Destroy(gameObject);
        }
    }

    public void SetShieldBehavior(ShieldBehavior _shieldBehavior)
    {
        shieldBehavior = _shieldBehavior;
    }

    public string GetParentTag()
    {
        return shieldBehavior.GetPlayableObject().gameObject.tag;
    }
}

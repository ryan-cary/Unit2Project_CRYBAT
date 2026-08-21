using UnityEngine;

public class BigEnemy : Enemy
{
    [SerializeField] private float moveRange;
    [SerializeField] private float maxHealth;
    [SerializeField] private float healthRegenRate;

    new private void Start()
    {
        base.Start();
        health = new Health(maxHealth, healthRegenRate);
    }

    protected override void Update()
    {
        if (target == null)
        {
            return;
        }
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            StartAttack();
        }
        if (distance <= moveRange)
        {
            speed = 0;
        } 
        else
        {
            speed = targetSpeed;
        }
        if (isAttacking)
        {
            Attack();
        }
        Move(target.position);
    }

    protected override void Attack()
    {
        if (timer <= attackRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            Shoot();
        }
    }

    public override void Shoot()
    {
        Debug.Log("Big Enemy Shoots");
    }
}

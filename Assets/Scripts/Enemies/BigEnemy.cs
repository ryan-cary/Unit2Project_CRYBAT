using UnityEngine;

public class BigEnemy : Enemy
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float moveRange;
    [SerializeField] private float maxHealth;
    [SerializeField] private float healthRegenRate;
    [SerializeField] private float bulletsPerShot;
    private float shootAngle;

    new private void Start()
    {
        base.Start();
		this.defeatScore = 50;
        health = new Health(maxHealth, healthRegenRate);
        weapon = new Weapon("Machine Gun", attackDamage, bulletSpeed);
        shootAngle = 360f / bulletsPerShot;
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
        for (int i = 0; i < bulletsPerShot; i++)
        {
            weapon.Shoot(bulletPrefab, this, "Player", i * shootAngle);
        }
    }
}

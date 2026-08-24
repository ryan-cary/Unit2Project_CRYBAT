using UnityEngine;

public class MachineGunEnemy : Enemy
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootingDuration;
    [SerializeField] private float shootingCooldown;
    [SerializeField] private float turnSpeed;

    private bool isInCooldown = true;

    new private void Start()
    {
        base.Start();
        weapon = new Weapon("Machine Gun", attackDamage, bulletSpeed);
    }

    protected override void Attack()
    {
        if (isInCooldown)
        {
            if (timer <= shootingCooldown)
            {
                timer += Time.deltaTime;
            } 
            else
            {
                timer = 0;
                isInCooldown = false;
            }
        }
        else
        {
            if (timer <= shootingDuration)
            {
                if (timer % attackRate > (timer + Time.deltaTime) % attackRate)
                {
                    Shoot();
                }
                timer += Time.deltaTime;
            } 
            else
            {
                timer = 0;
                isInCooldown = true;
            }
        }
    }

    public override void Shoot()
    {
        weapon.Shoot(bulletPrefab, this, "Player");
    }

    public override void Move(Vector2 direction)
    {
        direction.x -= transform.position.x;
        direction.y -= transform.position.y;

        float angle = Vector2.SignedAngle(transform.up, direction);
        rb.angularVelocity = angle * turnSpeed;
        rb.linearVelocity = direction.normalized * speed;
    }

}

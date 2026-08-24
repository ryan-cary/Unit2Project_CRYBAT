using UnityEngine;

public class ShooterEnemy : Enemy
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float bulletSpeed;

    private LineRenderer lineRenderer;

    new private void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
        weapon = new Weapon("Shooter", attackDamage, bulletSpeed);
    }

    protected override void Update()
    {
        if (target == null)
        {
            return;
        }
        base.Update();
        StretchLaserToTarget();
    }

    private void StretchLaserToTarget()
    {
        if (lineRenderer.enabled)
        {
            float lineRendererZPos = lineRenderer.GetPosition(0).z;
            lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, lineRendererZPos));
            lineRenderer.SetPosition(1, new Vector3(target.transform.position.x, target.transform.position.y, lineRendererZPos));
        }
    }

    protected override void StartAttack()
    {
        base.StartAttack();
        lineRenderer.enabled = true;
    }

    protected override void StopAttack()
    {
        base.StopAttack();
        lineRenderer.enabled = false;
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
        weapon.Shoot(bulletPrefab, this, "Player");
    }
}

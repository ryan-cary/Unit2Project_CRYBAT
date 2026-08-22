using UnityEngine;
using System;

public class Player : PlayableObject
{
    // Attack variables
    [SerializeField] private float weaponDamage = 10;
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private Bullet bulletPrefab;

    private Camera camera;

    // Pickup behavior references
    [SerializeField] GunPowerUpBehavior gunPowerUpBehavior;

    // Nuke Pickup variables
    [SerializeField] private GameObject nukeBlastPrefab;
    [SerializeField] private int maxNumOfNukes;
    private int numOfNukes = 0;

    public Action OnCollectNuke;
    public Action OnUseNuke;

    public override void Awake()
    {
        base.Awake();
        health.SetRegenRate(0.5f);
        weapon = new Weapon("Player Weapon", weaponDamage, bulletSpeed);
        camera = Camera.main;
    }

    void Update()
    {
        health.RegenHealth();
    }

    public void Move(Vector3 direction, Vector2 target)
    {
        rb.linearVelocity = direction * speed;

        Vector3 playerScreenPos = camera.WorldToScreenPoint(transform.position);

        target.x -= playerScreenPos.x;
        target.y -= playerScreenPos.y;

        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public override void Shoot()
    {
        weapon.Shoot(bulletPrefab, this, "Enemy");
    }

    public override void GetDamage(float damage)
    {
        health.DeductHealth(damage);

        if (health.GetHealth() <= 0)
        {
            Defeated();
        }
    }

    public override void Defeated()
    {
        base.Defeated();
        Destroy(gameObject);
    }

    public GunPowerUpBehavior GetGunPowerUpBehavior()
    {
        return this.gunPowerUpBehavior;
    }

    public void CollectGunPowerUp(float duration, float shootRate)
    {
        gunPowerUpBehavior.PowerUpWeapon(duration, shootRate);
    }

    public void CollectNukePickup()
    {
        if (numOfNukes < maxNumOfNukes)
        {
            numOfNukes++;
            OnCollectNuke.Invoke();
        }
    }

    public void UseNukePickup()
    {
        if (numOfNukes > 0)
        {
            numOfNukes--;
            OnUseNuke.Invoke();
            Vector3 blastPosition = new Vector3(transform.position.x, transform.position.y, nukeBlastPrefab.transform.position.z);
            Instantiate(nukeBlastPrefab, blastPosition, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pickup"))
        {
            Pickup pickup = collision.gameObject.GetComponent<Pickup>();
            pickup.OnPicked();
        }
    }
}

using UnityEngine;
using System;

public class Player : PlayableObject, IDifficultyOverridden
{
    [SerializeField] private float baseHealth = 100f;
	
	[Header("Weapon Variables")]
    [SerializeField] private float weaponDamage = 10;
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private Bullet bulletPrefab;

    private Camera camera;

    [Header("Pickup Behaviour References")]
    [SerializeField] HealthPickupBehavior healthPickupBehavior;
    [SerializeField] GunPowerUpBehavior gunPowerUpBehavior;
    [SerializeField] NukeBehavior nukeBehavior;
    [SerializeField] ShieldBehavior shieldBehavior;

    public override void Awake()
    {
        base.Awake();
		health = new Health(baseHealth);
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

    public bool HasGunPowerUp()
    {
        return gunPowerUpBehavior.HasGunPowerUp();
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
	
	// ====== Pickups ====== //

    public HealthPickupBehavior GetHealthPickupBehavior()
    {
        return healthPickupBehavior;
    }

    public GunPowerUpBehavior GetGunPowerUpBehavior()
    {
        return gunPowerUpBehavior;
    }

    public NukeBehavior GetNukeBehavior()
    {
        return nukeBehavior;
    }

    public ShieldBehavior GetShieldBehavior()
    {
        return shieldBehavior;
    }

    public void CollectHealthPickup(Pickup pickup)
    {
        healthPickupBehavior.Collect(pickup);
    }

    public void CollectGunPowerUp(Pickup pickup)
    {
        gunPowerUpBehavior.Collect(pickup);
    }

    public void CollectNuke(Pickup pickup)
    {
        nukeBehavior.Collect(pickup);
    }

    public void CollectShield(Pickup pickup)
    {
        shieldBehavior.Collect(pickup);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pickup"))
        {
            Pickup pickup = collision.gameObject.GetComponent<Pickup>();
            pickup.OnPicked();
        }
    }
	
	// ====== Difficulty ====== //
	public void DifficultyOverride(DifficultySetting difficulty)
	{
		this.weapon = new Weapon("Player Weapon", weaponDamage * (1 / difficulty.baseDifficultyModifier), bulletSpeed);
		this.health = new Health(baseHealth * (1 / difficulty.baseDifficultyModifier));
	}

}

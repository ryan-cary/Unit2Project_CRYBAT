using UnityEngine;
using System.Collections.Generic;

public class Player : PlayableObject
{
    // Attack variables
    [SerializeField] private float weaponDamage = 10;
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private Bullet bulletPrefab;

    private Camera camera;

    // Pickup behavior references
    [SerializeField] PowerUpBehavior[] powerUpBehaviors;

    private Dictionary<PickupType, PowerUpBehavior> pickupTypeToBehavior = new Dictionary<PickupType, PowerUpBehavior>();

    public override void Awake()
    {
        base.Awake();
        health.SetRegenRate(0.5f);
        weapon = new Weapon("Player Weapon", weaponDamage, bulletSpeed);
        camera = Camera.main;
        SetPickupDictionary();
    }

    private void SetPickupDictionary()
    {
        foreach (PowerUpBehavior powerUpBehavior in powerUpBehaviors)
        {
            pickupTypeToBehavior[powerUpBehavior.GetPickupType()] = powerUpBehavior;
        }
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
        GunPowerUpBehavior gunPowerUpBehavior = GetGunPowerUpBehavior();

        if (gunPowerUpBehavior != null)
        {
            return gunPowerUpBehavior.HasGunPowerUp();
        }
        return false;
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

    public PowerUpBehavior GetPowerUpBehavior(PickupType pickupType)
    {
        if (pickupTypeToBehavior.ContainsKey(pickupType))
        {
            return pickupTypeToBehavior[pickupType];
        }
        return null;
    }

    public GunPowerUpBehavior GetGunPowerUpBehavior()
    {
        PowerUpBehavior powerUpBehavior = GetPowerUpBehavior(PickupType.GunPowerUp);

        if (powerUpBehavior is GunPowerUpBehavior gunPowerUpBehavior)
        {
            return gunPowerUpBehavior;
        }
        return null;
    }

    public NukeBehavior GetNukeBehavior()
    {
        PowerUpBehavior powerUpBehavior = GetPowerUpBehavior(PickupType.Nuke);

        if (powerUpBehavior is NukeBehavior nukeBehavior)
        {
            return nukeBehavior;
        }
        return null;
    }

    public void CollectPickup(Pickup pickup)
    {
        GetPowerUpBehavior(pickup.GetPickupType())?.Collect(pickup);
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

using UnityEngine;

public class Player : PlayableObject, IDifficultyOverridden
{
    [SerializeField] private float baseHealth = 100f;
	
	[Header("Weapon Variables")]
    [SerializeField] private float weaponDamage = 10;
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private Bullet bulletPrefab;

    [SerializeField] private PickupBehaviorController pickupBehaviorController;

    private Camera camera;

    public override void Awake()
    {
        base.Awake();
		health = new Health(baseHealth);
        health.SetRegenRate(0.5f);
        weapon = new Weapon("Player Weapon", weaponDamage, bulletSpeed);
        camera = Camera.main;
        SetSpriteDrawOrder();
    }

    void SetSpriteDrawOrder()
    {
        foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>(true))
        {
            spriteRenderer.sortingOrder = 5;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pickup"))
        {
            Pickup pickup = collision.gameObject.GetComponent<Pickup>();
            pickup.OnPicked();
        }
    }

    public PickupBehaviorController GetPickupBehaviorController()
    {
        return pickupBehaviorController;
    }
	
	// ====== Difficulty ====== //
	public void DifficultyOverride(DifficultySetting difficulty)
	{
		this.weapon = new Weapon("Player Weapon", weaponDamage * (1 / difficulty.baseDifficultyModifier), bulletSpeed);
		this.health = new Health(baseHealth * (1 / difficulty.baseDifficultyModifier));
	}
}

using UnityEngine;
using System;
using System.Collections;

public class Player : PlayableObject
{
    // Attack variables
    [SerializeField] private float weaponDamage = 10;
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private Bullet bulletPrefab;

    [SerializeField] private GameObject powerUpSymbol;

    private Camera camera;

    // GunPowerUp variables
    private bool hasGunPowerUp = false;
    private float powerUpTimer = 0;
    private float powerUpShootRate;
    private float maxPowerUpTime;

    private Coroutine shootPowerUpCoroutine;

    public Action<bool, float, Vector2> OnPowerUpTimerChange;

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
        AdvancePowerUpTimer();
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

    public void StartShootCoroutine()
    {
        if (shootPowerUpCoroutine != null)
        {
            StopCoroutine(shootPowerUpCoroutine);
        }
        shootPowerUpCoroutine = StartCoroutine(ShootPowerUpCorountine());
    }

    public void StopShootCoroutine()
    {
        if (shootPowerUpCoroutine != null)
        {
            StopCoroutine(shootPowerUpCoroutine);
            shootPowerUpCoroutine = null;
        }
    }

    public IEnumerator ShootPowerUpCorountine()
    {
        while (hasGunPowerUp)
        {
            yield return new WaitForSeconds(powerUpShootRate);
            Shoot();
        }
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

    void AdvancePowerUpTimer()
    {
        if (hasGunPowerUp)
        {
            powerUpTimer += Time.deltaTime;
            OnPowerUpTimerChange.Invoke(true, Mathf.Max(maxPowerUpTime - powerUpTimer, 0), camera.WorldToScreenPoint(transform.position));

            if (powerUpTimer >= maxPowerUpTime)
            {
                PowerDownWeapon();
            }
        }
    }

    public void PowerUpWeapon(float duration, float shootRate)
    {
        hasGunPowerUp = true;
        powerUpTimer = 0;
        maxPowerUpTime = duration;
        powerUpShootRate = shootRate;
        powerUpSymbol.SetActive(true);
        OnPowerUpTimerChange.Invoke(true, 0, camera.WorldToScreenPoint(transform.position));
    }

    public void PowerDownWeapon()
    {
        hasGunPowerUp = false;
        powerUpTimer = 0;
        powerUpSymbol.SetActive(false);
        OnPowerUpTimerChange.Invoke(false, 0, Vector2.zero);
        StopShootCoroutine();
    }

    public bool HasGunPowerUp()
    {
        return hasGunPowerUp;
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

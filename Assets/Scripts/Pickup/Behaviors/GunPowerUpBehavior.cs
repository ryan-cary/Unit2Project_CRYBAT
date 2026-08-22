using UnityEngine;
using System.Collections;
using System;

public class GunPowerUpBehavior : MonoBehaviour
{
    [SerializeField] PlayableObject playableObject;
    [SerializeField] private GameObject powerUpSymbol;
    private bool hasGunPowerUp = false;
    private float timer = 0;
    private float shootRate;
    private float duration;
    private Camera camera;

    private Coroutine shootPowerUpCoroutine;

    public Action<bool, float, Vector2> OnPowerUpTimerChange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        AdvancePowerUpTimer();
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
            yield return new WaitForSeconds(shootRate);
            playableObject.Shoot();
        }
    }

    void AdvancePowerUpTimer()
    {
        if (hasGunPowerUp)
        {
            timer += Time.deltaTime;
            OnPowerUpTimerChange.Invoke(true, Mathf.Max(duration - timer, 0), camera.WorldToScreenPoint(playableObject.transform.position));

            if (timer >= duration)
            {
                PowerDownWeapon();
            }
        }
    }

    public void PowerUpWeapon(float _duration, float _shootRate)
    {
        hasGunPowerUp = true;
        timer = 0;
        duration = _duration;
        shootRate = _shootRate;
        powerUpSymbol.SetActive(true);
        OnPowerUpTimerChange.Invoke(true, 0, camera.WorldToScreenPoint(playableObject.transform.position));
    }

    public void PowerDownWeapon()
    {
        hasGunPowerUp = false;
        timer = 0;
        powerUpSymbol.SetActive(false);
        OnPowerUpTimerChange.Invoke(false, 0, Vector2.zero);
        StopShootCoroutine();
    }

    public bool HasGunPowerUp()
    {
        return hasGunPowerUp;
    }
}

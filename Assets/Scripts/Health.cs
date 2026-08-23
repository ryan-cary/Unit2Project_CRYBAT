using UnityEngine;
using System;

public class Health
{
    private float maxHealth;
    private float healthRegenRate;
    private float currentHealth;

    public Action<float> OnHealthUpdate;

    public Health(
        float _maxHealth , 
        float _healthRegenRate, 
        float _currentHealth = 100)
    {
        maxHealth = _maxHealth;
        healthRegenRate = _healthRegenRate;
        currentHealth = _currentHealth;
        OnHealthUpdate?.Invoke(currentHealth);
    }

    public Health(float _maxHealth, float _healthRegenRate)
    {
        maxHealth = _maxHealth;
        healthRegenRate = _healthRegenRate;
        currentHealth = _maxHealth;
        OnHealthUpdate?.Invoke(currentHealth);
    }

    public Health(float _maxHealth)
    {
        maxHealth = _maxHealth;
        currentHealth = _maxHealth;
        OnHealthUpdate?.Invoke(currentHealth);
    }

    public Health()
    {
        
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void SetRegenRate(float _healthRegenRate)
    {
        this.healthRegenRate = _healthRegenRate;
    }

    public void SetHealth(float health)
    {
        if (health < 0 || health > maxHealth)
        {
            throw new ArgumentOutOfRangeException("health", $"Tried to set health to an invalid number: {health}");
        }
        currentHealth = health;
        OnHealthUpdate?.Invoke(currentHealth);
    }

    public void RegenHealth()
    {
        AddHealth(healthRegenRate * Time.deltaTime);
    }

    public void AddHealth(float value)
    {
        if (currentHealth > maxHealth)
        {
            return;
        }
        currentHealth = Mathf.Min(currentHealth + value, maxHealth);
        OnHealthUpdate?.Invoke(currentHealth);
    }

    public void DeductHealth(float value)
    {
        if (currentHealth <= 0)
        {
            return;
        }
        currentHealth = Mathf.Max(currentHealth - value, 0);
        OnHealthUpdate?.Invoke(currentHealth);
    }
}

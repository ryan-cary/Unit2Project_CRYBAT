using UnityEngine;
using System.Collections.Generic;
using System;

public class ShieldBehavior : PowerUpBehavior
{
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private int maxNumOfShields;
    [SerializeField] private float offsetRadius;
    [SerializeField] private float rotationSpeed;

    private bool hasShield;
    private int pickupNumOfShields;
    private List<Shield> shieldList = new List<Shield>();

    private float angle = 0;

    private void Update()
    {
        Rotate();
    }

    public override void Collect(Pickup pickup)
    {
        if (pickup is ShieldPickup shieldPickup)
        {
            pickupNumOfShields = shieldPickup.GetStartingNumOfShields();
            Use();
        }
    }

    public override void Use()
    {
        UpdateNumOfShields();
    }

    private void UpdateNumOfShields()
    {
        int newNumOfShields;

        if (hasShield)
        {
            newNumOfShields = Mathf.Min(Mathf.Max(shieldList.Count + 1, pickupNumOfShields), maxNumOfShields);
        }
        else
        {
            newNumOfShields = Mathf.Min(pickupNumOfShields, maxNumOfShields);
        }
        AddShields(newNumOfShields - shieldList.Count);
    }

    private void AddShields(int count)
    {
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Shield shield = Instantiate(shieldPrefab, transform).GetComponent<Shield>();
                if (shield != null)
                {
                    shield.SetShieldBehavior(this);
                    shieldList.Add(shield);
                } else
                {
                    throw new NullReferenceException("Instantiated shield object without shield script");
                }
            }
            RepositionShields();
            hasShield = true;
        }
    }

    private void RepositionShields()
    {
        float patternAngle = 360f / shieldList.Count * Mathf.Deg2Rad;
        for (int i = 0; i < shieldList.Count; i++)
        {
            float currAngle = angle * Mathf.Rad2Deg + i * patternAngle;
            Vector3 offset = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));
            offset *= offsetRadius;
            shieldList[i].gameObject.transform.position = playableObject.transform.position + offset;
        }
    }

    private void Rotate()
    {
        angle += rotationSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
    }

    public void ReorderShieldList(Shield shield)
    {
        if (shieldList.Contains(shield))
        {
            shieldList.Remove(shield);
            if (shieldList.Count == 0)
            {
                hasShield = false;
            }
        }
    }
}

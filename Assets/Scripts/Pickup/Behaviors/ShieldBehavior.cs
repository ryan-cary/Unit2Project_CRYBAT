using UnityEngine;
using System.Collections.Generic;
using System;

public class ShieldBehavior : PowerUpBehavior
{
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private int maxNumOfShields;
    [SerializeField] private float offsetRadius;
    private bool hasShield;
    private int pickupNumOfShields;
    private List<Shield> shieldList = new List<Shield>();

    private void Update()
    {
        transform.rotation = Quaternion.identity;
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
            newNumOfShields = Mathf.Min(Mathf.Max(shieldList.Count, pickupNumOfShields) + 1, maxNumOfShields);
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
        float angle = 360f / shieldList.Count * Mathf.Deg2Rad;
        for (int i = 0; i < shieldList.Count; i++)
        {
            Vector3 offset = new Vector2(Mathf.Cos(i * angle), Mathf.Sin(i * angle));
            offset *= offsetRadius;
            shieldList[i].gameObject.transform.position = playableObject.transform.position + offset;
        }
    }
}

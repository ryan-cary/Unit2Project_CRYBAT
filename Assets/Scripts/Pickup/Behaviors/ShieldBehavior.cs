using UnityEngine;
using System.Collections.Generic;

public class ShieldBehavior : PowerUpBehavior
{
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private int maxNumOfShields;
    private bool hasShield;
    private int pickupNumOfShields;
    private List<GameObject> shieldList = new List<GameObject>();

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
            Debug.Log($"Added shields! Current number: {shieldList.Count + count}");
            hasShield = true;
        }
    }
}

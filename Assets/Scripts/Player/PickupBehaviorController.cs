using UnityEngine;
using System.Collections.Generic;

public class PickupBehaviorController : MonoBehaviour
{
    // Pickup behavior references
    [SerializeField] PowerUpBehavior[] powerUpBehaviors;

    private Dictionary<PickupType, PowerUpBehavior> pickupTypeToBehavior = new Dictionary<PickupType, PowerUpBehavior>();

    private void Awake()
    {
        SetPickupDictionary();
    }

    private void SetPickupDictionary()
    {
        foreach (PowerUpBehavior powerUpBehavior in powerUpBehaviors)
        {
            pickupTypeToBehavior[powerUpBehavior.GetPickupType()] = powerUpBehavior;
        }
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

    public bool HasGunPowerUp()
    {
        GunPowerUpBehavior gunPowerUpBehavior = GetGunPowerUpBehavior();

        if (gunPowerUpBehavior != null)
        {
            return gunPowerUpBehavior.HasGunPowerUp();
        }
        return false;
    }
}

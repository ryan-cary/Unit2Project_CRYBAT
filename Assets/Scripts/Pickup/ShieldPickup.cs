using UnityEngine;

public class ShieldPickup : Pickup
{
    [SerializeField] private int startingNumOfShields;

    protected override PickupType pickupType => PickupType.Shield;

    public int GetStartingNumOfShields()
    {
        return startingNumOfShields;
    }
}

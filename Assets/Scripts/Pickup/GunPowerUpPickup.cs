using UnityEngine;

public class GunPowerUpPickup : Pickup
{

    [SerializeField] private float duration;
    [SerializeField] private float shootRate;

    protected override PickupType pickupType => PickupType.GunPowerUp;

    public float GetDuration()
    {
        return duration;
    }

    public float GetShootRate()
    {
        return shootRate;
    }
}

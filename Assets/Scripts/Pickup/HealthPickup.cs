using UnityEngine;

public class HealthPickup : Pickup
{

    [SerializeField] private float healthMin = 25;
    [SerializeField] private float healthMax = 50;

    protected override PickupType pickupType => PickupType.Health;

    public float GetHealthMin()
    {
        return healthMin;
    }

    public float GetHealthMax()
    {
        return healthMax;
    }
}

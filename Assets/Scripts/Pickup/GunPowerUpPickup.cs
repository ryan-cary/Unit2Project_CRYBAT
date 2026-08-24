using UnityEngine;

public class GunPowerUpPickup : Pickup
{

    [SerializeField] private float duration;
    [SerializeField] private float shootRate;

    public float GetDuration()
    {
        return duration;
    }

    public float GetShootRate()
    {
        return shootRate;
    }
}

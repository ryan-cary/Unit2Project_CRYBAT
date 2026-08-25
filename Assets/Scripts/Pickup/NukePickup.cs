using UnityEngine;

public class NukePickup : Pickup
{
    [SerializeField] private float minBlastDuration;
    [SerializeField] private float maxBlastDuration;
    [SerializeField] private float minBlastRadius;
    [SerializeField] private float maxBlastRadius;
    protected override PickupType pickupType => PickupType.Nuke;

    public float GenerateBlastDuration()
    {
        return Random.Range(minBlastDuration, maxBlastDuration + 1);
    }

    public float GenerateBlastRadius()
    {
        return Random.Range(minBlastRadius, maxBlastRadius + 1);
    }
}

using UnityEngine;

public class NukePickup : Pickup
{
    [SerializeField] private float blastDuration;
    [SerializeField] private float blastRadius;
    protected override PickupType pickupType => PickupType.Nuke;

    public float GetBlastDuration()
    {
        return blastDuration;
    }

    public float GetBlastRadius()
    {
        return blastRadius;
    }
}

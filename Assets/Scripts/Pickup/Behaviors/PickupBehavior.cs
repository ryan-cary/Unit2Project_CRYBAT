using UnityEngine;

public abstract class PowerUpBehavior : MonoBehaviour
{
    [SerializeField] protected PlayableObject playableObject;
    protected abstract PickupType pickupType { get; }

    public abstract void Collect(Pickup pickup);

    public abstract void Use();

    public PlayableObject GetPlayableObject()
    {
        return playableObject;
    }

    public PickupType GetPickupType()
    {
        return pickupType;
    }
}

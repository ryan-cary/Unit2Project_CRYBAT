using UnityEngine;

public abstract class Pickup : MonoBehaviour, IDamageable
{
    protected abstract PickupType pickupType { get; }

    public void OnPicked()
    {
        GameManager.GetInstance().GetPlayer().GetPickupBehaviorController().CollectPickup(this);
        Destroy(gameObject);
    }

    public void GetDamage(float damage)
    {
        Destroy(gameObject);
    }

    public PickupType GetPickupType()
    {
        return pickupType;
    }
}

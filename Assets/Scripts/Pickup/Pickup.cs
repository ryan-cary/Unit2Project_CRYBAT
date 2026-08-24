using UnityEngine;

public abstract class Pickup : MonoBehaviour, IDamageable
{
    [SerializeField] private PickupType pickupType;

    public void OnPicked()
    {
        GameManager.GetInstance().GetPlayer().CollectPickup(this);
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

using UnityEngine;

public class HealthPickupBehavior : PowerUpBehavior
{
    protected override PickupType pickupType => PickupType.Health;
    private float healAmount;

    public override void Collect(Pickup pickup)
    {
        if (pickup is HealthPickup healthPickup)
        {
            healAmount = Random.Range(healthPickup.GetHealthMin(), healthPickup.GetHealthMax());
            Use();
        }
    }

    public override void Use()
    {
        playableObject.health.AddHealth(healAmount);
    }
}

using UnityEngine;

public class GunPowerUpPickup : Pickup
{

    [SerializeField] private float duration;
    [SerializeField] private float shootRate;

    public override void OnPicked()
    {
        base.OnPicked();
        GameManager.GetInstance().GetPlayer().CollectGunPowerUp(this);
    }

    public float GetDuration()
    {
        return duration;
    }

    public float GetShootRate()
    {
        return shootRate;
    }
}

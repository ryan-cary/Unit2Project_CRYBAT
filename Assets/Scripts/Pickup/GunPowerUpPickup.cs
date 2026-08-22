using UnityEngine;

public class GunPowerUpPickup : Pickup
{

    [SerializeField] private float duration;
    [SerializeField] private float shootRate;

    public override void OnPicked()
    {
        base.OnPicked();
        GameManager.GetInstance().GetPlayer().CollectGunPowerUp(duration, shootRate);
    }
}

using UnityEngine;

public class NukePickup : Pickup
{
    public override void OnPicked()
    {
        base.OnPicked();
        GameManager.GetInstance().GetPlayer().CollectNuke(this);
    }
}

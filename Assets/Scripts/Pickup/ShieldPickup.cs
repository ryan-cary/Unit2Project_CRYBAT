using UnityEngine;

public class ShieldPickup : Pickup
{
    [SerializeField] private int startingNumOfShields;

    public override void OnPicked()
    {
        base.OnPicked();
        GameManager.GetInstance().GetPlayer().CollectShield(this);
    }

    public int GetStartingNumOfShields()
    {
        return startingNumOfShields;
    }
}

using UnityEngine;

public class HealthPickup : Pickup
{

    [SerializeField] private float healthMin = 25;
    [SerializeField] private float healthMax = 50;

    public override void OnPicked()
    {
        base.OnPicked();
        GameManager.GetInstance().GetPlayer().CollectHealthPickup(this);
    }

    public float GetHealthMin()
    {
        return healthMin;
    }

    public float GetHealthMax()
    {
        return healthMax;
    }
}

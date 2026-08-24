using UnityEngine;

public class HealthPickup : Pickup
{

    [SerializeField] private float healthMin = 25;
    [SerializeField] private float healthMax = 50;

    public float GetHealthMin()
    {
        return healthMin;
    }

    public float GetHealthMax()
    {
        return healthMax;
    }
}

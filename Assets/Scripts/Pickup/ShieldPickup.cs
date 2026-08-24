using UnityEngine;

public class ShieldPickup : Pickup
{
    [SerializeField] private int startingNumOfShields;

    public int GetStartingNumOfShields()
    {
        return startingNumOfShields;
    }
}

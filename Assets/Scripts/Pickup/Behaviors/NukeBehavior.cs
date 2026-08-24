using UnityEngine;
using System;

public class NukeBehavior : PowerUpBehavior
{
    // Nuke Pickup variables
    [SerializeField] private GameObject nukeBlastPrefab;
    [SerializeField] private int maxNumOfNukes;
    protected override PickupType pickupType => PickupType.Nuke;
    private int numOfNukes = 0;

    public Action OnCollectNuke;
    public Action OnUseNuke;

    public override void Collect(Pickup pickup)
    {
        if (numOfNukes < maxNumOfNukes)
        {
            numOfNukes++;
            OnCollectNuke.Invoke();
        }
    }

    public override void Use()
    {
        if (numOfNukes > 0)
        {
            numOfNukes--;
            OnUseNuke.Invoke();
            Vector3 blastPosition = new Vector3(transform.position.x, transform.position.y, nukeBlastPrefab.transform.position.z);
            Instantiate(nukeBlastPrefab, blastPosition, Quaternion.identity);
        }
    }
}

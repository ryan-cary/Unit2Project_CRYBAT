using UnityEngine;
using System;
using System.Collections.Generic;

public class NukeBehavior : PowerUpBehavior
{
    // Nuke Pickup variables
    [SerializeField] private GameObject nukeBlastPrefab;
    [SerializeField] private int maxNumOfNukes;
    protected override PickupType pickupType => PickupType.Nuke;
    private Stack<(float blastDuration, float blastRadius)> nukeParameterList = new Stack<(float blastDuration, float blastRadius)>();

    public Action OnCollectNuke;
    public Action OnUseNuke;

    public override void Collect(Pickup pickup)
    {
        if (pickup is NukePickup nukePickup)
        {
            if (nukeParameterList.Count < maxNumOfNukes)
            {
                AppendNukeParameters(nukePickup.GenerateBlastDuration(), nukePickup.GenerateBlastRadius());
                OnCollectNuke.Invoke();
            }
        }
    }

    public override void Use()
    {
        if (nukeParameterList.Count > 0)
        {
            Vector3 blastPosition = new Vector3(transform.position.x, transform.position.y, nukeBlastPrefab.transform.position.z);
            GameObject nukeBlastObject = Instantiate(nukeBlastPrefab, blastPosition, Quaternion.identity);
            NukeBlast nukeBlast = nukeBlastObject.GetComponent<NukeBlast>();

            if (nukeBlast != null)
            {
                (float blastDuration, float blastRadius) = nukeParameterList.Pop();
                nukeBlast.SetBlastParemeters(blastDuration, blastRadius);
                OnUseNuke.Invoke();
            } else
            {
                Destroy(nukeBlastObject);
            }
        }
    }

    private void AppendNukeParameters(float _blastDuration, float _blastRadius)
    {
        nukeParameterList.Push((_blastDuration, _blastRadius));
    }
}

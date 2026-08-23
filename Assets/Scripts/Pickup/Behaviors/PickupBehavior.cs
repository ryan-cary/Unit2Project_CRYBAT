using UnityEngine;

public abstract class PowerUpBehavior : MonoBehaviour
{
    [SerializeField] protected PlayableObject playableObject;

    public abstract void Collect(Pickup pickup);

    public abstract void Use();
}

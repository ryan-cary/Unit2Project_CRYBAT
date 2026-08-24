using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct PickupSpawn
{
    public Pickup pickup;
    public int spawnWeight;
}

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] private PickupSpawn[] pickups;

    [Range(0, 1)]
    [SerializeField] private float pickupProbability;

    List<Pickup> pickupPool = new List<Pickup>();
    Pickup chosenPickup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (PickupSpawn spawn in pickups)
        {
            for (int i = 0; i < spawn.spawnWeight; i++)
            {
                pickupPool.Add(spawn.pickup);
            }
        }

        chosenPickup = pickupPool[Random.Range(0, pickupPool.Count)];
    }

    public void SpawnPickup(Vector2 spawnPosition)
    {
        if (pickupPool.Count == 0)
        {
            return;
        }

        if (Random.Range(0f, 1f) < pickupProbability)
        {
            chosenPickup = pickupPool[Random.Range(0, pickupPool.Count)];
            Instantiate(chosenPickup, spawnPosition, Quaternion.identity);
        }
    }
}

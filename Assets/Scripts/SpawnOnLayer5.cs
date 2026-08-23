using UnityEngine;


// Super annoyiong issue where I couldn't get any of the prefabs to spawn on top of the background.
// This script ensures that prefabs with specific tags are pushed to layer 5
public class SpawnOnLayer5 : MonoBehaviour
{
    static readonly string[] Tags = { "Enemy", "Projectile", "Shield", "Player", "Pickup" };
    const int DrawOrder = 5;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (FindAnyObjectByType<SpawnOnLayer5>() != null)
        {
            return;
        }

        GameObject driver = new GameObject("SpawnOnLayer5");
        driver.AddComponent<SpawnOnLayer5>();
        DontDestroyOnLoad(driver);
    }

    void LateUpdate()
    {
        Apply();
    }

    void Apply()
    {
        for (int t = 0; t < Tags.Length; t++)
        {
            GameObject[] taggedObjects;
            try
            {
                taggedObjects = GameObject.FindGameObjectsWithTag(Tags[t]);
            }
            catch (UnityException)
            {
                continue;
            }

            for (int i = 0; i < taggedObjects.Length; i++)
            {
                Renderer[] renderers = taggedObjects[i].GetComponentsInChildren<Renderer>(true);
                for (int r = 0; r < renderers.Length; r++)
                {
                    renderers[r].sortingOrder = DrawOrder;
                }
            }
        }
    }
}

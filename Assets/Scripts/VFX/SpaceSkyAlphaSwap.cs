using UnityEngine;

public class SpaceSkyAlphaSwap : MonoBehaviour
{
    const string RootName = "_Spacesky";
    const string TopName = "SpaceCloudsTop";
    const string BottomName = "SpaceCloudsBottom";

    [SerializeField] float cycleDuration = 16f;

    SpriteRenderer topLayer;
    SpriteRenderer bottomLayer;
    float topAlpha;
    float bottomAlpha;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AttachToSpaceSky()
    {
        GameObject root = GameObject.Find(RootName);
        if (root == null || root.GetComponent<SpaceSkyAlphaSwap>() != null)
        {
            return;
        }

        root.AddComponent<SpaceSkyAlphaSwap>();
    }

    void Awake()
    {
        topLayer = FindLayer(TopName);
        bottomLayer = FindLayer(BottomName);

        if (topLayer != null)
        {
            topAlpha = topLayer.color.a;
        }

        if (bottomLayer != null)
        {
            bottomAlpha = bottomLayer.color.a;
        }
    }

    void LateUpdate()
    {
        if (topLayer == null || bottomLayer == null || cycleDuration < 0.01f)
        {
            return;
        }

        float wave = 0.5f + 0.5f * Mathf.Sin(Time.time * (Mathf.PI * 2f / cycleDuration));
        SetAlpha(topLayer, Mathf.Lerp(topAlpha, bottomAlpha, wave));
        SetAlpha(bottomLayer, Mathf.Lerp(bottomAlpha, topAlpha, wave));
    }

    SpriteRenderer FindLayer(string childName)
    {
        Transform child = transform.Find(childName);
        if (child != null)
        {
            return child.GetComponent<SpriteRenderer>();
        }

        GameObject found = GameObject.Find(childName);
        return found != null ? found.GetComponent<SpriteRenderer>() : null;
    }

    static void SetAlpha(SpriteRenderer renderer, float alpha)
    {
        Color color = renderer.color;
        color.a = alpha;
        renderer.color = color;
    }
}

using UnityEngine;

public class SpaceSkyAlphaSwap : MonoBehaviour
{
    [SerializeField] SpriteRenderer topLayer;
    [SerializeField] SpriteRenderer bottomLayer;
    [SerializeField] float cycleDuration = 16f;

    float topAlpha;
    float bottomAlpha;

    void Awake()
    {
        ResolveLayers();

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

    void ResolveLayers()
    {
        if (topLayer == null)
        {
            topLayer = FindChildLayer("SpaceSkyTop");
        }

        if (bottomLayer == null)
        {
            bottomLayer = FindChildLayer("SpaceSkyBottom");
        }

        if (topLayer != null && bottomLayer != null)
        {
            return;
        }

        SpriteRenderer[] layers = GetComponentsInChildren<SpriteRenderer>(true);
        if (layers.Length < 2)
        {
            return;
        }

        if (topLayer == null)
        {
            topLayer = layers[0];
        }

        if (bottomLayer == null)
        {
            bottomLayer = layers[1] == topLayer ? layers[0] : layers[1];
        }
    }

    SpriteRenderer FindChildLayer(string childName)
    {
        Transform child = transform.Find(childName);
        return child != null ? child.GetComponent<SpriteRenderer>() : null;
    }

    static void SetAlpha(SpriteRenderer renderer, float alpha)
    {
        Color color = renderer.color;
        color.a = alpha;
        renderer.color = color;
    }
}

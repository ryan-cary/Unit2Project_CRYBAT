using UnityEngine;
using UnityEngine.UI;

public class PulseGlow : MonoBehaviour
{
    public Image glowImage;
    public float pulseSpeed = 2f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 0.9f;

    void Update()
    {
        if (glowImage != null)
        {
            Color c = glowImage.color;
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
            glowImage.color = c;
        }
    }
}
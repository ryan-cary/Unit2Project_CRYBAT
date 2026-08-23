using UnityEngine;

public class NukePickupFlameEffect : MonoBehaviour
{
    [SerializeField] float pulseSpeed = 3f;
    [SerializeField] float pulseScale = 0.15f;
    [SerializeField] float colorSpeed = 2f;

    static readonly Color[] FireColors =
    {
        new Color(1f, 0.95f, 0.55f, 1f),
        new Color(1f, 0.7f, 0.15f, 1f),
        new Color(1f, 0.4f, 0.05f, 1f),
        new Color(0.85f, 0.12f, 0.02f, 1f),
        new Color(1f, 0.55f, 0.1f, 1f)
    };

    SpriteRenderer spriteRenderer;
    Vector3 baseScale;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
    }

    void LateUpdate()
    {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
        transform.localScale = new Vector3(baseScale.x * pulse, baseScale.y * pulse, baseScale.z);

        if (spriteRenderer == null)
        {
            return;
        }

        float colorTravel = Time.time * colorSpeed;
        int colorIndex = Mathf.FloorToInt(colorTravel) % FireColors.Length;
        int nextColorIndex = (colorIndex + 1) % FireColors.Length;
        float blend = colorTravel - Mathf.Floor(colorTravel);
        Color fireColor = Color.Lerp(FireColors[colorIndex], FireColors[nextColorIndex], blend);
        fireColor.a = spriteRenderer.color.a;
        spriteRenderer.color = fireColor;
    }
}

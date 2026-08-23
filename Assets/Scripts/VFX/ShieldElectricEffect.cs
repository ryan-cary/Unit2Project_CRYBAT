using UnityEngine;

public class ShieldElectricEffect : MonoBehaviour
{
    [SerializeField] float orbitRadius = 0.52f;
    [SerializeField] float emitRate = 14f;
    [SerializeField] float spinSpeed = 2.4f;

    void Awake()
    {
        if (GetComponentInChildren<ParticleSystem>() != null)
        {
            return;
        }

        GameObject host = new GameObject("ElectricSpin");
        host.transform.SetParent(transform, false);
        host.transform.localPosition = Vector3.zero;
        host.transform.localRotation = Quaternion.identity;
        host.transform.localScale = Vector3.one;

        ParticleSystem particles = host.AddComponent<ParticleSystem>();
        Configure(particles);
        particles.Play(true);
    }

    void Configure(ParticleSystem particles)
    {
        ParticleSystem.MainModule main = particles.main;
        main.playOnAwake = true;
        main.loop = true;
        main.duration = 1f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.18f, 0.32f);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.04f, 0.08f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.75f, 0.95f, 1f, 0.85f),
            new Color(0.35f, 0.75f, 1f, 0.7f));
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.scalingMode = ParticleSystemScalingMode.Local;
        main.maxParticles = 28;
        main.gravityModifier = 0f;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.enabled = true;
        emission.rateOverTime = emitRate;

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = orbitRadius;
        shape.radiusThickness = 0f;
        shape.arc = 360f;
        shape.rotation = new Vector3(-90f, 0f, 0f);

        ParticleSystem.VelocityOverLifetimeModule velocity = particles.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.orbitalZ = spinSpeed;
        velocity.radial = new ParticleSystem.MinMaxCurve(-0.05f, 0.08f);

        ParticleSystem.ColorOverLifetimeModule color = particles.colorOverLifetime;
        color.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(new Color(0.4f, 0.85f, 1f), 0.45f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.8f, 0.15f),
                new GradientAlphaKey(0.35f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            });
        color.color = gradient;

        ParticleSystem.SizeOverLifetimeModule size = particles.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 0.15f));

        ParticleSystem.NoiseModule noise = particles.noise;
        noise.enabled = true;
        noise.strength = 0.12f;
        noise.frequency = 3.5f;
        noise.scrollSpeed = 1.2f;
        noise.octaveCount = 1;
        noise.quality = ParticleSystemNoiseQuality.Medium;

        ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.velocityScale = 0.06f;
        renderer.lengthScale = 2.8f;
        renderer.alignment = ParticleSystemRenderSpace.View;
        renderer.sortingOrder = 6;

        SpriteRenderer shieldSprite = GetComponent<SpriteRenderer>();
        if (shieldSprite != null)
        {
            renderer.sharedMaterial = shieldSprite.sharedMaterial;
            renderer.sortingLayerID = shieldSprite.sortingLayerID;
        }
    }
}

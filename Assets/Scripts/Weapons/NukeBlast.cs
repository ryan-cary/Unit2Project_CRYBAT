using UnityEngine;

public class NukeBlast : MonoBehaviour
{
    private float timer;
    private SpriteRenderer sr;
    private Color startingColor;

    private float blastDuration;
    private float blastRadius;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startingColor = sr.color;
    }

    // Update is called once per frame
    void Update()
    {
        AdvanceTimer();
        UpdateTransparency();

        if (timer > blastDuration)
        {
            Destroy(gameObject);
        }
    }

    public void SetBlastParemeters(float _blastDuration, float _blastRadius)
    {
        blastDuration = _blastDuration;
        blastRadius = _blastRadius;
        transform.localScale = new Vector3(blastRadius, blastRadius, transform.localScale.z);
    }

    void AdvanceTimer()
    {
        timer += Time.deltaTime;
    }

    void UpdateTransparency()
    {
        Color newColor = startingColor;
        float alpha = startingColor.a * (1 - (timer / blastDuration));
        newColor.a = alpha;
        sr.color = newColor;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();
            enemy.Defeated();
        }
        if (collider.gameObject.CompareTag("Pickup"))
        {
            Pickup pickup = collider.gameObject.GetComponent<Pickup>();
            pickup.GetDamage(0);
        }
    }
}

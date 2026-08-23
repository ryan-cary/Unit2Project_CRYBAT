using UnityEngine;

public class EngineFlame : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        spriteRenderer.sortingOrder = 5;
    }

    void LateUpdate()
    {
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f;
        spriteRenderer.enabled = isMoving;
        spriteRenderer.sortingOrder = 5;
    }
}

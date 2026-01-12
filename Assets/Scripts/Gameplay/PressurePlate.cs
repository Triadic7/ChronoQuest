using System;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    /// <summary>
    /// Fires when the plate is pressed.
    /// </summary>
    public event Action<PressurePlate> OnPressed;

    private SpriteRenderer spriteRenderer;

    private Sprite sprite;

    private Sprite activatedSprite;

    private void Awake()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Highlights the plate by changing its color for the specified duration.
    /// </summary>
    /// <param name="duration">The amount of time, in seconds, to display the highlight before reverting to the original color. Must be greater
    /// than zero.</param>
    public void Highlight(float duration)
    {
        this.spriteRenderer.sprite = this.activatedSprite;
        Invoke(nameof(ResetColor), duration);
    }

    private void ResetColor()
    {
        this.spriteRenderer.sprite = this.sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPressed?.Invoke(this);
        }
    }

    public void SetSprites(Sprite sprite, Sprite activatedSprite)
    {
        this.sprite = sprite;
        this.activatedSprite = activatedSprite;
        // Set the sprite of the plate to the activated sprite.
        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
    }
}

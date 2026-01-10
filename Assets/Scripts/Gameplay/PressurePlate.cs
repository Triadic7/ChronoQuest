using System;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    /// <summary>
    /// Fires when the plate is pressed.
    /// </summary>
    public event Action<PressurePlate> OnPressed;

    [SerializeField]
    /// <summary>
    /// The renderer for the plate to change color.
    /// </summary>
    private Renderer plateRenderer;

    /// <summary>
    /// The color to show when highlighted.
    /// </summary>
    [SerializeField]
    private Color highlightColor = Color.red;

    /// <summary>
    /// The original color of the plate.
    /// </summary>
    private Color originalColor;

    private void Awake()
    {
        if (this.plateRenderer != null)
        {
            this.originalColor = this.plateRenderer.material.color;
        }
    }

    /// <summary>
    /// Highlights the plate by changing its color for the specified duration.
    /// </summary>
    /// <param name="duration">The amount of time, in seconds, to display the highlight before reverting to the original color. Must be greater
    /// than zero.</param>
    public void Highlight(float duration)
    {
        this.plateRenderer.material.color = this.highlightColor;
        Invoke(nameof(ResetColor), duration);
    }

    private void ResetColor()
    {
        this.plateRenderer.material.color = this.originalColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPressed?.Invoke(this);
        }
    }
}

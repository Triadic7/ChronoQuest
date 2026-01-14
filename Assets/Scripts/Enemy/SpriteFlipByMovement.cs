using UnityEngine;

/// <summary>
/// Flips the sprite based on horizontal movement direction.
/// </summary>
public class SpriteFlipByMovement : MonoBehaviour
{
    /// <summary>
    /// If the sprite should invert.
    /// </summary>
    [SerializeField] 
    private bool invert = false;

    /// <summary>
    /// The sprite renderer.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// The last position.
    /// </summary>
    private Vector3 lastPosition;

    /// <summary>
    /// Cache fields.
    /// </summary>
    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.lastPosition = this.transform.position;
    }

    /// <summary>
    /// Swap sprite on late update based on movement.
    /// </summary>
    private void LateUpdate()
    {
        float deltaX = this.transform.position.x - this.lastPosition.x;

        // Flip sprite.
        if (Mathf.Abs(deltaX) > 0.001f)
        {
            bool facingLeft = deltaX < 0f;
            this.spriteRenderer.flipX = this.invert ? !facingLeft : facingLeft;
        }

        // Set last position.
        this.lastPosition = this.transform.position;
    }
}

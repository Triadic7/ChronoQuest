using UnityEngine;

/// <summary>
/// Linear movement to head in one direction.
/// </summary>
public class LinearMovement : MonoBehaviour
{
    /// <summary>
    /// Linear movement towards a direction.
    /// </summary>
    [SerializeField] 
    private Vector2 direction;

    /// <summary>
    /// How fast the speed is.
    /// </summary>
    [SerializeField] 
    private float speed = 5f;

    /// <summary>
    /// The rigidbody.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// On Awake, cache rb and head in direction.
    /// </summary>
    private void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.rb.linearVelocity = this.direction.normalized * this.speed;
    }
}

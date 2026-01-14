using System;
using UnityEngine;

/// <summary>
/// Moves the GameObject toward the first detected target in an AggroZone.
/// </summary>
public class ChaseMovement : MonoBehaviour
{
    /// <summary>
    /// Function that returns the target to chase.
    /// </summary>
    private Func<Transform> getTargetFunc;

    /// <summary>
    /// Movement speed of the enemy.
    /// </summary>
    [SerializeField]
    private float speed = 5f;

    /// <summary>
    /// Rigidbody2D used to move the GameObject.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Cache references to rigidbody.
    /// </summary>
    private void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Set target function.
    /// </summary>
    /// <param name="targetFunc">The list of transforms.</param>
    public void SetTargetFunction(Func<Transform> targetFunc)
    {
        this.getTargetFunc = targetFunc;
    }

    /// <summary>
    /// Called every physics update to move toward the target.
    /// </summary>
    private void FixedUpdate()
    {
        // If no targets, return.
        if (getTargetFunc == null)
        {
            return;
        }

        // Invoke function on target.
        Transform target = getTargetFunc.Invoke();
        if (target == null)
        {
            return;
        }

        // Head towards target position.
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }
}

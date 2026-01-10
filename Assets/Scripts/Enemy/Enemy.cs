using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /// <summary>
    /// The health of the boss.
    /// </summary>
    public int Health { get; set; } = 100;

    /// <summary>
    /// The movement speed of the enemy.
    /// </summary>
    public float movementSpeed { get; set; } = 100f;

    /// <summary>
    /// Where the Enemy starts at.
    /// </summary>
    public Vector3 startPosition;

    /// <summary>
    /// The aggro zone for the enemy.
    /// </summary>
    public AggroZone aggroZone;

    /// <summary>
    /// Event thats fired whenever the boss takes damage.
    /// </summary>
    public event Action OnTakeDamage;

    /// <summary>
    /// The current health.
    /// </summary>
    public int CurrentHealth { get; set; }

    /// <summary>
    /// The rigidbody of the enemy.
    /// </summary>
    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void FixedUpdate()
    {
        if (aggroZone.detectedObjects != null)
        {
            MoveTowardsTarget();
        }
    }

    void MoveTowardsTarget()
    {
        if (aggroZone.detectedObjects.Count > 0)
        {
            Debug.Log("Enemy moving towards target");

            Collider2D detectedObject = aggroZone.detectedObjects[0];

            if (detectedObject)
            {
                Vector2 direction = (detectedObject.transform.position - transform.position).normalized;

                rb.AddForce(direction * movementSpeed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Take damage, and fire event.
    /// </summary>
    /// <param name="damage">The amount of damage.</param>
    /// <returns>Returns true if damage killed boss.</returns>
    public void TakeDamage(int damage)
    {
        // Check for death.
        this.CurrentHealth -= damage;
        if (this.CurrentHealth < 0)
        {
            this.CurrentHealth = 0;
        }

        // Fire event.
        this.OnTakeDamage?.Invoke();
    }

    public void Defeated()
    {
        Destroy(gameObject);
    }
}

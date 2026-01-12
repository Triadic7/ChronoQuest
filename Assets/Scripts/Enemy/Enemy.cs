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
    [SerializeField]
    private float movementSpeed = 100f;

    /// <summary>
    /// The movement speed of the enemy.
    /// </summary>
    public float MovementSpeed { get => this.movementSpeed; set => this.movementSpeed = value; }

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
    /// Sprite renderer for flipping.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// The rigidbody of the enemy.
    /// </summary>
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
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
            Collider2D detectedObject = aggroZone.detectedObjects[0];

            if (detectedObject)
            {
                Vector2 direction = (detectedObject.transform.position - transform.position).normalized;

                rb.AddForce(direction * movementSpeed * Time.deltaTime);

                // Flip sprite based on X direction.
                if (this.spriteRenderer != null && Mathf.Abs(direction.x) > 0.01f)
                {
                    this.spriteRenderer.flipX = direction.x < 0f;
                }
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

using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float movementSpeed = 500f;

    public float _health;
    public float Health 
    { 
        set 
        { 
            _health = value;

            if (_health <= 0)
            {
                TakeDamage();
            }
        }
        get {
            return _health;
        }
    }

    public AggroZone aggroZone;

    public EnemyHitbox hitbox;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (aggroZone.detectedObjects.Count > 0)
        {
            Collider2D detectedObject = aggroZone.detectedObjects[0];

            if (detectedObject)
            {
                Vector2 direction = (detectedObject.transform.position - transform.position).normalized;

                rb.AddForce(direction * movementSpeed * Time.deltaTime);
            }
        }
    }

    public void TakeDamage()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public Collider2D col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col.GetComponent<Collider2D>();
    }

    // Triggers when a player or object enters the range
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            Debug.Log("Player Destroyed Enemy");
            Destroy(collider.gameObject);
        }
    }
}

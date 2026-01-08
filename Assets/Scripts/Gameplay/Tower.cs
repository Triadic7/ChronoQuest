using System;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    /// <summary>
    /// Event thats fired whenever the tower takes damage.
    /// </summary>
    public event Action OnTakeDamage;

    /// <summary>
    /// Where the tower starts at.
    /// </summary>
    private Vector3 startPosition;

    /// <summary>
    /// Called when an enemy impacts station.
    /// </summary>
    public event Action OnDamageTaken;

    public List<GameObject> detectedObjects = new List<GameObject>();

    public Collider2D hitbox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    // Triggers when a player or object enters the range
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            Debug.Log("Destroying Enemy");
            Destroy(collider.gameObject);
            TakeDamage();
        }
    }

    /// <summary>
    /// Take damage and fire event.
    /// </summary>
    public void TakeDamage()
    {
        this.OnDamageTaken?.Invoke();
    }
}

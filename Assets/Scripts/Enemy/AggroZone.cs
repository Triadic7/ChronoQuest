using UnityEngine;
using System.Collections.Generic;

public class AggroZone : MonoBehaviour
{
    public string tagTarget = "Player";

    public List<Collider2D> detectedObjects = new List<Collider2D>();

    public Collider2D col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col.GetComponent<Collider2D>();
    }

    // Triggers when a player or object enters the range
    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log($"Collider Tag: {collider.gameObject.tag} GameObject name: {collider.gameObject.name}");
        if (collider.gameObject.tag == tagTarget)
        {
            detectedObjects.Add(collider);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == tagTarget)
        {
            detectedObjects.Remove(collider);
        }
    }
}

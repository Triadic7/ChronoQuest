using UnityEngine;
using System.Collections.Generic;

public class AggroZone : MonoBehaviour
{
    [SerializeField]
    private string tagTarget = "Player";

    public List<Collider2D> detectedObjects = new List<Collider2D>();

    private Collider2D col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.col = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Triggers when an object enters the range.
    /// </summary>
    /// <param name="collider">The collider.</param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == tagTarget)
        {
            detectedObjects.Add(collider);
        }
    }

    /// <summary>
    /// Removes detected object if in list.
    /// </summary>
    /// <param name="collider">The collider.</param>
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == tagTarget)
        {
            detectedObjects.Remove(collider);
        }
    }
}

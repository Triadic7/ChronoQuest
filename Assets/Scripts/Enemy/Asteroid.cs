using UnityEngine;

/// <summary>
/// Asteroid that moves right, randomizes size, and destroys on impact or timeout.
/// </summary>
public class Asteroid : MonoBehaviour
{
    /// <summary>
    /// Minimum scale multiplier.
    /// </summary>
    [SerializeField]
    private float minSize = 0.5f;

    /// <summary>
    /// Maximum scale multiplier.
    /// </summary>
    [SerializeField]
    private float maxSize = 1.5f;

    /// <summary>
    /// Time before the asteroid despawns.
    /// </summary>
    [SerializeField]
    private float lifeTime = 10f;

    /// <summary>
    /// Cached rigidbody.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Cache the rb.
    /// </summary>
    private void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// On start randomize size and move to the right.
    /// </summary>
    private void Start()
    {
        // Randomize size on spawn.
        float randomSize = Random.Range(this.minSize, this.maxSize);
        this.transform.localScale = Vector3.one * randomSize;

        // Destroy after lifetime.
        Destroy(this.gameObject, this.lifeTime);
    }
}

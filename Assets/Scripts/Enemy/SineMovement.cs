using UnityEngine;

/// <summary>
/// Sine movemnt to go back and forth.
/// </summary>
public class SineMovement : MonoBehaviour
{
    /// <summary>
    /// Distance till turns around.
    /// </summary>
    [SerializeField] 
    private float distance = 5f;

    /// <summary>
    /// The speed.
    /// </summary>
    [SerializeField] 
    private float speed = 1f;

    /// <summary>
    /// The start position.
    /// </summary>
    private Vector3 startPos;

    /// <summary>
    /// The timer for movement.
    /// </summary>
    private float timer;

    /// <summary>
    /// Cache start position.
    /// </summary>
    private void Start()
    {
        this.startPos = this.transform.position;
    }

    /// <summary>
    /// Updates timer and the position of the object.
    /// </summary>
    private void Update()
    {
        this.timer += Time.deltaTime * this.speed;
        this.transform.position = this.startPos + Vector3.right * Mathf.Sin(this.timer) * this.distance;
    }
}

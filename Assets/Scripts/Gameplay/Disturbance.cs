using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// A disturbance object spawned in the time minigame.
/// </summary>
public class Disturbance : MonoBehaviour
{
    /// <summary>
    /// Action called when disturbance is destroyed.
    /// </summary>
    public event Action<Disturbance, bool> OnDestroyDisturbance;

    /// <summary>
    /// If the disturbance was collected.
    /// </summary>
    private bool wasCollected = false;

    /// <summary>
    /// Lifetime in seconds before this disturbance destroys itself.
    /// </summary>
    [SerializeField]
    private float lifetime = 5f;

    /// <summary>
    /// Starting alpha fully translucent.
    /// </summary>
    [SerializeField]
    private float startAlpha = 0.1f;

    /// <summary>
    /// Ending alpha fully solid.
    /// </summary>
    [SerializeField]
    private float endAlpha = 1f;

    /// <summary>
    /// Sprite renderer.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Coroutine for fading.
    /// </summary>
    private Coroutine fadeRoutine;

    /// <summary>
    /// Cache sprite renderer on awake.
    /// </summary>
    private void Awake()
    {
        this.spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Invoke destruction on start.
    /// </summary>
    private void Start()
    {
        this.SetAlpha(this.startAlpha);

        // Start fade and destruction timing
        this.fadeRoutine = StartCoroutine(this.FadeInOverLifetime());
        this.Invoke("DestroySelf", this.lifetime);
    }

    /// <summary>
    /// Fade sprite from translucent to solid over its lifetime.
    /// </summary>
    private IEnumerator FadeInOverLifetime()
    {
        float elapsed = 0f;

        // Get solid over lifetime.
        while (elapsed < this.lifetime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / this.lifetime);

            float alpha = Mathf.Lerp(this.startAlpha, this.endAlpha, t);
            this.SetAlpha(alpha);

            yield return null;
        }
    }

    /// <summary>
    /// Sets sprite alpha.
    /// </summary>
    private void SetAlpha(float alpha)
    {
        if (this.spriteRenderer == null)
        {
            return;
        }

        Color color = this.spriteRenderer.color;
        color.a = alpha;
        this.spriteRenderer.color = color;
    }

    /// <summary>
    /// If player collided, collect and destroy disturbance.
    /// </summary>
    /// <param name="collision">The collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            this.wasCollected = true;
            this.DestroySelf();
        }
    }

    /// <summary>
    /// Destroys self.
    /// </summary>
    private void DestroySelf()
    {
        this.CancelInvoke();

        // Stop coroutine.
        if (this.fadeRoutine != null)
        {
            StopCoroutine(this.fadeRoutine);
        }

        Destroy(this.gameObject);
    }

    /// <summary>
    /// Call event on destroy.
    /// </summary>
    private void OnDestroy()
    {
        this.OnDestroyDisturbance?.Invoke(this, this.wasCollected);
    }
}

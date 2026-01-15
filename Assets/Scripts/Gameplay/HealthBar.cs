using UnityEngine;
using System;

/// <summary>
/// Health bar for IHeath objects to use to display health.
/// </summary>
public class HealthBar : MonoBehaviour
{
    /// <summary>
    /// Health bar border.
    /// </summary>
    private GameObject healthBarRoot;

    /// <summary>
    /// Health bar foreground.
    /// </summary>
    private RectTransform healthForeground;

    /// <summary>
    /// Health source.
    /// </summary>
    private IHealth healthSource;

    /// <summary>
    /// Refreshes health bar.
    /// </summary>
    public void Refresh()
    {
        this.UpdateHealthBar();
    }

    /// <summary>
    /// Get foreground and fields.
    /// </summary>
    private void Awake()
    {
        // Get the IHealth component.
        this.healthSource = GetComponentInParent<IHealth>();
        if (this.healthSource == null)
        {
            Debug.LogError("HealthBar requires an IHealth component on the parent.");
            return;
        }

        // Find the health bar root.
        this.healthBarRoot = transform.GetChild(0).GetChild(0).gameObject;
        if (this.healthBarRoot == null)
        {
            Debug.LogError("HealthBarRoot not found.");
            return;
        }

        // Find the green foreground bar inside the root.
        this.healthForeground = this.healthBarRoot.transform.GetChild(1).GetComponent<RectTransform>();
        if (this.healthForeground == null)
        {
            Debug.LogError("HealthForeground not found.");
            return;
        }
    }

    /// <summary>
    /// Refreshes health bar.
    /// </summary>
    private void UpdateHealthBar()
    {
        if (this.healthForeground == null || this.healthBarRoot == null || this.healthSource == null)
        {
            Debug.LogError("Missing fields.");
            return;
        }

        // Get current and max health.
        int current = this.healthSource.CurrentHealth;
        int max = this.healthSource.Health;

        // If health not full, display bar.
        if (current < max)
        {
            this.healthBarRoot.SetActive(true);
            float percent = (float)current / (float)max;
            this.healthForeground.localScale = new Vector3(percent, 1f, 1f);
        }
        else
        {
            this.healthBarRoot.SetActive(false);
        }
    }


    /// <summary>
    /// Assign the IHealth source and start listening.
    /// </summary>
    private void OnEnable()
    {
        if (this.healthSource == null)
        {
            Debug.LogError("No IHealth found on this object for HealthBar.");
            return;
        }

        // Subscribe to damage taken to update health bar.
        this.healthSource.OnTakeDamage += this.UpdateHealthBar;

        // Subscribe to health changed event if player.
        if (this.healthSource is Player player)
        {
            player.OnHealthChanged += this.UpdateHealthBar;
        }

        this.UpdateHealthBar();
    }

    /// <summary>
    /// Unsubscribe on disabled.
    /// </summary>
    private void OnDisable()
    {
        if (this.healthSource == null)
        {
            return;
        }

        // Unsubscribe from damage event
        this.healthSource.OnTakeDamage -= this.UpdateHealthBar;

        // Unsubscribe from health changed event if healthSource is Player.
        if (this.healthSource is Player player)
        {
            player.OnHealthChanged -= this.UpdateHealthBar;
        }
    }

    /// <summary>
    /// Unsubscribes from health source on take damage.
    /// </summary>
    private void OnDestroy()
    {
        if (this.healthSource != null)
        {
            this.healthSource.OnTakeDamage -= this.UpdateHealthBar;
        }
    }
}

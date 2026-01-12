using UnityEngine;
using System.Collections;

/// <summary>
/// Handles a fade + slide transition for UI panels.
/// </summary>
public class FadeSlideTransition : MonoBehaviour
{
    /// <summary>
    /// The panel to animate.
    /// </summary>
    public RectTransform panel;

    /// <summary>
    /// CanvasGroup on the panel to control alpha for fade effect.
    /// </summary>
    public CanvasGroup canvasGroup;

    /// <summary>
    /// Duration of the transition in seconds.
    /// </summary>
    [SerializeField]
    private float duration = 0.5f;

    /// <summary>
    /// Distance to slide the panel from the top in local units.
    /// </summary>
    [SerializeField]
    private float slideDistance = 1000f;

    /// <summary>
    /// Gets the duration.
    /// </summary>
    public float Duration { get { return duration; } }

    /// <summary>
    /// Starts the top to bottom slide and fade transition.
    /// Sets the panel active, positions it offscreen at the top, and fades it in.
    /// </summary>
    public void PlayTopToBottomTransition()
    {
        this.panel.gameObject.SetActive(true);

        // Start fully covering the screen
        this.panel.anchoredPosition = Vector2.zero;
        this.canvasGroup.alpha = 1f;

        this.StartCoroutine(AnimatePanel());
    }

    /// <summary>
    /// Animates the panel sliding into place while fading in.
    /// </summary>
    private IEnumerator AnimatePanel()
    {
        float elapsed = 0f;

        // Get start position.
        Vector2 startPos = Vector2.zero;
        Vector2 endPos = new Vector2(0, -this.slideDistance);

        // Slide panel for duration.
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            this.panel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        this.panel.gameObject.SetActive(false);
    }
}

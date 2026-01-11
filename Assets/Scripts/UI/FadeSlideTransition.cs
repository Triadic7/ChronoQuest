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
        panel.gameObject.SetActive(true);

        // Start panel offscreen at the top.
        panel.anchoredPosition = new Vector2(0, slideDistance);

        // Start fully transparent.
        canvasGroup.alpha = 0f;

        // Begin animation coroutine.
        StartCoroutine(AnimatePanel());
    }

    /// <summary>
    /// Animates the panel sliding into place while fading in.
    /// </summary>
    private IEnumerator AnimatePanel()
    {
        float elapsed = 0f;

        // Store start and end positions.
        Vector2 startPos = panel.anchoredPosition;
        Vector2 endPos = Vector2.zero;

        // Store start and end alpha values.
        float startAlpha = 0f;
        float endAlpha = 1f;

        // Animate over duration.
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Normalized time.
            float t = elapsed / duration;

            // Smooth.
            t = Mathf.SmoothStep(0f, 1f, t);

            // Lerp position and alpha.
            panel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }


        // Small pause before fading out.
        yield return new WaitForSeconds(0.1f);

        // Fade out.
        elapsed = 0f;
        startAlpha = 1f;
        endAlpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        // Ensure final alpha is 0 and hide panel.
        canvasGroup.alpha = 0f;
        panel.gameObject.SetActive(false);
    }
}

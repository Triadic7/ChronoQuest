using System.Collections;
using UnityEngine;

/// <summary>
/// Changes the players visuals depending on the stage.
/// </summary>
public class PlayerVisuals : MonoBehaviour
{
    /// <summary>
    /// The sprite renderer.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// The thrusters on player ship.
    /// </summary>
    [SerializeField]
    private GameObject thrusters;

    /// <summary>
    /// Animator for player walking.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// For the animator.
    /// </summary>
    private bool isWalking = false;

    /// <summary>
    /// Tracks if final stage.
    /// </summary>
    private bool isFinalStage = false;

    /// <summary>
    /// Fades out the players sprite then disables it.
    /// </summary>
    /// <param name="duration">How long to shut off.</param>
    public void FadeOutAndDisable(float duration = 1f)
    {
        StartCoroutine(this.FadeOutRoutine(duration));
    }

    /// <summary>
    /// Fades in the player sprite and enables the GameObject.
    /// </summary>
    /// <param name="duration">How long the fade in should take.</param>
    public void FadeIn(float duration = 1f)
    {
        // Start the coroutine to fade in.
        StartCoroutine(this.FadeInRoutine(duration));
    }

    /// <summary>
    /// Called from InputManager when player moves.
    /// </summary>
    /// <param name="walking">If the player is walking.</param>
    public void SetWalking(bool walking)
    {
        this.isWalking = walking;

        // If animator isnt null, set walking.
        if (this.animator != null)
        {
            this.animator.SetBool("isWalking", walking);
        }
    }

    /// <summary>
    /// Fades out over time, then disables the player game object.
    /// </summary>
    /// <param name="duration">How long should it take to do this process.</param>
    /// <returns></returns>
    private IEnumerator FadeOutRoutine(float duration)
    {
        if (this.spriteRenderer == null)
        {
            yield break;
        }

        // Cache starting color.
        Color startColor = this.spriteRenderer.color;
        float timer = 0f;

        // Fades out over duration.
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / duration);

            // Change color over duration.
            this.spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // Make sprite renderer invisible.
        this.spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Disable the player after fade.
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Coroutine that fades in the sprite over time.
    /// </summary>
    /// <param name="duration">Duration of fade in.</param>
    /// <returns></returns>
    private IEnumerator FadeInRoutine(float duration)
    {
        if (this.spriteRenderer == null)
        {
            yield break;
        }

        // Cache the starting color. Current alpha may be 0 if faded out.
        Color startColor = this.spriteRenderer.color;
        float timer = 0f;

        // Start with alpha 0.
        this.spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Fade in over duration.
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / duration);

            // Turn sprite renderer color more solid.
            this.spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // Fully visible at the end.
        this.spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
    }

    /// <summary>
    /// Cache sprite renderer.
    /// </summary>
    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.animator = GetComponent<Animator>();
    }

    /// <summary>
    /// At start cache event for changing sprites.
    /// </summary>
    private void Start()
    {
        GameManager gm = GameManager.Instance;

        if(gm == null)
        {
            Debug.LogError("No GameManager found.");
            return;
        }

        // Apply stage immediately if already loaded.
        this.ApplyStageAppearance(gm.CurrentSave?.Stage);

        gm.GameStageManager.OnStageStarted += this.ApplyStageAppearance;
    }

    /// <summary>
    /// Changes the players sprite depending on the stage.
    /// </summary>
    /// <param name="stage">The stage.</param>
    private void ApplyStageAppearance(StageModel stage = null)
    {
        StageModel currentStage = stage ?? GameManager.Instance.CurrentSave?.Stage;
        this.isFinalStage = currentStage != null && currentStage.IsFinalStage;

        // Thrusters only active if ship.
        if (this.thrusters != null)
        {
            this.thrusters.SetActive(this.isFinalStage);
        }

        // Change animator based on stage.
        if (this.animator != null)
        {
            // Whether we are in final stage, set animator.
            this.animator.SetBool("isFinalStage", this.isFinalStage);

            // Only set walking if not final stage.
            if (!this.isFinalStage)
            {
                this.animator.SetBool("isWalking", this.isWalking);
            }
        }
    }
}

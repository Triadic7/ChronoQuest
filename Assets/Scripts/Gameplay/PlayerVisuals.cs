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
    /// The normal sprite.
    /// </summary>
    [SerializeField] 
    private Sprite normalSprite;

    /// <summary>
    /// The final stage sprite.
    /// </summary>
    [SerializeField] 
    private Sprite finalStageSprite;

    /// <summary>
    /// Cache sprite renderer.
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        gm.GameStageManager.OnStageStarted += ApplyStageAppearance;
    }

    /// <summary>
    /// Changes the players sprite depending on the stage.
    /// </summary>
    /// <param name="stage">The stage.</param>
    private void ApplyStageAppearance(StageModel stage)
    {
        // Changes sprite to final sprite if the stage is the final one.
        spriteRenderer.sprite = stage.IsFinalStage ? finalStageSprite : normalSprite;
    }
}

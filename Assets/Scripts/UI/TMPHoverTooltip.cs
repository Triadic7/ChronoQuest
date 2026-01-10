using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Handles hover tooltips for any TextMeshProUGUI component under the mouse.
/// Displays the word's name and description from a predefined dictionary.
/// </summary>
public class TMPHoverTooltipMulti : MonoBehaviour
{
    /// <summary>
    /// The panel to display when hovering over a word.
    /// </summary>
    [SerializeField] private GameObject tooltipPanel;

    /// <summary>
    /// The text component that shows the hovered word's description.
    /// </summary>
    [SerializeField] private TextMeshProUGUI tooltipText;

    /// <summary>
    /// The text component that shows the hovered word itself.
    /// </summary>
    [SerializeField] private TextMeshProUGUI tooltipNameText;

    /// <summary>
    /// The camera used for TMP text raycasting.
    /// </summary>
    private Camera cam;

    /// <summary>
    /// GraphicRaycaster for detecting UI elements under the mouse.
    /// </summary>
    private GraphicRaycaster raycaster;

    /// <summary>
    /// EventSystem reference used for pointer events.
    /// </summary>
    private EventSystem eventSystem;

    /// <summary>
    /// Dictionary mapping word IDs to their descriptions.
    /// </summary>
    private readonly Dictionary<string, string> wordDescriptions = new()
    {
        { "pyramid", "Ancient structures built by aliens that contain super weapons." },
        { "castle", "King Arthurs castle. It was constructed over ruins that are said to contain a code to closing the void." },
        { "anomalies", "The enemy of existence." },
        { "void", "A dimension where anomalies are banished, and come from." },
        { "disturbance", "Technology from the future. Too powerful for the people of this time." },
        { "pay", "Rate: $7.25 USD" },
        { "interloper", "A large anomaly formed when multiple come together. Interlopers take centuries to form and are always treated as apex threats." },
        { "incident", "After [REDACTED] fired a [REDACTED] from the year [REDACTED], a [REDACTED] appeared and started [REDACTED]. The result: [REDACTED] [REDACTED]." }
    };

    /// <summary>
    /// Cache references on awake and disable tooltip panel.
    /// </summary>
    private void Awake()
    {
        // Get the canvas from the tooltip panel.
        Canvas canvas = tooltipPanel.GetComponentInParent<Canvas>();

        // If the canvas is ScreenSpaceOverlay, set camera to null; otherwise use canvas camera.
        this.cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

        // Get the GraphicRaycaster.
        this.raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (this.raycaster == null)
        {
            Debug.LogError("No raycaster found.");
            return;
        }

        // Reference to the active EventSystem.
        this.eventSystem = EventSystem.current;

        this.tooltipPanel.SetActive(false);
    }

    /// <summary>
    /// Update is called once per frame to detect mouse hover over TMP links.
    /// </summary>
    private void Update()
    {
        // Get the current mouse position.
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Create a PointerEventData at the mouse position.
        PointerEventData pointerEventData = new PointerEventData(this.eventSystem) 
        { 
            position = mousePos 
        };

        // Raycast to detect all UI elements under the mouse.
        List<RaycastResult> results = new List<RaycastResult>();
        this.raycaster.Raycast(pointerEventData, results);

        bool foundLink = false;

        // Iterate through all UI elements under the mouse.
        foreach (RaycastResult result in results)
        {
            // Check if the UI element has a TextMeshProUGUI component.
            TMP_Text tmp = result.gameObject.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                // Check if the mouse is over a <link> in this TMP text.
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmp, mousePos, cam);
                if (linkIndex != -1)
                {
                    // Get the link info and its ID.
                    TMP_LinkInfo linkInfo = tmp.textInfo.linkInfo[linkIndex];
                    string id = linkInfo.GetLinkID();

                    // If the word ID exists in the dictionary, show tooltip.
                    if (this.wordDescriptions.TryGetValue(id, out string description))
                    {
                        this.tooltipPanel.SetActive(true);

                        // Set the word description.
                        this.tooltipText.text = description;

                        // Set the hovered word itself.
                        this.tooltipNameText.text = linkInfo.GetLinkText();

                        // Position tooltip above the cursor.
                        RectTransform tooltipRect = this.tooltipPanel.GetComponent<RectTransform>();

                        // Half height + padding.
                        float offsetY = tooltipRect.rect.height / 2 + 15f;
                        this.tooltipPanel.transform.position = mousePos + new Vector2(0, offsetY);

                        foundLink = true;
                        break;
                    }
                }
            }
        }

        // If no link is hovered, hide the tooltip panel.
        if (!foundLink)
        {
            this.tooltipPanel.SetActive(false);
        }
    }
}
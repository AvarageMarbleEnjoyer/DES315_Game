using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CursorManager : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite hoverSprite;

    [Header("References")]
    public Image cursorImage;

    private Canvas canvas;
    private RectTransform canvasRect;
    private RectTransform cursorRect;
    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

    private void Start()
    {
        canvas = cursorImage.canvas;
        canvasRect = canvas.GetComponent<RectTransform>();
        cursorRect = cursorImage.rectTransform;
        cursorRect.pivot = new Vector2(0f, 1f);

        Cursor.visible = false;
        cursorImage.sprite = defaultSprite;

        if (canvas == null)
            Debug.LogError("CursorManager: canvas is null. Check that cursorImage is assigned and is inside a Canvas.");
    }

    private void Update()
    {
        if (canvas == null || canvasRect == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );

        cursorRect.localPosition = localPoint;

        cursorImage.sprite = IsHoveringTarget(mousePosition) ? hoverSprite : defaultSprite;
    }

    /// <summary>
    /// Returns true if any UI element under the cursor has a HoverTarget component on it.
    /// </summary>
    private bool IsHoveringTarget(Vector2 screenPosition)
    {
        if (EventSystem.current == null) return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = screenPosition };
        raycastResults.Clear();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.GetComponent<HoverTarget>() != null)
                return true;
        }

        return false;
    }

    public void SetHoverCursor() => cursorImage.sprite = hoverSprite;
    public void SetDefaultCursor() => cursorImage.sprite = defaultSprite;

    private void OnEnable() => Cursor.visible = false;
    private void OnDisable() => Cursor.visible = true;
}
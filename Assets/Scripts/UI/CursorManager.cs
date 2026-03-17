using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Sprites")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverSprite;

    [Header("References")]
    [SerializeField] private Image cursorImage;

    [Header("Settings")]
    [Tooltip("Pixel offset of the hotspot (click point) from the top-left of the sprite")]
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    private PointerEventData _pointerEventData;
    private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Cursor.visible = false;
    }

    private void Start()
    {
        if (EventSystem.current != null)
        {
            _pointerEventData = new PointerEventData(EventSystem.current);
        }
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }

    private void Update()
    {
        UpdatePosition();
        UpdateSprite();
    }

    private void UpdatePosition()
    {
        if (cursorImage == null) return;

        Vector2 mousePos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : (Vector2)Input.mousePosition;

        cursorImage.rectTransform.position = new Vector3(
            mousePos.x - hotspot.x,
            mousePos.y + hotspot.y,
            0f
        );
    }

    private void UpdateSprite()
    {
        if (cursorImage == null) return;

        cursorImage.sprite = IsHoveringInteractable() ? hoverSprite : defaultSprite;
    }

    /// <summary>
    /// Raycasts through the EventSystem to check whether the cursor is over any Selectable UI element (Button, Toggle, etc).
    /// </summary>
    private bool IsHoveringInteractable()
    {
        if (EventSystem.current == null) return false;

        if (_pointerEventData == null)
        {
            _pointerEventData = new PointerEventData(EventSystem.current);
        }

        Vector2 mousePos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : (Vector2)Input.mousePosition;

        _pointerEventData.position = mousePos;
        _raycastResults.Clear();
        EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

        foreach (RaycastResult result in _raycastResults)
        {
            if (result.gameObject.GetComponentInParent<Selectable>() != null)
                return true;
        }

        return false;
    }
}
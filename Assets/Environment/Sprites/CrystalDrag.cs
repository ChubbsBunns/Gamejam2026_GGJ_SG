using UnityEngine;
using UnityEngine.InputSystem;

public class CrystalDrag : MonoBehaviour
{
    public SlashDetector slashDetector;

    public FloatyHover floatyHover;

    private PlayerInputActions input;
    private InputAction dragInteract;

    private bool isDragging = false;
    private Camera cam;

    void Awake()
    {
        input = new PlayerInputActions();
        slashDetector = GetComponent<SlashDetector>();
        floatyHover = GetComponent<FloatyHover>();
    }

    void OnEnable()
    {
        cam = Camera.main;

        dragInteract = input.Player.DragInteract;
        dragInteract.started += OnDragStarted;
        dragInteract.canceled += OnDragCanceled;

        dragInteract.Enable();
    }

    void OnDisable()
    {
        dragInteract.started -= OnDragStarted;
        dragInteract.canceled -= OnDragCanceled;
        dragInteract.Disable();
    }

    private void OnDragStarted(InputAction.CallbackContext ctx)
    {
        if (!IsMouseOverCrystal()) return;
        floatyHover.floating = false;
        isDragging = true;
        slashDetector.StartTracking();
    }

    private void OnDragCanceled(InputAction.CallbackContext ctx)
    {
        
        if (!isDragging) return;
        floatyHover.floating = true;
        isDragging = false;
        slashDetector.StopTracking();
    }

    void Update()
    {
        if (!isDragging) return;


        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, -cam.transform.position.z)
        );
        mouseWorld.z = 0f;

        transform.position = mouseWorld;
        slashDetector.TrackPoint(mouseWorld);
    }

    // Detects if the cursor is over this crystal when clicking
    private bool IsMouseOverCrystal()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, -cam.transform.position.z)
        );
        Vector2 p = mouseWorld;

        Collider2D col = GetComponent<Collider2D>();
        return col != null && col.OverlapPoint(p);
    }
}

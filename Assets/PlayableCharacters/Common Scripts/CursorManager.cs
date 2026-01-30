using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CursorManager : MonoBehaviour
{
    [System.Serializable]
    public class CursorVisual
    {
        public PlayerBase.PlayerCharacterID characterID;
        public Sprite sprite;
    }

    [Header("Cursor Settings")]
    [SerializeField] private float followSmoothness = 20f;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private SpriteRenderer cursorRenderer;
    [SerializeField] private List<CursorVisual> cursorVisuals = new();

    private Camera mainCam;
    private Vector3 velocity;

    private void Awake()
    {
        mainCam = Camera.main;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!mainCam || cursorRenderer == null) return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, -mainCam.transform.position.z));
        mouseWorld.z = 0;

        transform.position = Vector3.SmoothDamp(transform.position, mouseWorld + offset, ref velocity, 1f / followSmoothness);
    }
}

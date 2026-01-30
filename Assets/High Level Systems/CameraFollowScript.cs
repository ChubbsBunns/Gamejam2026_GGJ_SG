using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CameraFollowScript : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, -10f); 
    Camera mainCamera;
    PlayerCharacter playerCharacter;

    public float cameraLookAhead;
    private Vector3 cameraVelocity = Vector3.zero;
    public float followSpeed = 5f; 

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        playerCharacter = FindAnyObjectByType<PlayerCharacter>();
        if (playerCharacter == null)
        {
            Debug.Log("Player transform is not found");
        } else
        {
            Debug.Log("Player transform is found");
        }

    }

    void LateUpdate()
    {
        Vector3 targetPos = playerCharacter.GetActivePosition().position + offset;
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        float cameraToPlane = -mainCamera.transform.position.z;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(
            new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, cameraToPlane)
        );
        mouseWorld.z = 0f;

        //Data for look ahead depending on mouse position
        Vector3 playerPos = playerCharacter.GetActivePosition().position;
        Vector3 dirToMouse = mouseWorld - playerPos;        
        Vector3 lookOffset = dirToMouse.normalized * Mathf.Min(dirToMouse.magnitude, cameraLookAhead);

        targetPos += lookOffset;
        mainCamera.transform.position = Vector3.SmoothDamp(
            mainCamera.transform.position,
            targetPos,
            ref cameraVelocity,
            1f / followSpeed
        );


    }
}

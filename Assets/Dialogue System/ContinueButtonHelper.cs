using PixelCrushers.DialogueSystem;
using UnityEngine.InputSystem;
using UnityEngine;

public class ContinueButtonHelper : MonoBehaviour
{
    [SerializeField] StandardUIContinueButtonFastForward standardUIContinueButtonFastForward;
    [SerializeField] private PlayerInputActions input;
    [SerializeField] private InputAction detectClickInteract;

    [SerializeField] private bool onClickDownDuringDialogue = false;

    void Start()
    {
        onClickDownDuringDialogue = false;
        input = new PlayerInputActions();
        detectClickInteract = input.Dialogue.Interact;
        detectClickInteract.started += OnClickDuringDialogueStarted;
        detectClickInteract.canceled += OnClickDuringDialogueCanceled;
        detectClickInteract.Enable();
    }

    void OnEnable()
    {
        if (input != null)
        {
            detectClickInteract = input.Dialogue.Interact;
            detectClickInteract.started += OnClickDuringDialogueStarted;
            detectClickInteract.canceled += OnClickDuringDialogueCanceled;
            detectClickInteract.Disable();
        }
    }

    void OnDisable()
    {
        detectClickInteract.started -= OnClickDuringDialogueStarted;    
        detectClickInteract.canceled -= OnClickDuringDialogueCanceled;
        detectClickInteract.Disable();        
    }

    private void OnClickDuringDialogueStarted(InputAction.CallbackContext ctx)
    {
        if (CheckWhetherInDialogue())
        {
            onClickDownDuringDialogue = true;                    
        }

    }

    private void OnClickDuringDialogueCanceled(InputAction.CallbackContext ctx)
    {
        if (CheckWhetherInDialogue())
        {
            if (onClickDownDuringDialogue)
            {
                standardUIContinueButtonFastForward.OnFastForward();
                onClickDownDuringDialogue = false;
            }            
        }

    }

    private bool CheckWhetherInDialogue()
    {
        if (standardUIContinueButtonFastForward.gameObject.activeSelf)
        {
            return true;
        }
        return false;
    }
}

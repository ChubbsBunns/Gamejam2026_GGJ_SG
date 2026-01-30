// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers.Tutorials
{

    /// <summary>
    /// If the project is using the built-in input manager, checks that the EventSystem
    /// has a StandaloneInputModule. If the project is using the Input System package,
    /// checks that the EventSystem has an InputSystemUIInputModule.
    /// </summary>
    public class CheckEventSystem : MonoBehaviour
    {

        public UITextField messageText;

        private void Start()
        {
            if (EventSystem.current == null)
            {
                messageText.text = "ERROR: Add an EventSystem to the scene.";
                return;
            }
#if ENABLE_INPUT_SYSTEM || USE_NEW_INPUT
            if (EventSystem.current.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>() == null)
            {
                messageText.text = "ERROR: The EventSystem GameObject must have an Input System UI Input Module.";
                return;
            }
#else
            if (EventSystem.current.GetComponent<StandaloneInputModule>() == null)
            {
                messageText.text = "ERROR: The EventSystem GameObject must have a Standalone Input Module.";
                return;
            }
#endif
            gameObject.SetActive(false);
        }

    }

}
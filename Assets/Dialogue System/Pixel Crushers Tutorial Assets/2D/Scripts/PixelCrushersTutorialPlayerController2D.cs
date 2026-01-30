// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.Tutorials
{

    /// <summary>
    /// Simple 2D player controller.
    /// </summary>
    public class PixelCrushersTutorialPlayerController2D : MonoBehaviour
    {

        [Tooltip("Input System package: This action must be added to an Input Action Registry component (e.g., on Dialogue Manager).\nBuilt-in Input Manager: This action must be defined in Edit > Project Settings > Input.")]
        public string horizontalAxis = "Horizontal";
        [Tooltip("Input System package: This action must be added to an Input Action Registry component (e.g., on Dialogue Manager).\nBuilt-in Input Manager: This action must be defined in Edit > Project Settings > Input.")]
        public string verticalAxis = "Vertical";

        [Tooltip("The fastest the player can travel left and right.")]
        public float maxHorizontalSpeed = 8f;

        [Tooltip("The fastest the player can travel up and down.")]
        public float maxVerticalSpeed = 5f;

        [Tooltip("Tracks which direction the player is facing.")]
        public bool facingLeft = false;

        private Rigidbody2D m_rigidbody2D;
        private Animator m_animator;

        public const string RunParameter = "Run";

        private void Awake()
        {
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_animator = GetComponent<Animator>();
            var m_sortByY = GetComponent<PixelCrushersTutorialSortByY>();
            if (m_rigidbody2D == null) Debug.LogError("No Rigidbody2D found on " + name, this);
            if (m_animator == null) Debug.LogError("No Animator found on " + name, this);
            if (m_sortByY == null) m_sortByY = gameObject.AddComponent<PixelCrushersTutorialSortByY>();
        }

        private void FixedUpdate()
        {
            // Get movement axes input:
            var horizontal = InputDeviceManager.GetAxis(horizontalAxis);
            var vertical = InputDeviceManager.GetAxis(verticalAxis);

            // If axes haven't been set up for the Input System, read WASD/arrow keys:
#if ENABLE_INPUT_SYSTEM || USE_NEW_INPUT
            if (Mathf.Approximately(0, horizontal))
            {
                if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.A].isPressed ||
                    UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.LeftArrow].isPressed)
                {
                    horizontal = -1;
                }
                if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.D].isPressed ||
                    UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.RightArrow].isPressed)
                {
                    horizontal += 1;
                }
            }
            if (Mathf.Approximately(0, vertical))
            {
                if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.S].isPressed ||
                    UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.DownArrow].isPressed)
                {
                    vertical = -1;
                }
                if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.W].isPressed ||
                    UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.UpArrow].isPressed)
                {
                    vertical += 1;
                }
            }
#endif

            var move = new Vector2(horizontal * maxHorizontalSpeed, vertical * maxVerticalSpeed);
            m_rigidbody2D.linearVelocity = move;

            // Update the animator:
            m_animator.SetBool(RunParameter, move.magnitude > 0.1f);

            // Flip the character if necessary:
            var needToFlip = ((move.x < 0 && !facingLeft) || (move.x > 0 && facingLeft));
            if (needToFlip)
            {
                facingLeft = !facingLeft;
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }
    }
}
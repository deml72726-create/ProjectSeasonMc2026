using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Keep this so it uses the new backend

public class InputManager : MonoBehaviour
{
    private Vector2 moveDirection = Vector2.zero;
    private bool interactPressed = false;
    private bool submitPressed = false;

    private static InputManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }
        instance = this;
    }

    public static InputManager GetInstance() 
    {
        return instance;
    }

    private void Update()
    {
        // Clear direction every frame before checking keys
        float xInput = 0f;

        // Directly monitor keyboard states using the new Input System's underlying API
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                xInput = -1f;
            }
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                xInput = 1f;
            }

            // Monitor interaction keys
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                interactPressed = true;
            }
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                submitPressed = true;
            }
        }

        moveDirection = new Vector2(xInput, 0f);
    }

    public Vector2 GetMoveDirection() 
    {
        return moveDirection;
    }

    public bool GetInteractPressed() 
    {
        bool result = interactPressed;
        interactPressed = false; // Reset after reading
        return result;
    }

    public bool GetSubmitPressed() 
    {
        bool result = submitPressed;
        submitPressed = false; // Reset after reading
        return result;
    }

    public void RegisterSubmitPressed() 
    {
        submitPressed = false;
    }
}
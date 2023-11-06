using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Player Input Values")]
    public Vector2 _moveDirection;
    public Vector2 _lookDirection;
    public bool _jump;
    public bool _camera;
    public bool _photo;
    public bool _flashlight;

    [Header("Movement Settings")]
    [Tooltip("DESCRIPTION")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [Header("Is the menu")]
    public bool _inMenu;

    [Header("References")]
    [Tooltip("DESCRIPTION")]
    public GameObject _pauseMenu;
    public GameObject _album;


    [Header("Input Manager Instance")]
    //Instance of the InputManager
    public static InputManager _instance;
    public InputActionAsset _controls;


    //Get the instance if there is none currently, only happens first time
    public static InputManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<InputManager>();
            }
            return _instance;
        }
    }

    private void Start()
    {
        _inMenu = false;
        _controls.FindActionMap("BasicControls").Enable();
        _controls.FindActionMap("Menu").Disable();

    }


    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }
    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    
    public void OnFlashlight(InputValue value)
    {
        FlashlightInput();
    }




    public void OnAlbum(InputValue value)
    {
        _album.SetActive(!_album.activeSelf);
        _inMenu = !_inMenu;

        SetCursorState(!_inMenu);
    }
    public void OnAlbum()
    {
        _album.SetActive(!_album.activeSelf);
        _inMenu = !_inMenu;

        SetCursorState(!_inMenu);
    }

    public void OnMenu(InputValue value)
    {
        _pauseMenu.SetActive(!_pauseMenu.activeSelf);
        SetCursorState(!_pauseMenu.activeSelf);
        if (_pauseMenu.activeSelf)
        {
            print("Menu Opened");
            _controls.FindActionMap("BasicControls").Disable();
            _controls.FindActionMap("Menu").Enable();
        }
        else
        {
            print("Menu Closed");
            _controls.FindActionMap("BasicControls").Enable();
            _controls.FindActionMap("Menu").Disable();

        }
    }


    public void MoveInput(Vector2 newMoveDirection)
    {
        _moveDirection = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        _lookDirection = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        _jump = newJumpState;
    }

    public void FlashlightInput()
    {
        _flashlight = !_flashlight;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

}
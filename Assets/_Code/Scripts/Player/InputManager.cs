using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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
    public bool _inPauseMenu;

    [Header("References")]
    [Tooltip("DESCRIPTION")]
    public GameObject _album;
    public GameObject _pauseMenu;
    public CinemachineVirtualCamera _playerCam;
    public CinemachineVirtualCamera _menuCam;
    public CinemachineVirtualCamera _photoCam;
    public Volume _mainVolume;
    public Volume _menuVolume;


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
        _playerCam.Priority = 10;
        _menuCam.Priority = 0;
        _photoCam.Priority = -10;
        _mainVolume.priority = 10;
        _menuVolume.priority = 0;
        _inPauseMenu = false;

    }

    private void Update()
    {
        Vector2 vec = Mouse.current.scroll.ReadValue();
        _photoCam.m_Lens.FieldOfView += ((vec.y / 120)*5)*-1;

        if (_photoCam.m_Lens.FieldOfView < 20)
        {
            _photoCam.m_Lens.FieldOfView = 20;
        }
        if (_photoCam.m_Lens.FieldOfView > 120)
        {
            _photoCam.m_Lens.FieldOfView = 120;
        }
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
        if (_inPauseMenu == false)
        {
            _pauseMenu.SetActive(false);
            _album.SetActive(true);

            _inMenu = !_inMenu;

            SetCursorState(!_inMenu);

            if (_inMenu)
            {
                _controls.FindActionMap("BasicControls").Disable();
                _controls.FindActionMap("Menu").Enable();
                _playerCam.Priority = 0;
                _photoCam.Priority = -10;
                _menuCam.Priority = 10;
                _mainVolume.priority = 0;
                _menuVolume.priority = 10;
                gameObject.GetComponentInChildren<PlayerInventory>().OnAlbum();
            }
            else
            {
                _controls.FindActionMap("BasicControls").Enable();
                _controls.FindActionMap("Menu").Disable();
                _playerCam.Priority = 10;
                _photoCam.Priority = -10;
                _menuCam.Priority = 0;
                _mainVolume.priority = 10;
                _menuVolume.priority = 0;
                _pauseMenu.SetActive(true);
                _album.SetActive(false);
            }
        }

    }
    public void OnAlbum()
    {
        if (_inPauseMenu == false)
        {
            _pauseMenu.SetActive(false);
            _album.SetActive(true);

            _inMenu = !_inMenu;

            SetCursorState(!_inMenu);

            if (_inMenu)
            {
                _controls.FindActionMap("BasicControls").Disable();
                _controls.FindActionMap("Menu").Enable();
                _playerCam.Priority = 0;
                _photoCam.Priority = -10;
                _menuCam.Priority = 10;
                _mainVolume.priority = 0;
                _menuVolume.priority = 10;
                gameObject.GetComponentInChildren<PlayerInventory>().OnAlbum();

            }
            else
            {
                _controls.FindActionMap("BasicControls").Enable();
                _controls.FindActionMap("Menu").Disable();
                _playerCam.Priority = 10;
                _photoCam.Priority = -10;
                _menuCam.Priority = 0;
                _mainVolume.priority = 10;
                _menuVolume.priority = 0;
                _pauseMenu.SetActive(true);
                _album.SetActive(false);
            }
        }
    }

    public void OnMenu()
    {



        if (!_inMenu && !_album.activeSelf)
        {
            _inPauseMenu = true;
            _inMenu = true;
            _pauseMenu.SetActive(true);
            _album.SetActive(false);
            SetCursorState(false);
            gameObject.GetComponentInChildren<PlayerInventory>().OnAlbum();


            _controls.FindActionMap("BasicControls").Disable();
            _controls.FindActionMap("Menu").Enable();

            _playerCam.Priority = 0;
            _photoCam.Priority = -10;
            _menuCam.Priority = 10;
            _mainVolume.priority = 0;
            _menuVolume.priority = 10;
            
        }
        else if (_inMenu && _album.activeSelf)
        {
            _inPauseMenu = true;
            _pauseMenu.SetActive(true);
            _album.SetActive(false);
            gameObject.GetComponentInChildren<PlayerInventory>().OnAlbum();


        }
        else if (_inMenu)
        {
            _inMenu = false;
            _inPauseMenu = false;
            _pauseMenu.SetActive(true);
            _album.SetActive(false);
            SetCursorState(true);

            _controls.FindActionMap("BasicControls").Enable();
            _controls.FindActionMap("Menu").Disable();
            _playerCam.Priority = 10;
            _photoCam.Priority = -10;
            _menuCam.Priority = 0;
            _mainVolume.priority = 10;
            _menuVolume.priority = 0;
            
        }
    }

    public void OnCamera()
    {
        if (_photoCam.Priority == -10)
        {
            _playerCam.Priority = 0;
            _photoCam.Priority = 10;
            _menuCam.Priority = -10;
            _photoCam.m_Lens.FieldOfView = 70;
        }
        else
        {
            _playerCam.Priority = 10;
            _photoCam.Priority = -10;
            _menuCam.Priority = 0;
            _photoCam.m_Lens.FieldOfView = 70;
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
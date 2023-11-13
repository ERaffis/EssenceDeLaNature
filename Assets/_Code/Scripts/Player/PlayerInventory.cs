using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public GameObject _flashlight;
    public GameObject _camera;
    public GameObject _controlsHUD;

    public bool _flashlightState;
    public bool _cameraState;
    public bool _controlsHUDState;

    private void Awake()
    {

    }
    public void OnFlashlight()
    {
        _flashlight.SetActive(!_flashlight.activeSelf);
        _flashlightState = _flashlight.activeSelf;
    }


    public void OnCamera()
    {

        _camera.SetActive(!_camera.activeSelf);
        _cameraState = _camera.activeSelf;
        _controlsHUD.SetActive(!_cameraState);  

    }


    public void OnAlbum()
    {
        _camera.SetActive(false);
        _cameraState = false;

    }

}

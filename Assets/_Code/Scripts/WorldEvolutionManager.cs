using DistantLands.Cozy;
using DistantLands.Cozy.Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class WorldEvolutionManager : MonoBehaviour
{

    public WeatherProfile[] _weather;
    public AtmosphereProfile _atmosphere;
    public VolumeProfile[] _volumeProfiles;
    public Volume _mainVolume;

    [Header("Player Object")]
    [Space(5)]
    public Transform _playerTransform;
    public Transform _cameraTransform;
    public PlayerController _playerController;
    [Space(5)]
    public Vector3[] _teleportPosition;
    public Vector3[] _teleportRotation;
    public Vector3[] _teleportCameraAim;

    [Header("Scene Transition")]
    public string[] _SceneNames;
    public GameObject _transitionHolder;
    public GameObject[] _transitionText;


    [Header("Terrain Information")]
    [Space(5)]
    public CozyWeather _cozyManager;
    public WindZone _windZone;

    public float _currentTicks;

    public int _positionNumber = 0;


    private void Start()
    {
        SceneManager.LoadScene(_SceneNames[0], LoadSceneMode.Additive);
        _cozyManager.currentTicks = 60;
        _cozyManager.currentDay = 0;
        _transitionText[0].SetActive(true);
    }
    public void PhotoCaptured(int number)
    {
        print(number);
        switch (number)
        {
            case 0:
                _positionNumber++;
                print(_positionNumber);
                ChangeScene(_positionNumber, 1);
                ChangeCozySettings(0, 0.9f);
                break;
            case 1:
                _positionNumber++;
                print(_positionNumber);
                ChangeScene(_positionNumber, 0);
                ChangeCozySettings(0, 0.8f);
                break;
            case 2:
                _positionNumber++;
                ChangeScene(_positionNumber, 2);
                ChangeCozySettings(1, 0.4f);
                break;
            case 3:
                _positionNumber++;
                ChangeScene(_positionNumber, 3);
                ChangeCozySettings(1, 0.1f);
                break;
            case 4:
                _positionNumber++;
                ChangeScene(_positionNumber, 0);
                ChangeCozySettings(1, 0f);
                break;
            case 5:
                _positionNumber++;
                ChangeScene(_positionNumber, 2);
                ChangeCozySettings(2, 0f);
                break;
            //Fin Hiver
            case 6:
                _positionNumber++;
                ChangeScene(_positionNumber, 1);
                ChangeCozySettings(2, 0f);
                break;
            case 7:
                _positionNumber++;
                ChangeScene(_positionNumber, 3);
                ChangeCozySettings(2, 0.25f);
                break;
            case 8:
                _positionNumber++;
                ChangeScene(_positionNumber, 3);
                ChangeCozySettings(3, 0.6f);
                break;

            case 9:
                _positionNumber++;
                ChangeScene(_positionNumber, 0);
                ChangeCozySettings(3, 0.8f);
                break;
            case 10:
                _positionNumber++;
                ChangeScene(_positionNumber, 2);
                ChangeCozySettings(3, 0.9f);
                break;
        
                case 11:
                SceneManager.LoadSceneAsync(0);
                break;

            
            }
        }

        public void FastForward(int i)
    {
        switch (i)
        {
            case 0:
                _cozyManager.currentTicks = 15;
                break;
            case 1:
                _cozyManager.currentTicks = 30;
                break;
            case 2:
                _cozyManager.currentTicks = 45;
                break;
            case 3:
                _cozyManager.currentTicks = 60;
                break; 
            default:
                break;
        }
        _cozyManager.currentDay++;
    }

    public void ChangeScene(int sceneNumber, int timeChange )
    {
        SceneManager.UnloadSceneAsync(_SceneNames[sceneNumber-1]);
        SceneManager.LoadScene(_SceneNames[sceneNumber], LoadSceneMode.Additive);

        FastForward(timeChange);
        _transitionText[sceneNumber-1].SetActive(false);
        _transitionText[sceneNumber].SetActive(true);
        _transitionHolder.GetComponent<Animator>().Play("TransitionFadeOut");
        TeleportPlayer();
    } 

    private void TeleportPlayer()
    {
        _playerTransform.position = _teleportPosition[_positionNumber];
        _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
        _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
        print("Teleported player");
    }

    private void ChangeCozySettings(int volumeProfile, float snowAmount)
    {
        _cozyManager.currentDay = _positionNumber;
        _cozyManager.SetWeather(_weather[_positionNumber]);
        _mainVolume.profile = _volumeProfiles[volumeProfile];
        _cozyManager.cozyMaterials.snowAmount = snowAmount;
    }
}

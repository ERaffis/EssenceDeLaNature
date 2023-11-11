using DistantLands.Cozy;
using DistantLands.Cozy.Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;

public class WorldEvolutionManager : MonoBehaviour
{

    public Material[] _materials;
    public WeatherProfile[] _weather;
    public AtmosphereProfile _atmosphere;

    [Header("Player Object")]
    [Space(5)]
    public Transform _playerTransform;
    public Transform _cameraTransform;
    public PlayerController _playerController;
    [Space(5)]

    [Header("Scene Transition")]
    public string[] _SceneNames;
    public GameObject _transitionHolder;
    public GameObject[] _transitionText;


    [Header("Terrain Information")]
    [Space(5)]
    public Terrain[] _terrains;
    public CozyWeather _cozyManager;
    public WindZone _windZone;

    public float _currentTicks;

    public int _positionNumber = 0;

    private void Start()
    {/*
        _atmosphere.fogHeight.floatVal = 0.8f;
        _atmosphere.fogDensityMultiplier.floatVal = 0.5f;
        _windZone.windMain = 0.1f;
        _playerTransform.position = _startPosition;
        _playerTransform.rotation = Quaternion.Euler(_startRotation.x, _startRotation.y, _startRotation.z);
        _cameraTransform.rotation = Quaternion.Euler(_startCameraAim.x, _startCameraAim.y, _startCameraAim.z);
        */
        SceneManager.LoadScene(_SceneNames[0], LoadSceneMode.Additive);
        _transitionText[0].SetActive(true);
    }
    public void PhotoCaptured(int number)
    {

        switch (number)
        {
            case 0:
                ChangeScene(1, 0);

                /*
                print("Day 2");
                
                _cozyManager.SetWeather(_weather[17], 0);
                _atmosphere.fogHeight.floatVal = 0.8f;
                _atmosphere.fogDensityMultiplier.floatVal = 1.5f;
                _windZone.windMain = 1f;
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                _transitionText.text = "Un bon matin, en passant devant le camp de trappage du père Lacombre, à la rivière Toulnoustock, j'aperçus un grand duc exposé sur une des billes du camp; le père venait de le prendre dans un de ses collets à renard";
                */
                break;
            case 1:
                ChangeScene(2, 1);
                /*
                print("Day 3");
                FastForward(1);
                _cozyManager.SetWeather(_weather[13], 0);
                _atmosphere.fogHeight.floatVal = 1f;
                _atmosphere.fogDensityMultiplier.floatVal = 2f;
                _windZone.windMain = 2f;
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                _transitionText.text = "L'hiver il fa frette";
                */
                break;
            case 2:
                ChangeScene(3, 2);
                /*
                print("Day 4");
                FastForward(2);
                _cozyManager.SetWeather(_weather[17], 0);
                _atmosphere.fogHeight.floatVal = 0.8f;
                _atmosphere.fogDensityMultiplier.floatVal = 1.5f;
                _windZone.windMain = 1f;
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                */
                break;
            case 3:
                ChangeScene(4, 3);
                /*
                print("Day 5");
                FastForward(3);
                _cozyManager.SetWeather(_weather[18], 0);
                _atmosphere.fogHeight.floatVal = 0.8f;
                _atmosphere.fogDensityMultiplier.floatVal = 0.5f;
                _windZone.windMain = 0.1f;
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                */
                break;
            case 4:
                ChangeScene(5, 0);
                /*
                print("Day 6");
                FastForward(0);
                _cozyManager.SetWeather(_weather[3], 0);
                _windZone.windMain = 0.45f;
                for (int i = 0; i < _terrains.Length; i++)
                {
                    _terrains[i].materialTemplate = _materials[1];

                }
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                */
                break;
            case 5:
                ChangeScene(6, 1);
                /*
                print("Day 7 - Spring Starts");
                FastForward(1);
                _cozyManager.SetWeather(_weather[19], 0);
                _windZone.windMain = 0.05f;
                for (int i = 0; i < _terrains.Length; i++)
                {
                    _terrains[i].materialTemplate = _materials[2];

                }
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                */
                break;
            //Fin Hiver
            case 6:
                ChangeScene(7, 2);
                /*
                print("Day 8");
                FastForward(2);
                for (int i = 0; i < _terrains.Length; i++)
                {
                    _terrains[i].materialTemplate = _materials[3];

                }
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                */
                break;
            case 7:
                ChangeScene(8, 3);
                /*
                print("Day 9");
                FastForward(3);
                for (int i = 0; i < _terrains.Length; i++)
                {
                    _terrains[i].materialTemplate = _materials[4];

                }
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                */
                break;
            case 8:
                ChangeScene(9, 0);
                /*
                print("Day 10");
                FastForward(0);
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                */
                break;

            case 9:
                ChangeScene(10, 1);
                /*
                print("Day 11");
                FastForward(1);
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                */
                break;
            case 10:
                ChangeScene(11, 2);
                /*
                print("Day 12");
                FastForward(2);
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                */
                break;
        
                case 11:
                SceneManager.LoadSceneAsync(0);
                /*
                print("Day 13");
                FastForward(3);
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                */
                break;

            
            }
        }

        public void FastForward(int i)
    {
        switch (i)
        {
            case 0:
                _cozyManager.currentTicks = 120;
                break;
            case 1:
                _cozyManager.currentTicks = 240;
                break;
            case 2:
                _cozyManager.currentTicks = 360;
                break;
            case 3:
                _cozyManager.currentTicks = 480;
                break; 
            default:
                break;
        }
        _cozyManager.currentDay++;
    }

    public void ChangeScene(int sceneNumber, int timeChange )
    {
        SceneManager.UnloadSceneAsync(_SceneNames[sceneNumber-1]);
        SceneManager.LoadSceneAsync(_SceneNames[sceneNumber], LoadSceneMode.Additive);
        FastForward(timeChange);
        _transitionText[sceneNumber-1].SetActive(false);
        _transitionText[sceneNumber].SetActive(true);
        _transitionHolder.GetComponent<Animator>().Play("TransitionFadeOut");
    } 
}

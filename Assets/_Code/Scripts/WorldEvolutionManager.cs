using DistantLands.Cozy;
using DistantLands.Cozy.Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

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

    [Header("Teleport Presets")]
    [Space(5)]
    public Vector3 _startPosition;
    public Vector3 _startRotation;
    public Vector3 _startCameraAim; 
    [Space(2)]
    public Vector3[] _teleportPosition;
    public Vector3[] _teleportRotation;
    public Vector3[] _teleportCameraAim;

    [Header("Terrain Information")]
    [Space(5)]
    public Terrain[] _terrains;
    public CozyWeather _cozyManager;
    public WindZone _windZone;

    public float _currentTicks;
    public TMP_Text _transitionText;

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
    }
    public void PhotoCaptured(int number)
    {
        /*
        switch (number)
        {
            case 0:
                print("Day 2");
                FastForward(0);
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
                break;
            case 1:
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
                break;
            case 2:
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
                break;
            case 3:
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
                break;
            case 4:
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
                break;
            case 5:
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
                break;
            //Fin Hiver
            case 6:
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
                break;
            case 7:
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
                break;
            case 8:
                print("Day 10");
                FastForward(0);
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                break;
            case 9:
                print("Day 11");
                FastForward(1);
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                break;
            case 10:
                print("Day 12");
                FastForward(2);
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                break;
            
            case 11:
                print("Day 13");
                FastForward(3);
                _playerTransform.position = _teleportPosition[_positionNumber];
                _playerTransform.rotation = Quaternion.Euler(0, _teleportRotation[_positionNumber].y, 0);
                _cameraTransform.rotation = Quaternion.Euler(_teleportCameraAim[_positionNumber].x, _teleportCameraAim[_positionNumber].y, 0);
                _playerController._cinemachineTargetPitch = _teleportCameraAim[_positionNumber].x;
                _positionNumber++;
                break;

            //Fin Printemps
            case 12:
                print("Day 14");
                FastForward(0);
                break;
            case 13:
                print("Day 15");
                FastForward(1);
                break;
            case 14:
                print("Day 16 - Summer Starts");
                FastForward(2);
                break;
            case 15:
                print("Day 17");
                FastForward(3);
                break;
            case 16:
                print("Day 18");
                FastForward(0);
                break;
            
            case 17:
                print("Day 19");
                FastForward(1);
                break;
            //Fin Été
            case 18:
                print("Day 20");
                FastForward(2);
                break;
            case 19:
                print("Day 21");
                FastForward(3);
                break;
            case 20:
                print("Day 22");
                FastForward(0);
                break;
            case 21:
                print("Day 23 - Autumn Starts");
                FastForward(1);
                break;
            case 22:
                print("Day 24");
                FastForward(2);
                break;
            
            case 23:
                print("Day 25");
                FastForward(3);
                for (int i = 0; i < _terrains.Length; i++)
                {
                    _terrains[i].materialTemplate = _materials[0];

                }
                break;
            //Fin pour l'instant
            case 24:
                print("Day 26");
                FastForward(0);
                
                break;
            case 25:
                print("Day 27");
                FastForward(1);
                
                break;
            case 26:
                print("Day 28 - Winter Starts");
                FastForward(2);
                
                break;
            case 27:
                print("Day 29");
                FastForward(3);
                break;
            case 28:
                print("Day 30");
                FastForward(0);
                break;
            case 29:
                print("Day 31");
                FastForward(1);
                break;
            case 30:
                print("Day 32");
                FastForward(2);
                break;
            case 31:
                print("Day 33");
                FastForward(3);
                for (int i = 0; i < _terrains.Length; i++)
                {
                    _terrains[i].materialTemplate = _materials[3];

                }
                break;
            case 32:
                print("Day 34");
                FastForward(0);
                for (int i = 0; i < _terrains.Length; i++)
                {
                    _terrains[i].materialTemplate = _materials[2];

                }
                break;
            case 33:
                print("Day 35");
                FastForward(1);
                for (int i = 0; i < _terrains.Length; i++)
                {
                    _terrains[i].materialTemplate = _materials[1];

                }
                break;
            case 34:
                print("Day 36");
                FastForward(2);
                
                break;
            case 35:
                print("Day 37");
                FastForward(3);
                print("Et le cycle continue, c'est à vous de choisir. Souhaitez-vous rester en nature ou la quitter?");
                break;
            default:
                break;
        }*/
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
}

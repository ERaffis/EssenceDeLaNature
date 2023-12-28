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
    public InputManager _inputManager;
    [Space(5)]
    public Vector3[] _teleportPosition;

    [Header("Scene Transition")]
    public string[] _SceneNames;
    public GameObject _transitionHolder;
    public GameObject[] _transitionText;
    
    public string[] timeStamps;


    [Header("Terrain Information")]
    [Space(5)]
    public CozyWeather _cozyManager;
    //public WindZone _windZone;

    //public float _currentTicks;

    public int positionNumber = 0;


    private void Start()
    {
        SceneManager.LoadScene(_SceneNames[0], LoadSceneMode.Additive);
        _cozyManager.currentTicks = 60;
        _cozyManager.currentDay = 0;
        _transitionText[0].SetActive(true);
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public void PhotoCaptured(int number)
    {
        StartCoroutine(BlockMovement());
        
        switch (number)
        {
            case 0:
                positionNumber++;
                StartCoroutine(ChangeScene(positionNumber, timeStamps[0]));
                ChangeCozySettings(0, 0.9f);
                break;
            case 1:
                positionNumber++;
                StartCoroutine(ChangeScene(positionNumber, timeStamps[1]));
                ChangeCozySettings(0, 0.8f);
                break;
            case 2:
                positionNumber++;
                StartCoroutine(ChangeScene(positionNumber, timeStamps[2]));
                ChangeCozySettings(1, 0.4f);
                break;
            case 3:
                positionNumber++;
                StartCoroutine(ChangeScene(positionNumber, timeStamps[3]));
                ChangeCozySettings(1, 0.1f);
                break;
            case 4:
                positionNumber++;
                StartCoroutine(ChangeScene(positionNumber, timeStamps[4]));
                ChangeCozySettings(1, 0f);
                break;
            case 5:
                positionNumber++;
                StartCoroutine(ChangeScene(positionNumber, timeStamps[5]));
                ChangeCozySettings(2, 0f);
                break;
            //Fin Hiver
            case 6:
                positionNumber++;
                StartCoroutine(ChangeScene(positionNumber, timeStamps[6]));
                ChangeCozySettings(2, 0f);
                break;
            case 7:
                positionNumber++;
                StartCoroutine(ChangeScene(positionNumber, timeStamps[7]));
                ChangeCozySettings(2, 0.25f);
                break;
            case 8:
                positionNumber++;
                StartCoroutine(ChangeScene(positionNumber, timeStamps[8]));
                ChangeCozySettings(3, 0.6f);
                break;

            case 9:
                positionNumber++;
                StartCoroutine(ChangeScene(positionNumber, timeStamps[9]));
                ChangeCozySettings(3, 0.8f);
                break;
            case 10:
                positionNumber++;
                StartCoroutine(ChangeScene(positionNumber, timeStamps[10]));
                ChangeCozySettings(3, 0.9f);
                break;

            case 11:
                positionNumber++;
                StartCoroutine(ChangeScene(positionNumber, timeStamps[11]));
                ChangeCozySettings(3, 0.9f);
                break;

            case 12:
                StartCoroutine(LoadLastScene());
                break;

        } 
    }

    private void FastForward(string timeStamp)
    {
        _cozyManager.currentDay++;
        _cozyManager.currentTicks = timeStamp switch
        {
            "Dawn" => 12.5f,
            "Noon" => 30f,
            "Dusk" => 50f,
            "Night" => 60f,
            _ => _cozyManager.currentTicks
        };
    }

    public IEnumerator BlockMovement()
    {
        _inputManager._controls.FindActionMap("BasicControls").Disable();
        yield return new WaitForSeconds(5f);
        _inputManager._controls.FindActionMap("BasicControls").Enable();
    }

    private IEnumerator ChangeScene(int sceneNumber, string timeStamp )
    {
        SceneManager.UnloadSceneAsync(_SceneNames[sceneNumber-1]);
        SceneManager.LoadScene(_SceneNames[sceneNumber], LoadSceneMode.Additive);

        FastForward(timeStamp);
        _transitionText[sceneNumber-1].SetActive(false);
        _transitionText[sceneNumber].SetActive(true);
        _transitionHolder.GetComponent<Animator>().Play("TransitionFadeOut");
        yield return new WaitForSeconds(2);
        TeleportPlayer();
        _inputManager.OnCamera();
    } 

    private void TeleportPlayer()
    {
        _playerTransform.position = _teleportPosition[positionNumber];
    }

    private void ChangeCozySettings(int volumeProfile, float snowAmount)
    {
        _cozyManager.currentDay = positionNumber;
        _cozyManager.SetWeather(_weather[positionNumber]);
        _mainVolume.profile = _volumeProfiles[volumeProfile];
        _cozyManager.cozyMaterials.snowAmount = snowAmount;
    }

    private IEnumerator LoadLastScene()
    {
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(14);
    }
}

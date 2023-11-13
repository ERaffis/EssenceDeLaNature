using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class CameraController : MonoBehaviour
{
    public PlayerInventory _playerInventory;
    public InputManager _input;
    public WorldEvolutionManager _worldEvolutionManager;

    public bool _canTakePicture;
    public int _maxPhotoCount;

    [Header("Audio")]
    [Space(1)]
    [SerializeField] AudioSource shutterSound;

    [Header("Photo")]
    [Space(1)]
    [SerializeField] Image photoDisplayArea;

    [Header("Frame")]
    [Space(1)]
    [SerializeField] GameObject photoFrame;

    [Header("Flash")]
    [Space(1)]
    [SerializeField] GameObject cameraShutter;
    [SerializeField] float shutterDuration;

    [Header("Animation")]
    [Space(1)]
    [SerializeField] Animator frameAnimator;
    [SerializeField] Animator photoAnimator;

    [Header("Photo Storage")]
    [Space(1)]
    public int photoNumber;
    public SO_Picture[] photoAlbum;

    //private bool canTakePicture;
    private Texture2D screenCapture;

    //private bool _turnOnOrOff;
    public GameObject transitionText;

    [Header("Main UI Elements")]
    [Space(1)]
    public GameObject mainUI;
    public TMP_Text shotNumberUI;
    [Space(1)]
    public TMP_Text albumImageIndicator;
    public GameObject selectPhotoButton;
    public bool isSelectingPhoto;

    private Vector2 resolution = new Vector2(Screen.width, Screen.height);

    private void Awake()
    {
        for (int i = 0; i < photoAlbum.Length; i++)
        {
            photoAlbum[i].imageSprite = null;
            photoAlbum[i].imageTexture2D = null;
            photoAlbum[i].isPlaced = false;
            photoAlbum[i].isEmpty = true;
        }
        resolution = new Vector2(Screen.width, Screen.height);
    }
    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        photoNumber = 0;
        
    }

    private void Update()
    {
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            resolution.x = Screen.width;
            resolution.y = Screen.height;
        }
    }


    public void OnTakePhoto()
    {
        if (!_input._inMenu)
        {
            if (_playerInventory._cameraState)
            {
                InitiateCapture();
            }
        }
        
    }

    public void InitiateCapture()
    {
        if (_canTakePicture /*&& !photoAlbumUI.activeSelf */)
        {
            _canTakePicture = false;
            mainUI.SetActive(false);
            StartCoroutine(CapturePhoto());
        }
    }

    public IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        //Shutter sequence
        shutterSound.time = 0.5f;
        shutterSound.Play();

        //Take photo
        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);
        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();

        Sprite photoCapture = Sprite.Create(screenCapture, regionToRead, new Vector2(0.5f, 0.5f), 100.0f);
        photoCapture.name = "Image " + photoNumber;

        cameraShutter.SetActive(true);
        _worldEvolutionManager.PhotoCaptured(photoNumber);
        photoNumber++;

        _playerInventory.OnCamera();
        
        yield return new WaitForSeconds(shutterDuration);

        ShowPhoto(photoCapture);
        cameraShutter.SetActive(false);

        //Hide photo
        yield return new WaitForSeconds(3.31f);
        photoFrame.SetActive(false);
        photoFrame.GetComponent<CanvasGroup>().alpha = 0;
        //transitionText.SetActive(false);
        if (photoNumber < _maxPhotoCount)
            _canTakePicture = true;
    }

    void ShowPhoto(Sprite photoCapture)
    {
        byte[] itemBGBytes = photoCapture.texture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/SavedScreen" + photoNumber + ".png", itemBGBytes);

        photoAlbum[photoNumber - 1].imageSprite = LoadNewSprite(Application.dataPath + "/SavedScreen" + photoNumber + ".png");
        photoAlbum[photoNumber - 1].imageTexture2D = LoadTexture(Application.dataPath + "/SavedScreen" + photoNumber + ".png");
        photoAlbum[photoNumber - 1].isEmpty = false;


        photoDisplayArea.sprite = photoCapture;
        photoFrame.GetComponent<CanvasGroup>().alpha = 0;
        photoFrame.SetActive(true);

        mainUI.SetActive(true);
    }

    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
        Texture2D SpriteTexture = LoadTexture(FilePath);
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);

        return NewSprite;
    }

    public Texture2D LoadTexture(string FilePath)
    {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }

    
}

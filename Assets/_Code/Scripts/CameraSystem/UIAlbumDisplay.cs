using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAlbumDisplay : MonoBehaviour
{
    public Image _currentDisplay;
    public Animator _animator;
    public AudioSource _audioClip;

    public CameraController _cameraController;
    public CanvasGroup _canvasGroup;

    public int _photoDisplaying;
    public bool _isSelectingPhoto;
    public int _maxPhoto;

    private void OnEnable()
    {
        _currentDisplay.sprite = _cameraController.photoAlbum[_photoDisplaying].imageSprite;
    }
    private void OnDisable()
    {
        _canvasGroup.interactable = true;
        _canvasGroup.alpha = 1f;
        _isSelectingPhoto = false;
    }

    public void Next()
    {
        _photoDisplaying++;
        if (_photoDisplaying > _maxPhoto-1)
        {
            _photoDisplaying = 0;
        }
        StartCoroutine(ChangePhoto());
        
    }
    public void Previous()
    {
        _photoDisplaying--;
        if (_photoDisplaying < 0)
        {
            _photoDisplaying = _maxPhoto-1;
        }
        StartCoroutine(ChangePhoto());
        
    }

    public IEnumerator ChangePhoto()
    {
        _animator.Play("FadeInOut");
        _audioClip.Play();
        yield return new WaitForSeconds(0.5f);
        _currentDisplay.sprite = _cameraController.photoAlbum[_photoDisplaying].imageSprite;
    }

}

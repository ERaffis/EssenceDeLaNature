using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceEndImage : MonoBehaviour
{
    public Image[] _pictures;
    public GameObject[] _pictureParent;
    public SO_Picture[] _pictureHolder;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _pictures.Length+1; i++)
        {
           
            //_pictureParent[i].GetComponent<CanvasGroup>().alpha = 1f;
            _pictures[i].sprite = _pictureHolder[i].imageSprite;
            //_pictureParent[i].GetComponent<Button>().interactable = true;
            _pictureParent[i].GetComponent<CenterPicture>().bigImagePicture.sprite = _pictures[i].sprite;
            
        }
    }

}

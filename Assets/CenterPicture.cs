using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CenterPicture : MonoBehaviour
{
    public GameObject bigImage;
    public Image bigImagePicture;

    public void SelectPhoto()
    {
        bigImage.SetActive(true);

    }
    public void DeSelectPhoto()
    {
        this.gameObject.SetActive(false);
    }
}

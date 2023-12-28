using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using MoreMountains.Feedbacks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem; 

public class ChangeZoomImage : MonoBehaviour
{

    public Sprite[] cameraHUDImages;
    public Image cameraHUD;
    public CinemachineVirtualCamera photoCAM;
    public MMF_Player zoomSound;

    private bool _canPlaySound;

    private void Start()
    {
        _canPlaySound = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vec = Mouse.current.scroll.ReadValue();
        
        if (Mouse.current.scroll.ReadValue() == new Vector2(0,120) & photoCAM.m_Lens.FieldOfView > 10)
        {
            zoomSound.PlayFeedbacks();
        }
        
        if (Mouse.current.scroll.ReadValue() == new Vector2(0,-120) & photoCAM.m_Lens.FieldOfView < 130)
        {
            zoomSound.PlayFeedbacks();
        }
        
        
        photoCAM.m_Lens.FieldOfView += ((vec.y / 120)*10)*-1;  

        if (photoCAM.m_Lens.FieldOfView < 20)
        {
            photoCAM.m_Lens.FieldOfView = 10;
        }
        if (photoCAM.m_Lens.FieldOfView > 120)
        {
            photoCAM.m_Lens.FieldOfView = 130;
        }
        
        switch (photoCAM.m_Lens.FieldOfView)
        {
            case 10:
                cameraHUD.sprite = cameraHUDImages[12];
                break;
            case 20:
                cameraHUD.sprite = cameraHUDImages[11];
                break;
            case 30:
                cameraHUD.sprite = cameraHUDImages[10];
                break;
            case 40:
                cameraHUD.sprite = cameraHUDImages[9];
                break;
            case 50:
                cameraHUD.sprite = cameraHUDImages[8];
                break;
            case 60:
                cameraHUD.sprite = cameraHUDImages[7];
                break;
            case 70:
                cameraHUD.sprite = cameraHUDImages[6];
                break;
            case 80:
                cameraHUD.sprite = cameraHUDImages[5];
                break;
            case 90:
                cameraHUD.sprite = cameraHUDImages[4];
                break;
            case 100:
                cameraHUD.sprite = cameraHUDImages[3];
                break;
            case 110:
                cameraHUD.sprite = cameraHUDImages[2];
                break;
            case 120:
                cameraHUD.sprite = cameraHUDImages[1];
                break;
            case 130 :
                cameraHUD.sprite = cameraHUDImages[0];
                break;
        }

        
    }
}

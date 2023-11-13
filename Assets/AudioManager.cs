using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    public float volume = 0f;
    public Sprite[] buttonsprites;
    public Image buttonsprite;
    public void MuteMixer()
    {
        if (volume == 0f) 
        {
            volume = -80f;
            buttonsprite.sprite = buttonsprites[1];
        }
        else
        {
            volume = 0f;
            buttonsprite.sprite = buttonsprites[0];
        }
        mainMixer.SetFloat("Volume", volume);
    }
}

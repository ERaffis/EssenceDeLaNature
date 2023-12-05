using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opencredits : MonoBehaviour
{
    public GameObject menu;
    public GameObject credits;

    public void GoCredits()
    {
        menu.SetActive(false);
        credits.SetActive(true);
    }
    public void BackCredits()
    {
        menu.SetActive(true);
        credits.SetActive(false);
    }
}

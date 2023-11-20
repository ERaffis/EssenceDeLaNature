using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetCursorState(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimations : MonoBehaviour
{
    public Vector3 scaleAmount = new Vector3(1.25f,1.25f,1.25f);
    public void ScaleUp()
    {
        transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
    }
    public void ScaleDown()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }
    
    public void ScaleSpecific()
    {
        transform.localScale = scaleAmount;
    }

    private void OnDisable()
    {
        ScaleDown();
    }
}

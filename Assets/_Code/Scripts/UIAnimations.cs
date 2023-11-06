using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimations : MonoBehaviour
{
    public void ScaleUp()
    {
        transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
    }
    public void ScaleDown()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnDisable()
    {
        ScaleDown();
    }
}

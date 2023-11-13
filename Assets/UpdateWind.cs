using Broccoli.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateWind : MonoBehaviour
{

    public BroccoTerrainController _windController;
    public float _windStrenght;
    public float _windTurbulence;
    public Vector3 _windDirection;


    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        _windController.UpdateWind(_windStrenght, _windTurbulence, _windDirection);
    }
}

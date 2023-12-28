using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;

[ExecuteInEditMode]
public class Triggerscript : MonoBehaviour
{
    private ParticleSystem psystem;
    private GameObject collider;
    private void OnEnable()
    {
        psystem = this.GetComponent<ParticleSystem>();
        collider = GameObject.FindGameObjectWithTag("KillZone");
        psystem.trigger.AddCollider(collider.GetComponent<BoxCollider>());
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface IInteractable
{
    public void Interact();
}
public class PlayerInteractor : MonoBehaviour
{

    public Transform _source;
    public float _range;


    public void OnInteract()
    {
        Ray r = new Ray(_source.position, _source.forward);
        Debug.DrawRay(_source.position, _source.forward, Color.blue, 5f);
        if (Physics.Raycast(r, out RaycastHit hitInfo, _range))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.Interact();
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PressurePlate : Signal<bool>
{
    public NetworkVariable<bool> isActivated = new NetworkVariable<bool>(false);
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Material OffMaterial;
    [SerializeField] private Material OnMaterial;
    [SerializeField] private int objectsRequired = 1;
    
    /// <summary>
    /// Track Objects on the Pressure Plate to determine if it is active or not
    /// </summary>
    private List<GameObject> objectsOnPlate = new List<GameObject>();
    
    private MeshRenderer meshRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = OffMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check if the object is on the layerMask
        if (layerMask != (layerMask | (1 << other.gameObject.layer))) return;
        //Add the object to the list
        objectsOnPlate.Add(other.gameObject);
        CheckActive();
    }

    private void OnTriggerExit(Collider other)
    {
        //Check if the object is on the layerMask
        if (layerMask != (layerMask | (1 << other.gameObject.layer))) return;
        //Remove the object from the list
        objectsOnPlate.Remove(other.gameObject);
        CheckActive();
    }

    private void CheckActive()
    {
        //Check if the list is empty
        bool active = objectsOnPlate.Count >= objectsRequired;
        meshRenderer.material = active ? OnMaterial : OffMaterial;
        isActivated.Value = active;
    }

    public override NetworkVariable<bool> Value => isActivated;
}

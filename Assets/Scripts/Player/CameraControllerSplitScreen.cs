using System;
using Unity.Netcode;
using UnityEngine;


public class CameraControllerSplitScreen : MonoBehaviour
{
    public GameObject cameraHolder;
    [SerializeField] private Vector3 cameraOffset;

    private void Start()
    {
        cameraHolder.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        cameraHolder.transform.position = transform.position + cameraOffset;
    }
}

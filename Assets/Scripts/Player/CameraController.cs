using Unity.Netcode;
using UnityEngine;


public class CameraController : NetworkBehaviour
{
    public GameObject cameraHolder;
    [SerializeField] private Vector3 cameraOffset;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        cameraHolder.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        cameraHolder.transform.position = transform.position + cameraOffset;
    }
}

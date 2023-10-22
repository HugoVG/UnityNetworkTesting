using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerDebug : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    
    public void SpawnPlayer()
    {
        var player = Instantiate(playerPrefab, new Vector3(0,1,0), Quaternion.identity);
        player.GetComponent<CameraController>().cameraHolder.SetActive(true);
    }
}

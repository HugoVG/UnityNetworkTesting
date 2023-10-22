using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTestScript : MonoBehaviour
{
    
    public GameObject Prefab;
    
    public bool SpawnPlayer;
    
    [DrawIf("SpawnPlayer", true)]
    public Transform SpawnPoint;
    
    [Foldout("Test", readOnly: true, styled:true)]
    public bool Test;
    
    public Vector3 TestVector;
}
